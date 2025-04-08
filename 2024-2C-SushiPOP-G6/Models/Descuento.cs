using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_DESCUENTO")]
    public class Descuento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Range(0, 6, ErrorMessage = "El día debe estar entre {1} y {2}.")]
        public required int Dia { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Range(0, 50, ErrorMessage = "El porcentaje de descuentod debe estar entre {1} y {2}.")]
        public required int Porcentaje { get; set; } = 0;

        [Display(Name = "Descuento Maximo")]
        [Range(0, 3000, ErrorMessage = "El descuento máximo debe estar entre {1} y {2}.")]
        public decimal DescuentoMax { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public bool Activo { get; set; } = true;

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int ProductoId { get; set; }
        public Producto? Producto { get; set; }
    }
}
