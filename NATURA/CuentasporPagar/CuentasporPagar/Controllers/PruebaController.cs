using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuentasporPagar.Controllers
{
    public class PruebaController : Controller
    {
        // GET: Prueba
        public ActionResult Index()
        {
            CXPProcesoController cXPProcesoController = new CXPProcesoController();
            Console.WriteLine(cXPProcesoController.CalculoImpuesto("NATURAL", 5, 4));
            return View();
        }
    }
}