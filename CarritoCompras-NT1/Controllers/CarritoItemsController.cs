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
    //[Authorize(Roles = nameof(Rol.Cliente))]
    public class CarritoItemsController : Controller
    {
        private readonly Contexto _context;

        public CarritoItemsController(Contexto context)
        {
            _context = context;
        }

        // GET: CarritoItems
        [Authorize(Roles = nameof(Rol.Cliente))]
        public IActionResult Index(Guid? id)
        {

            var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Carrito carrito = null;
            if (id == null)
            {
                carrito = _context.Carritos.FirstOrDefault(car => car.ClienteID == clienteId && car.Activo);
            }
            else
            {
                carrito = _context.Carritos.FirstOrDefault(car => car.ClienteID == clienteId && car.Id == id);
            }

            if (carrito == null)
            {
                return NotFound();
            }

            var items = _context.CarritoItems.Include(c => c.Carrito)
                .Include(c => c.Producto)
                .Where(c => c.CarritoID == carrito.Id).ToList();

            float total = 0;
            foreach (CarritoItem item in items)
            {
                var prod = _context.Productos.Find(item.ProductoID);
                item.ValorUnitario = prod.PrecioVigente;
                item.Subtotal = item.ValorUnitario * item.Cantidad;
                total += item.Subtotal;
            }

            ViewBag.CarritoId = carrito.Id;
            ViewBag.Total = total;
            return View(items);
        }

        /* // GET: CarritoItems/Details/5
         public async Task<IActionResult> Details(Guid? id)
         {
             if (id == null)
             {
                 return NotFound();
             }

             var carritoItem = await _context.CarritoItems
                 .Include(c => c.Carrito)
                 .Include(c => c.Producto)
                 .FirstOrDefaultAsync(m => m.Id == id);
             if (carritoItem == null)
             {
                 return NotFound();
             }

             return View(carritoItem);
         }*/

        //GET: CarritoItems/Edit/5
        [Authorize(Roles = nameof(Rol.Cliente))]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItems.FindAsync(id);

            if (carritoItem == null)
            {
                return NotFound();
            }

            var prod = _context.Productos.Find(carritoItem.ProductoID);
            carritoItem.ValorUnitario = prod.PrecioVigente;
            carritoItem.Subtotal = prod.PrecioVigente * carritoItem.Cantidad;
            //ViewData["CarritoID"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoID);
            //ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoID);
            return View(carritoItem);
        }

        //POST: CarritoItems/Edit/5
        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, CarritoItem carritoItem)
        {
            if (id != carritoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (carritoItem.Cantidad <= 0)
                {
                    ViewBag.Error = "El carrito se encuentra vacio.";
                    return View(carritoItem);
                }
                Producto producto = _context.Productos.Find(carritoItem.ProductoID);
                carritoItem.Subtotal = carritoItem.Cantidad * producto.PrecioVigente;

                try
                {
                    _context.Update(carritoItem);
                    _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoItemExists(carritoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                Carrito carrito = _context.Carritos.FirstOrDefault(c => c.Id == carritoItem.CarritoID);
                var items = _context.CarritoItems.Where(item => item.CarritoID == carrito.Id);
                foreach(CarritoItem item in items)
                {
                carrito.Subtotal += item.Subtotal;
                }

                _context.Update(carrito);
                _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            // ViewData["CarritoID"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoID);
            // ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoID);
            return View(carritoItem);
        }

        // GET: CarritoItems/Delete/5
        [Authorize(Roles = nameof(Rol.Cliente))]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItems
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        // POST: CarritoItems/Delete/5
        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            CarritoItem carritoItem = _context.CarritoItems.FirstOrDefault(item => item.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }
            Carrito carrito = _context.Carritos.FirstOrDefault(c => c.Id == carritoItem.CarritoID);
            if (carrito == null)
            {
                return NotFound();
            }

            // Descontamos el subtotal del item eliminado del subtotal del carrito
            carrito.Subtotal -= carritoItem.Subtotal;

            _context.Update(carrito);
            _context.CarritoItems.Remove(carritoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoItemExists(Guid id)
        {
            return _context.CarritoItems.Any(e => e.Id == id);
        }
    }
}
