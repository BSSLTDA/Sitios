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
using System.Linq;
using System.Web.Mvc;

namespace PanelAsistentes.Controllers
{
    public class ContratoController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        SRGEntities db = new SRGEntities();

        public ActionResult Index()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Contrato") });
            }
            List<MdlContrato> lmContrato = GetAllContratos();

            SessionManager.Set("DContratos", lmContrato);

            RFPARAM mRFPARAM = db.RFPARAM.Where(m => m.CCTABL == "CONTRATOSAVISO" && m.CCCODE == "DIAS" && m.CCCODE2 == "1").SingleOrDefault();
            if (mRFPARAM != null)
            {
                ViewBag.AvisoUno = mRFPARAM.CCNOT1;
            }
            else
            {
                ViewBag.AvisoUno = 0;
            }

            return View();
        }

        public List<MdlContrato> GetAllContratos(bool NoArchivados = true)
        {
            List<MdlContrato> lmContrato = new List<MdlContrato>();
            try
            {
                var query = from p in db.Contrato
                            join p2 in db.ContratoArea on p.AreaSolicitante equals p2.IdContratoArea.ToString()
                            select new MdlContrato()
                            {
                                NContrato = p.NContrato,
                                RazonSocial = p.RazonSocial,
                                NIT = p.NIT,
                                FechaDeInicio = p.FechaDeInicio,
                                FechaDeTerminacion = p.FechaDeTerminacion,
                                DuracionDias = p.DuracionDias,
                                ProrrogaAutomatica = (p.ProrrogaAutomatica != null) ? (p.ProrrogaAutomatica == "1") ? "SI" : "NO" : null,
                                Ubicacion = p.Ubicacion,
                                ObjetoDelContrato = p.ObjetoDelContrato,
                                ValorUsd = p.ValorUsd,
                                ValorCop = p.ValorCop,
                                PeriodicidadDelPago = p.PeriodicidadDelPago,
                                AplicaImpTimbre = (p.AplicaImpTimbre != null) ? (p.AplicaImpTimbre == "1") ? "SI" : "NO" : null,
                                Cuantia = (p.Cuantia != null) ? (p.Cuantia == "1") ? "SI" : "NO" : null,
                                Observaciones = p.Observaciones,
                                SeguimientoMensual = p.SeguimientoMensual,
                                FechaDePago = p.FechaDePago,
                                AreaSolicitante = p2.Descripcion,
                                IdArea = p2.IdContratoArea.ToString(),
                                Inflacion = p.Inflacion,
                                ComentarioContrato = p.ComentarioContrato,
                                STS01 = p.STS01,
                                STS02 = p.STS02,
                                STS03 = p.STS03,
                                STS04 = p.STS04,
                                STS05 = p.STS05
                            };
                if (NoArchivados)
                {
                    lmContrato = query.ToList().Where(m => m.STS04 == 0).ToList();
                }
                else
                {
                    lmContrato = query.ToList().Where(m => m.STS04 == 1).ToList();
                }

            }
            catch (Exception ex)
            {
                //res = "ERROR: " + ex.Message;
            }
            return lmContrato;
        }

        public ActionResult DSContrato(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<MdlContrato>>("DContratos");
            if (Data == null)
            {
                Data = new List<MdlContrato>();
            }
            return fc.DSGenerico(dm, Data);
        }

        public void ExpToExc(string GridModel)
        {
            ExcelExport exp = new ExcelExport();
            var DataSource = SessionManager.Get<List<MdlContrato>>("DContratos");
            GridProperties obj = fc.ConvertGridObject(GridModel);

            obj.ServerExcelQueryCellInfo = fc.QueryCellInfo;
            try
            {
                exp.Export(obj, DataSource, "Contratos.xls", ExcelVersion.Excel97to2003, false, false, "flat-saffron");
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }
    }
}