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
    public class CXPImpuestosController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        SRGEntities db = new SRGEntities();

        // GET: CXPImpuestos
        public ActionResult Index()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Impuestos") });
            }
            List<CxPImpuestos> lmCxPImpuestos = db.CxPImpuestos.ToList();
            SessionManager.Set("DImpuestos", lmCxPImpuestos);
            return View();
        }

        public ActionResult Nuevo(string RegimenId, string ConceptoId, string TasaId)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                var nCxPImpuestos = new CxPImpuestos()
                {
                    CxPRegimenId = int.Parse(RegimenId.Trim()),
                    CxPConceptosId = int.Parse(ConceptoId.Trim()),
                    CxPTasasId = int.Parse(TasaId.Trim())
                };
                db.CxPImpuestos.Add(nCxPImpuestos);
                db.SaveChanges();
                SessionManager.Set("DTasas", db.CxPImpuestos.ToList());
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

        public ActionResult Editar(int IdCxPImpuestos, string RegimenId, string ConceptosId, string TasasId)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                var uCxPImpuestos = new CxPImpuestos()
                {
                    CxPIdImpuestos = IdCxPImpuestos,
                    CxPRegimenId = int.Parse(RegimenId.Trim()),
                    CxPConceptosId = int.Parse(ConceptosId.Trim()),
                    CxPTasasId = int.Parse(TasasId.Trim())
                };
                db.Entry(uCxPImpuestos).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DImpuestos", db.CxPImpuestos.ToList());
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

        public ActionResult Eliminar(string Id)
        {
            string Respu = "";
            try
            {
                var rCxPImpuestos = db.CxPImpuestos.Find(int.Parse(Id));
                db.CxPImpuestos.Remove(rCxPImpuestos);
                db.SaveChanges();
                SessionManager.Set("DImpuestos", db.CxPImpuestos.ToList());
                Respu = "OK";
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        [HttpPost]
        public JsonResult ConceptoComplete(string Prefix)
        {
            List<CxPConceptos> ObjList = db.CxPConceptos.Where(m => m.Codigo.Contains(Prefix.ToUpper())).Take(12).ToList();
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RegimenComplete(string Prefix)
        {
            List<CxPRegimen> ObjList = db.CxPRegimen.Where(m => m.Codigo.Contains(Prefix.ToUpper())).Take(12).ToList();
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TasaComplete(string Prefix)
        {
            List<CxPTasas> ObjList = db.CxPTasas.Where(m => m.Codigo.Contains(Prefix.ToUpper())).Take(12).ToList();
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DSImpuestos(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<CxPImpuestos>>("DImpuestos");
            if (Data == null)
            {
                Data = new List<CxPImpuestos>();
            }
            return fc.DSGenerico(dm, Data);
        }
    }
}