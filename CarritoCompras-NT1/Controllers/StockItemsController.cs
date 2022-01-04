using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarritoCompras_NT1.DataBase;
using CarritoCompras_NT1.Models;

namespace CarritoCompras_NT1.Controllers
{
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
            var contexto = _context.StockItems.Include(s => s.Producto).Include(s => s.Sucursal);
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
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", "Descripcion");
            ViewData["SucursalID"] = new SelectList(_context.Sucursales, "Id", "Direccion");
            return View();
        }

        // POST: StockItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SucursalID,ProductoID,Cantidad")] StockItem stockItem)
        {
            if (ModelState.IsValid)
            {
                stockItem.Id = Guid.NewGuid();
                _context.Add(stockItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", "Descripcion", stockItem.ProductoID);
            ViewData["SucursalID"] = new SelectList(_context.Sucursales, "Id", "Direccion", stockItem.SucursalID);
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
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", "Descripcion", stockItem.ProductoID);
            ViewData["SucursalID"] = new SelectList(_context.Sucursales, "Id", "Direccion", stockItem.SucursalID);
            return View(stockItem);
        }

        // POST: StockItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,SucursalID,ProductoID,Cantidad")] StockItem stockItem)
        {
            if (id != stockItem.Id)
            {
                return NotFound();
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
            ViewData["ProductoID"] = new SelectList(_context.Productos, "Id", "Descripcion", stockItem.ProductoID);
            ViewData["SucursalID"] = new SelectList(_context.Sucursales, "Id", "Direccion", stockItem.SucursalID);
            return View(stockItem);
        }

        // GET: StockItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
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

        // POST: StockItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
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
