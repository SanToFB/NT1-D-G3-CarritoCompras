using CarritoCompras_NT1.DataBase;
using CarritoCompras_NT1.Extensions;
using CarritoCompras_NT1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CarritoCompras_NT1.Controllers
{

    public class ClientesController : Controller
    {
        private readonly Contexto _context;

        public ClientesController(Contexto context)
        {
            _context = context;
        }

        // GET: Clientes
        public IActionResult Index()
        {
            return View(_context.Clientes.ToList());
        }

        // GET: Clientes/Details/5          
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = _context.Clientes
                .FirstOrDefault(m => m.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente, string pass)
        {
                try
                {
                    pass.ValidarPassword();
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(nameof(Cliente.Password), e.Message);
                }

            if (_context.Empleados.Any(emple => emple.UserName == cliente.UserName) ||
               (_context.Clientes.Any(clien => clien.UserName == cliente.UserName)) ||
               (_context.Administradores.Any(admin => admin.UserName == cliente.UserName)))
            {
                ModelState.AddModelError(nameof(cliente.UserName), "El nombre de Usuario ya se encuentra utilizado");
            }

            if (ModelState.IsValid)
            {
                cliente.Id = Guid.NewGuid();
                cliente.FechaAlta = DateTime.Now;
                cliente.Password = pass.Encriptar();
                cliente.Carrito = new Carrito()
                {
                    Id = Guid.NewGuid(),
                    ClienteID = cliente.Id,
                    Activo = true,
                    Subtotal = 0
                };
                _context.Add(cliente);
                _context.Add(cliente.Carrito);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }



        // GET: Clientes/Edit/5

       // [Authorize(Roles = ("Administrador, Cliente"))]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }


       // [Authorize(Roles = ("Administrador, Cliente"))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Cliente cliente, string pass)
        {
            if (!string.IsNullOrWhiteSpace(pass))
            {
                try
                {
                    pass.ValidarPassword();
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(nameof(Cliente.Password), e.Message);
                }
            }

            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.nombre = cliente.Nombre;
                    ViewBag.apellido = cliente.Apellido;
                    ViewBag.userName = cliente.UserName;

                    var clienteBD = _context.Clientes.FirstOrDefault(c => c.Id == id);

                    clienteBD.Email = cliente.Email;
                    clienteBD.Telefono = cliente.Telefono;
                    clienteBD.Direccion = cliente.Direccion;

                    if (!string.IsNullOrWhiteSpace(pass))
                    {
                        clienteBD.Password = pass.Encriptar();
                    }

                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = _context.Clientes
                .FirstOrDefault(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var cliente = _context.Clientes.Find(id);
            _context.Clientes.Remove(cliente);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(Guid id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
