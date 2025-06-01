using RegistroGastosPersonales.Models;
using RegistroGastosPersonales.Models.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RegistroGastosPersonales.Controllers
{
    public class GastosController : Controller
    {
        private GastoRepositorio _repositorio = new GastoRepositorio();

        // GET: Gastos
        public ActionResult Index()
        {
            var gastos = _repositorio.ObtenerTodosLosGastos();
            return View(gastos);
        }

        // GET: Gastos/Details/5
        public ActionResult Details(int id)
        {
            var gasto = _repositorio.ObtenerGastoPorId(id);
            if (gasto == null)
            {
                return HttpNotFound();
            }
            return View(gasto);
        }

        // GET: Gastos/Create
        public ActionResult Create()
        {
            ViewBag.Categorias = new SelectList(_repositorio.ObtenerCategorias(), "Id", "Nombre");
            return View();
        }

        // POST: Gastos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.InsertarGasto(gasto);
                    TempData["Mensaje"] = "Gasto registrado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar el gasto: " + ex.Message);
                }
            }

            ViewBag.Categorias = new SelectList(_repositorio.ObtenerCategorias(), "Id", "Nombre", gasto.CategoriaId);
            return View(gasto);
        }

        // GET: Gastos/Edit/5
        public ActionResult Edit(int id)
        {
            var gasto = _repositorio.ObtenerGastoPorId(id);
            if (gasto == null)
            {
                return HttpNotFound();
            }

            ViewBag.Categorias = new SelectList(_repositorio.ObtenerCategorias(), "Id", "Nombre", gasto.CategoriaId);
            return View(gasto);
        }

        // POST: Gastos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repositorio.ActualizarGasto(gasto);
                    TempData["Mensaje"] = "Gasto actualizado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el gasto: " + ex.Message);
                }
            }

            ViewBag.Categorias = new SelectList(_repositorio.ObtenerCategorias(), "Id", "Nombre", gasto.CategoriaId);
            return View(gasto);
        }

        // GET: Gastos/Delete/5
        public ActionResult Delete(int id)
        {
            var gasto = _repositorio.ObtenerGastoPorId(id);
            if (gasto == null)
            {
                return HttpNotFound();
            }
            return View(gasto);
        }

        // POST: Gastos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _repositorio.EliminarGasto(id);
                TempData["Mensaje"] = "Gasto eliminado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el gasto: " + ex.Message;
                return RedirectToAction("Delete", new { id = id });
            }
        }

        // GET: Gastos/Resumen
        public ActionResult Resumen()
        {
            var resumen = _repositorio.ObtenerResumenGastos();
            return View(resumen);
        }
    }
}