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
            ViewBag.Subtotal = item.Cantidad * item.ValorUnitario; 
            ViewBag.CantidadTotal = cantidadTotal;
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

                var item2 = _context.CarritoItems.FirstOrDefault(i => i.CarritoID == carrito.Id && i.ProductoID == id);
                var producto = _context.Productos.FirstOrDefault(prod => prod.Id == id);

                if (item2 != null)
                {
                    item2.Cantidad += item.Cantidad;
                    item2.Subtotal = producto.PrecioVigente * item2.Cantidad;
                    _context.Update(item2);
                }
                else
                {
                    item.Subtotal = producto.PrecioVigente * item.Cantidad;
                    _context.Add(item);
                }

                _context.Update(carrito);
                _context.SaveChanges();
                carrito.Subtotal = 0;
                if (carrito.CarritoItems != null)
                {
                    foreach (CarritoItem carritoItem in carrito.CarritoItems)
                    {
                        carrito.Subtotal += carritoItem.Subtotal;
                    }
                }


                return RedirectToAction("IndexClientes", "Productos");
            }
            return View(item);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
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

            Carrito carrito = _context.Carritos.FirstOrDefault(c => c.Id == clienteId);
            if (carrito == null)
            {
                return NotFound();
            }
            /*
            //_context.CarritoItems   PROBANDO QUERYS
            var query = from item in carrito.CarritoItems where (item.CarritoID == carrito.Id) select item ;
            foreach(CarritoItem item in query)
            {
                _context.CarritoItems.Remove(item);
            }
            _context.SaveChanges();
            */


            foreach (CarritoItem item in _context.CarritoItems.Where(i => i.CarritoID == id))
            {
                _context.CarritoItems.Remove(item);
            }
            //_context.SaveChanges();

            // Dejamos en 0 el subtotal del carrito
            carrito.Subtotal = 0;
            _context.Update(carrito);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");

        }

        /*
        // GET: Carritos
        public async Task<IActionResult> Index()
        {
            var contexto = _context.Carritos.Include(c => c.Cliente);
            return View(await contexto.ToListAsync());
        }

        // GET: Carritos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // GET: Carritos/Create
        public IActionResult Create()
        {
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "Id", "Apellido");
            return View();
        }

        // POST: Carritos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Activo,ClienteID,Subtotal")] Carrito carrito)
        {
            if (ModelState.IsValid)
            {
                carrito.Id = Guid.NewGuid();
                _context.Add(carrito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "Id", "Apellido", carrito.ClienteID);
            return View(carrito);
        }

        // GET: Carritos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos.FindAsync(id);
            if (carrito == null)
            {
                return NotFound();
            }
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "Id", "Apellido", carrito.ClienteID);
            return View(carrito);
        }

        // POST: Carritos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Activo,ClienteID,Subtotal")] Carrito carrito)
        {
            if (id != carrito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "Id", "Apellido", carrito.ClienteID);
            return View(carrito);
        }

        // GET: Carritos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var carrito = await _context.Carritos.FindAsync(id);
            _context.Carritos.Remove(carrito);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoExists(Guid id)
        {
            return _context.Carritos.Any(e => e.Id == id);
        }

        */

    }
}
