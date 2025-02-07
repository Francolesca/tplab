using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tplab.Models;
using tplab.ViewsModel;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace tplab.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDbContext _context;
        public AccesoController(AppDbContext context) {
            _context = context;
        }
        [HttpGet]
        public IActionResult Registrarse()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public async Task<IActionResult> Registrarse(UsuarioVM model)
        {

            if(model.Password != model.ConfirmPassword)
            {
                ViewData["AlertMessage"] = "Las contraseñas no coinciden!!!";
                return View();
            }

            try
            {
                Usuario usuario = new Usuario()
                {
                    Username = model.Username,
                    Password = model.Password,
                };
                
                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync();

                if (usuario.Id != 0) return RedirectToAction("Login", "Acceso");
            }
            catch (Exception)
            {
                ViewData["AlertMessage"] = "Ha ocurrido un prblema, no se puedo crear el usuario.";
                return View();
                
            }
            return View();

        }
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public async Task<IActionResult> Login(LoginVM model)
        {
            Usuario? usuario = await _context.Usuarios
                .Where(u => u.Password == model.Password && u.Username == model.UserName)
                .FirstOrDefaultAsync();

            if(usuario == null)
            {
                ViewData["AlertMessage"] = "Ha ocurrido un prblema, no se puedo iniciar sesion.";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario.Username)
            };
            
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties =new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties
                );


            return RedirectToAction("Index", "Home");
        }


    }
}
