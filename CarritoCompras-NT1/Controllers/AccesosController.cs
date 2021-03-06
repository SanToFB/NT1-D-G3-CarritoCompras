using CarritoCompras_NT1.DataBase;
using CarritoCompras_NT1.Extensions;
using CarritoCompras_NT1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace CarritoCompras_NT1.Controllers
{
    [AllowAnonymous]
    public class AccesosController : Controller
    {
        private readonly Contexto _context;
        private const string _Return_Url = "ReturnUrl";

        public AccesosController(Contexto context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Ingresar(string returnUrl)
        {
            // Guardamos la url de retorno para que una vez concluído el login del 
            // usuario lo podamos redirigir a la página en la que se encontraba antes
            //El TempData dura dos request.
            TempData[_Return_Url] = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Ingresar(string username, string password, Rol rol)
        {
            string returnUrl = TempData[_Return_Url] as string;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                Usuario usuario = null;

                if (rol == Rol.Cliente)
                {
                    usuario = _context.Clientes.FirstOrDefault(cliente => cliente.UserName == username);
                    //Me parece que falta validacion aca creamos un carrito nuevo cada vez que se loguea?
                    //Carrito carrito = new Carrito()
                    
                   // TempData["CarritoId"] = carrito.Id;
                }
                else if (rol == Rol.Administrador)
                {
                    usuario = _context.Administradores.FirstOrDefault(administrador => administrador.UserName == username);
                }
                else /*if(rol == Rol.Empleado)*/
                {
                    usuario = _context.Empleados.FirstOrDefault(empleado => empleado.UserName == username);
                }

                if (usuario != null)
                {
                    var passwordEncriptada = password.Encriptar();

                    //Comparamos la password de la BD con la que se esta logueando.
                    if (usuario.Password.SequenceEqual(passwordEncriptada))
                    {
                        // Se crean las credenciales del usuario que serán incorporadas al contexto
                        ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                        // El lo que luego obtendré al acceder a User.Identity.Name
                        identity.AddClaim(new Claim(ClaimTypes.Name, username));

                        // Se utilizará para la autorización por roles
                        identity.AddClaim(new Claim(ClaimTypes.Role, rol.ToString()));

                        // Lo utilizaremos para acceder al Id del usuario que se encuentra en el sistema.
                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));

                        // Lo utilizaremos cuando querramos mostrar el nombre del usuario logueado en el sistema.
                        identity.AddClaim(new Claim(ClaimTypes.GivenName, usuario.Nombre));

                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        // En este paso se hace el login del usuario al sistema
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();

                        _context.SaveChanges();

                        //toast de bienvenida. En Layout.
                        TempData["LoggedIn"] = true;


                        if (!string.IsNullOrWhiteSpace(returnUrl))
                            return Redirect(returnUrl);

                        ViewBag.Productos = _context.Productos.Include(p => p.Categoria).ToList();

                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            ViewBag.UserName = username;
            TempData[_Return_Url] = returnUrl;

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Salir()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Denegado()
        {
            return View();
        }
    }
}
