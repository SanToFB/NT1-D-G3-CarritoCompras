using CarritoCompras_NT1.DataBase;
using CarritoCompras_NT1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarritoCompras_NT1.Controllers
{
    [Authorize(Roles = nameof(Rol.Cliente))]
    public class CarritosController : Controller
    {
        private readonly Contexto _context;

        public CarritosController(Contexto context)
        {
            _context = context;
        }


        //GET
        [Authorize(Roles = nameof(Rol.Cliente))]
        public IActionResult AgregarItem(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = _context.Productos.FirstOrDefault(prod => prod.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var carrito = _context.Carritos.FirstOrDefault(c => c.ClienteID == clienteId && c.Activo);
            if (carrito == null)
            {
                return NotFound();
            }

            // ver si ya tiene ese item cargado y ahi sumarle sino crear uno nuevo. 

            CarritoItem item = new CarritoItem()
            {
                Id = Guid.NewGuid(),
                ProductoID = producto.Id,
                Producto = producto,
                CarritoID = carrito.Id,
                ValorUnitario = producto.PrecioVigente,
                Cantidad = 1,
                Subtotal = producto.PrecioVigente * 1
            };
            
            var stock = _context.StockItems.Where(s => s.ProductoID == producto.Id).ToList();
            var cantidadTotal = 0;
            if (stock != null)
            {
                foreach (StockItem stockItem in stock)
                {
                    cantidadTotal += stockItem.Cantidad;
                }
            }
            ViewBag.CantidadTotal = cantidadTotal;
            
            ViewBag.Subtotal = item.Cantidad * item.ValorUnitario;
            return View(item);
        }

        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarItem(Guid? id, CarritoItem item)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var carrito = _context.Carritos.FirstOrDefault(c => c.Id == item.CarritoID && c.Activo);

                if (carrito == null)
                {
                    return NotFound();
                }
                

                var item2 = _context.CarritoItems
                    .Include(i => i.Carrito).ThenInclude(c => c.Cliente)
                    .Include(i=>i.Producto)
                    .FirstOrDefault(i => i.CarritoID == carrito.Id && i.ProductoID == id);

                var producto = _context.Productos.FirstOrDefault(prod => prod.Id == id);
               

                if (item.Cantidad <= 0)
                {
                    ViewBag.Error = "Debe seleccionar al menos una unidad para continuar.";
                    ViewBag.NombreProducto = producto.Nombre;
                    ViewBag.Reenvio = 1;
                    return View(item);
                }

                if (item2 != null)
                {
                    item2.Cantidad += item.Cantidad;
                    item2.Subtotal = producto.PrecioVigente * item2.Cantidad;
                    _context.Update(item2);
                    _context.SaveChanges();
                }
                else
                {
                    item.Id = Guid.NewGuid();
                    item.Subtotal = producto.PrecioVigente * item.Cantidad;
                    _context.CarritoItems.Add(item);
                }

                carrito.Subtotal = 0;
                
                var items = _context.CarritoItems
                    .Include(c => c.Carrito).ThenInclude(ca => ca.Cliente)
                    .Include(c => c.Producto)
                    .Where(c => c.CarritoID == carrito.Id);

                if (items != null)
                {
                    foreach (CarritoItem carritoItem in items)
                    {
                        carrito.Subtotal += carritoItem.Subtotal;
                    }
                }

                _context.Update(carrito);
                _context.SaveChanges();

                return RedirectToAction("IndexClientes", "Productos");
            }
            return View(item);
        }


        [Authorize(Roles = nameof(Rol.Cliente))]
        public IActionResult LimpiarCarrito(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (clienteId == null)
            {
                return NotFound();
            }

            Carrito carrito = _context.Carritos.FirstOrDefault(c => c.ClienteID == clienteId);
            if (carrito == null)
            {
                return NotFound();
            }

            var items = _context.CarritoItems.Where(i => i.CarritoID == id);
            ViewBag.CarritoID = carrito.Id;


            foreach(CarritoItem item in items)
            {
                carrito.Subtotal += item.Subtotal;
            }

            ViewBag.Total = carrito.Subtotal;
            return View(items);

            //ViewBag.Items = items;
        }

        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LimpiarCarritoConfirmado(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (clienteId == null)
            {
                return NotFound();
            }

            Carrito carrito = _context.Carritos.FirstOrDefault(c => c.ClienteID == clienteId);
            if (carrito == null)
            {
                return NotFound();
            }

            foreach (CarritoItem item in _context.CarritoItems.Where(i => i.CarritoID == id))
            {
                _context.CarritoItems.Remove(item);
            }


            ViewBag.CarritoId = carrito.Id;
            _context.SaveChanges();
            // Dejamos en 0 el subtotal del carrito
            carrito.Subtotal = 0;
            _context.Carritos.Update(carrito);
            _context.SaveChanges();

            return RedirectToAction("Index","CarritoItems");
        }






    }
}
