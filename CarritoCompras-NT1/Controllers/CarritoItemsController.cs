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
    public class CarritoItemsController : Controller
    {
        private readonly Contexto _context;

        public CarritoItemsController(Contexto context)
        {
            _context = context;
        }

        // GET: CarritoItems
        public async Task<IActionResult> Index()
        {
            var contexto = _context.CarritoItems.Include(c => c.Carrito).Include(c => c.Producto);
            return View(await contexto.ToListAsync());
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
            var carritoItem = await _context.CarritoItems.FindAsync(id);
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
