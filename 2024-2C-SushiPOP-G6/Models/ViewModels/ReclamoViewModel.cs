using System.ComponentModel.DataAnnotations;

namespace _2024_2C_SushiPOP_G6.Models.ViewModels
{
    public class ReclamoViewModel
    {
        
        public required string NombreCompleto { get; set; }
        public required string Email { get; set; }
        public required string Telefono { get; set; }
        public required string DetalleReclamo { get; set; }
        public required int PedidoId { get; set; }

    }
}
