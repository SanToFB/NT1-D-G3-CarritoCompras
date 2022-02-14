using CarritoCompras_NT1.DataBase;
using CarritoCompras_NT1.Extensions;
using CarritoCompras_NT1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarritoCompras_NT1.Controllers
{
    [Authorize(Roles = nameof(Rol.Administrador))]
    public class AdministradoresController : Controller
    {
        private readonly Contexto _context;

        public AdministradoresController(Contexto context)
        {
            _context = context;
        }

        // GET: Administradores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Administradores.ToListAsync());
        }

        // GET: Administradores/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }

        // GET: Administradores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administradores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Administrador administrador, string pass)
        {
            try
            {
                pass.ValidarPassword();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(Administrador.Password), e.Message);
            }
            if (_context.Administradores.Any(admin => admin.UserName == administrador.UserName) ||
                (_context.Empleados.Any(empleado => empleado.UserName == administrador.UserName)))
            {
                ModelState.AddModelError(nameof(Administrador.UserName), "El nombre de Ususario ya se encuentra utilizado");
            }


            if (ModelState.IsValid)
            {
                administrador.Id = Guid.NewGuid();
                administrador.FechaAlta = DateTime.Now;
                administrador.Password = pass.Encriptar();
                _context.Add(administrador);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(administrador);
        }

        // GET: Administradores/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradores.FindAsync(id);
            if (administrador == null)
            {
                return NotFound();
            }
            return View(administrador);
        }

        // POST: Administradores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Administrador administrador, string pass)
        {
            if (!string.IsNullOrWhiteSpace(pass))
            {
                try
                {
                    pass.ValidarPassword();
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(nameof(Administrador.Password), e.Message);
                }
            }

            if (id != administrador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Administrador adminBD = _context.Administradores.Find(id);

                    adminBD.Nombre = administrador.Nombre;
                    adminBD.Apellido = administrador.Apellido;
                    adminBD.Direccion = administrador.Direccion;
                    adminBD.Email = administrador.Email;
                    adminBD.Telefono = administrador.Telefono;

                    if (!string.IsNullOrWhiteSpace(pass))
                    {
                        adminBD.Password = pass.Encriptar();
                    }

                    _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministradorExists(administrador.Id))
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
            return View(administrador);
        }

        // GET: Administradores/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administradores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }

        // POST: Administradores/Delete/5   
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var administrador = await _context.Administradores.FindAsync(id);
            _context.Administradores.Remove(administrador);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool AdministradorExists(Guid id)
        {
            return _context.Administradores.Any(e => e.Id == id);
        }
    }
}
