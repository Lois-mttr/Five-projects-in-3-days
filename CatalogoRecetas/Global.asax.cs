using CatalogoRecetas.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CatalogoRecetas
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Probar conexi�n a la base de datos al iniciar la aplicaci�n
            try
            {
                var conexion = new ConexionBaseDatos();
                conexion.ProbarConexion();
                System.Diagnostics.Debug.WriteLine("Conexi�n a base de datos establecida correctamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al conectar con la base de datos: {ex.Message}");
                // Log del error pero no detener la aplicaci�n
            }
        }

        protected void Session_Start()
        {
            // Inicializar sesi�n si es necesario
            Session["FechaInicioSesion"] = DateTime.Now;
        }
    }
}
