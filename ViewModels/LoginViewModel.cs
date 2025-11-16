using System.ComponentModel.DataAnnotations;

namespace tl2_tp8_2025_ElAguhs.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El Usuario es obligatorio.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "La Contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }
    }
}