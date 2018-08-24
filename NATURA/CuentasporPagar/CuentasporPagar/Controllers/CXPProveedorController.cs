using CuentasporPagar.Models;
using CuentasporPagar.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuentasporPagar.Controllers
{
    public class CXPProveedorController : Controller
    {
        FuncionesComunes fc = new FuncionesComunes();
        SRGEntities db = new SRGEntities();

        // GET: CXPProveedor
        public ActionResult Index()
        {
            if (SessionManager.Get<RCAU>("VarUsuario") == null)
            {
                return RedirectToAction("Login", "Account", new { donde = Url.Action("Index", "Proveedor") });
            }
            List<CxPProveedor> lmCxPProveedor= db.CxPProveedor.ToList();
            SessionManager.Set("DProveedor", lmCxPProveedor);
            return View();
        }
    }
}