using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CarritoCompras_NT1.Models
{
    public class CarritoItem
    {
        [NotMapped]
        const string mensajeError = "El campo {0} es requerido";

        [Key]
        public Guid Id { get; set; }

        public bool Activo { get; set; }

        [ForeignKey(nameof(Producto))]
        public Guid ProductoID { get; set; }
        public Producto Producto { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [Range(0 , 100, ErrorMessage = "La {0} se debe encontrar entre {1} y {2}")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [Range(0, 999999.99, ErrorMessage = "El {0} se debe encontrar entre {1} y {2}")]
        [Display(Name ="Valor Unitario")]
        public float ValorUnitario { get; set; }

        public float Subtotal { get; set; }

    }
}
