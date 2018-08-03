using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using PanelAsistentes.Models;
using PanelAsistentes.Models.EF;

namespace PanelAsistentes.Controllers
{
    public class AccountController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();        
        SRGEntities db = new SRGEntities();
        InfoNaturaEntities dbIN = new InfoNaturaEntities();

        public ActionResult Login()
        {
            return View();
        }

        public JsonResult Logueo(MdlLogin model, string returnUrl, string donde)
        {
            var res = new RCAU();
            var resu = "";

            try
            {               
                res = db.RCAU.Where(m => m.UUSR == model.User && m.USTS == "A" && (m.UTIPO == "POOLASISTENTE" || (m.UMOD == "SRGUSR" || m.UMOD == "ASISTENTE"))).SingleOrDefault();
                if (res != null)
                {
                    if (res.UPASS == model.Password)
                    {
                        //if (res.UACC03 == 1)
                        //{
                        //    res.UMOD = "JEFE";
                        //}
                        SessionManager.Set("VarUsuario", res);
                        resu = "OK";
                    }
                    else
                    {
                        resu = "La contraseña es incorrecta";
                    }                    
                }
                else
                {
                    resu = "Usuario no existe";
                }                
            }
            catch (Exception ex)
            {
                //res = "ERROR: " + ex.Message;
            }

            var tipousr = "";
            if (resu == "OK")
            {
                if (res.UACC01 == null || res.UACC01 == 1)
                {
                    tipousr = "SRGUSR";
                }

                if (res.UACC02 == 1)
                {
                    tipousr = "ASISTENTE";
                }

                if (res.UACC03 == 1)
                {
                    tipousr = "JEFE";
                }

                if (res.UACC04 == 1)
                {
                    tipousr = "JURIDICO";
                }
            }
            

            return Json(new { result = resu, redir = donde, Tipo = tipousr });
        }

        public ActionResult LogOff()
        {
            SessionManager.DelAll();
            var hostname = Request.Url;
            if (hostname.Host == "localhost")
            {
                return Redirect("~/");
            }
            else
            {
                return Redirect("http://" + hostname.Host + ":8073/PanelAsistentes");
            }

        }

        public ActionResult OlvidePass()
        {
            return View();
        }

        public ActionResult Recuperar(MdlRecuperaUsr Mdl)
        {
            if (!ModelState.IsValid)
            {
                return View(Mdl);
            }
            var res = new Respuesta();

            var eRCAU = ValidaCorreoExistente(Mdl.Email);

            if (eRCAU != null)
            {
                if (eRCAU.Count() == 1)
                {
                    var nRMAILX = new RMAILX()
                    {
                        LMOD = "PEDIDOS",
                        LREF = "Envio Correo",
                        LASUNTO = "Recuperar Contraseña",
                        LENVIA = "Pedidos SAP",
                        LPARA = Mdl.Email,
                        LBCC = "pruebas.bssltda@gmail.com",
                        //LBCC = "jsantamaria.bssltda@gmail.com",
                        LCATALOGO = "USE SRG",
                        LCRTDAT = DateTime.Now,
                        LUPDDAT = DateTime.Now,
                        LCUERPO = "http://" + ConfigurationManager.AppSettings["local"] + "/PanelAsistentes/Account/CorreoPass?tk=" + eRCAU[0].UTOKEN
                        //LCUERPO = "http://201.234.71.92/CorreoPass?tk=" + eRCAU[0].UTOKEN
                    };
                    try
                    {
                        dbIN.RMAILX.Add(nRMAILX);
                        dbIN.SaveChanges();
                        //res = "OK";

                        //var data = "{ 'ACCION': 'CREAR', 'Data': " + Newtonsoft.Json.JsonConvert.SerializeObject(nRMAILX) + " }";
                        //var resu = fc.LlamadoApi(data, Baseurl + "RMAILX/CRUD");

                        //if (!resu.Contains("ERROR"))
                        //{
                        //    res = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(resu);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //res = "ERROR: " + ex.Message;
                    }
                }
                else if (eRCAU.Count() > 1)
                {
                    res.Resultado = "Existe mas de un usuario con el mismo correo</br>Comuniquese con tecnologia para solucionar el problema";
                }
                else
                {
                    res.Resultado = "El correo no existe";
                }
            }


            return Json(new { result = res.Resultado });
        }

        public List<RCAU> ValidaCorreoExistente(string Correo)
        {
            TokenGenerator.Generator gen = new TokenGenerator.Generator();
            //var vRCAU = new RCAU()
            //{
            //    UEMAIL = Correo,
            //    UTOKEN = gen.Token()
            //};
            var res = new List<RCAU>();
            try
            {
                var eRCAU = db.RCAU.Where(m => m.UEMAIL == Correo).ToList();
                if (eRCAU != null)
                {
                    if (eRCAU.Count == 1)
                    {
                        eRCAU[0].UTOKEN = gen.Token();
                        db.Entry(eRCAU[0]).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                res = eRCAU;
                //var data = "{ 'ACCION': 'EXISTECORREO', 'Data': " + Newtonsoft.Json.JsonConvert.SerializeObject(vRCAU) + " }";
                //var resu = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");

                //if (!resu.Contains("ERROR"))
                //{
                //    res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RCAU>>(resu);
                //}
            }
            catch (Exception ex)
            {
                //res = "ERROR: " + ex.Message;
            }

            return res;
        }

        public ActionResult CambiaPass(string tk)
        {
            //tk = "mNZDPI9CHyV3egmfBp0qOUgLwBkaO4RVKrpmEHY15DLOOMOw";
            var nPss = new MdlCambioPass()
            {
                Token = tk
            };
            return View(nPss);
        }

        public ActionResult ActualizaPass(MdlCambioPass model)
        {
            string res = "";
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState.Values)
                {
                    if (item.Errors.Count() > 0)
                    {
                        foreach (var row in item.Errors)
                        {
                            res = row.ErrorMessage;
                        }
                    }
                }
                return Json(new { result = res });
            }

            try
            {
                RCAU uRCAU = db.RCAU.Where(m => m.UTOKEN == model.Token).SingleOrDefault();
                uRCAU.UPASS = model.Password;
                uRCAU.UTOKEN = "";
                db.Entry(uRCAU).State = EntityState.Modified;
                db.SaveChanges();
                res = "OK";
            }
            catch (Exception ex)
            {
                res = "ERROR: " + ex.ToString();
            }

            return Json(new { result = res });
        }

        public ActionResult CorreoPass(string tk)
        {
            RCAU mRCAU = new RCAU();
            //var tkRCAU = new RCAU()
            //{
            //    UTOKEN = tk
            //};

            //var res = new RCAU();
            try
            {
                mRCAU = db.RCAU.Where(m => m.UTOKEN == tk).SingleOrDefault();

                //var data = "{ 'ACCION': 'FINDTOKEN', 'Data': " + Newtonsoft.Json.JsonConvert.SerializeObject(tkRCAU) + " }";
                //var resu = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");

                //if (!resu.Contains("ERROR"))
                //{
                //    res = Newtonsoft.Json.JsonConvert.DeserializeObject<RCAU>(resu);
                //}
            }
            catch (Exception ex)
            {
                //res = "ERROR: " + ex.Message;
            }
            return View(mRCAU);
        }
    }
}