using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_ElAguhs.Models;
using tl2_tp8_2025_ElAguhs.Repositorios;
using System.Collections.Generic;

namespace tl2_tp8_2025_ElAguhs.Controllers
{
    public class PresupuestosController : Controller
    {
        // Declaración del campo privado (Plural y Nullable)
        private readonly PresupuestosRepository? _presupuestosRepository; 

        public PresupuestosController()
        {
            // Inicialización en el constructor
            _presupuestosRepository = new PresupuestosRepository();
        }

        // --- ETAPA I: LECTURA ---

        [HttpGet]
        public IActionResult Index()
        {
            List<Presupuesto> presupuestos = _presupuestosRepository!.Listar(); 
            return View(presupuestos);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Presupuesto? presupuesto = _presupuestosRepository!.ObtenerPorId(id);

            if (presupuesto == null)
            {
                return NotFound();
            }
            return View(presupuesto);
        }
        
        // --- ETAPA II: ESCRITURA (CREATE) ---

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Presupuesto());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult Create(Presupuesto presupuesto)
        {
            if (ModelState.IsValid)
            {
                int newId = _presupuestosRepository!.Crear(presupuesto); 
                
                // Redirige al Edit para que puedan agregar productos al presupuesto creado
                return RedirectToAction(nameof(Edit), new { id = newId });
            }
            return View(presupuesto); 
        }

        // --- ETAPA II: ESCRITURA (EDIT) ---
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Presupuesto? presupuesto = _presupuestosRepository!.ObtenerPorId(id);

            if (presupuesto == null)
            {
                return NotFound();
            }
            return View(presupuesto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Presupuesto presupuesto)
        {
            if (id != presupuesto.IdPresupuesto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _presupuestosRepository!.Modificar(id, presupuesto); 
                }
                catch (Exception)
                {
                    if (_presupuestosRepository!.ObtenerPorId(id) == null) 
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(presupuesto);
        }

        // --- ETAPA II: ESCRITURA (DELETE) ---

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Presupuesto? presupuesto = _presupuestosRepository!.ObtenerPorId(id);

            if (presupuesto == null)
            {
                return NotFound();
            }

            return View(presupuesto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _presupuestosRepository!.Eliminar(id); 
            
            return RedirectToAction(nameof(Index));
        }
    }
}