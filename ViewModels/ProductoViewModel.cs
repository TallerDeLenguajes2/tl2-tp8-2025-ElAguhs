using System.ComponentModel.DataAnnotations;

namespace tl2_tp8_2025_ElAguhs.ViewModels 
{
    public class ProductoViewModel
    {
        public int IdProducto { get; set; } 

        [Display(Name = "Descripción del Producto")]
        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string? Descripcion { get; set; } // Opcional

        [Display(Name = "Precio Unitario")]
        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Precio { get; set; }
    }
}