using CuentasporPagar.Models;
using CuentasporPagar.Models.EF;
using Syncfusion.JavaScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
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
            lmPanelAsistProceso = db.PanelAsistProceso.Where(m => m.NumPedidoSAP != null).ToList();
            SessionManager.Set("DProceso", lmPanelAsistProceso);

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

        [HttpPost]
        public JsonResult SolicitanteComplete(string Prefix)
        {
            List<RCAU> ObjList = db.RCAU.Where(m => m.USTS == "A" && m.UMOD == "SRGUSR" && (m.UUSR.ToUpper() + " - " + m.UNOM.ToUpper()).Contains(Prefix.ToUpper())).Take(12).ToList();
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

        public ActionResult DSProceso(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<PanelAsistProceso>>("DProceso");
            if (Data == null)
            {
                Data = new List<PanelAsistProceso>();
            }
            return fc.DSGenerico(dm, Data);
        }

        public ActionResult GuardarCxPProceso(string CentroCosto, string CuentaContable, string Nota, string Area, string DescGastos, string NmeProveedor, string NitProveedor, string MailProveedor, string Solicitante, string VlrAntesIva, string NumPedSAP, string FecMaxRadicacion, string NFactura, string FecFac, string VlrTotal, string Mon, string CategFactu, string Asist)
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Proceso") });
            }
            string res = "";
            StringBuilder Err = new StringBuilder();            

            try
            {                
                CxPProceso mCxPProceso = new CxPProceso()
                {
                    NitProveedor = NitProveedor,
                    NombreProveedor = NmeProveedor,
                    EmailProveedor = MailProveedor,
                    NPedidoSAP = NumPedSAP,
                    FechaMaxRadicacion = DateTime.Parse(FecMaxRadicacion),
                    Observaciones = Nota,
                    CentroCostos = CentroCosto,
                    CuentaContable = CuentaContable,
                    IdArea = int.Parse(Area),
                    NFactura = NFactura,                    
                    FechaRadicacion = DateTime.Now,
                    FechaFactura = DateTime.Parse(FecFac),
                    Valor = double.Parse(VlrAntesIva),
                    ValorTotal = double.Parse(VlrTotal),
                    Moneda = Mon,
                    CategorizacionFactura = CategFactu,
                    NombreRecibe = Asist,
                    
                    STS001 = "1",
                    STS002 = "0",
                    STS003 = "0",
                    STS004 = "0",
                    STS005 = "0",
                    STS006 = "0",
                    STS007 = "0"
                };
                db.CxPProceso.Add(mCxPProceso);
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Err.AppendLine(ex.ToString());
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Err.AppendLine("");
                    Err.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Err.AppendLine("");
                        Err.AppendFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                res = "ERROR: " + Err.ToString();
            }
            catch (Exception ex)
            {
                res = "ERROR: " + ex.ToString() + ((ex.InnerException != null) ? ex.InnerException.ToString() : "");
            }


            return Json(new { result = res });
        }

        public ActionResult IniciaCxpProceso(string IdP, string FecFac, string NFactura, string VlrAntesImpu, string VlrTotal, string Mon, string CategFactu, string Asist)
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Proceso") });
            }
            string res = "";
            StringBuilder Err = new StringBuilder();

            try
            {
                PanelAsistProceso mPanelAsistProceso = db.PanelAsistProceso.Find(int.Parse(IdP));
                if (mPanelAsistProceso != null)
                {
                    CxPProceso mCxPProceso = new CxPProceso()
                    {
                        NitProveedor = mPanelAsistProceso.NitProveedor,
                        NombreProveedor = mPanelAsistProceso.NombreProveedor,
                        EmailProveedor = mPanelAsistProceso.CorreoProveedor,
                        NPedidoSAP = mPanelAsistProceso.NumPedidoSAP,
                        NFactura = NFactura,
                        FechaMaxRadicacion = mPanelAsistProceso.FechaMaximaRadicacion,
                        FechaRadicacion = DateTime.Now,
                        FechaFactura = DateTime.Parse(FecFac),
                        Valor = double.Parse(VlrAntesImpu),
                        ValorTotal = double.Parse(VlrTotal),
                        Moneda = Mon,
                        CategorizacionFactura = CategFactu,
                        Observaciones = mPanelAsistProceso.NotaAprobador,
                        CentroCostos = mPanelAsistProceso.CodCentroCosto,
                        CuentaContable = mPanelAsistProceso.CodCuentaContable,
                        IdArea = mPanelAsistProceso.IdPanelAsistArea,
                        NombreRecibe = Asist,
                        
                        STS001 = "1",
                        STS002 = "0",
                        STS003 = "0",
                        STS004 = "0",
                        STS005 = "0",
                        STS006 = "0",
                        STS007 = "0"
                    };
                    db.CxPProceso.Add(mCxPProceso);
                    db.SaveChanges();
                    res = "OK";
                }
                else
                {
                    res = "No existe Proceso";
                }                
            }
            catch (DbEntityValidationException ex)
            {
                Err.AppendLine(ex.ToString());
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Err.AppendLine("");
                    Err.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Err.AppendLine("");
                        Err.AppendFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                res = "ERROR: " + Err.ToString();
            }
            catch (Exception ex)
            {
                res = "ERROR: " + ex.ToString() + ((ex.InnerException != null) ? ex.InnerException.ToString() : "");
            }


            return Json(new { result = res });
        }
    }
}