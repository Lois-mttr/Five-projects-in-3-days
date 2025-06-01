using Biblioteca.Models;
using Biblioteca.Models.Repositorio;
using Biblioteca.Models.ViewModels;
using Biblioteca.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Biblioteca.Controllers
{
    public class LibrosController : Controller
    {
        private LibroRepositorio _repositorio = new LibroRepositorio();
        private ImagenService _imagenService = new ImagenService();

        // GET: Libros
        public ActionResult Index(string busqueda = "")
        {
            try
            {
                var viewModel = new BibliotecaViewModel();
                viewModel.Estadisticas = _repositorio.ObtenerEstadisticas();
                viewModel.TerminoBusqueda = busqueda;

                if (!string.IsNullOrEmpty(busqueda))
                {
                    viewModel.Libros = _repositorio.BuscarLibros(busqueda);
                }
                else
                {
                    viewModel.Libros = _repositorio.ObtenerLibros();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los libros: " + ex.Message;
                return View(new BibliotecaViewModel());
            }
        }

        // GET: Libros/Detalles/5
        public ActionResult Detalles(int id)
        {
            try
            {
                var libro = _repositorio.ObtenerLibroPorId(id);
                if (libro == null)
                {
                    return HttpNotFound();
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el libro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Libros/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // POST: Libros/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Libro libro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Procesar imagen si se subió
                    if (libro.ArchivoImagen != null && libro.ArchivoImagen.ContentLength > 0)
                    {
                        libro.ImagenPortada = _imagenService.GuardarImagen(libro.ArchivoImagen);
                    }

                    _repositorio.InsertarLibro(libro);
                    TempData["Mensaje"] = "Libro creado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el libro: " + ex.Message);
                }
            }
            return View(libro);
        }

        // GET: Libros/Editar/5
        public ActionResult Editar(int id)
        {
            try
            {
                var libro = _repositorio.ObtenerLibroPorId(id);
                if (libro == null)
                {
                    return HttpNotFound();
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el libro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Libros/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Libro libro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener libro actual para preservar imagen si no se cambia
                    var libroActual = _repositorio.ObtenerLibroPorId(libro.Id);
                    if (libroActual == null)
                    {
                        return HttpNotFound();
                    }

                    // Procesar nueva imagen si se subió
                    if (libro.ArchivoImagen != null && libro.ArchivoImagen.ContentLength > 0)
                    {
                        // Eliminar imagen anterior si existe
                        if (!string.IsNullOrEmpty(libroActual.ImagenPortada))
                        {
                            _imagenService.EliminarImagen(libroActual.ImagenPortada);
                        }

                        libro.ImagenPortada = _imagenService.GuardarImagen(libro.ArchivoImagen);
                    }
                    else
                    {
                        // Mantener imagen actual
                        libro.ImagenPortada = libroActual.ImagenPortada;
                    }

                    _repositorio.ActualizarLibro(libro);
                    TempData["Mensaje"] = "Libro actualizado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el libro: " + ex.Message);
                }
            }
            return View(libro);
        }

        // GET: Libros/Eliminar/5
        public ActionResult Eliminar(int id)
        {
            try
            {
                var libro = _repositorio.ObtenerLibroPorId(id);
                if (libro == null)
                {
                    return HttpNotFound();
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el libro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Libros/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarEliminar(int id)
        {
            try
            {
                var libro = _repositorio.ObtenerLibroPorId(id);
                if (libro != null)
                {
                    // Eliminar imagen si existe
                    if (!string.IsNullOrEmpty(libro.ImagenPortada))
                    {
                        _imagenService.EliminarImagen(libro.ImagenPortada);
                    }

                    _repositorio.EliminarLibro(id);
                    TempData["Mensaje"] = "Libro eliminado correctamente.";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el libro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}