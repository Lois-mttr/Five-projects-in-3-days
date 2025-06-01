// Ubicación: ~/Controllers/RecetasController.cs
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CatalogoRecetas.Data;
using CatalogoRecetas.Models;

namespace CatalogoRecetas.Controllers
{
    public class RecetasController : Controller
    {
        private readonly RepositorioRecetas _repositorio;

        public RecetasController()
        {
            _repositorio = new RepositorioRecetas();
        }

        // GET: Recetas (Página principal)
        public ActionResult Index(string buscar, int? categoriaId)
        {
            try
            {
                // Obtener categorías para el dropdown
                var categorias = _repositorio.ObtenerTodasLasCategorias();
                ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre");

                // Obtener recetas con filtros
                var recetas = _repositorio.ObtenerRecetasConFiltros(buscar, categoriaId);

                ViewBag.Buscar = buscar;
                ViewBag.CategoriaSeleccionada = categoriaId;
                ViewBag.TotalRecetas = recetas.Count;

                return View(recetas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las recetas: {ex.Message}";
                return View();
            }
        }

        // GET: Recetas/Detalles/5
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID de receta no válido");
            }

            try
            {
                var receta = _repositorio.ObtenerRecetaPorId(id.Value);
                if (receta == null)
                {
                    TempData["Error"] = "La receta solicitada no existe";
                    return RedirectToAction("Index");
                }

                return View(receta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la receta: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // GET: Recetas/Crear
        public ActionResult Crear()
        {
            try
            {
                var categorias = _repositorio.ObtenerTodasLasCategorias();
                ViewBag.CategoriaId = new SelectList(categorias, "Id", "Nombre");
                return View(new Receta());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las categorías: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Recetas/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Receta receta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Manejar subida de imagen
                    if (receta.ArchivoImagen != null && receta.ArchivoImagen.ContentLength > 0)
                    {
                        var resultadoImagen = GuardarImagen(receta.ArchivoImagen);
                        if (resultadoImagen.Exitoso)
                        {
                            receta.ImagenRuta = resultadoImagen.RutaArchivo;
                        }
                        else
                        {
                            ModelState.AddModelError("ArchivoImagen", resultadoImagen.MensajeError);
                            CargarCategoriasEnViewBag(receta.CategoriaId);
                            return View(receta);
                        }
                    }

                    // Crear receta en la base de datos
                    int nuevoId = _repositorio.CrearReceta(receta);

                    TempData["Mensaje"] = "¡Receta creada exitosamente!";
                    return RedirectToAction("Detalles", new { id = nuevoId });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al crear la receta: {ex.Message}";
                    ModelState.AddModelError("", "Ocurrió un error al guardar la receta. Por favor, inténtelo de nuevo.");
                }
            }

            // Recargar categorías en caso de error
            CargarCategoriasEnViewBag(receta.CategoriaId);
            return View(receta);
        }

        // GET: Recetas/Editar/5
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID de receta no válido");
            }

            try
            {
                var recetaCompleta = _repositorio.ObtenerRecetaPorId(id.Value);
                if (recetaCompleta == null)
                {
                    TempData["Error"] = "La receta solicitada no existe";
                    return RedirectToAction("Index");
                }

                // Convertir RecetaCompleta a Receta para el formulario
                var receta = new Receta
                {
                    Id = recetaCompleta.Id,
                    Nombre = recetaCompleta.Nombre,
                    Ingredientes = recetaCompleta.Ingredientes,
                    Instrucciones = recetaCompleta.Instrucciones,
                    CategoriaId = recetaCompleta.CategoriaId,
                    ImagenRuta = recetaCompleta.ImagenRuta,
                    TiempoPreparacion = recetaCompleta.TiempoPreparacion,
                    Porciones = recetaCompleta.Porciones,
                    FechaCreacion = recetaCompleta.FechaCreacion,
                    FechaModificacion = recetaCompleta.FechaModificacion
                };

                CargarCategoriasEnViewBag(receta.CategoriaId);
                return View(receta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la receta: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Recetas/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Receta receta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener la receta actual para preservar la imagen si no se sube una nueva
                    var recetaActual = _repositorio.ObtenerRecetaPorId(receta.Id);
                    if (recetaActual == null)
                    {
                        TempData["Error"] = "La receta no existe";
                        return RedirectToAction("Index");
                    }

                    // Preservar la imagen actual si no se sube una nueva
                    receta.ImagenRuta = recetaActual.ImagenRuta;

                    // Manejar subida de nueva imagen
                    if (receta.ArchivoImagen != null && receta.ArchivoImagen.ContentLength > 0)
                    {
                        var resultadoImagen = GuardarImagen(receta.ArchivoImagen);
                        if (resultadoImagen.Exitoso)
                        {
                            // Eliminar imagen anterior si existe
                            if (!string.IsNullOrEmpty(recetaActual.ImagenRuta))
                            {
                                EliminarImagen(recetaActual.ImagenRuta);
                            }

                            receta.ImagenRuta = resultadoImagen.RutaArchivo;
                        }
                        else
                        {
                            ModelState.AddModelError("ArchivoImagen", resultadoImagen.MensajeError);
                            CargarCategoriasEnViewBag(receta.CategoriaId);
                            return View(receta);
                        }
                    }

                    // Actualizar receta en la base de datos
                    _repositorio.ActualizarReceta(receta);

                    TempData["Mensaje"] = "¡Receta actualizada exitosamente!";
                    return RedirectToAction("Detalles", new { id = receta.Id });
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al actualizar la receta: {ex.Message}";
                    ModelState.AddModelError("", "Ocurrió un error al actualizar la receta. Por favor, inténtelo de nuevo.");
                }
            }

            // Recargar categorías en caso de error
            CargarCategoriasEnViewBag(receta.CategoriaId);
            return View(receta);
        }

        // GET: Recetas/Eliminar/5
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID de receta no válido");
            }

            try
            {
                var receta = _repositorio.ObtenerRecetaPorId(id.Value);
                if (receta == null)
                {
                    TempData["Error"] = "La receta solicitada no existe";
                    return RedirectToAction("Index");
                }

                return View(receta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la receta: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Recetas/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarEliminacion(int id)
        {
            try
            {
                // Eliminar receta de la base de datos
                string imagenEliminada = _repositorio.EliminarReceta(id);

                // Eliminar imagen del sistema de archivos si existe
                if (!string.IsNullOrEmpty(imagenEliminada))
                {
                    EliminarImagen(imagenEliminada);
                }

                TempData["Mensaje"] = "¡Receta eliminada exitosamente!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar la receta: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // GET: Recetas/Estadisticas
        public ActionResult Estadisticas()
        {
            try
            {
                var estadisticas = _repositorio.ObtenerEstadisticas();
                return View(estadisticas);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las estadísticas: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // 
        // MÉTODOS AUXILIARES
        // 

        private void CargarCategoriasEnViewBag(int? categoriaSeleccionada = null)
        {
            try
            {
                var categorias = _repositorio.ObtenerTodasLasCategorias();
                ViewBag.CategoriaId = new SelectList(categorias, "Id", "Nombre", categoriaSeleccionada);
            }
            catch
            {
                ViewBag.CategoriaId = new SelectList(Enumerable.Empty<Categoria>(), "Id", "Nombre");
            }
        }

        private ResultadoImagen GuardarImagen(HttpPostedFileBase archivo)
        {
            try
            {
                // Validar archivo
                if (archivo == null || archivo.ContentLength == 0)
                {
                    return new ResultadoImagen { Exitoso = false, MensajeError = "No se seleccionó ningún archivo" };
                }

                // Validar tamaño (máximo 5MB)
                if (archivo.ContentLength > 5 * 1024 * 1024)
                {
                    return new ResultadoImagen { Exitoso = false, MensajeError = "El archivo es demasiado grande. Máximo 5MB permitido" };
                }

                // Validar extensión
                string extension = Path.GetExtension(archivo.FileName).ToLower();
                string[] extensionesPermitidas = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

                if (!extensionesPermitidas.Contains(extension))
                {
                    return new ResultadoImagen { Exitoso = false, MensajeError = "Formato de archivo no válido. Use JPG, PNG, GIF o BMP" };
                }

                // Generar nombre único
                string nombreArchivo = $"{Guid.NewGuid()}{extension}";
                string rutaCarpeta = Server.MapPath("~/Content/Imagenes/");

                // Crear carpeta si no existe
                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                // Guardar archivo
                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                archivo.SaveAs(rutaCompleta);

                return new ResultadoImagen
                {
                    Exitoso = true,
                    RutaArchivo = $"/Content/Imagenes/{nombreArchivo}"
                };
            }
            catch (Exception ex)
            {
                return new ResultadoImagen
                {
                    Exitoso = false,
                    MensajeError = $"Error al guardar la imagen: {ex.Message}"
                };
            }
        }

        private void EliminarImagen(string rutaImagen)
        {
            try
            {
                if (!string.IsNullOrEmpty(rutaImagen))
                {
                    string rutaCompleta = Server.MapPath(rutaImagen);
                    if (System.IO.File.Exists(rutaCompleta))
                    {
                        System.IO.File.Delete(rutaCompleta);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log del error pero no interrumpir el flujo
                System.Diagnostics.Debug.WriteLine($"Error al eliminar imagen: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Limpiar recursos si es necesario
            }
            base.Dispose(disposing);
        }
    }

    // Clase auxiliar para resultados de operaciones con imágenes
    public class ResultadoImagen
    {
        public bool Exitoso { get; set; }
        public string RutaArchivo { get; set; }
        public string MensajeError { get; set; }
    }
}