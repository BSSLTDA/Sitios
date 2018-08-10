using CuentasporPagar.Models;
using CuentasporPagar.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuentasporPagar.Controllers
{
    public class ProcesoController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        List<PanelAsistProceso> lmPanelAsistProceso = new List<PanelAsistProceso>();
        SRGEntities db = new SRGEntities();
        InfoNaturaEntities dbIN = new InfoNaturaEntities();

        // GET: Procreso
        public ActionResult Index()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Proceso") });
            }
            lmPanelAsistProceso = GetAllProcess();

            return View();
        }

        public List<PanelAsistProceso> GetAllProcess()
        {
            try
            {
                RCAU mRCAU = SessionManager.Get<RCAU>("VarUsuario");
                if (mRCAU.UACC01 == null || mRCAU.UACC01 == 1) //USUARIO
                {
                    lmPanelAsistProceso = db.PanelAsistProceso.Where(m => m.Creador == mRCAU.UUSR && m.STS01 == 1).OrderByDescending(m => m.IdPanelAsistProceso).ToList();
                    SessionManager.Set("DSProcesosUsr", CargaTablas(lmPanelAsistProceso));
                }
                if (mRCAU.UACC02 == 1) //ASISTENTE
                {
                    lmPanelAsistProceso = db.PanelAsistProceso.Where(m => m.Asistente == mRCAU.UUSR && m.STS01 == 1 && m.STS02 == 0 && m.STS03 == 0).ToList();
                    var query2 = db.PanelAsistProceso.Where(m => m.Asistente != mRCAU.UUSR && m.STS01 == 1 && m.STS02 == 0 && m.STS03 == 0).OrderBy(m => m.Asistente).ToList();
                    foreach (var item in query2)
                    {
                        var mPanelAsistProceso = item;
                        lmPanelAsistProceso.Add(mPanelAsistProceso);
                    }
                    SessionManager.Set("DSProcesosAst", CargaTablas(lmPanelAsistProceso));
                }
                if (mRCAU.UACC03 == 1) //JEFE
                {

                    lmPanelAsistProceso = db.PanelAsistProceso.OrderByDescending(m => m.IdPanelAsistProceso).ToList();
                    SessionManager.Set("DSProcesosJefe", CargaTablas(lmPanelAsistProceso));
                }
            }
            catch (Exception ex)
            {
                //res = "ERROR: " + ex.Message;
            }
            return lmPanelAsistProceso;
        }

        private List<PanelAsistProceso> CargaTablas(List<PanelAsistProceso> Pap)
        {
            var lmRCAU = db.RCAU;
            RCAU solicita, Asiste, crea;
            PanelAsistArea area;
            RFPARAM cc;
            RFPARAM ctac;

            var lmRFPARAMCTA = db.RFPARAM.Where(m => m.CCTABL == "CUENTA");

            var lmRFPARAMCC = db.RFPARAM.Where(m => m.CCTABL == "CENTROCOSTO");

            var lmPanelAsistArea = db.PanelAsistArea;

            var lmRFADJUNTO = db.RFADJUNTO.Where(m => m.FACATEG01 == "PANELASISTENTE").ToList();

            foreach (var item in Pap)
            {
                if (item.STS02 == 1 || item.STS03 == 1)
                {
                    item.NotaSolicitante = item.NotaAprobador;
                }
                if (item.STS01 == 1)
                {
                    item.Estado = "Pendiente";
                }
                if (item.STS02 == 1)
                {
                    item.Estado = "Procesada";
                }
                if (item.STS03 == 1)
                {
                    item.Estado = "Rechazada";
                }
                if (item.STS04 == 1)
                {
                    item.Estado = "Correo Enviado";
                }
                if (item.STS05 == 1)
                {
                    item.Estado = "Reenviando Correo";
                }
                solicita = lmRCAU.Where(m => m.UUSR.Trim() == item.Solicitante.Trim()).SingleOrDefault();
                if (solicita != null)
                {
                    item.Solicitante = solicita.UNOM.Trim();
                }
                Asiste = lmRCAU.Where(m => m.UUSR.Trim() == item.Asistente.Trim()).SingleOrDefault();
                if (Asiste != null)
                {
                    item.Asistente = Asiste.UNOM.Trim();
                }
                crea = lmRCAU.Where(m => m.UUSR.Trim() == item.Creador.Trim()).SingleOrDefault();
                if (crea != null)
                {
                    item.Creador = crea.UNOM.Trim();
                }
                area = lmPanelAsistArea.Where(m => m.IdPanelAsistArea == item.IdPanelAsistArea).SingleOrDefault();
                if (area != null)
                {
                    item.Area = area.Descripcion;
                }
                cc = lmRFPARAMCC.Where(m => m.CCCODE == item.CodCentroCosto.ToString()).SingleOrDefault();
                if (cc != null)
                {
                    item.CodCentroCosto = cc.CCCODE + "-" + cc.CCDESC;
                }
                ctac = lmRFPARAMCTA.Where(m => m.CCCODE.Trim() == item.CodCuentaContable.Trim()).SingleOrDefault();
                if (ctac != null)
                {
                    item.CodCuentaContable = ctac.CCCODE + "-" + ctac.CCDESC;
                }
                item.Adjunto = null;
                item.AdjuntoNme = null;
                var lRFADJU = lmRFADJUNTO.Where(m => m.FAPROCNUM == item.IdPanelAsistProceso).ToList();
                if (lRFADJU != null)
                {
                    if (lRFADJU.Count() > 0)
                    {
                        foreach (var adju in lRFADJU)
                        {
                            if (item.Adjunto == null)
                            {
                                item.Adjunto = adju.FAURL.Replace("~", "..");
                                item.AdjuntoNme = adju.FAFILE;
                            }
                            else
                            {
                                item.Adjunto += "|" + adju.FAURL.Replace("~", "..");
                                item.AdjuntoNme += "|" + adju.FAFILE;
                            }
                        }
                    }
                }
            }

            return Pap;
        }
    }
}