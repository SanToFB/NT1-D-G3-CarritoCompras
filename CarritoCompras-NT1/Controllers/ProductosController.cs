using CarritoCompras_NT1.DataBase;
using CarritoCompras_NT1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarritoCompras_NT1.Controllers
{
   
    public class ProductosController : Controller
    {
        private readonly Contexto _context;

        public ProductosController(Contexto context)
        {
            _context = context;
        }

        // GET: Productos
        //[Authorize(Roles = ("Administrador,Empleado"))]   NO DEJAR ESPACIO ENTRE LA COMA
        public async Task<IActionResult> Index()
        {
            var contexto = _context.Productos.Include(p => p.Categoria);

            return View(await contexto.ToListAsync());
        }

        //Get: Productos para clientes.y sin loguear cosa que los lleve al Login al seleccionar
        public async Task<IActionResult> IndexClientes()
        {
            var productos = _context.Productos.Include(p => p.Categoria)
             .Where(p => p.Activo);

            return View(await productos.ToListAsync());
        }

        // ver si hace falta aclarar: p.Activo = true;
        public IActionResult FiltrarCategoria (Guid? id)
        {
            if(id != null)
            {
                var producto = _context.Productos.Where(p => p.CategoriaID == id && p.Activo);

                ViewBag.Categorias = new SelectList(_context.Categorias, nameof(Categoria.Id), nameof(Categoria.Nombre), id);
                ViewBag.CategoriaId = id;

                return View(producto.ToList());
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Productos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        [Authorize(Roles = ("Administrador,Empleado"))]
        public IActionResult Create()
        {
            ViewData["CategoriaID"] = new SelectList(_context.Categorias, "Id", "Nombre", "Descripcion");
            return View();
        }

        // POST: Productos/Create
        //[Authorize(Roles = ("Administrador,Empleado"))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Activo,PrecioVigente,CategoriaID")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.Id = Guid.NewGuid();
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaID"] = new SelectList(_context.Categorias, "Id", "Descripcion", producto.CategoriaID);
            return View(producto);
        }

        // GET: Productos/Edit/5
        [Authorize(Roles = ("Administrador,Empleado"))]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaID"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaID);
            return View(producto);
        }

        // POST: Productos/Edit/5
        [Authorize(Roles = ("Administrador,Empleado"))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nombre,Descripcion,Activo,PrecioVigente,CategoriaID")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            ViewData["CategoriaID"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaID);
            return View(producto);
        }


        // GET: Productos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var producto = await _context.Productos.FindAsync(id);
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        private bool ProductoExists(Guid id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        /*
        private List<Producto> ProductosEnStock()
        {
            var stock = _context.StockItems.Where(s=>s.Cantidad > 0).ToList();
            List<Producto> productos = new List<Producto>();

            foreach(StockItem item in stock)
            {
                productos.Add(item.Producto);
            }

            return productos;   
        }
        */
    }
}
