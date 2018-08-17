using CuentasporPagar.Models;
using CuentasporPagar.Models.EF;
using Syncfusion.JavaScript;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CuentasporPagar.Controllers
{
    public class CXPTasasController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        SRGEntities db = new SRGEntities();

        // GET: CXPTasas
        public ActionResult Index()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Tasas") });
            }
            List<CxPTasas> lmCxPTasas = db.CxPTasas.ToList();
            SessionManager.Set("DTasas", lmCxPTasas);
            return View();
        }

        public ActionResult Nuevo(string Codigo, string Concepto, string Tasa, string Divisor)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                var nCxPTasas = new CxPTasas()
                {
                    Codigo = Codigo,
                    Concepto = Concepto,
                    Tasa = double.Parse(Tasa.Replace('.', ',')),
                    Divisor = double.Parse(Divisor),
                    Factor = Math.Truncate((double.Parse(Tasa.Replace('.', ',')) / double.Parse(Divisor)) * 10000000) / 10000000
                };
                db.CxPTasas.Add(nCxPTasas);
                db.SaveChanges();
                SessionManager.Set("DTasas", db.CxPTasas.ToList());
                Respu = "OK";
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
                Respu = "ERROR: " + Error.ToString();
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString() + ((ex.InnerException != null) ? ex.InnerException.ToString() : "");
            }
            return Json(new { result = Respu });
        }

        public ActionResult Editar(int IdCxPTasas, string Codigo, string Concepto, string Tasa, string Divisor)
        {
            string Respu = "";
            StringBuilder Error = new StringBuilder();
            try
            {
                var uCxPTasas = new CxPTasas()
                {
                    IdCxPTasas = IdCxPTasas,
                    Codigo = Codigo,
                    Concepto = Concepto,
                    Tasa = double.Parse(Tasa.Replace('.', ',')),
                    Divisor = double.Parse(Divisor),
                    Factor = Math.Truncate((double.Parse(Tasa.Replace('.', ',')) / double.Parse(Divisor)) * 10000000) / 10000000
                };
                db.Entry(uCxPTasas).State = EntityState.Modified;
                db.SaveChanges();
                SessionManager.Set("DTasas", db.CxPTasas.ToList());
                Respu = "OK";
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
                Respu = "ERROR: " + Error.ToString();
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
                var rCxPTasas = db.CxPTasas.Find(int.Parse(Id));
                db.CxPTasas.Remove(rCxPTasas);
                db.SaveChanges();
                SessionManager.Set("DTasas", db.CxPTasas.ToList());
                Respu = "OK";
            }
            catch (Exception ex)
            {
                Respu = "ERROR: " + ex.ToString();
            }
            return Json(new { result = Respu });
        }

        public ActionResult DSTasas(DataManager dm)
        {
            IEnumerable Data = SessionManager.Get<List<CxPTasas>>("DTasas");
            if (Data == null)
            {
                Data = new List<CxPTasas>();
            }
            return fc.DSGenerico(dm, Data);
        }
    }
}