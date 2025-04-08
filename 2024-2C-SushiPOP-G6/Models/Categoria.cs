using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_CATEGORIA")]
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public required string Nombre { get; set; }

        public string? Descripcion { get; set; }

        // Relaciones
        public List<Producto>? Productos { get; set; }
    }
}
