using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RenderGallery.Util;
using RenderGalleyRazor.Models;

namespace RenderGalleyRazor.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly DatabaseContext db;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, DatabaseContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.db = db;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(VMRegistro registro)
        {
            if (ModelState.IsValid)
            {
                //Copia os dados do VMRegistro para o IdentityUser
                IdentityUser user = new IdentityUser();

                user.UserName = registro.Email;
                user.Email = registro.Email;



                var path = "";
                var name = "";
                if (registro.File != null)
                {
                    path = Functions.WriteFile(registro.File);
                    var fileName = Path.GetFileName(path);
                    name = "images/"+ fileName;
                }

                if (name == "")
                {
                    name = "images/user.jpg";
                }
                //Armazena os dados do usuário na tabela AspNetUsers
                IdentityResult result = await _userManager.CreateAsync(user, registro.Password);

                //Se o usuário foi criado com sucesso, faz o login atravez do signInManager
                if (result.Succeeded)
                {
                    User user1 = new User();
                    user1.Name = registro.Nome;
                    user1.Email = registro.Email;
                    user1.Pic = name;
                    db.Users.Add(user1);
                    db.SaveChanges();
                    await _userManager.AddToRoleAsync(user, "Artista");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "home");
                }

                //Se houver erros entrão inclui no ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Error", error.Description);
                }
            }
            ViewBag.btn = "register";
            return View();
        }



        //Login

        [HttpPost]
        public async Task<IActionResult> Login(VMLogin login)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "home");
                }

                ModelState.AddModelError("Error", "Login Inválido");
            }
            ViewBag.btn = "login";
            return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "home");
        }
    }
}
