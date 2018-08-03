using Newtonsoft.Json;
using PanelAsistentes.Models;
using PanelAsistentes.Models.EF;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PanelAsistentes.Controllers
{
    public class ProcesoController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        List<PanelAsistProceso> lmPanelAsistProceso = new List<PanelAsistProceso>();
        //string Baseurl = ConfigurationManager.AppSettings["ApiDir"];
        SRGEntities db = new SRGEntities();
        InfoNaturaEntities dbIN = new InfoNaturaEntities();

        public ActionResult Index()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Proceso") });
            }
            lmPanelAsistProceso = GetAllProcess();

            return View();
        }

        public List<PanelAsistProceso> GetAllProcess()
        {
            try
            {
                RCAU mRCAU = SessionManager.Get<RCAU>("VarUsuario");
                if (mRCAU.UACC01 == null || mRCAU.UACC01 == 1) //USUARIO
                {
                    lmPanelAsistProceso = db.PanelAsistProceso.Where(m => m.Creador == mRCAU.UUSR && m.STS01 == 1).OrderByDescending(m => m.IdPanelAsistProceso).ToList();
                    SessionManager.Set("DSProcesosUsr", CargaTablas(lmPanelAsistProceso));
                }
                if (mRCAU.UACC02 == 1) //ASISTENTE
                {
                    lmPanelAsistProceso = db.PanelAsistProceso.Where(m => m.Asistente == mRCAU.UUSR && m.STS01 == 1 && m.STS02 == 0 && m.STS03 == 0).ToList();
                    var query2 = db.PanelAsistProceso.Where(m => m.Asistente != mRCAU.UUSR && m.STS01 == 1 && m.STS02 == 0 && m.STS03 == 0).OrderBy(m => m.Asistente).ToList();
                    foreach (var item in query2)
                    {
                        var mPanelAsistProceso = item;
                        lmPanelAsistProceso.Add(mPanelAsistProceso);
                    }
                    SessionManager.Set("DSProcesosAst", CargaTablas(lmPanelAsistProceso));
                }
                if (mRCAU.UACC03 == 1) //JEFE
                {

                    lmPanelAsistProceso = db.PanelAsistProceso.OrderByDescending(m => m.IdPanelAsistProceso).ToList();
                    SessionManager.Set("DSProcesosJefe", CargaTablas(lmPanelAsistProceso));
                }
            }
            catch (Exception ex)
            {
                //res = "ERROR: " + ex.Message;
            }
            return lmPanelAsistProceso;
        }

        private List<RFPARAM> ListCTACProceso(string prefix, string Codigo)
        {
            var res = new List<RFPARAM>();
            prefix = prefix.ToUpper();

            try
            {
                var query = from p in db.RFPARAM
                            join p2 in db.RFPARAM on p.CCCODE2 equals p2.CCCODE
                            where new { cc = p.CCTABL, ctaa = p2.CCTABL, cod = p.CCCODE } ==
                            new { cc = "CC_CTA", ctaa = "CUENTA", cod = Codigo }
                            select new MdlRFPARAM()
                            {
                                CCCODE = p.CCCODE,
                                CCDESC = p2.CCCODE + " " + p2.CCDESC,
                                CCCODE2 = p.CCCODE2
                            };
                var lista = query.ToList();
                var query2 = query.Where(m => (m.CCDESC.ToUpper()).Contains(prefix)).Take(12).ToList();
                foreach (var item in query2)
                {
                    var nRFPARAM = new RFPARAM()
                    {
                        CCCODE = item.CCCODE,
                        CCCODE2 = item.CCCODE2,
                        CCDESC = item.CCDESC
                    };
                    res.Add(nRFPARAM);
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return res;
        }

        private List<RFPARAM> ListCCProceso(string prefix)
        {
            var res = new List<RFPARAM>();
            prefix = prefix.ToUpper();

            try
            {
                var query = db.RFPARAM.Where(m => m.CCTABL == "CENTROCOSTO" && (m.CCCODE.ToUpper() + " " + m.CCDESC.ToUpper()).Contains(prefix)).Take(12).ToList();
                res = query;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return res;
        }

        [HttpPost]
        public JsonResult CentroCostoComplete(string Prefix)
        {
            List<RFPARAM> ObjList = ListCCProceso(Prefix);
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CuentaContableComplete(string Prefix, string CC)
        {
            List<RFPARAM> ObjList = ListCTACProceso(Prefix, CC.Split('-')[0].ToString().Trim());
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AreaComplete(string Prefix)
        {
            List<PanelAsistArea> ObjList = db.PanelAsistArea.Where(m => m.Descripcion.ToUpper().Contains(Prefix.ToUpper())).Take(12).ToList();
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SolicitanteComplete(string Prefix)
        {
            List<RCAU> ObjList = db.RCAU.Where(m => m.USTS == "A" && m.UMOD == "SRGUSR" && (m.UUSR.ToUpper() + " - " + m.UNOM.ToUpper()).Contains(Prefix.ToUpper())).Take(12).ToList();
            return Json(ObjList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveFile(int Id)
        {
            var res = "";
            RFADJUNTO nRFADJUNTO;
            HttpPostedFileBase file;
            string targetFolder = Server.MapPath(ConfigurationManager.AppSettings["CarpetaArchivos"]);
            res = DirectoryExists(targetFolder, true);

            try
            {

                foreach (string fileName in Request.Files)
                {
                    file = Request.Files[fileName];
                    res = file.FileName.Replace("#", "");
                    if (file != null && file.ContentLength > 0)
                    {
                        var pathFile = Path.Combine(targetFolder, res);
                        WriteFileFromStream(file.InputStream, pathFile);

                        nRFADJUNTO = new RFADJUNTO()
                        {
                            FID = "FL",
                            FACATEG01 = "PANELASISTENTE",
                            FAPROCNUM = Id,
                            FAPROCLIN = Id,
                            FADESCRIP = pathFile,
                            FACRTUSR = SessionManager.Get<RCAU>("VarUsuario").UUSR,
                            FACRTDAT = DateTime.Now,
                            FAFILE = Path.GetFileNameWithoutExtension(res),
                            FATITURL = Path.GetFileNameWithoutExtension(res),
                            FAURL = string.Format(ConfigurationManager.AppSettings["CarpetaArchivos"] + "/{0}", res)
                        };
                        db.RFADJUNTO.Add(nRFADJUNTO);
                        db.SaveChanges();
                        GetAllProcess();
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
            }

            return Json(new { Message = res });
        }

        public static string DirectoryExists(string name, bool create = false)
        {
            if (!Directory.Exists(name))
            {
                if (create)
                    Directory.CreateDirectory(name);
                return "OK";
            }
            return "";
        }

        public static void WriteFileFromStream(Stream stream, string toFile)
        {
            using (FileStream fileToSave = new FileStream(toFile, FileMode.Create))
            {
                stream.CopyTo(fileToSave);
            }
        }

        public ActionResult SaveProcess(string CentroCosto, string CuentaContable, string Nota, string Area, string DescGastos, string NmeProveedor, string NitProveedor, string MailProveedor, string Solicitante, string VlrAntesIva)
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Proceso") });
            }
            string res = "", resAsistente = "";
            string[] NitProv = NitProveedor.Split('-');
            List<RCAU> Asistentes = new List<RCAU>();

            PanelAsistProceso nPanelAsistProceso = new PanelAsistProceso()
            {
                FechaCrea = DateTime.Now,
                FechaCierra = null,
                Creador = SessionManager.Get<RCAU>("VarUsuario").UUSR,
                Solicitante = Solicitante,
                Asistente = " ",
                IdPanelAsistArea = int.Parse(Area),
                CodCentroCosto = CentroCosto,
                CodCuentaContable = CuentaContable,
                NotaSolicitante = Nota,
                DescripcionGastos = DescGastos,
                VlrAntesIVA = double.Parse(VlrAntesIva == "" ? "0" : VlrAntesIva.Replace(".", "").Replace(",", "").Replace("$", "")),
                NombreProveedor = NmeProveedor,
                NitProveedor = NitProveedor.Replace(".", ""),
                CorreoProveedor = MailProveedor,
                STS01 = 1
            };
            db.PanelAsistProceso.Add(nPanelAsistProceso);
            db.SaveChanges();
            Proveedor mProveedordb = new Proveedor();
            Proveedor mProveedor = new Proveedor()
            {
                Nit = NitProv[0].Replace(".", ""),
                DV = (NitProv.Count() == 2) ? (NitProv[1].Length > 1) ? NitProv[1].Substring(0, 1) : NitProv[1] : "",
                Nombre = NmeProveedor,
                Correo = MailProveedor
            };
            mProveedordb = db.Proveedor.Where(m => m.Nit == mProveedor.Nit).SingleOrDefault();
            if (mProveedordb == null)
            {
                db.Proveedor.Add(mProveedor);
                db.SaveChanges();
            }
            else
            {
                mProveedordb.Nombre = mProveedor.Nombre;
                mProveedordb.Correo = mProveedor.Correo;
                mProveedordb.DV = (mProveedor.DV != "") ? mProveedor.DV : mProveedordb.DV;
                db.Entry(mProveedordb).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<PanelAsistProceso> luPanelAsistProceso = db.PanelAsistProceso.Where(m => m.NitProveedor.Contains(mProveedor.Nit)).ToList();
            luPanelAsistProceso = luPanelAsistProceso.Select(c => { c.NombreProveedor = mProveedor.Nombre; c.CorreoProveedor = mProveedor.Correo; return c; }).ToList();
            db.SaveChanges();

            Asistentes = db.RCAU.Where(m => m.UACC02 == 1 && m.USTS == "A").ToList();

            //ALGORITMO CARGA
            res = AsignaCarga(Asistentes, ref nPanelAsistProceso);

            if (res == "OK")
            {
                string para = "";
                if (nPanelAsistProceso.Creador.Trim() != nPanelAsistProceso.Asistente.Trim())
                {
                    para = nPanelAsistProceso.Creador + "," + nPanelAsistProceso.Asistente;
                    if (!para.Contains(nPanelAsistProceso.Solicitante))
                    {
                        para += "," + nPanelAsistProceso.Solicitante;
                    }
                }
                else
                {
                    para = nPanelAsistProceso.Creador;
                    if (!para.Contains(nPanelAsistProceso.Solicitante))
                    {
                        para += "," + nPanelAsistProceso.Solicitante;
                    }
                }

                RMAILX nRMAILX = new RMAILX()
                {
                    LKEY = nPanelAsistProceso.IdPanelAsistProceso.ToString(),
                    LMOD = "PEDIDOS",
                    LREF = "Envio Correo",
                    LASUNTO = "Solicitud Creada " + nPanelAsistProceso.IdPanelAsistProceso,
                    LENVIA = "Pedidos SAP",
                    LPARA = para,
                    LBCC = "pruebas.bssltda@gmail.com",
                    LCRTDAT = DateTime.Now,
                    LUPDDAT = DateTime.Now,
                    //LBCC = "jsantamaria.bssltda@gmail.com",
                    LCATALOGO = "USE SRG",
                    LCUERPO = "http://" + ConfigurationManager.AppSettings["local"] + "/PanelAsistentes/Proceso/CorreoProceso?Id=" + nPanelAsistProceso.IdPanelAsistProceso
                    //LCUERPO = "http://201.234.71.92/CorreoProceso?Id=" + Proceso.IdPanelAsistProceso
                };
                dbIN.RMAILX.Add(nRMAILX);
                dbIN.SaveChanges();
                res = "OK";
                GetAllProcess();
            }

            if (Asistentes.Count > 0)
            {
                resAsistente = Asistentes.Where(m => m.UNOM.Trim() == nPanelAsistProceso.Asistente.Trim()).Single().UNOM;
            }

            return Json(new { result = res, idp = nPanelAsistProceso.IdPanelAsistProceso, asi = resAsistente }, JsonRequestBehavior.AllowGet);
        }

        private List<PanelAsistCarga> CargaHoy()
        {
            var res = new List<PanelAsistCarga>();
            //(CONVERT(DATE, Fecha) = CONVERT(DATE, '2018-01-31'))

            var datos = db.PanelAsistCarga.ToList();
            var query = datos.Where(m => DateTime.Compare(m.Fecha.Value.Date, DateTime.Now.Date) == 0 && (m.Carga01.Trim() == "1" || m.Carga02.Trim() == "1" || m.Carga03.Trim() == "1" || m.Carga04.Trim() == "1" || m.Carga05.Trim() == "1" || m.Carga06.Trim() == "1" || m.Carga07.Trim() == "1" || m.Carga08.Trim() == "1" || m.Carga09.Trim() == "1" || m.Carga10.Trim() == "1"));

            try
            {
                res = query.ToList();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return res;
        }

        private string AsignaCarga(List<RCAU> lmAsistentes, ref PanelAsistProceso mProceso)
        {
            string res = "", data = "";
            bool Hecho = false;
            List<PanelAsistCarga> lmAsistCarga = new List<PanelAsistCarga>();
            PanelAsistCarga mAsistCarga = new PanelAsistCarga();
            PanelAsistCarga nPanelAsistCarga;

            lmAsistentes = lmAsistentes.OrderBy(x => Guid.NewGuid()).ToList();

            lmAsistCarga = ObtenerCargas(lmAsistentes);

            if (lmAsistCarga.Count == 0)
            {
                foreach (var row in lmAsistentes.Select((value, i) => new { i, value }))
                {
                    nPanelAsistCarga = new PanelAsistCarga()
                    {
                        Orden = row.i + 1,
                        Fecha = DateTime.Now,
                        Asistente = row.value.UUSR,
                        Carga01 = (int.Parse(row.value.UGRP01 ?? "0") >= 10) ? "1" : "0",
                        Carga02 = (int.Parse(row.value.UGRP01 ?? "0") >= 20) ? "1" : "0",
                        Carga03 = (int.Parse(row.value.UGRP01 ?? "0") >= 30) ? "1" : "0",
                        Carga04 = (int.Parse(row.value.UGRP01 ?? "0") >= 40) ? "1" : "0",
                        Carga05 = (int.Parse(row.value.UGRP01 ?? "0") >= 50) ? "1" : "0",
                        Carga06 = (int.Parse(row.value.UGRP01 ?? "0") >= 60) ? "1" : "0",
                        Carga07 = (int.Parse(row.value.UGRP01 ?? "0") >= 70) ? "1" : "0",
                        Carga08 = (int.Parse(row.value.UGRP01 ?? "0") >= 80) ? "1" : "0",
                        Carga09 = (int.Parse(row.value.UGRP01 ?? "0") >= 90) ? "1" : "0",
                        Carga10 = (int.Parse(row.value.UGRP01 ?? "0") == 100) ? "1" : "0"
                    };
                    //CREA CARGA
                    db.PanelAsistCarga.Add(nPanelAsistCarga);
                    db.SaveChanges();

                    //data = "{ 'ACCION': 'CREAR', 'Data': " + JsonConvert.SerializeObject(nPanelAsistCarga) + " }";
                    //res = fc.LlamadoApi(data, Baseurl + "PanelAsistCarga/CRUD");

                    //if (!res.Contains("ERROR"))
                    //{
                    lmAsistCarga = CargaHoy();
                    //}
                }

                //Hallando minimo Cargas
                Hecho = AsignaSegunCarga(MinimoCarga(lmAsistCarga), ref mAsistCarga, ref mProceso, lmAsistCarga);

                if (Hecho)
                {
                    //ACTUALIZA CARGA
                    db.Entry(mAsistCarga).State = EntityState.Modified;
                    db.SaveChanges();

                    db.Entry(mProceso).State = EntityState.Modified;
                    db.SaveChanges();

                    if (mProceso.Creador != null)
                    {
                        string cread = mProceso.Creador;
                        SessionManager.Set("NuevosProcesos", db.PanelAsistProceso.Where(m => m.Creador == cread && m.STS01 == 1).OrderByDescending(m => m.IdPanelAsistProceso).ToList());
                    }
                    else
                    {
                        SessionManager.Set("NuevosProcesos", db.PanelAsistProceso.OrderByDescending(m => m.IdPanelAsistProceso).ToList());
                    }

                    res = "OK";

                    //data = "{ 'ACCION': 'UpdCarga', 'Data': " + JsonConvert.SerializeObject(mAsistCarga) + " }";
                    //res = fc.LlamadoApi(data, Baseurl + "PanelAsistCarga/CRUD");

                    //if (!res.Contains("ERROR"))
                    //{
                    //    data = "{ 'ACCION': 'UpdProceso', 'Data': " + JsonConvert.SerializeObject(mProceso) + " }";
                    //    res = fc.LlamadoApi(data, Baseurl + "PanelAsistProceso/CRUD");
                    //    if (!res.Contains("ERROR"))
                    //    {
                    //        SessionManager.Set("NuevosProcesos", JsonConvert.DeserializeObject<List<PanelAsistProceso>>(res));
                    //        res = "OK";
                    //    }
                    //}
                }
            }
            else
            {
                //Hallando minimo Cargas
                Hecho = AsignaSegunCarga(MinimoCarga(lmAsistCarga), ref mAsistCarga, ref mProceso, lmAsistCarga);

                if (Hecho)
                {
                    //ACTUALIZA CARGA
                    db.Entry(mAsistCarga).State = EntityState.Modified;
                    db.SaveChanges();

                    db.Entry(mProceso).State = EntityState.Modified;
                    db.SaveChanges();

                    if (mProceso.Creador != null)
                    {
                        string cread = mProceso.Creador;
                        SessionManager.Set("NuevosProcesos", db.PanelAsistProceso.Where(m => m.Creador == cread && m.STS01 == 1).OrderByDescending(m => m.IdPanelAsistProceso).ToList());
                    }
                    else
                    {
                        SessionManager.Set("NuevosProcesos", db.PanelAsistProceso.OrderByDescending(m => m.IdPanelAsistProceso).ToList());
                    }

                    res = "OK";

                    //data = "{ 'ACCION': 'UpdCarga', 'Data': " + JsonConvert.SerializeObject(mAsistCarga) + " }";
                    //res = fc.LlamadoApi(data, Baseurl + "PanelAsistCarga/CRUD");

                    //if (!res.Contains("ERROR"))
                    //{
                    //    data = "{ 'ACCION': 'UpdProceso', 'Data': " + JsonConvert.SerializeObject(mProceso) + " }";
                    //    res = fc.LlamadoApi(data, Baseurl + "PanelAsistProceso/CRUD");
                    //    if (!res.Contains("ERROR"))
                    //    {
                    //        SessionManager.Set("NuevosProcesos", CargaTablas(JsonConvert.DeserializeObject<List<PanelAsistProceso>>(res)));
                    //        res = "OK";
                    //    }
                    //}
                }
            }

            return res;
        }

        private bool AsignaSegunCarga(Dictionary<string, string> Cargas, ref PanelAsistCarga mAsistCarga, ref PanelAsistProceso mProceso, List<PanelAsistCarga> lmAsistCarga)
        {
            RCAU mRCAU = new RCAU();
            var Hecho = false;
            var C1 = Cargas.Where(m => m.Key == "C1").SingleOrDefault().Value;
            var C2 = Cargas.Where(m => m.Key == "C2").SingleOrDefault().Value;
            var C3 = Cargas.Where(m => m.Key == "C3").SingleOrDefault().Value;
            var C4 = Cargas.Where(m => m.Key == "C4").SingleOrDefault().Value;
            var C5 = Cargas.Where(m => m.Key == "C5").SingleOrDefault().Value;
            var C6 = Cargas.Where(m => m.Key == "C6").SingleOrDefault().Value;
            var C7 = Cargas.Where(m => m.Key == "C7").SingleOrDefault().Value;
            var C8 = Cargas.Where(m => m.Key == "C8").SingleOrDefault().Value;
            var C9 = Cargas.Where(m => m.Key == "C9").SingleOrDefault().Value;
            var C10 = Cargas.Where(m => m.Key == "C10").SingleOrDefault().Value;

            if (C1 != null)
            {
                //mRCAU.UUSR = C1;
                //string data = "{ 'ACCION': 'VALIDAASISTENTE', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                //string res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");
                //if (!res.Contains("ERROR"))
                //{
                mRCAU = db.RCAU.Where(m => m.UUSR == C1 && m.UACC02 == 1).SingleOrDefault();
                //}
                if (mRCAU != null)
                {
                    mAsistCarga = lmAsistCarga.Where(m => m.Asistente.Trim() == C1).Single();
                    mAsistCarga.Carga01 = mProceso.IdPanelAsistProceso.ToString();
                    mProceso.Asistente = C1;
                    Hecho = true;
                }
            }

            if (!Hecho)
            {
                if (C2 != null)
                {
                    mRCAU = db.RCAU.Where(m => m.UUSR == C2 && m.UACC02 == 1).SingleOrDefault();
                    //mRCAU.UUSR = C2;
                    //string data = "{ 'ACCION': 'VALIDAASISTENTE', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                    //string res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");
                    //if (!res.Contains("ERROR"))
                    //{
                    //    mRCAU = JsonConvert.DeserializeObject<RCAU>(res);
                    //}
                    if (mRCAU != null)
                    {
                        mAsistCarga = lmAsistCarga.Where(m => m.Asistente.Trim() == C2).Single();
                        mAsistCarga.Carga02 = mProceso.IdPanelAsistProceso.ToString();
                        mProceso.Asistente = C2;
                        Hecho = true;
                    }
                }
            }

            if (!Hecho)
            {
                if (C3 != null)
                {
                    mRCAU = db.RCAU.Where(m => m.UUSR == C3 && m.UACC02 == 1).SingleOrDefault();
                    //mRCAU.UUSR = C3;
                    //string data = "{ 'ACCION': 'VALIDAASISTENTE', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                    //string res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");
                    //if (!res.Contains("ERROR"))
                    //{
                    //    mRCAU = JsonConvert.DeserializeObject<RCAU>(res);
                    //}
                    if (mRCAU != null)
                    {
                        mAsistCarga = lmAsistCarga.Where(m => m.Asistente.Trim() == C3).Single();
                        mAsistCarga.Carga03 = mProceso.IdPanelAsistProceso.ToString();
                        mProceso.Asistente = C3;
                        Hecho = true;
                    }
                }
            }

            if (!Hecho)
            {
                if (C4 != null)
                {
                    mRCAU = db.RCAU.Where(m => m.UUSR == C4 && m.UACC02 == 1).SingleOrDefault();
                    //mRCAU.UUSR = C4;
                    //string data = "{ 'ACCION': 'VALIDAASISTENTE', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                    //string res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");
                    //if (!res.Contains("ERROR"))
                    //{
                    //    mRCAU = JsonConvert.DeserializeObject<RCAU>(res);
                    //}
                    if (mRCAU != null)
                    {
                        mAsistCarga = lmAsistCarga.Where(m => m.Asistente.Trim() == C4).Single();
                        mAsistCarga.Carga04 = mProceso.IdPanelAsistProceso.ToString();
                        mProceso.Asistente = C4;
                        Hecho = true;
                    }
                }
            }

            if (!Hecho)
            {
                if (C5 != null)
                {
                    mRCAU = db.RCAU.Where(m => m.UUSR == C5 && m.UACC02 == 1).SingleOrDefault();
                    //mRCAU.UUSR = C5;
                    //string data = "{ 'ACCION': 'VALIDAASISTENTE', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                    //string res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");
                    //if (!res.Contains("ERROR"))
                    //{
                    //    mRCAU = JsonConvert.DeserializeObject<RCAU>(res);
                    //}
                    if (mRCAU != null)
                    {
                        mAsistCarga = lmAsistCarga.Where(m => m.Asistente.Trim() == C5).Single();
                        mAsistCarga.Carga05 = mProceso.IdPanelAsistProceso.ToString();
                        mProceso.Asistente = C5;
                        Hecho = true;
                    }
                }
            }

            if (!Hecho)
            {
                if (C6 != null)
                {
                    mRCAU = db.RCAU.Where(m => m.UUSR == C6 && m.UACC02 == 1).SingleOrDefault();
                    //mRCAU.UUSR = C6;
                    //string data = "{ 'ACCION': 'VALIDAASISTENTE', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                    //string res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");
                    //if (!res.Contains("ERROR"))
                    //{
                    //    mRCAU = JsonConvert.DeserializeObject<RCAU>(res);
                    //}
                    if (mRCAU != null)
                    {
                        mAsistCarga = lmAsistCarga.Where(m => m.Asistente.Trim() == C6).Single();
                        mAsistCarga.Carga06 = mProceso.IdPanelAsistProceso.ToString();
                        mProceso.Asistente = C6;
                        Hecho = true;
                    }
                }
            }

            if (!Hecho)
            {
                if (C7 != null)
                {
                    mRCAU = db.RCAU.Where(m => m.UUSR == C7 && m.UACC02 == 1).SingleOrDefault();
                    //mRCAU.UUSR = C7;
                    //string data = "{ 'ACCION': 'VALIDAASISTENTE', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                    //string res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");
                    //if (!res.Contains("ERROR"))
                    //{
                    //    mRCAU = JsonConvert.DeserializeObject<RCAU>(res);
                    //}
                    if (mRCAU != null)
                    {
                        mAsistCarga = lmAsistCarga.Where(m => m.Asistente.Trim() == C7).Single();
                        mAsistCarga.Carga07 = mProceso.IdPanelAsistProceso.ToString();
                        mProceso.Asistente = C7;
                        Hecho = true;
                    }
                }
            }

            if (!Hecho)
            {
                if (C8 != null)
                {
                    mRCAU = db.RCAU.Where(m => m.UUSR == C8 && m.UACC02 == 1).SingleOrDefault();
                    //mRCAU.UUSR = C8;
                    //string data = "{ 'ACCION': 'VALIDAASISTENTE', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                    //string res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");
                    //if (!res.Contains("ERROR"))
                    //{
                    //    mRCAU = JsonConvert.DeserializeObject<RCAU>(res);
                    //}
                    if (mRCAU != null)
                    {
                        mAsistCarga = lmAsistCarga.Where(m => m.Asistente.Trim() == C8).Single();
                        mAsistCarga.Carga08 = mProceso.IdPanelAsistProceso.ToString();
                        mProceso.Asistente = C8;
                        Hecho = true;
                    }
                }
            }

            if (!Hecho)
            {
                if (C9 != null)
                {
                    mRCAU = db.RCAU.Where(m => m.UUSR == C9 && m.UACC02 == 1).SingleOrDefault();
                    //mRCAU.UUSR = C9;
                    //string data = "{ 'ACCION': 'VALIDAASISTENTE', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                    //string res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");
                    //if (!res.Contains("ERROR"))
                    //{
                    //    mRCAU = JsonConvert.DeserializeObject<RCAU>(res);
                    //}
                    if (mRCAU != null)
                    {
                        mAsistCarga = lmAsistCarga.Where(m => m.Asistente.Trim() == C9).Single();
                        mAsistCarga.Carga09 = mProceso.IdPanelAsistProceso.ToString();
                        mProceso.Asistente = C9;
                        Hecho = true;
                    }
                }
            }

            if (!Hecho)
            {
                if (C10 != null)
                {
                    mRCAU = db.RCAU.Where(m => m.UUSR == C10 && m.UACC02 == 1).SingleOrDefault();
                    //mRCAU.UUSR = C10;
                    //string data = "{ 'ACCION': 'VALIDAASISTENTE', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                    //string res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");
                    //if (!res.Contains("ERROR"))
                    //{
                    //    mRCAU = JsonConvert.DeserializeObject<RCAU>(res);
                    //}
                    if (mRCAU != null)
                    {
                        mAsistCarga = lmAsistCarga.Where(m => m.Asistente.Trim() == C10).Single();
                        mAsistCarga.Carga10 = mProceso.IdPanelAsistProceso.ToString();
                        mProceso.Asistente = C10;
                        Hecho = true;
                    }
                }
            }
            return Hecho;
        }

        private Dictionary<string, string> MinimoCarga(List<PanelAsistCarga> PlmPanelAsistCarga)
        {
            //Dictionary(Carga, Asistente)
            var CA = new Dictionary<string, string>();

            var lC1 = PlmPanelAsistCarga.Where(m => m.Carga01 == "1").ToList();
            var lC2 = PlmPanelAsistCarga.Where(m => m.Carga02 == "1").ToList();
            var lC3 = PlmPanelAsistCarga.Where(m => m.Carga03 == "1").ToList();
            var lC4 = PlmPanelAsistCarga.Where(m => m.Carga04 == "1").ToList();
            var lC5 = PlmPanelAsistCarga.Where(m => m.Carga05 == "1").ToList();
            var lC6 = PlmPanelAsistCarga.Where(m => m.Carga06 == "1").ToList();
            var lC7 = PlmPanelAsistCarga.Where(m => m.Carga07 == "1").ToList();
            var lC8 = PlmPanelAsistCarga.Where(m => m.Carga08 == "1").ToList();
            var lC9 = PlmPanelAsistCarga.Where(m => m.Carga09 == "1").ToList();
            var lC10 = PlmPanelAsistCarga.Where(m => m.Carga10 == "1").ToList();

            if (lC1.Count > 0)
            {
                CA.Add("C1", lC1[0].Asistente.Trim());
            }
            if (lC2.Count > 0)
            {
                CA.Add("C2", lC2[0].Asistente.Trim());
            }
            if (lC3.Count > 0)
            {
                CA.Add("C3", lC3[0].Asistente.Trim());
            }
            if (lC4.Count > 0)
            {
                CA.Add("C4", lC4[0].Asistente.Trim());
            }
            if (lC5.Count > 0)
            {
                CA.Add("C5", lC5[0].Asistente.Trim());
            }
            if (lC6.Count > 0)
            {
                CA.Add("C6", lC6[0].Asistente.Trim());
            }
            if (lC7.Count > 0)
            {
                CA.Add("C7", lC7[0].Asistente.Trim());
            }
            if (lC8.Count > 0)
            {
                CA.Add("C8", lC8[0].Asistente.Trim());
            }
            if (lC9.Count > 0)
            {
                CA.Add("C9", lC9[0].Asistente.Trim());
            }
            if (lC10.Count > 0)
            {
                CA.Add("C10", lC10[0].Asistente.Trim());
            }

            return CA;
        }

        public ActionResult Usuario()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Usuario", "Proceso") });
            }
            GetAllProcess();

            return View();
        }

        public ActionResult CorreoProceso(string Id)
        {
            var res = "";
            PanelAsistProceso mProceso = new PanelAsistProceso();
            //var mProceso = new PanelAsistProceso()
            //{
            //    IdPanelAsistProceso = int.Parse(Id)
            //};

            try
            {


                //var data = "{ 'ACCION': 'ENCUENTRAPROCESO', 'Data': " + JsonConvert.SerializeObject(mProceso) + " }";
                //res = fc.LlamadoApi(data, Baseurl + "PanelAsistProceso/CRUD");

                //if (!res.Contains("ERROR"))
                //{
                mProceso = db.PanelAsistProceso.Find(int.Parse(Id));
                //mProceso.FechaRadicacion = mProceso.FechaMaximaRadicacion?.ToString("yyyy-MM-dd");
                RCAU mRCAU = db.RCAU.Find(mProceso.Asistente);
                //var mRCAU = new RCAU()
                //{
                //    UUSR = mProceso.Asistente
                //};
                //data = "{ 'ACCION': 'ENCUENTRAUSUARIO', 'Data': " + JsonConvert.SerializeObject(mRCAU) + " }";
                //res = fc.LlamadoApi(data, Baseurl + "RCAU/CRUD");

                //if (!res.Contains("ERROR"))
                //{
                //    mRCAU = JsonConvert.DeserializeObject<RCAU>(res);
                if (mRCAU != null)
                {
                    mProceso.Asistente = mRCAU.UNOM.Trim();
                }

                //var mCentroCosto = new RFPARAM()
                //{
                //    CCCODE = mProceso.CodCentroCosto.ToString()
                //};
                RFPARAM mCentroCosto = db.RFPARAM.Where(m => m.CCCODE == mProceso.CodCentroCosto.ToString() && m.CCTABL == "CENTROCOSTO").SingleOrDefault();

                //data = "{ 'ACCION': 'ENCUENTRACC', 'Data': " + JsonConvert.SerializeObject(mCentroCosto) + " }";
                //res = fc.LlamadoApi(data, Baseurl + "RFPARAM/CRUD");
                //if (!res.Contains("ERROR"))
                //{
                //    mCentroCosto = JsonConvert.DeserializeObject<RFPARAM>(res);
                if (mCentroCosto != null)
                {
                    mProceso.CodCentroCosto = mCentroCosto.CCCODE.ToString() + " - " + mCentroCosto.CCDESC;
                }
                //}

                RFPARAM mCtaContable = db.RFPARAM.Where(m => m.CCCODE == mProceso.CodCuentaContable.ToString() && m.CCTABL == "CUENTA").SingleOrDefault();
                //{
                //    CCCODE = mProceso.CodCuentaContable.ToString()
                //};

                //data = "{ 'ACCION': 'ENCUENTRACTAC', 'Data': " + JsonConvert.SerializeObject(mCtaContable) + " }";
                //res = fc.LlamadoApi(data, Baseurl + "RFPARAM/CRUD");
                //if (!res.Contains("ERROR"))
                //{
                //    mCtaContable = JsonConvert.DeserializeObject<RFPARAM>(res);
                if (mCtaContable != null)
                {
                    mProceso.CodCuentaContable = mCtaContable.CCCODE.ToString() + " - " + mCtaContable.CCDESC;
                }
                //}
                //}
                //}
            }
            catch (Exception ex)
            {
                res = "ERROR: " + ex.Message;
            }
            return View(mProceso);
        }

        public ActionResult Asistente()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Asistente", "Proceso") });
            }

            GetAllProcess();

            return View();
        }

        private List<PanelAsistCarga> ObtenerCargas(List<RCAU> ListaAsistentes)
        {
            List<PanelAsistCarga> lmPanelAsistCargas = new List<PanelAsistCarga>();

            lmPanelAsistCargas = CargaHoy();

            if (lmPanelAsistCargas.Count() != 0)
            {
                //VALIDA QUE TODAS LAS ASISTENTES ESTEN EN CARGA
                foreach (RCAU item in ListaAsistentes)
                {
                    PanelAsistCarga cargav = lmPanelAsistCargas.Where(m => m.Asistente == item.UUSR).SingleOrDefault();
                    //SI NO EXISTE CARGA LA CREA
                    if (cargav == null)
                    {
                        var nPanelAsistCarga = new PanelAsistCarga()
                        {
                            Orden = lmPanelAsistCargas.Count() + 1,
                            Fecha = DateTime.Now,
                            Asistente = item.UUSR,
                            Carga01 = (int.Parse(item.UGRP01 ?? "0") >= 10) ? "1" : "0",
                            Carga02 = (int.Parse(item.UGRP01 ?? "0") >= 20) ? "1" : "0",
                            Carga03 = (int.Parse(item.UGRP01 ?? "0") >= 30) ? "1" : "0",
                            Carga04 = (int.Parse(item.UGRP01 ?? "0") >= 40) ? "1" : "0",
                            Carga05 = (int.Parse(item.UGRP01 ?? "0") >= 50) ? "1" : "0",
                            Carga06 = (int.Parse(item.UGRP01 ?? "0") >= 60) ? "1" : "0",
                            Carga07 = (int.Parse(item.UGRP01 ?? "0") >= 70) ? "1" : "0",
                            Carga08 = (int.Parse(item.UGRP01 ?? "0") >= 80) ? "1" : "0",
                            Carga09 = (int.Parse(item.UGRP01 ?? "0") >= 90) ? "1" : "0",
                            Carga10 = (int.Parse(item.UGRP01 ?? "0") == 100) ? "1" : "0"
                        };
                        //CREA CARGA
                        db.PanelAsistCarga.Add(nPanelAsistCarga);
                        db.SaveChanges();

                        lmPanelAsistCargas = CargaHoy();

                    }
                }
            }

            return lmPanelAsistCargas;
        }

        private List<PanelAsistProceso> CargaTablas(List<PanelAsistProceso> Pap)
        {
            var lmRCAU = db.RCAU;
            RCAU solicita, Asiste, crea;
            PanelAsistArea area;
            RFPARAM cc;
            RFPARAM ctac;

            var lmRFPARAMCTA = db.RFPARAM.Where(m => m.CCTABL == "CUENTA");

            var lmRFPARAMCC = db.RFPARAM.Where(m => m.CCTABL == "CENTROCOSTO");

            var lmPanelAsistArea = db.PanelAsistArea;

            var lmRFADJUNTO = db.RFADJUNTO.Where(m => m.FACATEG01 == "PANELASISTENTE").ToList();

            foreach (var item in Pap)
            {
                if (item.STS02 == 1 || item.STS03 == 1)
                {
                    item.NotaSolicitante = item.NotaAprobador;
                }
                if (item.STS01 == 1)
                {
                    item.Estado = "Pendiente";
                }
                if (item.STS02 == 1)
                {
                    item.Estado = "Procesada";
                }
                if (item.STS03 == 1)
                {
                    item.Estado = "Rechazada";
                }
                if (item.STS04 == 1)
                {
                    item.Estado = "Correo Enviado";
                }
                if (item.STS05 == 1)
                {
                    item.Estado = "Reenviando Correo";
                }
                solicita = lmRCAU.Where(m => m.UUSR.Trim() == item.Solicitante.Trim()).SingleOrDefault();
                if (solicita != null)
                {
                    item.Solicitante = solicita.UNOM.Trim();
                }
                Asiste = lmRCAU.Where(m => m.UUSR.Trim() == item.Asistente.Trim()).SingleOrDefault();
                if (Asiste != null)
                {
                    item.Asistente = Asiste.UNOM.Trim();
                }
                crea = lmRCAU.Where(m => m.UUSR.Trim() == item.Creador.Trim()).SingleOrDefault();
                if (crea != null)
                {
                    item.Creador = crea.UNOM.Trim();
                }
                area = lmPanelAsistArea.Where(m => m.IdPanelAsistArea == item.IdPanelAsistArea).SingleOrDefault();
                if (area != null)
                {
                    item.Area = area.Descripcion;
                }
                cc = lmRFPARAMCC.Where(m => m.CCCODE == item.CodCentroCosto.ToString()).SingleOrDefault();
                if (cc != null)
                {
                    item.CodCentroCosto = cc.CCCODE + "-" + cc.CCDESC;
                }
                ctac = lmRFPARAMCTA.Where(m => m.CCCODE.Trim() == item.CodCuentaContable.Trim()).SingleOrDefault();
                if (ctac != null)
                {
                    item.CodCuentaContable = ctac.CCCODE + "-" + ctac.CCDESC;
                }
                item.Adjunto = null;
                item.AdjuntoNme = null;
                var lRFADJU = lmRFADJUNTO.Where(m => m.FAPROCNUM == item.IdPanelAsistProceso).ToList();
                if (lRFADJU != null)
                {
                    if (lRFADJU.Count() > 0)
                    {
                        foreach (var adju in lRFADJU)
                        {
                            if (item.Adjunto == null)
                            {
                                item.Adjunto = adju.FAURL.Replace("~", "..");
                                item.AdjuntoNme = adju.FAFILE;
                            }
                            else
                            {
                                item.Adjunto += "|" + adju.FAURL.Replace("~", "..");
                                item.AdjuntoNme += "|" + adju.FAFILE;
                            }
                        }
                    }
                }
            }

            return Pap;
        }

        private List<PanelAsistProceso> ListProcesoAsistente(string Usr)
        {
            var res = new List<PanelAsistProceso>();

            try
            {
                res = db.PanelAsistProceso.Where(m => m.Asistente == Usr && m.STS01 == 1 && m.STS02 == 0 && m.STS03 == 0).ToList();
                var query2 = db.PanelAsistProceso.Where(m => m.Asistente != Usr && m.STS01 == 1 && m.STS02 == 0 && m.STS03 == 0).OrderBy(m => m.Asistente).ToList();

                foreach (var item in query2)
                {
                    var mPanelAsistProceso = item;

                    res.Add(mPanelAsistProceso);
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return res;
        }

        public ActionResult RefuseProcess(string Nota, string IdProceso)
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Asistente", "Proceso") });
            }
            var res = "";

            PanelAsistProceso uPanelAsistProceso = db.PanelAsistProceso.Find(int.Parse(IdProceso));
            //RECHAZA
            uPanelAsistProceso.STS03 = 1;
            uPanelAsistProceso.NotaAprobador = Nota;
            //PEDIDO EN SAP            
            uPanelAsistProceso.AsistenteCierra = SessionManager.Get<RCAU>("VarUsuario").UUSR;

            db.Entry(uPanelAsistProceso).State = EntityState.Modified;
            db.SaveChanges();

            lmPanelAsistProceso = ListProcesoAsistente(uPanelAsistProceso.Asistente);

            SessionManager.Set("DSProcesosAst", CargaTablas(lmPanelAsistProceso));

            var para = "";
            if (uPanelAsistProceso.Creador.Trim() != uPanelAsistProceso.Asistente.Trim())
            {
                para = uPanelAsistProceso.Creador + "," + uPanelAsistProceso.Asistente;
                if (!para.Contains(uPanelAsistProceso.AsistenteCierra.Trim()))
                {
                    para += "," + uPanelAsistProceso.AsistenteCierra.Trim();
                }
                if (!para.Contains(uPanelAsistProceso.Solicitante.Trim()))
                {
                    para += "," + uPanelAsistProceso.Solicitante.Trim();
                }
            }
            else
            {
                para = uPanelAsistProceso.Creador;
                if (!para.Contains(uPanelAsistProceso.AsistenteCierra.Trim()))
                {
                    para += "," + uPanelAsistProceso.AsistenteCierra.Trim();
                }
                if (!para.Contains(uPanelAsistProceso.Solicitante.Trim()))
                {
                    para += "," + uPanelAsistProceso.Solicitante.Trim();
                }
            }
            RMAILX nRMAILX = new RMAILX()
            {
                LMOD = "PEDIDOS",
                LREF = "Envio Correo",
                LASUNTO = "Solicitud Rechazada " + uPanelAsistProceso.IdPanelAsistProceso,
                LENVIA = "Pedidos SAP",
                LPARA = para,
                LBCC = "pruebas.bssltda@gmail.com",
                //LBCC = "jsantamaria.bssltda@gmail.com",
                LCATALOGO = "USE SRG",
                LCRTDAT = DateTime.Now,
                LUPDDAT = DateTime.Now,
                LCUERPO = "http://" + ConfigurationManager.AppSettings["local"] + "/PanelAsistentes/Proceso/CorreoProceso?Id=" + uPanelAsistProceso.IdPanelAsistProceso
                //LCUERPO = "http://201.234.71.92:8073/CorreoProceso?Id=" + rPanelAsistProceso.IdPanelAsistProceso
            };
            dbIN.RMAILX.Add(nRMAILX);
            dbIN.SaveChanges();
            res = "OK";

            return Json(new { result = res }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EndProcess(string Numero, string Fecha, string Nota, string IdProceso)
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Asistente", "Proceso") });
            }
            var res = "";

            PanelAsistProceso uPanelAsistProceso = db.PanelAsistProceso.Find(int.Parse(IdProceso));

            uPanelAsistProceso.NotaAprobador = Nota;
            //PEDIDO EN SAP
            uPanelAsistProceso.STS02 = 1;
            uPanelAsistProceso.FechaCierra = DateTime.Now;
            uPanelAsistProceso.AsistenteCierra = SessionManager.Get<RCAU>("VarUsuario").UUSR;
            uPanelAsistProceso.NumPedidoSAP = Numero;
            uPanelAsistProceso.FechaMaximaRadicacion = DateTime.Parse(Fecha);

            db.Entry(uPanelAsistProceso).State = EntityState.Modified;
            db.SaveChanges();

            lmPanelAsistProceso = ListProcesoAsistente(uPanelAsistProceso.Asistente);

            SessionManager.Set("DSProcesosAst", CargaTablas(lmPanelAsistProceso));

            var para = "";
            if (uPanelAsistProceso.Creador.Trim() != uPanelAsistProceso.Asistente.Trim())
            {
                para = uPanelAsistProceso.Creador + "," + uPanelAsistProceso.Asistente;
                if (!para.Contains(uPanelAsistProceso.AsistenteCierra))
                {
                    para += "," + uPanelAsistProceso.AsistenteCierra;
                }
                if (!para.Contains(uPanelAsistProceso.Solicitante))
                {
                    para += "," + uPanelAsistProceso.Solicitante;
                }
                if (uPanelAsistProceso.CorreoProveedor != "")
                {
                    if (!para.Contains(uPanelAsistProceso.CorreoProveedor))
                    {
                        para += "," + uPanelAsistProceso.CorreoProveedor;
                    }
                }
            }
            else
            {
                para = uPanelAsistProceso.Creador;
                if (!para.Contains(uPanelAsistProceso.AsistenteCierra))
                {
                    para += "," + uPanelAsistProceso.AsistenteCierra;
                }
                if (!para.Contains(uPanelAsistProceso.Solicitante))
                {
                    para += "," + uPanelAsistProceso.Solicitante;
                }
                if (uPanelAsistProceso.CorreoProveedor != "")
                {
                    if (!para.Contains(uPanelAsistProceso.CorreoProveedor))
                    {
                        para += "," + uPanelAsistProceso.CorreoProveedor;
                    }
                }
            }

            RMAILX nRMAILX = new RMAILX()
            {
                LKEY = uPanelAsistProceso.IdPanelAsistProceso.ToString(),
                LSQL = "UPDATE PanelAsistProceso SET STS04 = 1 WHERE IdPanelAsistProceso = " + uPanelAsistProceso.IdPanelAsistProceso,
                LMOD = "PEDIDOS",
                LREF = "Envio Correo",
                LASUNTO = "Solicitud Procesada " + uPanelAsistProceso.IdPanelAsistProceso,
                LENVIA = "Pedidos SAP",
                LPARA = para,
                LBCC = "pruebas.bssltda@gmail.com",
                //LBCC = "jsantamaria.bssltda@gmail.com",
                LCATALOGO = "USE SRG",
                LCRTDAT = DateTime.Now,
                LUPDDAT = DateTime.Now,
                LCUERPO = "http://" + ConfigurationManager.AppSettings["local"] + "/PanelAsistentes/Proceso/CorreoProceso?Id=" + uPanelAsistProceso.IdPanelAsistProceso
                //LCUERPO = "http://201.234.71.92/CorreoProceso?Id=" + fPanelAsistProceso.IdPanelAsistProceso

            };
            dbIN.RMAILX.Add(nRMAILX);
            dbIN.SaveChanges();
            res = "OK";

            return Json(new { result = res }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReenviarCorreo(int IdProceso)
        {
            string res = "";

            PanelAsistProceso mPanelAsistProceso = db.PanelAsistProceso.Where(m => m.IdPanelAsistProceso == IdProceso).SingleOrDefault();
            mPanelAsistProceso.STS05 = 1;
            mPanelAsistProceso.NumeroReenvios = mPanelAsistProceso.NumeroReenvios + 1;

            RMAILX mRMAILX = dbIN.RMAILX.Where(m => m.LMOD == "PEDIDOS" && m.LASUNTO.Contains("Solicitud Procesada " + IdProceso.ToString())).OrderByDescending(m => m.LID).Take(1).SingleOrDefault();
            if (mRMAILX != null)
            {
                mRMAILX.LMSGERR = "REENVIO";
                mRMAILX.LENVIADO = 0;
                mRMAILX.LSTS01 = 0;
                mRMAILX.LSTS02 = 0;
                mRMAILX.LSTS03 = 0;
                mRMAILX.LUPDDAT = DateTime.Now;
                mRMAILX.LKEY = IdProceso.ToString();
                mRMAILX.LSQL = "UPDATE PanelAsistProceso SET STS04 = 1, STS05 = 0 WHERE IdPanelAsistProceso = " + IdProceso;
                dbIN.Entry(mRMAILX).State = EntityState.Modified;
                dbIN.SaveChanges();
            }
            else
            {
                var para = "";
                if (mPanelAsistProceso.Creador.Trim() != mPanelAsistProceso.Asistente.Trim())
                {
                    para = mPanelAsistProceso.Creador + "," + mPanelAsistProceso.Asistente;
                    if (!para.Contains(mPanelAsistProceso.AsistenteCierra))
                    {
                        para += "," + mPanelAsistProceso.AsistenteCierra;
                    }
                    if (!para.Contains(mPanelAsistProceso.Solicitante))
                    {
                        para += "," + mPanelAsistProceso.Solicitante;
                    }
                    if (mPanelAsistProceso.CorreoProveedor != "")
                    {
                        if (!para.Contains(mPanelAsistProceso.CorreoProveedor))
                        {
                            para += "," + mPanelAsistProceso.CorreoProveedor;
                        }
                    }
                }
                else
                {
                    para = mPanelAsistProceso.Creador;
                    if (!para.Contains(mPanelAsistProceso.AsistenteCierra))
                    {
                        para += "," + mPanelAsistProceso.AsistenteCierra;
                    }
                    if (!para.Contains(mPanelAsistProceso.Solicitante))
                    {
                        para += "," + mPanelAsistProceso.Solicitante;
                    }
                    if (mPanelAsistProceso.CorreoProveedor != "")
                    {
                        if (!para.Contains(mPanelAsistProceso.CorreoProveedor))
                        {
                            para += "," + mPanelAsistProceso.CorreoProveedor;
                        }
                    }
                }
                RMAILX nRMAILX = new RMAILX()
                {
                    LKEY = IdProceso.ToString(),
                    LSQL = "UPDATE PanelAsistProceso SET STS04 = 1, STS05 = 0 WHERE IdPanelAsistProceso = " + IdProceso,
                    LMOD = "PEDIDOS",
                    LREF = "Envio Correo",
                    LASUNTO = "Solicitud Procesada " + IdProceso,
                    LENVIA = "Pedidos SAP",
                    LPARA = para,
                    LBCC = "pruebas.bssltda@gmail.com",
                    //LBCC = "jsantamaria.bssltda@gmail.com",
                    LCATALOGO = "USE SRG",
                    LCRTDAT = DateTime.Now,
                    LUPDDAT = DateTime.Now,
                    LCUERPO = "http://" + ConfigurationManager.AppSettings["local"] + "/PanelAsistentes/Proceso/CorreoProceso?Id=" + IdProceso
                    //LCUERPO = "http://201.234.71.92/CorreoProceso?Id=" + fPanelAsistProceso.IdPanelAsistProceso
                };
                dbIN.RMAILX.Add(nRMAILX);
                dbIN.SaveChanges();
            }

            db.Entry(mPanelAsistProceso).State = EntityState.Modified;
            db.SaveChanges();
            res = "OK";
            GetAllProcess();

            return Json(new
            {
                result = res
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DSProcesoJefe(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<PanelAsistProceso>>("DSProcesosJefe");
            if (Data == null)
            {
                Data = new List<PanelAsistProceso>();
            }
            return fc.DSGenerico(dm, Data);
        }

        public ActionResult DSProcesoUsr(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<PanelAsistProceso>>("DSProcesosUsr");
            if (Data == null)
            {
                Data = new List<PanelAsistProceso>();
            }
            return fc.DSGenerico(dm, Data);
        }

        public ActionResult DSProcesoAst(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<PanelAsistProceso>>("DSProcesosAst");
            if (Data == null)
            {
                Data = new List<PanelAsistProceso>();
            }
            return fc.DSGenerico(dm, Data);
        }

        public void ExpToExc(string GridModel)
        {
            ExcelExport exp = new ExcelExport();
            var DataSource = SessionManager.Get<List<PanelAsistProceso>>("DSProcesosJefe");
            GridProperties obj = fc.ConvertGridObject(GridModel);
            obj.ServerExcelQueryCellInfo = fc.QueryCellInfo;
            try
            {
                exp.Export(obj, DataSource, "Procesos.xls", ExcelVersion.Excel97to2003, false, false, "flat-saffron");
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }
    }
}