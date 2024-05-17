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
        public IActionResult Login()
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
                        user1.Saldo = 0;
                        user1.status = (User.tipo)1;
                        db.Users.Add(user1);
                        db.SaveChanges();
                        await _userManager.AddToRoleAsync(user, "Artista");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        ViewBag.success = true;
                        return View("Login");
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
            return View("Login");
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
                    return View("Login");
                }

                User userId = db.Users.Where(x => x.Email == user.Email).FirstOrDefault();

                if (userId.status == 0)
                {
                    return RedirectToAction("AccessDenied", "Account");
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
            return View("Login");
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

                    var result = await _userManager.UpdateAsync(identityUser);

                    if (result.Succeeded)
                    {
                        // Atualiza a senha se foi fornecida
                        if (!string.IsNullOrEmpty(editar.NewPassword))
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


                        // Atualiza os dados 
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
                        ViewBag.success = true;
                        return View();
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(editar);
        }

        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost("api/mobile/login")]
        public async Task<IActionResult> MobileLogin([FromBody] VMLogin login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(login.Email);

                if (user == null)
                {
                    return BadRequest(new { Message = "Email não encontrado" });
                }

                User userId = db.Users.FirstOrDefault(x => x.Email == user.Email);

                if (userId.status == 0)
                {
                    return BadRequest(new { Message = "Acesso negado" });
                }

                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, false);

                if (result.Succeeded)
                {
                    var favoritosDoUsuario = db.Favoritos
                        .Where(f => f.user_id == userId.Id)
                        .Select(f => new
                        {
                            Id = f.Art.Id,
                            Name = f.Art.Arte,
                            Path = f.Art.Path,
                            Valor = f.Art.Valor,
                            Tipo = f.Art.Tipo,
                            Quantidade = f.Art.Quantidade,
                            Categoria = f.Art.categoria_id,
                            Publicacao = f.Art.publi_id
                        })
                        .ToList();

                    var pedidosDoUsuario = db.Pedidos
                        .Where(p => p.User_id == userId.Id)
                        .Select(p => new
                        {
                            IdPedido = p.Id,
                            Subtotal = p.sub_total,
                            Total = p.total,
                            Status = p.Status,
                            Produtos = p.Produtos.Select(pp => new
                            {
                                IdProduto = pp.Arte.Id,
                                NomeProduto = pp.Arte.Arte,
                                Path = pp.Arte.Path,
                                ValorProduto = pp.Arte.Valor,
                                Quantidade = pp.Quantidade,
                                Categoria = pp.Arte.categoria_id,
                                Publicacao = pp.Arte.publi_id
                            }).ToList()
                        })
                        .ToList();

                    var produtosCarrinhoDoUsuario = db.Produtos
                        .Where(pc => pc.User_id == userId.Id)
                        .Select(pc => new
                        {
                            IdProduto = pc.Arte.Id,
                            NomeProduto = pc.Arte.Arte,
                            Path = pc.Arte.Path,
                            ValorProduto = pc.Arte.Valor,
                            Quantidade = pc.Quantidade,
                            Categoria = pc.Arte.categoria_id,
                            Publicacao = pc.Arte.publi_id
                        })
                        .ToList();

                    var userInfo = new
                    {
                        Id = userId.Id,
                        Name = userId.Name,
                        Email = userId.Email,
                        Pic = userId.Pic,
                        Saldo = userId.Saldo,
                        Plano = userId.plano_id,
                        Favoritos = favoritosDoUsuario,
                        Pedidos = pedidosDoUsuario,
                        Carrinho = produtosCarrinhoDoUsuario,
                    };

                    return Ok(userInfo);
                }
                else
                {
                    return BadRequest(new { Message = "Senha incorreta" });
                }
            }

            var info = new
            {
                email = login.Email,
                password = login.Password,
            };

            return BadRequest(info);
        }




    }
}
