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
    [Authorize(Roles = ("Administrador,Empleado"))]
    public class SucursalesController : Controller
    {
        private readonly Contexto _context;

        public SucursalesController(Contexto context)
        {
            _context = context;
        }

        // GET: Sucursales
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sucursales.ToListAsync());
        }

        // GET: Sucursales/Details/5
        [Authorize(Roles = ("Administrador,Empleado"))]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursales
                .Include(s => s.StockItems)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }

        // GET: Sucursales/Create
        [Authorize(Roles = ("Administrador,Empleado"))]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sucursales/Create
        [Authorize(Roles = ("Administrador,Empleado"))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Email,Telefono,Direccion")] Sucursal sucursal)
        {
            if (ModelState.IsValid)
            {
                sucursal.Id = Guid.NewGuid();
                _context.Sucursales.Add(sucursal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sucursal);
        }

        // GET: Sucursales/Edit/5
        [Authorize(Roles = ("Administrador,Empleado"))]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
            {
                return NotFound();
            }
            return View(sucursal);
        }

        // POST: Sucursales/Edit/5
        [Authorize(Roles = ("Administrador,Empleado"))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nombre,Email,Telefono,Direccion")] Sucursal sucursal)
        {
            if (id != sucursal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sucursal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SucursalExists(sucursal.Id))
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
            return View(sucursal);
        }
        // HACER QUE EL CLIENTE TENGA ACCESO AL INDEX DE SUCURSALES PERO SIN VER BOTONES.
        // GET: Sucursales/Delete/5 VALIDAR QUE SI TIENE STOCK NO SE PUEDA Y PONER BOTON DE TRASFERIR STOCK
        [Authorize(Roles = ("Administrador,Empleado"))]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sucursal == null)
            {
                return NotFound();
            }

            return View(sucursal);
        }

        // POST: Sucursales/Delete/5
        [Authorize(Roles = ("Administrador,Empleado"))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var sucursal = await _context.Sucursales.FindAsync(id);
            if (sucursal == null)
            {
                return NotFound();
            }
            var stockItemsDeSucursal = _context.StockItems.Where(stock => stock.SucursalID == id);
            if (stockItemsDeSucursal.Count() > 0)
            {
                ViewData["ErrorStock"] = "El Stock debe estar en cero antes de eliminarla, puede trasferirlo a otra sucursal";
                return View(sucursal);
            }
            else
            {
            _context.Sucursales.Remove(sucursal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            }
        }


        [Authorize(Roles = ("Administrador,Empleado"))]
        public IActionResult TransferirStock()
        {
            ViewData["SucursalesId1"] = new SelectList(_context.Sucursales, "Id", "Nombre");
            ViewData["SucursalesId2"] = new SelectList(_context.Sucursales, "Id", "Nombre");
            return View();
        }

        [Authorize(Roles = ("Administrador,Empleado"))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TransferirStock(Guid? sucursal1Id, Guid? sucursal2Id)
        {
            if (sucursal1Id == null || sucursal2Id==null)
            {
                ViewBag.Error = "Debe seleccionar una sucursal";
                ViewData["SucursalesId1"] = new SelectList(_context.Sucursales, "Id", nameof(Sucursal.Nombre));
                ViewData["SucursalesId2"] = new SelectList(_context.Sucursales, "Id", "Nombre");
                return View();
            }

            if (sucursal1Id == sucursal2Id)
            {
                ViewBag.Error = "Debe seleccionar una sucursal destino distinta a la de origen";
                ViewData["SucursalesId1"] = new SelectList(_context.Sucursales, "Id", nameof(Sucursal.Nombre));
                ViewData["SucursalesId2"] = new SelectList(_context.Sucursales, "Id", "Nombre");
                return View();
            }

            var stockSucursal1 = _context.StockItems.Where(s => s.SucursalID == sucursal1Id);

            if( stockSucursal1 == null)
            {
                ViewBag.Error = "La sucursal seleccionada no tiene stock";
                ViewData["SucursalesId1"] = new SelectList(_context.Sucursales, "Id", nameof(Sucursal.Nombre));
                ViewData["SucursalesId2"] = new SelectList(_context.Sucursales, "Id", "Nombre");
                return View();
            }

            foreach( StockItem stock in stockSucursal1)
            {
                var cant = stock.Cantidad;
                var stockOtraSucursal = _context.StockItems
                    .FirstOrDefault(s => s.SucursalID == sucursal2Id && s.ProductoID == stock.ProductoID);

                if (stockOtraSucursal != null)
                {

                    _context.StockItems.Find(stockOtraSucursal.Id).Cantidad += cant;
                }
                else
                {
                    Sucursal destino =_context.Sucursales.Find(sucursal2Id);
                    StockItem nuevoStock = new StockItem()
                    {
                        Id = Guid.NewGuid(),
                        SucursalID = destino.Id,
                        ProductoID = stock.ProductoID,
                        Cantidad = stock.Cantidad
                    };
                    _context.Add(nuevoStock);
                    destino.StockItems.Add(nuevoStock);
                    _context.Update(destino);
                    _context.SaveChanges();
                }
                _context.Remove(stock);
            }
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        private bool SucursalExists(Guid id)
        {
            return _context.Sucursales.Any(e => e.Id == id);
        }





    }
}
