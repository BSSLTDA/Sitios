using CuentasporPagar.Models;
using CuentasporPagar.Models.EF;
using Syncfusion.JavaScript;
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

        public ActionResult Factura()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Factura", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }

        public ActionResult Entrega()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Entrega", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }

        public ActionResult HojaServicio()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("HojaServicio", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }

        public ActionResult GenerarImpuesto()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("GenerarImpuesto", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }

        public ActionResult GenerarDocumentoGED()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("GenerarDocumentoGED", "CXPProceso") });
            }
            List<CxPProceso> lmCxPProceso = db.CxPProceso.ToList();
            SessionManager.Set("DCXPProceso", lmCxPProceso);
            return View();
        }

        public ActionResult NuevoPaso1(string CentroCostos, string CuentaContable, string IdArea, string NitProveedor, string NombreProveedor, string EmailProveedor)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                int Area = int.Parse(IdArea);
                var nCxPProceso = new CxPProceso()
                {
                    CentroCostos = CentroCostos,
                    CuentaContable = CuentaContable,
                    IdArea = Area,
                    NitProveedor = NitProveedor,
                    NombreProveedor = NombreProveedor,
                    EmailProveedor = EmailProveedor
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

        public ActionResult EditarPaso1(int IdCxPProceso, string CentroCostos, string CuentaContable, string IdArea, string NitProveedor, string NombreProveedor, string EmailProveedor)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                int Area = int.Parse(IdArea);
                var uCxPProceso = new CxPProceso()
                {
                    IdCxPProceso = IdCxPProceso,
                    CentroCostos = CentroCostos,
                    CuentaContable = CuentaContable,
                    IdArea = Area,
                    NitProveedor = NitProveedor,
                    NombreProveedor = NombreProveedor,
                    EmailProveedor = EmailProveedor
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

        public ActionResult EditarPaso2(int IdCxPProceso, DateTime FechaMaxRadicado, string NPedidoSAP)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                var uCxPProceso = new CxPProceso()
                {
                    IdCxPProceso = IdCxPProceso,
                    FechaMaxRadicacion = FechaMaxRadicado,
                    NPedidoSAP = NPedidoSAP
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
        
        public ActionResult NuevoPaso3(DateTime FechaRadicacion, DateTime FechaFactura, string NFactura, string Valor, string ValorTotal, string Moneda, string CategorizacionFactura, string Observaciones)
        {
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
                switch (TipoPersona)
                {
                    case "NATURAL":
                        Respu = "Es natural";
                        break;
                    case "JURIDICA":
                        Respu = "Es juridica";
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