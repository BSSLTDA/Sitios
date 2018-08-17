using CuentasporPagar.Models;
using CuentasporPagar.Models.EF;
using Syncfusion.JavaScript;
using Syncfusion.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CuentasporPagar.Controllers
{
    public class CXPProcesoController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        SRGEntities db = new SRGEntities();

        // GET: CXPProceso
        public ActionResult Index()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }

        public ActionResult Factura()//Paso de radicar factura
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Factura", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }

        public ActionResult Entrega()//Paso de enviar correo al asistente
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Entrega", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }

        public ActionResult HojaServicio()//Paso de liquidar impuestos
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("HojaServicio", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }

        public ActionResult GenerarImpuesto()//No deberia estar aqui pero era pruebas del algoritmo de liquidar impuesto
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("GenerarImpuesto", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }

        public ActionResult GenerarDocumentoGED()//Paso de generar documento para el GED
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("GenerarDocumentoGED", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }
        
        public ActionResult NuevoPaso3(DateTime FechaRadicacion, DateTime FechaFactura, string NFactura, string Valor, string ValorTotal, string Moneda, string CategorizacionFactura, string Observaciones)
        {//Este metodo no deberia estar ya que debe existir la factura desde panel de asistentes y solo debe estar el metodo para editar dicho registro
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                var nCxPProceso = new CxPProceso()
                {
                    FechaRadicacion =  FechaRadicacion,
                    FechaFactura = FechaFactura,
                    NFactura = NFactura,
                    Valor = double.Parse(Valor),
                    ValorTotal = double.Parse(ValorTotal),
                    Moneda = Moneda,
                    CategorizacionFactura = CategorizacionFactura,
                    Observaciones = Observaciones
                };
                db.CxPProceso.Add(nCxPProceso);
                db.SaveChanges();
                SessionManager.Set("DCXPProceso", db.CxPProceso.ToList());
                Respu = "OK";
            }
            catch (DbEntityValidationException ex)
            {
                Error.AppendLine(ex.ToString());
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Error.AppendLine("");
                    Error.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Error.AppendLine("");
                        Error.AppendFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                Respu = "ERROR: " + Error.ToString();
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString() + ((ex.InnerException != null) ? ex.InnerException.ToString() : "");
            }
            return Json(new { result = Respu });
        }

        public ActionResult EditarPaso3(int IdCxPProceso, DateTime FechaRadicacion, DateTime FechaFactura, string NFactura, string Valor, string ValorTotal, string Moneda, string CategorizacionFactura, string Observaciones)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                var uCxPProceso = new CxPProceso()
                {
                    IdCxPProceso = IdCxPProceso,
                    FechaRadicacion = FechaRadicacion,
                    FechaFactura = FechaFactura,
                    NFactura = NFactura,
                    Valor = double.Parse(Valor),
                    ValorTotal = double.Parse(ValorTotal),
                    Moneda = Moneda,
                    CategorizacionFactura = CategorizacionFactura,
                    Observaciones = Observaciones
                };
                db.Entry(uCxPProceso).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DCXPProceso", db.CxPProceso.ToList());
                Respu = "OK";
            }
            catch (DbEntityValidationException ex)
            {
                Error.AppendLine(ex.ToString());
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Error.AppendLine("");
                    Error.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Error.AppendLine("");
                        Error.AppendFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                Respu = "ERROR: " + Error.ToString();
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult EditarPaso4(int IdCxPProceso, DateTime FechaEntrega, string NombreRecibe)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                var uCxPProceso = new CxPProceso()
                {
                    IdCxPProceso = IdCxPProceso,
                    FechaEntrega = FechaEntrega,
                    NombreRecibe = NombreRecibe
                };
                db.Entry(uCxPProceso).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DCXPProceso", db.CxPProceso.ToList());
                Respu = "OK";
            }
            catch (DbEntityValidationException ex)
            {
                Error.AppendLine(ex.ToString());
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Error.AppendLine("");
                    Error.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Error.AppendLine("");
                        Error.AppendFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                Respu = "ERROR: " + Error.ToString();
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult EditarPaso5(int IdCxPProceso, DateTime FechaRecepcion, string NHojaServicio)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                var uCxPProceso = new CxPProceso()
                {
                    IdCxPProceso = IdCxPProceso,
                    FechaRecepcion = FechaRecepcion,
                    NHojaServicio = NHojaServicio
                };
                db.Entry(uCxPProceso).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DCXPProceso", db.CxPProceso.ToList());
                Respu = "OK";
            }
            catch (DbEntityValidationException ex)
            {
                Error.AppendLine(ex.ToString());
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Error.AppendLine("");
                    Error.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Error.AppendLine("");
                        Error.AppendFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                Respu = "ERROR: " + Error.ToString();
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public JsonResult CalculoImpuesto(string TipoPersona, int IdRegimen, int IdConcepto)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                CxPRegimen mCxPRegimen = db.CxPRegimen.Find(IdRegimen);
                List<CxPImpuestos> mCxPImpuestosList = db.CxPImpuestos.Where(m => m.CxPRegimenId == IdRegimen && m.CxPConceptosId == IdConcepto).ToList();
                string sRegimen = mCxPRegimen.Codigo.Trim();
                switch (TipoPersona)
                {
                    case "NATURAL":// solo régimen simplificado y común
                        if (sRegimen == "SN" || sRegimen == "SB" || sRegimen == "SC"
                            || sRegimen == "CN" || sRegimen == "CB" || sRegimen == "CC")
                        {
                            foreach (CxPImpuestos impuesto in mCxPImpuestosList)
                            {
                                Respu += impuesto.CxPTasas.Concepto + " - " + impuesto.CxPConceptos.Descripcion + "\n";
                            }
                        }
                        break;
                    case "JURIDICA": //por defecto es régimen común
                        if (sRegimen == "CN" || sRegimen == "CB" || sRegimen == "CC")
                        {
                            foreach (CxPImpuestos impuesto in mCxPImpuestosList)
                            {
                                Respu += impuesto.CxPTasas.Concepto + " - " + impuesto.CxPConceptos.Descripcion + "\n";
                            }
                        }
                        break;
                    default: //para otros régimen como el gran contribuyente y los especiales
                        if (sRegimen == "GN" || sRegimen == "GB" || sRegimen == "GC"
                            || sRegimen == "RCA" || sRegimen == "RGA" || sRegimen == "RE")
                        {
                            Respu = "Estoy en caso de régimen gran contribuyente o de tipo especial";
                        }
                        break;
                }
            }
            catch (DbEntityValidationException ex)
            {
                Error.AppendLine(ex.ToString());
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Error.AppendLine("");
                    Error.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Error.AppendLine("");
                        Error.AppendFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                Respu = "ERROR: " + Error.ToString();
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult DSCXPProceso(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<CxPProceso>>("DCXPProceso");
            if (Data == null)
            {
                Data = new List<CxPProceso>();
            }
            return fc.DSGenerico(dm, Data);
        }
    }
}