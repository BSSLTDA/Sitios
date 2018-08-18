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
            List<MdlImpuestos> lmMdlImpuestos = CargarImpuestos();
            SessionManager.Set("DImpuestos", lmMdlImpuestos);
            return View();
        }

        private List<MdlImpuestos> CargarImpuestos()
        {
            return (from p in db.CxPImpuestos
                    join p2 in db.CxPConceptos on p.CxPConceptosId equals p2.IdCxPConcepto
                    join p3 in db.CxPRegimen on p.CxPRegimenId equals p3.IdCxPRegimen
                    join p4 in db.CxPTasas on p.CxPTasasId equals p4.IdCxPTasas
                    select new MdlImpuestos()
                    {
                        IdCxPImpuesto = p.CxPIdImpuestos,
                        IdCxPConcepto = p.CxPConceptosId,
                        IdCxPRegimen = p.CxPRegimenId,
                        IdCxPTasa = p.CxPTasasId,
                        Concepto = p2.Codigo + " - " + p2.Descripcion,
                        Regimen = p3.Codigo + " - " + p3.Descripcion,
                        Tasa = p4.Codigo + " - " + p4.Concepto
                    }).ToList();
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
                SessionManager.Set("DImpuestos", CargarImpuestos());
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
                SessionManager.Set("DImpuestos", CargarImpuestos());
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
                SessionManager.Set("DImpuestos", CargarImpuestos());
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
            List<MdlImpuestos> ObjList = db.CxPConceptos.Where(m => (m.Codigo + " - " + m.Descripcion).ToUpper().Contains(Prefix.ToUpper())).Take(12).ToList().Select(s => new MdlImpuestos() { Concepto = s.Codigo + " - " + s.Descripcion, IdCxPConcepto = s.IdCxPConcepto }).ToList();
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RegimenComplete(string Prefix)
        {
            List<MdlImpuestos> ObjList = db.CxPRegimen.Where(m => (m.Codigo + " - " + m.Descripcion).ToUpper().Contains(Prefix.ToUpper())).Take(12).ToList().Select(s => new MdlImpuestos() { Regimen = s.Codigo + " - " + s.Descripcion, IdCxPRegimen = s.IdCxPRegimen }).ToList();
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TasaComplete(string Prefix)
        {
            List<MdlImpuestos> ObjList = db.CxPTasas.Where(m => (m.Codigo + " - " + m.Concepto).ToUpper().Contains(Prefix.ToUpper())).Take(12).ToList().Select(s => new MdlImpuestos() { Tasa = s.Codigo + " - " + s.Concepto, IdCxPTasa = s.IdCxPTasas}).ToList();
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DSImpuestos(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<MdlImpuestos>>("DImpuestos");
            if (Data == null)
            {
                Data = new List<MdlImpuestos>();
            }
            return fc.DSGenerico(dm, Data);
        }
    }
}