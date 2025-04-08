using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_CONTACTO")]
    public class Contacto
    {
        public int Id { get; set; }

        [Display(Name ="Nombre completo")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(255, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public required string NombreCompleto { get; set; }

        [Display(Name = "Correo electronico")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public required string Email { get; set; }

        [Display(Name = "Telefono")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(10, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        [MinLength(10, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        public string Telefono { get; set; } = string.Empty;
    
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(2000, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public required string Mensaje { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required bool Leido { get; set; } = false;
    }
}
