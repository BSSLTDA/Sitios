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

        public ActionResult Nuevo(string Des)
        {
            string Respu = "";
            var nPanelAsistArea = new PanelAsistArea()
            {
                Descripcion = Des
            };
            try
            {
                db.PanelAsistArea.Add(nPanelAsistArea);
                db.SaveChanges();
                SessionManager.Set("DArea", db.PanelAsistArea);
                Respu = "OK";
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult Editar(string Des, string IdArea)
        {
            string Respu = "";
            var uPanelAsistArea = new PanelAsistArea()
            {
                Descripcion = Des,
                IdPanelAsistArea = int.Parse(IdArea)
            };
            try
            {
                db.Entry(uPanelAsistArea).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DArea", db.PanelAsistArea);
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
            //var res = new List<PanelAsistArea>();
            try
            {
                var rPanelAsistArea = db.PanelAsistArea.Find(int.Parse(Id));
                db.PanelAsistArea.Remove(rPanelAsistArea);
                db.SaveChanges();
                SessionManager.Set("DArea", db.PanelAsistArea);
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