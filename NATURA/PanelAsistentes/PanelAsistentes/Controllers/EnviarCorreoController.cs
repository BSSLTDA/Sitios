﻿using System;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace PanelAsistentes.Controllers
{
    public class EnviarCorreoController : Controller
    {        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string para, string asunto, string mensaje, HttpPostedFileBase fichero)
        {
            try
            {
                MailMessage correo = new MailMessage
                {
                    From = new MailAddress("JSantamariaCorreo@gmail.com", "Jesus", System.Text.Encoding.UTF8)//Correo que se usara para enviar los correos
                };
                correo.To.Add(para);
                correo.To.Add("jsantamaria.bssltda@gmail.com");
                correo.Subject = "Prueba 2 ñ";

                //correo.SubjectEncoding = System.Text.Encoding.UTF8;
                //correo.Body = body;
                //<style>.ii a[href] {color: white;text-decoration:none;}</style>
                correo.Body = "<table align='center' border='0' cellpadding='0' cellspacing='0' width='800' style='border-collapse: collapse;'>        <tbody>            <tr>                <td align='center' bgcolor='#F1A831' style='padding: 20px 0 25px 0;'>                    <table border='0' cellpadding='0' cellspacing='0' width='100%'>                        <tbody>                            <tr>                                <td>&nbsp;</td>                                <td>                                    <img src='~/Content/Img/natura.png' alt='Creating Email Magic' width='70' style='display: block;'>                                </td>                                <td style='text-align: center; color: #fff; font-family: Arial, sans-serif; font-size: 28px;'>                                    Panel de Ordenes de Compra                                </td>                            </tr>                        </tbody>                    </table>                </td>            </tr>            <tr>                <td bgcolor='#ffffff' style='padding: 40px 30px 40px 30px;'>                    <table border='0' cellpadding='0' cellspacing='0' width='100%'>                        <tbody>                            <tr>                                <td style='color: #153643; font-family: Arial, sans-serif; font-size: 24px;'><b>Solicitud de recuperar contraseña</b></td>                            </tr>                            <tr>                                <td style='padding: 20px 0 30px 0;color: #153643; font-family: Arial, sans-serif; font-size: 12px;'>                                    <p>Siga el siguiente enlace para continuar el proceso:</p><br>                                    <table border='0' cellpadding='0' cellspacing='0' width='100%'>                                        <tbody>                                            <tr valign='top'><td><b>Usuario</b></td><td>Model.UUSR</td></tr>                                            <tr valign='top'><td><b>Nombre</b></td><td>Model.UNOM</td></tr>                                        </tbody>                                    </table>                                    <br>                                </td>                            </tr>                            <tr>                                <td align='center'>                                    <table>                                        <tr>                                            <td></td>                                            <td style='text-align: center; border-radius:4px; color:#fff; background-color:#F1A831; font-family: Arial, sans-serif; width:80px;text-transform:uppercase; font-weight:600;padding:6px 12px; font-size:12px;'>                                                <table style='width:80px;border-radius:4px!important;background-color:#F1A831;float:none!important;width:80px!important;padding:0!important;text-decoration:none!important' cellspacing='0' cellpadding='0' border='0' align='center'>                                                    <tbody>                                                        <tr>                                                            <td style='line-height:16px;border-radius:4px;font-family:Helvetica,Arial,sans-serif;padding:6px 12px;text-decoration:none!important' valign='top' align='center'>                                                                <a href='http://" + System.Configuration.ConfigurationManager.AppSettings["local2"] + "/Account/CambiaPass?tk=Model.UTOKEN' target='_blank' style='font-weight:bold;color:#00d900;font-size:12px;text-decoration:none'>                                                                    <font style='font-weight:bold;color:#ffffff;font-size:12px;text-decoration:none'>REVISAR</font>                                                                </a>                                                            </td>                                                        </tr>                                                    </tbody>                                                </table>                                            </td>                                            <td></td>                                        </tr>                                    </table>                                </td>                            </tr>                        </tbody>                    </table>                </td>            </tr>            <tr>                <td bgcolor='#323232' style='padding: 30px 30px 30px 30px;'>                    <table border='0' cellpadding='0' cellspacing='0' width='100%'>                        <tbody>                            <tr>                                <td width='75%' style='color: #ffffff; font-family: Arial, sans-serif; font-size: 14px;'>                                    ® InfoNatura 2018<br>                                </td>                                <td align='right'>                                    <table border='0' cellpadding='0' cellspacing='0'>                                        <tbody>                                            <tr>                                                <td>&nbsp;</td>                                                <td style='font-size: 0; line-height: 0;' width='20'>&nbsp;</td>                                                <td>&nbsp;</td>                                            </tr>                                        </tbody>                                    </table>                                </td>                            </tr>                        </tbody>                    </table>                </td>            </tr>            <tr>                <td>                    <table style='width:80px;border-radius:4px!important;background-color:#F1A831;float:none!important;width:80px!important;padding:0!important;text-decoration:none!important' cellspacing='0' cellpadding='0' border='0' align='center'>                        <tbody>                            <tr>                                <td style='line-height:16px;border-radius:4px;font-family:Helvetica,Arial,sans-serif;padding:6px 12px;text-decoration:none!important' valign='top' align='center'>                                    <a href='http://" + System.Configuration.ConfigurationManager.AppSettings["local2"] + "/Account/CambiaPass?tk=Model.UTOKEN' target='_blank' style='font-weight:bold;color:#00d900;font-size:12px;text-decoration:none'>                                        <font style='font-weight:bold;color:#ffffff;font-size:12px;text-decoration:none'>REVISAR</font>                                    </a>                                </td>                            </tr>                        </tbody>                    </table>                </td>            </tr>        </tbody>    </table>";

                //correo.BodyEncoding = System.Text.Encoding.UTF8;
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 25,
                    EnableSsl = true,
                    UseDefaultCredentials = true
                };
                var sCuentaCorreo = "JSantamariaCorreo@gmail.com";
                var sPasswordCorreo = "JSantamaria92";
                smtp.Credentials = new System.Net.NetworkCredential(sCuentaCorreo, sPasswordCorreo);

                smtp.Send(correo);
                ViewBag.Mensaje = "Mensaje enviado correctamente";
                //System.IO.File.Delete(ruta + "\\" + fichero.FileName);
            }
            catch (SmtpException ex)
            {
                ViewBag.Error = ex.Message;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }
    }
}