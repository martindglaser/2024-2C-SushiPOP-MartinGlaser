using System.ComponentModel.DataAnnotations;

namespace _2024_2C_SushiPOP_G6.Models.ViewModels
{
    public class Login
    {
        [Display(Name= "Correo electronico")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public string Email {  get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")]
        public string Clave { get; set; } = string.Empty;
    }
}
