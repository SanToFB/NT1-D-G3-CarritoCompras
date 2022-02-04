using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarritoCompras_NT1.Models
{
    public class StockItem
    {
        [NotMapped]
        const string mensajeError = "El campo {0} es requerido";

        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Sucursal))]
        public Guid SucursalID { get; set; }
        public Sucursal Sucursal { get; set; }

        [ForeignKey(nameof(Producto))]
        public Guid ProductoID { get; set; }
        public Producto Producto { get; set; }

        [Required(ErrorMessage = mensajeError)]
        [Range(0, 100, ErrorMessage = "La {0} se debe encontrar entre {1} y {2}")]
        public int Cantidad { get; set; }
    }
}
