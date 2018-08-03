using PanelAsistentes.Models;
using PanelAsistentes.Models.EF;
using Syncfusion.JavaScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace PanelAsistentes.Controllers
{
    public class AreaController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        //string Baseurl = ConfigurationManager.AppSettings["ApiDir"];
        SRGEntities db = new SRGEntities();

        public ActionResult Index()
        {
            List<PanelAsistArea> lmPanelAsistArea = db.PanelAsistArea.ToList();
            //List<PanelAsistProceso> r = db.PanelAsistProceso;
            //var data = "{ 'ACCION': 'LEERTODO' }";
            //var resu = fc.LlamadoApi(data, Baseurl + "PanelAsistArea/CRUD");
            //var res = new List<PanelAsistArea>();
            //if (!resu.Contains("ERROR"))
            //{
            //lmPanelAsistArea = db.PanelAsistArea;
            //}
            SessionManager.Set("DArea", lmPanelAsistArea);
            return View();
        }

        public ActionResult Nuevo(string Des)
        {
            string Respu = "";
            var nPanelAsistArea = new PanelAsistArea()
            {
                Descripcion = Des
            };
            //var res = new List<PanelAsistArea>();
            try
            {
                db.PanelAsistArea.Add(nPanelAsistArea);
                db.SaveChanges();
                SessionManager.Set("DArea", db.PanelAsistArea);
                Respu = "OK";

                //var data = "{ 'ACCION': 'CREAR', 'Data': " + Newtonsoft.Json.JsonConvert.SerializeObject(nPanelAsistArea) + " }";
                //var resu = fc.LlamadoApi(data, Baseurl + "PanelAsistArea/CRUD");

                //if (!resu.Contains("ERROR"))
                //{
                //    res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PanelAsistArea>>(resu);
                //    SessionManager.Set("DArea", res);
                //    Respu = "OK";
                //}
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
            //var res = new List<PanelAsistArea>();
            try
            {
                db.Entry(uPanelAsistArea).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DArea", db.PanelAsistArea);
                Respu = "OK";

                //var data = "{ 'ACCION': 'EDITAR', 'Data': " + Newtonsoft.Json.JsonConvert.SerializeObject(uPanelAsistArea) + " }";
                //var resu = fc.LlamadoApi(data, Baseurl + "PanelAsistArea/CRUD");

                //if (!resu.Contains("ERROR"))
                //{
                //    res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PanelAsistArea>>(resu);
                //    SessionManager.Set("DArea", res);
                //    Respu = "OK";
                //}
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


                //PanelAsistArea PAA = new PanelAsistArea()
                //{
                //    IdPanelAsistArea = int.Parse(Id)
                //};
                //var data = "{ 'ACCION': 'BORRAR', 'Data': " + Newtonsoft.Json.JsonConvert.SerializeObject(PAA) + " }";
                //var resu = fc.LlamadoApi(data, Baseurl + "PanelAsistArea/CRUD");

                //if (!resu.Contains("ERROR"))
                //{
                //    res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PanelAsistArea>>(resu);
                //    SessionManager.Set("DArea", res);
                //    Respu = "OK";
                //}
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult DSArea(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<PanelAsistArea>>("DArea");
            if (Data == null)
            {
                Data = new List<PanelAsistArea>();
            }
            return fc.DSGenerico(dm, Data);
        }
    }
}