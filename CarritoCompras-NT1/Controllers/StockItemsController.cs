using CarritoCompras_NT1.DataBase;
using CarritoCompras_NT1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarritoCompras_NT1.Controllers
{
    [Authorize(Roles = ("Administrador,Empleado"))]
    public class StockItemsController : Controller
    {
        private readonly Contexto _context;

        public StockItemsController(Contexto context)
        {
            _context = context;
        }

        // GET: StockItems
        public async Task<IActionResult> Index()
        {
            var contexto = _context.StockItems.Include(s => s.Producto).Include(s => s.Sucursal)
                .OrderBy(s => s.Sucursal.Nombre).ThenBy(s => s.Producto.Nombre);
            return View(await contexto.ToListAsync());
        }


        // GET: StockItems/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockItem = await _context.StockItems
                .Include(s => s.Producto)
                .Include(s => s.Sucursal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockItem == null)
            {
                return NotFound();
            }

            return View(stockItem);
        }

        // GET: StockItems/Create
        public IActionResult Create()
        {
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", nameof(Producto.Nombre));
            ViewData["SucursalID"] = new SelectList(_context.Sucursales, "Id", nameof(Sucursal.Nombre));
            return View();
        }

        // POST: StockItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StockItem stockItem)
        {
            var stockItem2 = _context.StockItems.FirstOrDefault(stock => stock.SucursalID == stockItem.SucursalID && stock.ProductoID == stockItem.ProductoID);
            
            if (stockItem.Cantidad < 1)
            {
                ModelState.AddModelError(nameof(StockItem.Cantidad), "La cantidad no puede ser menor a 1");
            }

            if (ModelState.IsValid)
            {
                if ( stockItem2 != null)
                {
                    stockItem2.Cantidad += stockItem.Cantidad;
                    _context.Update(stockItem2);
                    _context.SaveChangesAsync();
                }
                else
                {
                stockItem.Id = Guid.NewGuid();
                _context.Add(stockItem);
                _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", nameof(Producto.Nombre), stockItem.ProductoID);
            ViewData["SucursalID"] = new SelectList(_context.Sucursales, "Id", nameof(Sucursal.Nombre), stockItem.SucursalID);
            return View(stockItem);
        }

        // GET: StockItems/Edit/5  
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockItem = await _context.StockItems.FindAsync(id);
            if (stockItem == null)
            {
                return NotFound();
            }
            if (stockItem.Cantidad == 0)
            {
                var contexto = _context.StockItems.Include(s => s.Producto).Include(s => s.Sucursal)
                .OrderBy(s => s.Sucursal.Nombre).ThenBy(s => s.Producto.Nombre);
                TempData["ErrorStock"] = "No cuenta con Stock para modificar.";
                return View("Index",contexto);
            }

            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", nameof(Producto.Nombre), stockItem.ProductoID);
            ViewData["SucursalID"] = new SelectList(_context.Sucursales, "Id", nameof(Sucursal.Nombre), stockItem.SucursalID);
            return View(stockItem);
        }

        // POST: StockItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,SucursalID,ProductoID,Cantidad")] StockItem stockItem)
        {
            if (id != stockItem.Id)
            {
                return NotFound();
            }

            if (stockItem.Cantidad < 0)
            {
                ModelState.AddModelError(nameof(StockItem.Cantidad), "La cantidad no puede ser negativa");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockItemExists(stockItem.Id))
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
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", nameof(Producto.Nombre), stockItem.ProductoID);
            ViewData["SucursalID"] = new SelectList(_context.Sucursales, "Id", nameof(Sucursal.Nombre), stockItem.SucursalID);
            return View(stockItem);
        }



        // GET: StockItems/Delete/5
        [Authorize(Roles = nameof(Rol.Administrador))]
        public async Task<IActionResult> Delete(Guid? id, Sucursal sucursal)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stockItem = await _context.StockItems
                .Include(s => s.Producto)
                .Include(s => s.Sucursal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockItem == null)
            {
                return NotFound();
            }
            ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Direccion", stockItem.SucursalID);
            return View(stockItem);
        }

        // POST: StockItems/Delete/5
        [Authorize(Roles = nameof(Rol.Administrador))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, Guid sucursalId)
        {
            var stockItem = await _context.StockItems.FindAsync(id);
            _context.StockItems.Remove(stockItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        


        private bool StockItemExists(Guid id)
        {
            return _context.StockItems.Any(e => e.Id == id);
        }
    }
}
