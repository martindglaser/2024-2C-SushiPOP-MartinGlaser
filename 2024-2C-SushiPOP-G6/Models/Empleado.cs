using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_EMPLEADO")]
    public class Empleado : Usuario
    {
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int Legajo { get; set; } = 99000;
    }
}