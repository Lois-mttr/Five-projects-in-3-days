using Contactos.Models;
using Contactos.Models.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contactos.Controllers
{
    public class ContactosController : Controller
    {
        private ContactoRepositorio _repositorio = new ContactoRepositorio();

        // GET: Contactos
        public ActionResult Index(string busqueda = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(busqueda))
                {
                    return View(_repositorio.BuscarContactos(busqueda));
                }
                return View(_repositorio.ObtenerContactos());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los contactos: " + ex.Message;
                return View(new System.Collections.Generic.List<Contacto>());
            }
        }

        // GET: Contactos/Detalles/5
        public ActionResult Detalles(int id)
        {
            try
            {
                var contacto = _repositorio.ObtenerContactoPorId(id);
                if (contacto == null)
                {
                    return HttpNotFound();
                }
                return View(contacto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el contacto: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Contactos/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // POST: Contactos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Contacto contacto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.InsertarContacto(contacto);
                    TempData["Mensaje"] = "Contacto creado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el contacto: " + ex.Message);
                }
            }
            return View(contacto);
        }

        // GET: Contactos/Editar/5
        public ActionResult Editar(int id)
        {
            try
            {
                var contacto = _repositorio.ObtenerContactoPorId(id);
                if (contacto == null)
                {
                    return HttpNotFound();
                }
                return View(contacto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el contacto: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Contactos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Contacto contacto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.ActualizarContacto(contacto);
                    TempData["Mensaje"] = "Contacto actualizado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el contacto: " + ex.Message);
                }
            }
            return View(contacto);
        }

        // GET: Contactos/Eliminar/5
        public ActionResult Eliminar(int id)
        {
            try
            {
                var contacto = _repositorio.ObtenerContactoPorId(id);
                if (contacto == null)
                {
                    return HttpNotFound();
                }
                return View(contacto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el contacto: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Contactos/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarEliminar(int id)
        {
            try
            {
                _repositorio.EliminarContacto(id);
                TempData["Mensaje"] = "Contacto eliminado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el contacto: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}