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
    public class CXPAsistentesController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        SRGEntities db = new SRGEntities();

        // GET: CXPAsistentes
        public ActionResult Index()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Asistentes") });
            }
            List<CxPAsistentes> lmCxPAsistentes = db.CxPAsistentes.ToList();
            SessionManager.Set("DAsistentes", lmCxPAsistentes);
            return View();
        }

        public ActionResult Nuevo(string Nombre, string Email)
        {
            string Respu = "";
            var nCxPAsistentes = new CxPAsistentes()
            {
                Nombre = Nombre,
                Email = Email
            };
            try
            {
                db.CxPAsistentes.Add(nCxPAsistentes);
                db.SaveChanges();
                SessionManager.Set("DAsistentes", db.CxPAsistentes.ToList());
                Respu = "OK";
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult Editar(int IdCxPAsistentes, string Nombre, string Email)
        {
            string Respu = "";
            var uCxPAsistentes = new CxPAsistentes
            {
                IdCxPAsistentes = IdCxPAsistentes,
                Nombre = Nombre,
                Email = Email
            };
            try
            {
                db.Entry(uCxPAsistentes).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DAsistentes", db.CxPAsistentes.ToList());
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
                var rCxPAsistentes = db.CxPAsistentes.Find(int.Parse(Id));
                db.CxPAsistentes.Remove(rCxPAsistentes);
                db.SaveChanges();
                SessionManager.Set("DAsistentes", db.CxPAsistentes.ToList());
                Respu = "OK";
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult DSAsistentes(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<CxPAsistentes>>("DAsistentes");
            if (Data == null)
            {
                Data = new List<CxPAsistentes>();
            }
            return fc.DSGenerico(dm, Data);
        }
    }
}