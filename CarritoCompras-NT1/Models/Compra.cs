using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CarritoCompras_NT1.Models
{
    public class Compra
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Cliente))]
        public Guid ClienteID { get; set; }
        public Cliente Cliente { get; set; }

        [ForeignKey(nameof(Carrito))]
        public Guid CarritoID { get; set; }
        public Carrito Carrito { get; set; }

        public float Total { get; set; }
    }
}
