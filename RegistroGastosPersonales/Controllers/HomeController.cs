using RegistroGastosPersonales.Models.Repositorio;
using System;
using System.Web.Mvc;

namespace RegistroGastosMVC.Controllers
{
    public class HomeController : Controller
    {
        private GastoRepositorio _repositorio = new GastoRepositorio();

        public ActionResult Index()
        {
            var resumen = _repositorio.ObtenerResumenGastos();
            return View(resumen);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Aplicación de Registro de Gastos Personales";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Página de contacto";
            return View();
        }
    }
}