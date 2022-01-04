using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarritoCompras_NT1.Models
{
    public class Carrito
    {
        [Key]
        public Guid Id { get; set; }

        public bool Activo { get; set; }

        [ForeignKey(nameof(Cliente))]
        public Guid ClienteID { get; set; }
        public Cliente Cliente { get; set; }

        public List<CarritoItem> CarritoItems { get; set; }


        public float Subtotal { get; set; }



    }
}
