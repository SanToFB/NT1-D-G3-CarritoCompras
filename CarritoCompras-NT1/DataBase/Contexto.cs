using CarritoCompras_NT1.Models;
using Microsoft.EntityFrameworkCore;

namespace CarritoCompras_NT1.DataBase
{
    public class Contexto : DbContext
    {
        #region Constructor
        public Contexto(DbContextOptions<Contexto> opciones) : base(opciones)
        {

        }
        #endregion

        #region Propiedades

        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }

        #endregion



    }
}
