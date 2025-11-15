using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace tl2_tp8_2025_ElAguhs.ViewModels 
{
    public class AgregarProductoViewModel
    {
        // ID del presupuesto al que se va a agregar (se envía desde la vista Details)
        public int IdPresupuesto { get; set; } 
        
        [Display(Name = "Producto a agregar")]
        [Required(ErrorMessage = "Debe seleccionar un producto.")]
        public int IdProducto { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        // Propiedad para que el Controlador cargue el dropdown de selección
        public SelectList? ListaProductos { get; set; }
    }
}