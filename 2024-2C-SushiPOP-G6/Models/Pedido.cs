
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_PEDIDO")]
    public class Pedido
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int NroPedido { get; set; } = 30000;

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required DateTime FechaCompra { get; set; } = DateTime.Now;

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required decimal Subtotal { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required decimal GastoEnvio { get; set; } = 80;

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required decimal Total { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int Estado { get; set; } = 1;

 

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required int CarritoId { get; set; }
        public Carrito? Carrito { get; set; }
        public Reclamo? Reclamo { get; set; }    

    }
}
