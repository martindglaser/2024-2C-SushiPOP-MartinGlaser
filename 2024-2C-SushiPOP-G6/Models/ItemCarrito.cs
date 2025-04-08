using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_ITEMCARRITO")]
    public class ItemCarrito
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required decimal PrecioUnitConDto { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int Cantidad { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int CarritoId { get; set; }
        public Carrito? Carrito { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int ProductoId { get; set; }
        public List<Producto>? Productos { get; set; }
    }
}
