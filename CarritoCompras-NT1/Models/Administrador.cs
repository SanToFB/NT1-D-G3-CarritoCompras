﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarritoCompras_NT1.Models
{
    public class Administrador : Usuario
    {
        public override Rol Rol => Rol.Administrador;

        [NotMapped]
        const string mensajeError = "El campo {0} es requerido";


        [Required(ErrorMessage = mensajeError)]
        [MaxLength(30, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MinLength(2, ErrorMessage = "{0} debe tener un mínimo de {1} caracteres")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El campo {0} sólo admite caracteres alfabéticos")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [Phone]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

    }
}
