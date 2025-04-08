using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2024_2C_SushiPOP_G6.Models
{
    [Table("T_USUARIO")]
    public abstract class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(30, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        [MinLength(5, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(30, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        [MinLength(5, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        public required string Apellido { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public required string Direccion { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        //[Range(10, 10, ErrorMessage = ErrorViewModel.ValorMinMax)] //-- ESTO ESTA MAL, RANGE ESPECIFICA RANGO DE POSIBLES NUMEROS
        [MinLength(10, ErrorMessage = ErrorViewModel.CaracteresMinimos)]
        [MaxLength(15, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public required string Telefono { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Display(Name = "Fecha de nacimiento")]
        public required DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [Display(Name = "Fecha de alta")]
        public required DateTime FechaAlta { get; set; } = DateTime.Now;

        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        public required bool Activo { get; set; } = true;

        [Display(Name = "Correo electronico")]
        [Required(ErrorMessage = ErrorViewModel.CampoRequerido)]
        [MaxLength(100, ErrorMessage = ErrorViewModel.CaracteresMaximos)]
        public required string Email { get; set; } // FALTA ESTO

        [NotMapped] // NotMapped es para que no se guarde en texto plano y la encripte
        [Display(Name = "Contraseña")]
        public required string? Clave { get; set; }
    }
}
