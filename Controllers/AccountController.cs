using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using RenderGallery.Util;
using RenderGalleyRazor.Models;
using System.IO;

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
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "PasswordMismatch")
                        {
                            ModelState.AddModelError("Password", "Senha Incorreta.");
                        }
                        else if (error.Code == "PasswordRequiresLower")
                        {
                            ModelState.AddModelError("Password", "As senhas devem ter pelo menos uma letra minúscula ('a'-'z').");
                        }
                        else if (error.Code == "PasswordRequiresUpper")
                        {
                            ModelState.AddModelError("Password", "As senhas devem ter pelo menos uma letra maiúscula ('A'-'Z').");
                        }
                        else if (error.Code == "PasswordTooShort")
                        {
                            ModelState.AddModelError("Password", "As senhas devem ter pelo menos 5 caracteres.");
                        }
                        else
                        {
                            ModelState.AddModelError("Password", error.Code);
                        }
                    }
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
                var user = await _userManager.FindByEmailAsync(login.Email);

                if (user == null)
                {
                    ModelState.AddModelError("Email", "Email não encontrado.");
                    return View("Register");
                }

                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "home");
                }
                else
                {
                    if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError("Email", "Login não permitido.");
                    }
                    else if (result.IsLockedOut)
                    {
                        ModelState.AddModelError("Email", "A conta está bloqueada. Tente novamente mais tarde.");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Senha incorreta.");
                    }
                }
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

        [HttpGet]
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

        [HttpPost]
        public async Task<IActionResult> Editar(VMEditar editar)
        {
            if (ModelState.IsValid)
            {
                string userEmail = User.Identity.Name;
                IdentityUser identityUser = await _userManager.FindByEmailAsync(userEmail); 

                if (identityUser != null)
                {
                    // Atualiza os dados do IdentityUser
                    identityUser.Email = editar.Email;
                    identityUser.UserName = editar.Email;

                    var result = await _userManager.UpdateAsync(identityUser);

                    if (result.Succeeded)
                    {
                        // Atualiza a senha se foi fornecida
                        if (!string.IsNullOrEmpty(editar.Password))
                        {
                            var changePasswordResult = await _userManager.ChangePasswordAsync(identityUser, editar.Password, editar.NewPassword);

                            if (!changePasswordResult.Succeeded)
                            {
                                foreach (var error in changePasswordResult.Errors)
                                {
                                    if (error.Code == "PasswordMismatch")
                                    {
                                        ModelState.AddModelError("Password", "Senha Incorreta.");
                                    }
                                    else if(error.Code == "PasswordRequiresLower")
                                    {
                                        ModelState.AddModelError("NewPassword", "As senhas devem ter pelo menos uma letra minúscula ('a'-'z').");
                                    }
                                    else if (error.Code == "PasswordRequiresUpper")
                                    {
                                        ModelState.AddModelError("NewPassword", "As senhas devem ter pelo menos uma letra maiúscula ('A'-'Z').");
                                    }
                                    else if (error.Code == "PasswordTooShort")
                                    {
                                        ModelState.AddModelError("NewPassword", "As senhas devem ter pelo menos 5 caracteres.");
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("NewPassword", error.Code);
                                    }
                                }
                                return View(editar);
                            }
                        }


                        // Atualiza os dados do seu modelo de usuário personalizado (caso necessário)
                        User user = db.Users.FirstOrDefault(u => u.Email == userEmail);
                        if (user != null)
                        {
                            user.Name = editar.Nome;
                            user.Telefone = editar.Telefone;
                            var path = "";
                            var name = "";
                            if (editar.File != null)
                            {
                                path = Functions.WriteFilePerfil(editar.File);
                                var fileName = Path.GetFileName(path);
                                name = "images/" + fileName;
                                user.Pic = name;
                            }
                            db.SaveChanges();
                        }

                        // Redireciona para alguma página de confirmação/sucesso
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // Se ocorrerem erros de validação ou outras falhas, retorna para a view de edição com os erros
            return View(editar);
        }

    }
}
