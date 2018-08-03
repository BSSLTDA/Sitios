#region Copyright Syncfusion Inc. 2001-2017.
// Copyright Syncfusion Inc. 2001-2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CuentasporPagar.Models;

namespace CuentasporPagar.Controllers
{
    [Authorize]
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
                        LCATALOGO = "USE SRG",
                        LCRTDAT = DateTime.Now,
                        LUPDDAT = DateTime.Now,
                        LCUERPO = "http://" + ConfigurationManager.AppSettings["local"] + "/PanelAsistentes/Account/CorreoPass?tk=" + eRCAU[0].UTOKEN
                    };
                    try
                    {
                        dbIN.RMAILX.Add(nRMAILX);
                        dbIN.SaveChanges();
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
            }
            catch (Exception ex)
            {
                //res = "ERROR: " + ex.Message;
            }

            return res;
        }

        public ActionResult CambiaPass(string tk)
        {
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
            try
            {
                mRCAU = db.RCAU.Where(m => m.UTOKEN == tk).SingleOrDefault();
            }
            catch (Exception ex)
            {
                //res = "ERROR: " + ex.Message;
            }
            return View(mRCAU);
        }
    }
}