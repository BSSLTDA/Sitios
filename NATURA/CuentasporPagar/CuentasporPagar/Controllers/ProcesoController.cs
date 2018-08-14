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

        private List<RFPARAM> ListCTACProceso(string prefix, string Codigo)
        {
            var res = new List<RFPARAM>();
            prefix = prefix.ToUpper();

            try
            {
                var query = from p in db.RFPARAM
                            join p2 in db.RFPARAM on p.CCCODE2 equals p2.CCCODE
                            where new { cc = p.CCTABL, ctaa = p2.CCTABL, cod = p.CCCODE } ==
                            new { cc = "CC_CTA", ctaa = "CUENTA", cod = Codigo }
                            select new MdlRFPARAM()
                            {
                                CCCODE = p.CCCODE,
                                CCDESC = p2.CCCODE + " " + p2.CCDESC,
                                CCCODE2 = p.CCCODE2
                            };
                var lista = query.ToList();
                var query2 = query.Where(m => (m.CCDESC.ToUpper()).Contains(prefix)).Take(12).ToList();
                foreach (var item in query2)
                {
                    var nRFPARAM = new RFPARAM()
                    {
                        CCCODE = item.CCCODE,
                        CCCODE2 = item.CCCODE2,
                        CCDESC = item.CCDESC
                    };
                    res.Add(nRFPARAM);
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return res;
        }

        private List<RFPARAM> ListCCProceso(string prefix)
        {
            var res = new List<RFPARAM>();
            prefix = prefix.ToUpper();

            try
            {
                var query = db.RFPARAM.Where(m => m.CCTABL == "CENTROCOSTO" && (m.CCCODE.ToUpper() + " " + m.CCDESC.ToUpper()).Contains(prefix)).Take(12).ToList();
                res = query;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return res;
        }

        [HttpPost]
        public JsonResult CentroCostoComplete(string Prefix)
        {
            List<RFPARAM> ObjList = ListCCProceso(Prefix);
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CuentaContableComplete(string Prefix, string CC)
        {
            List<RFPARAM> ObjList = ListCTACProceso(Prefix, CC.Split('-')[0].ToString().Trim());
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AreaComplete(string Prefix)
        {
            List<PanelAsistArea> ObjList = db.PanelAsistArea.Where(m => m.Descripcion.ToUpper().Contains(Prefix.ToUpper())).Take(12).ToList();
            return Json(ObjList, JsonRequestBehavior.AllowGet);
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
            }

            return Pap;
        }
    }
}