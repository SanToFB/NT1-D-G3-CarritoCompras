using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarritoCompras_NT1.Models
{
    public class Cliente : Usuario
    {
        public override Rol Rol => Rol.Cliente;

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

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [RegularExpression(@"[0-9]{2}\.[0-9]{3}\.[0-9]{3}", ErrorMessage = "El dni debe tener un formato NN.NNN.NNN")]
        public string DNI { get; set; }

        public List<Compra> Compras { get; set; }

        public Carrito Carrito { get; set; }

    }
}
