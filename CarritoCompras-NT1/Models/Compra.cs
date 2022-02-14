    using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarritoCompras_NT1.Models
{
    public class Compra
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Cliente")]
        [ForeignKey(nameof(Cliente))]
        public Guid ClienteID { get; set; }
        public Cliente Cliente { get; set; }

        [ForeignKey(nameof(Carrito))]
        public Guid CarritoID { get; set; }
        public Carrito Carrito { get; set; }

        public float Total { get; set; }

        public DateTime FechaCompra { get; set; }

        [Display(Name = "Sucursal")]
        [ForeignKey(nameof(Sucursal))]
        public Guid SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }
    }
}
