using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_PRODUCTO")]
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(250, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        [MinLength(20, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required decimal Precio { get; set; }
        public string? Foto { get; set; } = "DEFAULT.png";

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Display(Name = "Stock")]
        public required int Strock { get; set; } = 100;

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required decimal Costo { get; set; }

        // Relaciones
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public List<Descuento>? Descuentos { get; set; }
        public List<ItemCarrito>? ItemCarritos { get; set; }
    }
}
