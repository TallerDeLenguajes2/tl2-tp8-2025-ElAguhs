using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_ElAguhs.Models;
using tl2_tp8_2025_ElAguhs.ViewModels;
using tl2_tp8_2025_ElAguhs.Interfaces; // Inyección de Dependencias
using System.Collections.Generic;
using System;
using System.Linq;

namespace tl2_tp8_2025_ElAguhs.Controllers
{
    public class ProductosController : Controller
    {
        // Dependencias inyectadas por el constructor (TP10 - Fase 1)
        private readonly IProductoRepository _productoRepository;
        private readonly IAuthenticationService _authService;

        public ProductosController(IProductoRepository productoRepo, IAuthenticationService authService)
        {
            _productoRepository = productoRepo;
            _authService = authService;
        }

        // --- LÓGICA DE SEGURIDAD (FASE 3) ---

        private IActionResult CheckAdminPermissions()
        {
            // 1. No logueado? -> redirige al login
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Index", "Login");

            // 2. No es Administrador? -> Acceso Denegado
            if (!_authService.HasAccessLevel("Administrador"))
                return RedirectToAction(nameof(AccesoDenegado)); 

            return null!; // Permiso concedido
        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            // Vista de error de acceso denegado (Punto 8)
            return View(); 
        }

        // --- CRUD PROTEGIDO (Acceso solo para Administrador) ---

        [HttpGet]
        public IActionResult Index()
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck; // Aplica la seguridad
            
            List<Producto> productos = _productoRepository.Listar(); // Uso de la interfaz DI
            return View(productos);
        }

        // --- CREATE ---

        [HttpGet]
        public IActionResult Create()
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;
            
            return View(new ProductoViewModel()); // Devuelve ViewModel
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductoViewModel productoVM) // Recibe ViewModel
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            if (!ModelState.IsValid)
            {
                return View(productoVM);
            }

            // Mapeo y FIX: Usamos ?? para asegurar que la Descripción nunca sea NULL.
            var nuevoProducto = new Producto
            {
                // FIX: Convierte null a string.Empty para evitar el error de base de datos
                Descripcion = productoVM.Descripcion ?? string.Empty, 
                Precio = (int)productoVM.Precio // Asume casteo a INT
            };

            _productoRepository.Crear(nuevoProducto);
            return RedirectToAction(nameof(Index));
        }

        // --- EDIT ---

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

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
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            if (!ModelState.IsValid) return View(productoVM);

            // Mapeo de VM a Modelo
            var productoAEditar = new Producto
            {
                IdProducto = productoVM.IdProducto,
                Descripcion = productoVM.Descripcion ?? string.Empty,
                Precio = (int)productoVM.Precio
            };

            _productoRepository.Modificar(id, productoAEditar);
            return RedirectToAction(nameof(Index));
        }

        // --- DELETE ---

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            Producto? producto = _productoRepository.ObtenerPorId(id);

            if (producto == null) return NotFound();

            return View(producto); // Retorna la vista de confirmación
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            _productoRepository.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}