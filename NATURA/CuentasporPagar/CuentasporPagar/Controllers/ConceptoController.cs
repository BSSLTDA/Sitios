using CuentasporPagar.Models;
using CuentasporPagar.Models.EF;
using Syncfusion.JavaScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuentasporPagar.Controllers
{
    public class ConceptoController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        SRGEntities db = new SRGEntities();

        // GET: Concepto
        public ActionResult Index()
        {
            List<CxPConceptos> lmCxPConceptos = db.CxPConceptos.ToList();
            SessionManager.Set("DConcepto", lmCxPConceptos);
            return View();
        }

        public ActionResult Nuevo(string Codigo, string Nombre)
        {
            string Respu = "";
            var nCxPConcepto = new CxPConceptos()
            {
                Nombre = Nombre,
                Codigo = Codigo
            };
            try
            {
                db.CxPConceptos.Add(nCxPConcepto);
                db.SaveChanges();
                SessionManager.Set("DConcepto", db.CxPConceptos);
                Respu = "OK";
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult Editar(int IdCxPConcepto, string Codigo, string Nombre)
        {
            string Respu = "";
            var uCxPConceptos = new CxPConceptos()
            {
                Codigo = Codigo,
                IdCxPConcepto = IdCxPConcepto,
                Nombre = Nombre
            };
            try
            {
                db.Entry(uCxPConceptos).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DConcepto", db.CxPConceptos);
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
                SessionManager.Set("DConcepto", db.CxPConceptos);
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