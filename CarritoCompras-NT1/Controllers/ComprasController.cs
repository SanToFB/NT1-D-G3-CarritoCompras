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
    public class ComprasController : Controller
    {
        private readonly Contexto _context;

        public ComprasController(Contexto context)
        {
            _context = context;
        }
        
        // GET: Compras
        public async Task<IActionResult> Index()
        {
            var contexto = _context.Compras.Include(c => c.Carrito).Include(c => c.Cliente);
            return View(await contexto.ToListAsync());
        }

        // GET: Compras Por Fecha
        public async Task<IActionResult> IndexFecha(DateTime? fechaDesde, DateTime fechaHasta)
        {
            var contexto = _context.Compras.Include(c => c.Carrito).Include(c => c.Cliente)
                .Where(c => c.FechaCompra >= fechaDesde && c.FechaCompra <= fechaHasta);

            if ( User.IsInRole("Cliente"))
            {
                var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                //var comprasCliente = contexto.Where(c => c.ClienteID == clienteId);
                contexto = contexto.Where(c => c.ClienteID == clienteId);

            }
            return View(await contexto.ToListAsync());
        }

        // GET: ConfirmarCompra
        public IActionResult ConfirmarCompra(Guid? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Cliente cliente = _context.Clientes.FirstOrDefault(c => c.Id == clienteId);
            // Controlamos que el carritoId recibido por parámetro sea del usuario y esté activo
            Carrito carrito = _context.Carritos.FirstOrDefault(c => c.ClienteID == clienteId && c.Activo);
            if(carrito == null)
            {
                return NotFound();
            }

            //Validamos Stock cuando se agrega un carritoItem.

            Compra compra = new Compra()
            {
                Id = Guid.NewGuid(),
                Cliente = cliente,
                Carrito = carrito,
                FechaCompra = DateTime.Now,
                Total = carrito.Subtotal,
               //Sucursal 
            };

            //_context.Add(compra);

            //

            return View();
        }



            // GET: Compras/Details/5
            public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // GET: Compras/Create
        public IActionResult Create()
        {
            ViewData["CarritoID"] = new SelectList(_context.Carritos, "Id", "Id");
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "Id", "Apellido");
            return View();
        }

        // POST: Compras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteID,CarritoID,Total")] Compra compra)
        {
            if (ModelState.IsValid)
            {
                compra.Id = Guid.NewGuid();
                _context.Add(compra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoID"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoID);
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "Id", "Apellido", compra.ClienteID);
            return View(compra);
        }

        // GET: Compras/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }
            ViewData["CarritoID"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoID);
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "Id", "Apellido", compra.ClienteID);
            return View(compra);
        }

        // POST: Compras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ClienteID,CarritoID,Total")] Compra compra)
        {
            if (id != compra.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompraExists(compra.Id))
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
            ViewData["CarritoID"] = new SelectList(_context.Carritos, "Id", "Id", compra.CarritoID);
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "Id", "Apellido", compra.ClienteID);
            return View(compra);
        }

        // GET: Compras/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var compra = await _context.Compras.FindAsync(id);
            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompraExists(Guid id)
        {
            return _context.Compras.Any(e => e.Id == id);
        }

        private bool hayStock(Guid sucursalId, Guid productoId)
        {
            Sucursal sucursal = _context.Sucursales.FirstOrDefault(s => s.Id == sucursalId);
            bool hay = false;
            foreach(StockItem stock in sucursal.StockItems)
            {
                if(stock.ProductoID == productoId)
                {
                    hay = true;
                    break;
                }
            }
            return hay;
        }


    }
}
