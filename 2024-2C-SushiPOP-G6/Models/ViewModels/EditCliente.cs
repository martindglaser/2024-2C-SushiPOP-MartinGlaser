using System.ComponentModel.DataAnnotations;

namespace _2024_2C_SushiPOP_G6.Models.ViewModels
{
    public class EditCliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int NumeroCliente { get; set; }
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Direccion { get; set; } = string.Empty;
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(10, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(15, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public string Telefono { get; set; } = string.Empty;
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public bool Activo { get; set; } = true;
    }
}