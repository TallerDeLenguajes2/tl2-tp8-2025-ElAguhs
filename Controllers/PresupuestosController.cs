using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_ElAguhs.Models;
using tl2_tp8_2025_ElAguhs.ViewModels;
using tl2_tp8_2025_ElAguhs.Interfaces; // Inyección de Dependencias
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tl2_tp8_2025_ElAguhs.Controllers
{
    public class PresupuestosController : Controller
    {
        // Dependencias inyectadas por el constructor
        private readonly IPresupuestoRepository _presupuestosRepository;
        private readonly IProductoRepository _productoRepo;
        private readonly IAuthenticationService _authService;

        public PresupuestosController(IPresupuestoRepository presupuestoRepo,
                                     IProductoRepository productoRepo,
                                     IAuthenticationService authService)
        {
            _presupuestosRepository = presupuestoRepo;
            _productoRepo = productoRepo;
            _authService = authService;
        }

        // --- LÓGICA DE SEGURIDAD (HELPERS) ---

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        private IActionResult CheckReadPermissions()
        {
            // Chequeo de que el usuario esté logueado
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Index", "Login");

            // Permite acceso si es Administrador O Cliente
            if (!_authService.HasAccessLevel("Administrador") && !_authService.HasAccessLevel("Cliente"))
                return RedirectToAction(nameof(AccesoDenegado));

            return null!;
        }

        private IActionResult CheckWritePermissions()
        {
            // Chequeo de que el usuario esté logueado
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Index", "Login");

            // Permite acceso SOLO si es Administrador
            if (!_authService.HasAccessLevel("Administrador"))
                return RedirectToAction(nameof(AccesoDenegado));

            return null!;
        }

        // --- LECTURA PROTEGIDA (Index / Details) ---

        [HttpGet]
        public IActionResult Index()
        {
            var securityCheck = CheckReadPermissions();
            if (securityCheck != null) return securityCheck;
            
            List<Presupuesto> presupuestos = _presupuestosRepository!.Listar();
            return View(presupuestos);
        }

        [HttpGet]
        public IActionResult Details(int id) 
        {
            var securityCheck = CheckReadPermissions();
            if (securityCheck != null) return securityCheck;

            Presupuesto? presupuesto = _presupuestosRepository!.ObtenerPorId(id);

            if (presupuesto == null) return NotFound();

            return View(presupuesto);
        }

        // --- ESCRITURA PROTEGIDA (Create / Edit / Delete) ---

        [HttpGet]
        public IActionResult Create()
        {
            var securityCheck = CheckWritePermissions(); // SOLO ADMIN
            if (securityCheck != null) return securityCheck;

            return View(new PresupuestoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PresupuestoViewModel presupuestoVM)
        {
            var securityCheck = CheckWritePermissions();
            if (securityCheck != null) return securityCheck;

            // Validación de Regla de Negocio: La fecha no puede ser futura
            if (presupuestoVM.FechaCreacion.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("FechaCreacion", "La fecha de creación no puede ser futura.");
            }

            if (!ModelState.IsValid) return View(presupuestoVM);

            Presupuesto nuevoPresupuesto = new Presupuesto
            {
                NombreDestinatario = presupuestoVM.NombreDestinatario!,
                FechaCreacion = presupuestoVM.FechaCreacion
            };

            int newId = _presupuestosRepository!.Crear(nuevoPresupuesto);
            return RedirectToAction(nameof(Details), new { id = newId });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var securityCheck = CheckWritePermissions();
            if (securityCheck != null) return securityCheck;

            Presupuesto? presupuesto = _presupuestosRepository!.ObtenerPorId(id);
            if (presupuesto == null) return NotFound();

            // Mapeo de Entidad a ViewModel
            PresupuestoViewModel viewModel = new PresupuestoViewModel
            {
                IdPresupuesto = presupuesto.IdPresupuesto,
                NombreDestinatario = presupuesto.NombreDestinatario,
                FechaCreacion = presupuesto.FechaCreacion
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PresupuestoViewModel viewModel)
        {
            var securityCheck = CheckWritePermissions();
            if (securityCheck != null) return securityCheck;

            if (id != viewModel.IdPresupuesto) return NotFound();

            if (viewModel.FechaCreacion.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("FechaCreacion", "La fecha de creación no puede ser futura.");
            }

            if (!ModelState.IsValid) return View(viewModel);

            try
            {
                Presupuesto presupuestoActualizado = new Presupuesto
                {
                    IdPresupuesto = viewModel.IdPresupuesto,
                    NombreDestinatario = viewModel.NombreDestinatario!,
                    FechaCreacion = viewModel.FechaCreacion
                };

                _presupuestosRepository!.Modificar(id, presupuestoActualizado);
            }
            catch (Exception)
            {
                if (_presupuestosRepository!.ObtenerPorId(id) == null) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var securityCheck = CheckWritePermissions();
            if (securityCheck != null) return securityCheck;

            Presupuesto? presupuesto = _presupuestosRepository!.ObtenerPorId(id);
            if (presupuesto == null) return NotFound();
            return View(presupuesto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var securityCheck = CheckWritePermissions();
            if (securityCheck != null) return securityCheck;

            _presupuestosRepository!.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }

        // --- LÓGICA RELACIONAL (Agregar/Quitar Producto) ---

        [HttpGet]
        public IActionResult AgregarProducto(int idPresupuesto)
        {
            var securityCheck = CheckWritePermissions();
            if (securityCheck != null) return securityCheck;

            // FIX para CS0103: Inicializar el ViewModel dentro del método
            var productos = _productoRepo.Listar();
            var viewModel = new AgregarProductoViewModel 
            {
                IdPresupuesto = idPresupuesto,
                ListaProductos = new SelectList(productos, "IdProducto", "Descripcion")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarProducto(AgregarProductoViewModel model)
        {
            var securityCheck = CheckWritePermissions();
            if (securityCheck != null) return securityCheck;
            
            // Lógica Crítica de Recarga del SelectList
            if (!ModelState.IsValid)
            {
                var productos = _productoRepo.Listar();
                model.ListaProductos = new SelectList(productos, "IdProducto", "Descripcion");
                return View(model); 
            }

            _presupuestosRepository!.AgregarProductoDetalle(model.IdPresupuesto, model.IdProducto, model.Cantidad);
            return RedirectToAction(nameof(Details), new { id = model.IdPresupuesto });
        }

        [HttpGet]
        public IActionResult QuitarProducto(int idPresupuesto, int idProducto)
        {
            // Requiere permisos de escritura porque modifica la base de datos
            var securityCheck = CheckWritePermissions();
            if (securityCheck != null) return securityCheck;

            _presupuestosRepository!.QuitarProductoDetalle(idPresupuesto, idProducto);

            // Redirige al detalle del presupuesto
            return RedirectToAction(nameof(Details), new { id = idPresupuesto });
        }
    }
}