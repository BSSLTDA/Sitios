using System;
using System.Linq;
using System.Web.Mvc;
using CuentasporPagar.Models;
using CuentasporPagar.Models.EF;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text;

namespace CuentasporPagar.Controllers
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
            RCAU mRCAU = new RCAU();
            string res = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                mRCAU = db.RCAU.Where(m => m.UUSR == model.User && m.USTS == "A").SingleOrDefault();
                if (mRCAU != null)
                {
                    if (mRCAU.UPASS == model.Password)
                    {
                        SessionManager.Set("VarUsuario", mRCAU);
                        res = "OK";
                    }
                    else
                    {
                        res = "La contraseña es incorrecta";
                    }
                }
                else
                {
                    res = "Usuario no existe";
                }
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
                res = "ERROR: " + Error.ToString();
            }
            catch (Exception ex)
            {
                res = "ERROR: " + ex.ToString() + ((ex.InnerException != null) ? ex.InnerException.ToString() : "");
            }

            var tipousr = "";
            //if (resu == "OK")
            //{
            //    if (res.UACC01 == null || res.UACC01 == 1)
            //    {
            //        tipousr = "SRGUSR";
            //    }

            //    if (res.UACC02 == 1)
            //    {
            //        tipousr = "ASISTENTE";
            //    }

            //    if (res.UACC03 == 1)
            //    {
            //        tipousr = "JEFE";
            //    }

            //    if (res.UACC04 == 1)
            //    {
            //        tipousr = "JURIDICO";
            //    }
            //}


            return Json(new { result = res, redir = donde, Tipo = tipousr });
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
            string res = "";

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
                    res = "Existe mas de un usuario con el mismo correo</br>Comuniquese con tecnologia para solucionar el problema";
                }
                else
                {
                    res = "El correo no existe";
                }
            }


            return Json(new { result = res });
        }

        public List<RCAU> ValidaCorreoExistente(string Correo)
        {
            var res = new List<RCAU>();
            try
            {
                var eRCAU = db.RCAU.Where(m => m.UEMAIL == Correo).ToList();
                if (eRCAU != null)
                {
                    if (eRCAU.Count == 1)
                    {
                        eRCAU[0].UTOKEN = Guid.NewGuid().ToString();
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