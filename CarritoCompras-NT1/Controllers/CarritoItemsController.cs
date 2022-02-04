using CarritoCompras_NT1.DataBase;
using CarritoCompras_NT1.Models;
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
        public async Task<IActionResult> Index(Guid? id)
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

            if (carrito== null)
            {
                return NotFound();
            }
            

            var items = _context.CarritoItems.Include(c => c.Carrito)
                .Include(c => c.Producto)
                .Where(c => c.CarritoID == carrito.Id);

            float total = 0;
            foreach (CarritoItem item in items)
            {
                total += item.Subtotal;
            }

            ViewBag.CarritoId = carrito.Id;
            ViewBag.Total = total;
            return View(await items.ToListAsync());
        }

        // GET: CarritoItems/Details/5
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
        }

        /*

        // GET: CarritoItems/Create
        public IActionResult Create()
        {
            ViewData["CarritoID"] = new SelectList(_context.Carritos, "Id", "Id");
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", "Descripcion");
            return View();
        }

        // POST: CarritoItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Activo,CarritoID,ProductoID,Cantidad,ValorUnitario,Subtotal")] CarritoItem carritoItem)
        {
            if (ModelState.IsValid)
            {
                carritoItem.Id = Guid.NewGuid();
                _context.Add(carritoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoID"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoID);
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoID);
            return View(carritoItem);
        }

        // GET: CarritoItems/Edit/5
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
            ViewData["CarritoID"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoID);
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoID);
            return View(carritoItem);
        }

        // POST: CarritoItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Activo,CarritoID,ProductoID,Cantidad,ValorUnitario,Subtotal")] CarritoItem carritoItem)
        {
            if (id != carritoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carritoItem);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoID"] = new SelectList(_context.Carritos, "Id", "Id", carritoItem.CarritoID);
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", "Descripcion", carritoItem.ProductoID);
            return View(carritoItem);
        }

        */

        // GET: CarritoItems/Delete/5
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
