using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_RECLAMO")]
    public class Reclamo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(255, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        [Display(Name = "Nombre Completo:")]
        public required string NombreCompleto { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Display(Name = "Correo electrónico:")]
        public required string Email { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
       
        [Display(Name = "Teléfono:")]
        public required string Telefono { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MinLength(50, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [Display(Name = "Detalle: ")]
        public required string DetalleReclamo { get; set; }


        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Display(Name = "Numero de pedido: ")]
        public required int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }
    }
}
