using CuentasporPagar.Models;
using CuentasporPagar.Models.EF;
using CuentasporPagar.Models.Enums;
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
                CxPRegimen mCxPRegimen = db.CxPRegimen.Find(IdRegimen);
                CxPConceptos mCxPConcepto = db.CxPConceptos.Find(IdConcepto);
                List<CxPImpuestos> mCxPImpuestosList = db.CxPImpuestos.ToList();
                string sRegimen = mCxPRegimen.Codigo.Trim();
                string sConcepto = mCxPConcepto.Codigo.Trim();
                switch (TipoPersona)
                {
                    case "NATURAL":// solo régimen simplificado y común
                        switch (sRegimen)
                        {
                            case "SN": case "SB": case "SC":
                                switch (sConcepto)
                                {
                                    case "R"://Arrendamiento
                                        Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 2).Nombre;
                                        if (sRegimen == "SC") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 15).Nombre; }
                                        break;
                                    case "S"://Comerciales
                                        break;
                                    case "T"://Compras
                                        Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 4).Nombre;
                                        if (sRegimen == "SB") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 12).Nombre; }
                                        else if (sRegimen == "SC") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 13).Nombre; }
                                        break;
                                    case "U"://Financiera
                                        break;
                                    case "V"://Honorarios
                                        Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 5).Nombre;
                                        if (sRegimen == "SB") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 16).Nombre; }
                                        break;
                                    case "W"://Industriales
                                        break;
                                    case "X"://Inventario
                                        break;
                                    case "Y"://Otros
                                        break;
                                    case "Z"://Servicios
                                        Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 10).Nombre;
                                        if (sRegimen == "SB") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 17).Nombre; }
                                        else if (sRegimen == "SC") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 14).Nombre; }
                                        break;
                                }
                                break;
                            case "CN": case "CB": case "CC":
                                switch (sConcepto)
                                {
                                    case "R":
                                        Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 2).Nombre;
                                        if (sRegimen == "CC") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 15).Nombre; }
                                        break;
                                    case "S":
                                        break;
                                    case "T":
                                        Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 12).Nombre;
                                        if (sRegimen == "CB") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 12).Nombre; }
                                        else if (sRegimen == "CC") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 13).Nombre; }
                                        break;
                                    case "U":
                                        break;
                                    case "V":
                                        Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 6).Nombre;
                                        if (sRegimen == "CB") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 16).Nombre; }
                                        break;
                                    case "W":
                                        break;
                                    case "X":
                                        break;
                                    case "Y":
                                        break;
                                    case "Z":
                                        Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 9).Nombre;
                                        if (sRegimen == "CB") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 17).Nombre; }
                                        else if (sRegimen == "CC") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 14).Nombre; }
                                        break;
                                }
                                break;
                        }
                        break;
                    case "JURIDICA": //por defecto es régimen común
                        if (sRegimen == "CN" || sRegimen == "CB" || sRegimen == "CC")
                        {
                            switch (sConcepto)
                            {
                                case "R":
                                    Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 2).Nombre;
                                    if (sRegimen == "CC") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 15).Nombre; }
                                    break;
                                case "S":
                                    break;
                                case "T":
                                    Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 12).Nombre;
                                    if (sRegimen == "CB") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 12).Nombre; }
                                    else if (sRegimen == "CC") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 13).Nombre; }
                                    break;
                                case "U":
                                    break;
                                case "V":
                                    Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 6).Nombre;
                                    if (sRegimen == "CB") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 16).Nombre; }
                                    break;
                                case "W":
                                    break;
                                case "X":
                                    break;
                                case "Y":
                                    break;
                                case "Z":
                                    Respu = "Se aplica " + mCxPImpuestosList.Find(x => x.Codigo == 9).Nombre;
                                    if (sRegimen == "CB") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 17).Nombre; }
                                    else if (sRegimen == "CC") { Respu += " y " + mCxPImpuestosList.Find(x => x.Codigo == 14).Nombre; }
                                    break;
                            }
                            break;
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