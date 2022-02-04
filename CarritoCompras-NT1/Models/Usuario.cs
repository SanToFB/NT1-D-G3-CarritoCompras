using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarritoCompras_NT1.Models
{
    public abstract class Usuario
    {
        [NotMapped]
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

        //[Required(ErrorMessage = mensajeError)]
        [Display(Name = "Fecha de Alta")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaAlta { get; set; }

        /* [Required(ErrorMessage = mensajeError)]
           [DataType(DataType.Password)] */

        [ScaffoldColumn(false)]
        [Display(Name = "Constraseña")]
        public byte[] Password { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [MaxLength(20, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MinLength(6, ErrorMessage = "{0} debe tener un mínimo de {1} caracteres")]
        public string UserName { get; set; }

        public abstract Rol Rol { get; }
    }
}
