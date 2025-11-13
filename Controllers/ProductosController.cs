using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_ElAguhs.Models;
using tl2_tp8_2025_ElAguhs.Repositorios;

namespace tl2_tp8_2025_ElAguhs.Controllers
{
    public class ProductosController : Controller
    {
        // Instancia del repositorio de Productos
        private readonly ProductoRepository _productoRepository;

        public ProductosController()
        {
            // Inicialización del repositorio
            _productoRepository = new ProductoRepository();
        }

        // --- ETAPA I: LECTURA ---
        // GET: /Productos
        [HttpGet]
        public IActionResult Index()
        {
            // Nota: Asume que el método en tu ProductoRepository es GetAll()
            List<Producto> productos = _productoRepository.Listar();
            return View(productos);
        }

        // --- ETAPA II: ESCRITURA (CREATE) ---

        // GET: /Productos/Create (Muestra el formulario)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Productos/Create (Recibe y guarda el formulario)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                // Nota: Asume que el método en tu ProductoRepository es Crear()
                _productoRepository.Crear(producto);
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }
        // GET: /Productos/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Corregido: Usamos ObtenerPorId
            Producto? producto = _productoRepository.ObtenerPorId(id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: /Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Producto producto)
        {
            // ... (código de verificación y validación) ...

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificado: El método se llama Modificar y recibe (id, producto)
                    _productoRepository.Modificar(id, producto);
                }
                catch (Exception)
                {
                    // Verificación de existencia del producto
                    if (_productoRepository.ObtenerPorId(id) == null)
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }
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