using System;
using System.Web.Mvc;
using ToDo.Models;
using ToDo.Models.Repositorio;
using ToDo.Models.ViewModels;

namespace ToDo.Controllers
{
    public class HomeController : Controller
    {
        private TareaRepositorio _repositorio = new TareaRepositorio();

        public ActionResult Index(string vista = "todas", int? estado = null)
        {
            var viewModel = new TareasViewModel();

            try
            {
                // Obtener estados para los filtros
                viewModel.Estados = _repositorio.ObtenerEstados();

                // Obtener estadísticas
                viewModel.Estadisticas = _repositorio.ObtenerEstadisticas();

                // Obtener tareas según el filtro
                switch (vista.ToLower())
                {
                    case "vencidas":
                        viewModel.Tareas = _repositorio.ObtenerTareasVencidas();
                        viewModel.VistaActual = "vencidas";
                        break;
                    case "estado":
                        if (estado.HasValue)
                        {
                            viewModel.Tareas = _repositorio.ObtenerTareasPorEstado(estado.Value);
                            viewModel.FiltroEstado = estado.Value;
                            viewModel.VistaActual = "estado";
                        }
                        else
                        {
                            viewModel.Tareas = _repositorio.ObtenerTodasLasTareas();
                            viewModel.VistaActual = "todas";
                        }
                        break;
                    default:
                        viewModel.Tareas = _repositorio.ObtenerTodasLasTareas();
                        viewModel.VistaActual = "todas";
                        break;
                }

                // Configurar nueva tarea con fecha por defecto
                viewModel.NuevaTarea.FechaVencimiento = DateTime.Now.Date.AddDays(1);
                viewModel.NuevaTarea.EstadoId = 1; // Pendiente por defecto
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las tareas: " + ex.Message;
                viewModel = new TareasViewModel();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearTarea(TareasViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.InsertarTarea(model.NuevaTarea);
                    TempData["Mensaje"] = "Tarea creada correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear la tarea: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Por favor, complete todos los campos requeridos.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult MarcarCompletada(int id)
        {
            try
            {
                bool resultado = _repositorio.MarcarTareaCompletada(id);
                return Json(new { success = resultado, message = resultado ? "Tarea marcada como completada" : "Error al actualizar la tarea" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CambiarEstado(int id, int estadoId)
        {
            try
            {
                bool resultado = _repositorio.CambiarEstadoTarea(id, estadoId);
                return Json(new { success = resultado, message = resultado ? "Estado actualizado correctamente" : "Error al actualizar el estado" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EliminarTarea(int id)
        {
            try
            {
                bool resultado = _repositorio.EliminarTarea(id);
                return Json(new { success = resultado, message = resultado ? "Tarea eliminada correctamente" : "Error al eliminar la tarea" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerTarea(int id)
        {
            try
            {
                var tarea = _repositorio.ObtenerTareaPorId(id);
                if (tarea != null)
                {
                    return Json(new
                    {
                        success = true,
                        tarea = new
                        {
                            id = tarea.Id,
                            descripcion = tarea.Descripcion,
                            fechaVencimiento = tarea.FechaVencimiento.ToString("yyyy-MM-dd"),
                            estadoId = tarea.EstadoId
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Tarea no encontrada" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ActualizarTarea(Tarea tarea)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool resultado = _repositorio.ActualizarTarea(tarea);
                    return Json(new { success = resultado, message = resultado ? "Tarea actualizada correctamente" : "Error al actualizar la tarea" });
                }
                return Json(new { success = false, message = "Datos inválidos" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}