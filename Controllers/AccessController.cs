using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RenderGalleyRazor.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RenderGalleyRazor.Controllers
{
    [Route("login")]
    [ApiController]
    public class AccessController : Controller
    {

        private readonly DatabaseContext db;

        public AccessController(DatabaseContext rendergalleryContext)
        {
            db = rendergalleryContext;
        }


        [HttpPost]
        public async Task<IActionResult> Login(VMLogin modelLogin)
        {
            User usuario = db.Users.FirstOrDefault(a => a.Email == modelLogin.Email);

            if (usuario == null)
            {
                TempData["erro"] = "Usuário ou senha inválidos";
                return Json(TempData);
            }
            if (BCrypt.Net.BCrypt.Verify(modelLogin.Password, usuario.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Email),
                    new Claim(ClaimTypes.Sid, usuario.Id.ToString())
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = modelLogin.RememberMe
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);

                var userData = new
                {
                    access = 3,
                    name = "",
                    role = "Visitante",
                    user_id = 0,
                    email = "",

   
                };

                if (usuario.Usuario == Models.User.TipoUsuario.Administrador)
                {
                userData = new
                {
               
                        access = (int)Models.User.TipoUsuario.Administrador,
                    name = usuario.Name,
                    role = "Administrador",
                        user_id = usuario.Id,
                        email = usuario.Email,
                    };
                }
                else if (usuario.Usuario == Models.User.TipoUsuario.Cliente)
                {
                    userData = new
                    {
                        access = (int)Models.User.TipoUsuario.Cliente,
                        name = usuario.Name,
                        role = "Cliente",
                        user_id = usuario.Id,
                        email = usuario.Email,
                    };
                }
                else if (usuario.Usuario == Models.User.TipoUsuario.Artista)
                {
                    userData = new
                    {
                        access = (int)Models.User.TipoUsuario.Artista,
                        name = usuario.Name,
                        role = "Artista",
                        user_id = usuario.Id,
                        email = usuario.Email,
                    };
                }
                return Json(userData);
            }
            else
            {
                TempData["erro"] = "Usuário ou senha inválidos";
                return Json(TempData);
            }
        }

        public async Task<IActionResult> Logoff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acess");
        }


    }
}
