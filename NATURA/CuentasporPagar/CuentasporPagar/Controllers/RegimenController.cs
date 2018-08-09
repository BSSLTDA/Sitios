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
    public class RegimenController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        SRGEntities db = new SRGEntities();

        // GET: Regimen
        public ActionResult Index()
        {
            List<CxPRegimen> lmCxPRegimen = db.CxPRegimen.ToList();
            SessionManager.Set("DRegimen", lmCxPRegimen);
            return View();
        }

        public ActionResult Nuevo(string Nombre)
        {
            string Respu = "";
            var nCxPRegimen = new CxPRegimen()
            {
                Nombre = Nombre
            };
            try
            {
                db.CxPRegimen.Add(nCxPRegimen);
                db.SaveChanges();
                SessionManager.Set("DRegimen", db.CxPRegimen);
                Respu = "OK";
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult Editar(int IdCxPRegimen, string Codigo, string Nombre)
        {
            string Respu = "";
            var uCxPRegimen = new CxPRegimen
            {
                IdCxPRegimen = IdCxPRegimen,
                Nombre = Nombre
            };
            try
            {
                db.Entry(uCxPRegimen).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DRegimen", db.CxPRegimen);
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
                var rCxPRegimen = db.CxPRegimen.Find(int.Parse(Id));
                db.CxPRegimen.Remove(rCxPRegimen);
                db.SaveChanges();
                SessionManager.Set("DRegimen", db.CxPRegimen);
                Respu = "OK";
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult DSRegimen(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<CxPRegimen>>("DRegimen");
            if (Data == null)
            {
                Data = new List<CxPRegimen>();
            }
            return fc.DSGenerico(dm, Data);
        }
    }
}