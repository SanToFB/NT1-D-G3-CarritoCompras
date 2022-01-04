using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarritoCompras_NT1.Models
{
    public class Sucursal
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

        [Required(ErrorMessage = mensajeError)]
        [Phone]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        public List<StockItem> StockItems { get; set; }
    }
}
