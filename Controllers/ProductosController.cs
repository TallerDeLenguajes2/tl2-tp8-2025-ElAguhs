using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_ElAguhs.Models;
using tl2_tp8_2025_ElAguhs.Repositorios;
using tl2_tp8_2025_ElAguhs.ViewModels; // Nuevo
using System.Collections.Generic;
using System;

namespace tl2_tp8_2025_ElAguhs.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ProductoRepository _productoRepository = new ProductoRepository();

        // [ Index y Delete se mantienen usando la entidad Producto ]

        [HttpGet]
        public IActionResult Index()
        {
            List<Producto> productos = _productoRepository.Listar();
            return View(productos);
        }

        // --- CREATE ---

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProductoViewModel()); // Devuelve ViewModel
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductoViewModel productoVM) // Recibe ViewModel
        {
            if (!ModelState.IsValid)
            {
                return View(productoVM);
            }

            // Mapeo y FIX: Usamos ?? para asegurar que la Descripción nunca sea NULL.
            var nuevoProducto = new Producto
            {
                Descripcion = productoVM.Descripcion ?? string.Empty, // FIX
                Precio = (int)productoVM.Precio
            };

            _productoRepository.Crear(nuevoProducto);
            return RedirectToAction(nameof(Index));
        }

        // --- EDIT ---

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Producto? producto = _productoRepository.ObtenerPorId(id);
            if (producto == null) return NotFound();

            // Mapeo Inverso de Modelo a ViewModel
            var productoVM = new ProductoViewModel
            {
                IdProducto = producto.IdProducto,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio
            };

            return View(productoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProductoViewModel productoVM) // Recibe ViewModel
        {
            if (!ModelState.IsValid)
            {
                return View(productoVM);
            }

            // Mapeo y FIX: Mapeamos de VM a Modelo
            var productoAEditar = new Producto
            {
                IdProducto = productoVM.IdProducto,
                Descripcion = productoVM.Descripcion ?? string.Empty, // FIX
                Precio = (int)productoVM.Precio
            };

            _productoRepository.Modificar(id, productoAEditar);
            return RedirectToAction(nameof(Index));
        }

        // ... (Acciones Delete se mantienen igual) ...


        // GET: /Productos/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Producto? producto = _productoRepository.ObtenerPorId(id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }
        // POST: /Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Llamamos al método correcto del repositorio
            _productoRepository.Eliminar(id);

            return RedirectToAction(nameof(Index));
        }
    }
}