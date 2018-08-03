using Newtonsoft.Json;
using PanelAsistentes.Models;
using PanelAsistentes.Models.EF;
using Syncfusion.JavaScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace PanelAsistentes.Controllers
{
    public class UsuarioController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        //string Baseurl = ConfigurationManager.AppSettings["ApiDir"];
        List<RCAU> lmRCAU = new List<RCAU>();
        SRGEntities db = new SRGEntities();

        public ActionResult Index()
        {
            lmRCAU = GetAllUsers();
            foreach (var item in lmRCAU)
            {
                if (item.UGRP01 != null)
                {
                    if (item.UGRP01.Trim() != "")
                    {
                        item.UGRP01 = item.UGRP01 + "%";
                    }
                }
                if (item.UACC01 == 1)
                {
                    item.Tipo = "Usuario";
                }
                if (item.UACC02 == 1)
                {
                    item.Tipo = "Asistente";
                }
                if (item.UACC03 == 1)
                {
                    item.Tipo = "Jefe";
                }
            }
            SessionManager.Set("DUsuarios", lmRCAU);
            return View();
        }

        public ActionResult DSUsuario(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<RCAU>>("DUsuarios");
            if (Data == null)
            {
                Data = new List<RCAU>();
            }
            return fc.DSGenerico(dm, Data);
        }

        public JsonResult Datos()
        {
            lmRCAU = GetAllUsers();
            JsonResult jr = new JsonResult()
            {
                MaxJsonLength = Int32.MaxValue,
                Data = lmRCAU,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return jr;
        }

        public List<RCAU> GetAllUsers()
        {
            var res = new List<RCAU>();
            try
            {


                //var data = "{ 'ACCION': 'LEERTODOSACTIVOS' }";
                //var resu = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");

                //if (!resu.Contains("ERROR"))
                //{
                    res = db.RCAU.Where(m => m.USTS == "A" && ((m.UMOD == "SRGUSR" || m.UMOD == "ASISTENTE") || m.UTIPO == "POOLASISTENTE")).ToList();
                //}
            }
            catch (Exception ex)
            {
                //res = "ERROR: " + ex.Message;
            }
            return res;
        }

        public ActionResult Nuevo(string Usr, string Nme, string Eml, string Tpo, int CargaUsr)
        {
            string Respu = "";
            var nRCAU = new RCAU()
            {
                USTS = "A",
                UTIPO = "PEDIDOS",
                UMOD = "PEDIDOS",
                UFECCAD = DateTime.Now,
                UUPDDAT = DateTime.Now,
                UUSR = Usr,
                UNOM = Nme,
                UEMAIL = Eml,
                UACC01 = (Tpo == "1") ? 1 : 0, //USUARIO
                UACC02 = (Tpo == "2") ? 1 : 0, //ASISTENTE
                UACC03 = (Tpo == "3") ? 1 : 0, //JEFE
                UGRP01 = CargaUsr.ToString() + "0"
            };
            var res = new List<RCAU>();
            try
            {                
                db.RCAU.Add(nRCAU);
                db.SaveChanges();



                //var data = "{ 'ACCION': 'CREAR', 'Data': " + JsonConvert.SerializeObject(nRCAU) + " }";
                //var resu = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");

                //if (!resu.Contains("ERROR"))
                //{
                    res = db.RCAU.Where(m => m.USTS == "A" && ((m.UMOD == "SRGUSR" || m.UMOD == "ASISTENTE") || m.UTIPO == "POOLASISTENTE")).ToList();
                    foreach (var item in res)
                    {
                        if (item.UGRP01 != null)
                        {
                            if (item.UGRP01.Trim() != "")
                            {
                                item.UGRP01 = item.UGRP01 + "%";
                            }
                        }
                        if (item.UACC01 == 1)
                        {
                            item.Tipo = "Usuario";
                        }
                        if (item.UACC02 == 1)
                        {
                            item.Tipo = "Asistente";
                        }
                        if (item.UACC03 == 1)
                        {
                            item.Tipo = "Jefe";
                        }
                    }
                    SessionManager.Set("DUsuarios", res);
                    Respu = "OK";
                //}
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new {result = Respu });
        }

        public ActionResult Edita(string Usr, string Nme, string Eml, string Tpo, int CargaUsr)
        {
            string Respu = "";
            //var uRCAU = new RCAU()
            //{
            //    UUSR = Usr,
            //    UNOM = Nme,
            //    UEMAIL = Eml,
            //    UACC01 = (Tpo == "1") ? 1 : 0,
            //    UACC02 = (Tpo == "2") ? 1 : 0,
            //    UACC03 = (Tpo == "3") ? 1 : 0,
            //    UGRP01 = CargaUsr.ToString() + "0"
            //};
            var res = new List<RCAU>();
            try
            {
                var uRCAU = db.RCAU.Find(Usr);
                uRCAU.UNOM = Nme;
                uRCAU.UEMAIL = Eml;
                uRCAU.UACC01 = (Tpo == "1") ? 1 : 0;
                uRCAU.UACC02 = (Tpo == "2") ? 1 : 0;
                uRCAU.UACC03 = (Tpo == "3") ? 1 : 0;
                uRCAU.UGRP01 = CargaUsr.ToString() + "0";
                db.Entry(uRCAU).State = EntityState.Modified;
                db.SaveChanges();



                //var data = "{ 'ACCION': 'EDITAR', 'Data': " + JsonConvert.SerializeObject(uRCAU) + " }";
                //var resu = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");

                //if (!resu.Contains("ERROR"))
                //{
                    res = db.RCAU.Where(m => m.USTS == "A" && ((m.UMOD == "SRGUSR" || m.UMOD == "ASISTENTE") || m.UTIPO == "POOLASISTENTE")).ToList();
                    foreach (var item in res)
                    {
                        if (item.UGRP01 != null)
                        {
                            if (item.UGRP01.Trim() != "")
                            {
                                item.UGRP01 = item.UGRP01 + "%";
                            }
                        }
                        if (item.UACC01 == 1)
                        {
                            item.Tipo = "Usuario";
                        }
                        if (item.UACC02 == 1)
                        {
                            item.Tipo = "Asistente";
                        }
                        if (item.UACC03 == 1)
                        {
                            item.Tipo = "Jefe";
                        }
                    }
                    SessionManager.Set("DUsuarios", res);
                    Respu = "OK";
                //}
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult Elimina(string Id)
        {
            string Respu = "";
            var res = new List<RCAU>();
            try
            {
                RCAU rCAU = db.RCAU.Find(Id);


                rCAU.USTS = "Z";
                
                db.Entry(rCAU).State = EntityState.Modified;
                db.SaveChanges();



                //var data = "{ 'ACCION': 'BORRAR', 'Data': " + JsonConvert.SerializeObject(rCAU) + " }";
                //var resu = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");

                //if (!resu.Contains("ERROR"))
                //{
                    res = db.RCAU.Where(m => m.USTS == "A" && ((m.UMOD == "SRGUSR" || m.UMOD == "ASISTENTE") || m.UTIPO == "POOLASISTENTE")).ToList();
                    foreach (var item in res)
                    {
                        if (item.UGRP01 != null)
                        {
                            if (item.UGRP01.Trim() != "")
                            {
                                item.UGRP01 = item.UGRP01 + "%";
                            }
                        }
                        if (item.UACC01 == 1)
                        {
                            item.Tipo = "Usuario";
                        }
                        if (item.UACC02 == 1)
                        {
                            item.Tipo = "Asistente";
                        }
                        if (item.UACC03 == 1)
                        {
                            item.Tipo = "Jefe";
                        }
                    }
                    SessionManager.Set("DUsuarios", res);
                    Respu = "OK";
                //}
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }
    }
}