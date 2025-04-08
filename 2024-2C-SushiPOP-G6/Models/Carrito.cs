using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_CARRITO")]
    public class Carrito
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required bool Procesado { get; set; } = false;

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required bool Cancelado { get; set; } = false;

        // Relaciones
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public List<ItemCarrito>? ItemsCarrito { get; set; }
    }
}
