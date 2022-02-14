using CarritoCompras_NT1.DataBase;
using CarritoCompras_NT1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarritoCompras_NT1.Controllers
{
    [Authorize]
    public class ComprasController : Controller
    {
        private readonly Contexto _context;

        public ComprasController(Contexto context)
        {
            _context = context;
        }

        // GET: Compras
        [Authorize(Roles = ("Administrador,Empleado"))]
        public async Task<IActionResult> Index()
        {
            var contexto = _context.Compras.Include(c => c.Carrito).Include(c => c.Cliente)
                .OrderByDescending(c => c.Total);

            ViewBag.Sucursales = _context.Sucursales;
            return View(await contexto.ToListAsync());
        }

        [Authorize(Roles = ("Administrador,Empleado"))]
        public async Task<IActionResult> IndexByApellido()
        {
            var contexto = _context.Compras.Include(c => c.Carrito).Include(c => c.Cliente)
                .OrderBy(c => c.Cliente.Apellido).ThenByDescending(c => c.Total);

            ViewBag.Sucursales = _context.Sucursales;
            return View("Index", await contexto.ToListAsync());
        }

        // GET: Compras Por Fecha
        [Authorize]
        public async Task<IActionResult> IndexFecha(DateTime? fechaDesde, DateTime fechaHasta)
        {
            var contexto = _context.Compras
                .Include(c => c.Carrito)
                .Include(c => c.Cliente)
                .OrderByDescending(c => c.Total)
                .Where(c => c.FechaCompra >= fechaDesde && c.FechaCompra <= fechaHasta);

            if (User.IsInRole("Cliente"))
            {
                var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                contexto = _context.Compras
                    .Include(c => c.Carrito)
                    .Include(c => c.Cliente)
                    .Where(c => c.ClienteID == clienteId);

                ViewBag.Cliente = _context.Clientes.Find(clienteId);
            }

            ViewBag.Sucursales = _context.Sucursales;
            return View(await contexto.ToListAsync());
        }

        // GET: Compra
        [Authorize(Roles = nameof(Rol.Cliente))]
        public IActionResult Compra(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Cliente cliente = _context.Clientes.FirstOrDefault(c => c.Id == clienteId);

            // Controlamos que el carritoId recibido por parámetro sea del usuario y esté activo

            Carrito carrito = _context.Carritos.FirstOrDefault(c => c.Id == id && c.Activo);
            if (carrito == null)
            {
                return NotFound();
            }


            carrito.Subtotal = 0;

            if (_context.CarritoItems.Any(item => item.CarritoID == id))
            {
                var items = _context.CarritoItems.Where(item => item.CarritoID == id);
                foreach (CarritoItem item in items)
                {
                    var prod = _context.Productos.Find(item.ProductoID);
                    item.ValorUnitario = prod.PrecioVigente;
                    item.Subtotal = item.ValorUnitario * item.Cantidad;
                    carrito.Subtotal += item.Subtotal;
                }
            }

            //Agregar el cliente en la compra tiene sentido? O solo con el ClienteId ya me alcanza...

            Compra compra = new Compra()
            {
                Id = Guid.NewGuid(),
                Cliente = cliente,
                ClienteID = clienteId,
                CarritoID = carrito.Id,
                Carrito = carrito,
                FechaCompra = DateTime.Now,
                Total = carrito.Subtotal,
            };

            ViewData["SucursalId"] = new SelectList(_context.Sucursales, nameof(Sucursal.Id), (nameof(Sucursal.Nombre)));
            ViewData["Carritoitems"] = _context.CarritoItems.Where(c => c.CarritoID == carrito.Id);

            return View(compra);
        }


        private Guid? IdStockEnOtraSucursal(Guid? id, CarritoItem item)
        {
            var stockOtrasSucursales = _context.StockItems.Include(s => s.Producto).Where(s => s.ProductoID == item.ProductoID &&
                      s.SucursalID != id).ToList();
            //ver si lo agarra en caso de null sino usar un try catch (ArgumentNullException) => where

            Guid? sucId = null;
            if (stockOtrasSucursales != null)
            {
                int cant = 0;
                foreach (StockItem stock in stockOtrasSucursales)
                {
                    if (stock.Cantidad >= item.Cantidad)
                    {
                        return stock.SucursalID;
                    }
                    else if (stock.Cantidad > cant)
                    {
                        cant = stock.Cantidad;
                        sucId = stock.SucursalID;
                    }
                }
            }
            return sucId;
        }
        //De las sucursales con stock devuelve la primera que cumpla con la cantidad o la que mas tiene.

        [Authorize(Roles = nameof(Rol.Cliente))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Compra(Guid? id, Compra compra)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                Carrito carrito = _context.Carritos.FirstOrDefault(c => c.ClienteID == clienteId && c.Activo);
                if (carrito == null)
                {
                    return NotFound();
                }

                Sucursal sucursal = _context.Sucursales.FirstOrDefault(s => s.Id == compra.SucursalId);
                if (sucursal == null)
                {
                    return NotFound();
                }

                var carritoItems = _context.CarritoItems.Where(c => c.CarritoID == id).ToList();

                if (carritoItems.Count <= 0)
                {
                    ViewBag.Error = "El carrito se encuentra vacio.";
                    ViewData["SucursalId"] = new SelectList(_context.Sucursales, nameof(Sucursal.Id), nameof(Sucursal.Nombre));
                    return View(compra);
                }

                foreach (CarritoItem item in carritoItems)
                {

                    var stockSuc = _context.StockItems.Include(s => s.Producto).FirstOrDefault(s => s.ProductoID == item.ProductoID &&
                       s.SucursalID == sucursal.Id);

                    if (stockSuc == null || stockSuc.Cantidad < item.Cantidad)
                    {
                        var OtraSucursalId = IdStockEnOtraSucursal(sucursal.Id, item);
                        string nombreProducto = _context.Productos.Find(item.ProductoID).Nombre;

                        if (OtraSucursalId != null)
                        {
                            var stockOtraSuc = _context.StockItems.FirstOrDefault(s => s.SucursalID == OtraSucursalId);
                            string nombreSucursal = _context.Sucursales.Find(OtraSucursalId).Nombre;

                            if (stockOtraSuc.Cantidad >= item.Cantidad)
                            {
                                ViewBag.Error = "Podra encontrar el producto: " + nombreProducto + " y la cantidad solicitada en: " + nombreSucursal;
                                ViewData["SucursalId"] = new SelectList(_context.Sucursales, nameof(Sucursal.Id), nameof(Sucursal.Nombre));
                                return View(compra);
                            }
                            else
                            {
                                ViewBag.Error = "Podra encontrar: " + stockOtraSuc.Cantidad + " unidades del producto: " + nombreProducto + " en: " + nombreSucursal;
                                ViewData["SucursalId"] = new SelectList(_context.Sucursales, nameof(Sucursal.Id), nameof(Sucursal.Nombre));
                                return View(compra);
                            }
                        }
                        else
                        {
                            ViewBag.Error = "Actualmente no contamos con Stock en nuestras sucursales para el producto: " + nombreProducto;
                            return View(compra);
                        }

                    }
                }


                //Si llegamos aca es por que ya quedaron todos los carritoItems con stock. Actualizamos Stock.

                foreach (CarritoItem item in carritoItems)
                {
                    var stockSuc = _context.StockItems.FirstOrDefault(s => s.ProductoID == item.ProductoID &&
                     s.SucursalID == compra.SucursalId);

                    stockSuc.Cantidad -= item.Cantidad;
                    _context.Update(stockSuc);
                    _context.SaveChanges();
                }

                Compra nuevaCompra = new Compra()
                {
                    Id = Guid.NewGuid(),
                    ClienteID = clienteId,
                    Carrito = carrito,
                    CarritoID = carrito.Id,
                    SucursalId = compra.SucursalId,
                    FechaCompra = DateTime.Now,
                    Total = carrito.Subtotal
                };
                //Agrego y guardo compra
                _context.Add(nuevaCompra);
                _context.SaveChanges();

                //Desactivo carrito y actualizo DB
                carrito.Activo = false;
                _context.Update(carrito);
                _context.SaveChanges();

                Carrito nuevoCarrito = new Carrito()
                {
                    Id = Guid.NewGuid(),
                    Activo = true,
                    ClienteID = clienteId,
                    Subtotal = 0
                };

                _context.Carritos.Add(nuevoCarrito);
                _context.SaveChanges();

                TempData["CompraClienteId"] = nuevaCompra.ClienteID;
                TempData["CompraSucursalId"] = nuevaCompra.SucursalId;
                TempData["CompraId"] = nuevaCompra.Id;

                return RedirectToAction("CompraRealizada", "Compras");
            }

            return View(compra);
        }

        //ver si funciona pasando directamente la sucursal.Si
        [Authorize]
        public IActionResult CompraRealizada()
        {
            Guid? compraClienteId = TempData["CompraClienteId"] as Guid?;
            Guid? compraSucursalId = TempData["CompraSucursalId"] as Guid?;
            Guid? compraId = TempData["CompraId"] as Guid?;
            ViewData["NombreCliente"] = _context.Clientes.Find(compraClienteId).Nombre;
            ViewData["NombreSucursal"] = _context.Sucursales.Find(compraSucursalId).Nombre;
            ViewData["DireccionSucursal"] = _context.Sucursales.Find(compraSucursalId).Direccion;
            ViewData["compraId"] = compraId;
            ViewData["Sucursal"] = _context.Sucursales.Find(compraSucursalId);
            return View();
        }

    }
}
