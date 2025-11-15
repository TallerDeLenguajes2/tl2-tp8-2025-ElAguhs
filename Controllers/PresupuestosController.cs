using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_ElAguhs.Models;
using tl2_tp8_2025_ElAguhs.Repositorios;
using tl2_tp8_2025_ElAguhs.ViewModels; 
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering; 
using System;
using System.Linq;

namespace tl2_tp8_2025_ElAguhs.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly PresupuestosRepository? _presupuestosRepository;
        // Repositorio de Productos para cargar el Dropdown de AgregarProducto
        private readonly ProductoRepository _productoRepo = new ProductoRepository(); 

        public PresupuestosController()
        {
            _presupuestosRepository = new PresupuestosRepository();
        }

        // --- LECTURA ---

        [HttpGet]
        public IActionResult Index()
        {
            List<Presupuesto> presupuestos = _presupuestosRepository!.Listar(); 
            return View(presupuestos);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            // MÉTODO CRÍTICO QUE SOLUCIONA EL ERROR CS0103
            Presupuesto? presupuesto = _presupuestosRepository!.ObtenerPorId(id);

            if (presupuesto == null) return NotFound();
            
            return View(presupuesto);
        }

        // --- ESCRITURA (CREATE) con ViewModel ---

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PresupuestoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PresupuestoViewModel viewModel)
        {
            // Validación de Regla de Negocio: La fecha de creación no puede ser futura.
            if (viewModel.FechaCreacion.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("FechaCreacion", "La fecha de creación no puede ser futura.");
            }

            if (ModelState.IsValid)
            {
                // Mapeo de ViewModel a Entidad (Model)
                Presupuesto nuevoPresupuesto = new Presupuesto
                {
                    NombreDestinatario = viewModel.NombreDestinatario!,
                    FechaCreacion = viewModel.FechaCreacion
                };

                int newId = _presupuestosRepository!.Crear(nuevoPresupuesto);
                
                // Redirigir a Details para ver el presupuesto recién creado
                return RedirectToAction(nameof(Details), new { id = newId });
            }
            
            return View(viewModel);
        }
        
        // --- ESCRITURA (EDIT) con ViewModel ---

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Presupuesto? presupuesto = _presupuestosRepository!.ObtenerPorId(id);
            if (presupuesto == null) return NotFound();

            // Mapeo de Entidad a ViewModel para la vista
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
            if (id != viewModel.IdPresupuesto) return NotFound();

            // Validación de Regla de Negocio: La fecha de creación no puede ser futura.
            if (viewModel.FechaCreacion.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("FechaCreacion", "La fecha de creación no puede ser futura.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Mapeo de ViewModel a Entidad (Model)
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
                
                // Redirigir a Details para ver los cambios
                return RedirectToAction(nameof(Details), new { id = id });
            }
            
            return View(viewModel);
        }
        
        // --- AGREGAR PRODUCTO (Many-to-Many) con ViewModel ---

        [HttpGet]
        public IActionResult AgregarProducto(int idPresupuesto)
        {
            if (_presupuestosRepository!.ObtenerPorId(idPresupuesto) == null) return NotFound();
            
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
            // Si la validación (DataAnnotations) falla
            if (!ModelState.IsValid)
            {
                // Recargar el SelectList antes de volver a mostrar la vista
                var productos = _productoRepo.Listar(); 
                model.ListaProductos = new SelectList(productos, "IdProducto", "Descripcion");
                return View(model); 
            }

            // Si es VÁLIDO: Llamar al repositorio para guardar la relación
            _presupuestosRepository!.AgregarProductoDetalle(model.IdPresupuesto, model.IdProducto, model.Cantidad);

            // Redirigir al detalle del presupuesto
            return RedirectToAction(nameof(Details), new { id = model.IdPresupuesto });
        }
        
        // --- ELIMINAR PRESUPUESTO COMPLETO ---

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Presupuesto? presupuesto = _presupuestosRepository!.ObtenerPorId(id);
            if (presupuesto == null) return NotFound();
            return View(presupuesto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _presupuestosRepository!.Eliminar(id); 
            
            return RedirectToAction(nameof(Index));
        }
        
        // --- ACCIÓN AUXILIAR PARA ELIMINAR UN DETALLE (Opcional) ---

        [HttpGet]
        public IActionResult QuitarProducto(int idPresupuesto, int idProducto)
        {
            // Asumiendo que has implementado QuitarProductoDetalle en tu repositorio
            // Si no lo tienes, puedes comentarlo o implementarlo.
            // _presupuestosRepository!.QuitarProductoDetalle(idPresupuesto, idProducto);
            return RedirectToAction(nameof(Details), new { id = idPresupuesto });
        }
    }
}