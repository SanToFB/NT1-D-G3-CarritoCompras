using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarritoCompras_NT1.Models
{
    public class Producto
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
        [MaxLength(50, ErrorMessage = "El campo {0} admite un máximo de {1} caracteres")]
        [MinLength(4, ErrorMessage = "{0} debe tener un mínimo de {1} caracteres")]
        public string Descripcion { get; set; }

        public bool Activo { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [Range(0, 999999.99, ErrorMessage = "El {0} se debe encontrar entre {1} y {2}")]
        [Display(Name = "Precio Vigente")]
        public float PrecioVigente { get; set; }

        [ForeignKey(nameof(Categoria))]
        [Display(Name = "Categoría")]
        public Guid CategoriaID { get; set; }
        public Categoria Categoria { get; set; }


    }
}
