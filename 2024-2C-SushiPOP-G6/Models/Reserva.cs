using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_RESERVA")]
    public class Reserva
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required string Local { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Display(Name = "Fecha y hora: ")]
        public required DateTime FechaHora { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required bool Confirmada { get; set; } = false;

        // APARECEN EN DML Y NO EN DICT DE DATOS
        [AllowNull]
        [Display(Name = "Nombre: ")]
        public string Nombre { get; set; }
        [AllowNull]
        [Display(Name = "Apellido: ")]
        public  string Apellido { get; set; }
     

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
    }
}
