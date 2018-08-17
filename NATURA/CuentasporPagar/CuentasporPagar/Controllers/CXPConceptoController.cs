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
    public class CXPConceptoController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        SRGEntities db = new SRGEntities();

        // GET: CXPConcepto
        public ActionResult Index()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Concepto") });
            }
            List<CxPConceptos> lmCxPConceptos = db.CxPConceptos.ToList();
            SessionManager.Set("DConcepto", lmCxPConceptos);
            return View();
        }

        public ActionResult Nuevo(string Codigo, string Descripcion)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            var nCxPConcepto = new CxPConceptos()
            {
                Descripcion = Descripcion,
                Codigo = Codigo
            };
            try
            {
                db.CxPConceptos.Add(nCxPConcepto);
                db.SaveChanges();
                SessionManager.Set("DConcepto", db.CxPConceptos.ToList());
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

        public ActionResult Editar(int IdCxPConcepto, string Codigo, string Descripcion)
        {
            string Respu = "";
            var uCxPConceptos = new CxPConceptos()
            {
                Codigo = Codigo,
                IdCxPConcepto = IdCxPConcepto,
                Descripcion = Descripcion
            };
            try
            {
                db.Entry(uCxPConceptos).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DConcepto", db.CxPConceptos.ToList());
                Respu = "OK";
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult Eliminar(string Id)
        {
            string Respu = "";
            try
            {
                var rCxPConceptos = db.CxPConceptos.Find(int.Parse(Id));
                db.CxPConceptos.Remove(rCxPConceptos);
                db.SaveChanges();
                SessionManager.Set("DConcepto", db.CxPConceptos.ToList());
                Respu = "OK";
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult DSConcepto(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<CxPConceptos>>("DConcepto");
            if (Data == null)
            {
                Data = new List<CxPConceptos>();
            }
            return fc.DSGenerico(dm, Data);
        }
    }
}