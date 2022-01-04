using System;
using System.ComponentModel.DataAnnotations;

namespace CarritoCompras_NT1.Models
{
    public abstract class Usuario
    {

        const string mensajeError = "El campo {0} es requerido";

        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [MaxLength(30, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MinLength(2, ErrorMessage = "{0} debe tener un mínimo de {1} caracteres")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El campo {0} sólo admite caracteres alfabéticos")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Fecha de Alta")]
        [Required(ErrorMessage = mensajeError)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaAlta { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [MaxLength(20, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MinLength(6, ErrorMessage = "{0} debe tener un mínimo de {1} caracteres")]
        public string UserName { get; set; }
    }
}
