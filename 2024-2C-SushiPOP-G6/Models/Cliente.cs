using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_CLIENTES")]
    public class Cliente : Usuario
    {
       

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Display(Name = "Numero de cliente")]
        public required int NumeroCliente { get; set; } = 4200000;

        // Relaciones
        public List<Reserva>? Reservas { get; set; }
        public List<Carrito>? Carritos { get; set; }
    }
}
