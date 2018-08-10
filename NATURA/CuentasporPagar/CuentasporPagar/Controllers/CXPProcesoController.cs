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

        // GET: CPProceso
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

        public ActionResult DSConcepto(DataManager dm)
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