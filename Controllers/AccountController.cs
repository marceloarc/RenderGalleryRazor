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
            ViewBag.success = false;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(VMRegistro registro)
        {
            ViewBag.success = false;
            if (ModelState.IsValid)
            {
                //Copia os dados do VMRegistro para o IdentityUser
                IdentityUser user = new IdentityUser();

                user.UserName = registro.Email;
                user.Email = registro.Email;

                User userExists = db.Users.Where(x => x.Email == registro.Email).FirstOrDefault();

                if(userExists != null) {
                    ModelState.AddModelError("Email", "Email já cadastrado!");

                }
                else
                {

                

                var path = "";
                var name = "";
                if (registro.File != null)
                {
                    path = Functions.WriteFilePerfil(registro.File);
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
                    user1.plano_id = 1;
                    db.Users.Add(user1);
                    db.SaveChanges();
                    await _userManager.AddToRoleAsync(user, "Artista");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    ViewBag.success = true;
                    return View();
                }



                //Se houver erros entrão inclui no ModelState
                if (result.Errors != null)
                {
                    ModelState.AddModelError("Error", "A senha deve possuir mais de 6 caracteres, uma letra minúscula, uma maiúscula e um caractere especial.");

                }
            }
            }
            ViewBag.btn = "register";
            return View();
        }



        //Login

        [HttpPost]
        public async Task<IActionResult> Login(VMLogin login)
        {
            ViewBag.success = false;
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


        public IActionResult Editar()
        {
            int user_id = 0;

            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                user_id = user.Id;
            }
            ViewBag.user_id = user_id;

            return View();
        }
    }
}
