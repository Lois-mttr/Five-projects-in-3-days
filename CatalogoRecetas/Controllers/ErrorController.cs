using System.Web.Mvc;

namespace CatalogoRecetas.Controllers
{
    public class ErrorController : Controller 
    {
        // GET: Error
        public ActionResult Index()
        {
            ViewBag.Title = "Error - Catálogo de Recetas";
            return View();
        }

        // GET: Error/NotFound
        public ActionResult NotFound()
        {
            ViewBag.Title = "Página No Encontrada - Catálogo de Recetas";
            Response.StatusCode = 404;
            return View();
        }

        // GET: Error/ServerError
        public ActionResult ServerError()
        {
            ViewBag.Title = "Error del Servidor - Catálogo de Recetas";
            Response.StatusCode = 500;
            return View();
        }
    }
}