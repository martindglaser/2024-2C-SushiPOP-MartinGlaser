namespace _2024_2C_SushiPOP_G6.Models.ViewModels;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    public class DescuentoViewModel
    { 
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public int ? Dia { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Range(0, 50, ErrorMessage = "El porcentaje de descuentod debe estar entre {1} y {2}.")]
        public int ? Porcentaje { get; set; } = 0;


        [Display(Name = "Descuento Maximo")]
        [Range(0, 3000, ErrorMessage = "El descuento máximo debe estar entre {1} y {2}.")]
        public decimal DescuentoMax { get; set; } = 1000;

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public bool Activo { get; set; } = true;

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public int ProductoId { get; set; }

        public string ? NombreProducto { get; set; }

         public IEnumerable<SelectListItem>? Productos { get; set; } // Para crear o editar

}
