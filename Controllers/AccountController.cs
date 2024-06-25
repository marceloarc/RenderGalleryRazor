using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using NuGet.Protocol;
using RenderGallery.Util;
using RenderGalleyRazor.Models;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace RenderGalleyRazor.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration; // Injeta a configuração do appsettings.json
        private readonly DatabaseContext db;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, DatabaseContext db, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.db = db;
            _configuration = configuration;
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
                    return Json(new { Message = "Email não encontrado" });
                }

                User userId = db.Users.FirstOrDefault(x => x.Email == user.Email);

                if (userId.status == 0) // Ajustei aqui de userId.status para user.Status
                {
                    return BadRequest(new { Message = "Acesso negado" });
                }

                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, false);

                if (result.Succeeded)
                {
                    var token = GenerateJwtToken(userId);

                    var favoritosDoUsuario = db.Favoritos
                        .Where(f => f.user_id == userId.Id)
                        .Select(f => new
                        {
                            Id = f.Art.Id,
                            Name = f.Art.Arte,
                            Path = "http://" + Request.Host.ToString() + "/" + f.Art.Path,
                            Price = f.Art.Valor,
                            Tipo = f.Art.Tipo,
                            Quantidade = f.Art.Quantidade,
                            Categoria = f.Art.categoria_id,
                            Publicacao = f.Art.publi_id,
                            Artista = new
                            {
                                Id = f.Art.Publicacao.User.Id,
                                Nome = f.Art.Publicacao.User.Name,
                            }
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
                                Path = "http://" + Request.Host.ToString() + "/" + pp.Arte.Path,
                                Price = pp.Arte.Valor,
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
                            Path = "http://" + Request.Host.ToString() + "/" + pc.Arte.Path,
                            Price = pc.Arte.Valor,
                            Quantidade = pc.Quantidade,
                            Categoria = pc.Arte.categoria_id,
                            Artista = pc.Arte.Publicacao.User.Name,
                            Publicacao = pc.Arte.publi_id
                        })
                        .ToList();

                    var publicacoesDoUsuario = db.Publicacoes
                        .Where(p => p.User_id == userId.Id)
                        .SelectMany(p => p.Artes)
                        .Select(a => new
                        {
                            Id = a.Id,
                            Name = a.Arte,
                            Path = "http://" + Request.Host.ToString() + "/" + a.Path,
                            Price = a.Valor,
                            Tipo = a.Tipo,
                            Quantidade = a.Quantidade,
                            CategoriaId = a.categoria_id,
                            PublicacaoId = a.publi_id,
                            UserId = a.Publicacao.User_id,
                        })
                        .ToList();

                    // Filtra os ChatsAsUser1 para o usuário
                    var chatsAsUser1 = userId.ChatsAsUser1
                        .Where(chat => chat.user_two != userId.Id) // Filtra chats onde o user_two não é o userId
                        .Select(chat => new
                        {
                            chat_id = chat.chat_id,
                            user_chat = new
                            {
                                id = chat.user_two,
                                name = db.Users.Where(u => u.Id == chat.user_two).Select(u => u.Name).FirstOrDefault(),
                                pic = "http://" + Request.Host.ToString() + "/" + db.Users.Where(u => u.Id == chat.user_two).Select(u => u.Pic).FirstOrDefault()
                            },
                            time = chat.time,
                            status = chat.status,
                            messages = chat.Messages.Select(msg => new
                            {
                                user_from = msg.user_id_from,
                                user_to = msg.user_id_to,
                                id = msg.id,
                                msg_content = msg.msg_content,
                                user_id_from = msg.user_id_from,
                                user_id_to = msg.user_id_to,
                                dataHora = msg.dataHora,
                                visu_status = msg.visu_status,
                                chat_id = msg.chat_id
                            })
                        });

                    // Filtra os ChatsAsUser2 para o usuário
                    var chatsAsUser2 = userId.ChatsAsUser2
                        .Where(chat => chat.user_one != userId.Id) // Filtra chats onde o user_one não é o userId
                        .Select(chat => new
                        {
                            chat_id = chat.chat_id,
                            user_chat = new
                            {
                                id = chat.user_one,
                                name = db.Users.Where(u => u.Id == chat.user_one).Select(u => u.Name).FirstOrDefault(),
                                pic = "http://" + Request.Host.ToString() + "/" + db.Users.Where(u => u.Id == chat.user_one).Select(u => u.Pic).FirstOrDefault()
                            },
                            time = chat.time,
                            status = chat.status,
                            messages = chat.Messages.Select(msg => new
                            {
                                user_from = msg.user_id_from,
                                user_to = msg.user_id_to,
                                id = msg.id,
                                msg_content = msg.msg_content,
                                user_id_from = msg.user_id_from,
                                user_id_to = msg.user_id_to,
                                dataHora = msg.dataHora,
                                visu_status = msg.visu_status,
                                chat_id = msg.chat_id
                            })
                        });

                    // Combina os resultados de chatsAsUser1 e chatsAsUser2 em uma única lista
                    var allChats = chatsAsUser1.Concat(chatsAsUser2);
                    var NewMessages = 0;

                    foreach (var chat in allChats)
                    {
                        foreach (var message in chat.messages)
                        {
                            if (message.user_id_from != userId.Id)
                            {
                                if (message.visu_status == 0)
                                {
                                    NewMessages++;
                                }
                            }
                        }
                    }
                    // Constrói o objeto userInfo com as variáveis filtradas
                    var userInfo = new
                    {
                        Id = userId.Id,
                        Name = userId.Name,
                        Email = userId.Email,
                        Pic = "http://" + Request.Host.ToString() + "/" + userId.Pic,
                        Saldo = userId.Saldo,
                        Plano = userId.plano_id,
                        Favoritos = favoritosDoUsuario,
                        Pedidos = pedidosDoUsuario,
                        Carrinho = produtosCarrinhoDoUsuario,
                        Publicacoes = publicacoesDoUsuario,
                        Token = token,
                        Chats = allChats,
                        NewMessages = NewMessages
                    };

                    return Ok(userInfo);
                }
                else
                {
                    return Json(new { Message = "E-mail ou Senha incorretos" });
                }
            }

            return Json(new { Message = "E-mail ou Senha incorretos" });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Adicione quaisquer outros claims necessários aqui
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost("api/mobile/UserInfoAll")]
        public IActionResult UserInfoAll([FromBody] User user)
        {
            try
            {   
                var loggedInUser = db.Users.FirstOrDefault(u => u.Id == user.Id);
                if (loggedInUser == null)
                {
                    return BadRequest(new { Message = "Usuário não encontrado" });
                }

                var favoritosDoUsuario = db.Favoritos
                    .Where(f => f.user_id == loggedInUser.Id)
                    .Select(f => new
                    {
                        Id = f.Art.Id,
                        Name = f.Art.Arte,
                        Path = "http://" + Request.Host.ToString() + "/" + f.Art.Path,
                        Price = f.Art.Valor,
                        Tipo = f.Art.Tipo,
                        Quantidade = f.Art.Quantidade,
                        Categoria = f.Art.categoria_id,
                        Publicacao = f.Art.publi_id,
                        Artista = new
                        {
                            Id = f.Art.Publicacao.User.Id,
                            Nome = f.Art.Publicacao.User.Name,
                        }
                    })
                    .ToList();

                var pedidosDoUsuario = db.Pedidos
                    .Where(p => p.User_id == user.Id)
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
                            Path = "http://" + Request.Host.ToString() + "/" + pp.Arte.Path,
                            Price = pp.Arte.Valor,
                            Quantidade = pp.Quantidade,
                            Categoria = pp.Arte.categoria_id,
                            Publicacao = pp.Arte.publi_id
                        }).ToList()
                    })
                    .ToList();

                var produtosCarrinhoDoUsuario = db.Produtos
                    .Where(pc => pc.User_id == user.Id)
                    .Select(pc => new
                    {
                        IdProduto = pc.Arte.Id,
                        NomeProduto = pc.Arte.Arte,
                        Path = "http://" + Request.Host.ToString() + "/" + pc.Arte.Path,
                        Price = pc.Arte.Valor,
                        Quantidade = pc.Quantidade,
                        Categoria = pc.Arte.categoria_id,
                        Publicacao = pc.Arte.publi_id,
                        Artista = new
                        {
                            Id = pc.Arte.Publicacao.User.Id,
                            Nome = pc.Arte.Publicacao.User.Name,
                        }
                    })
                    .ToList();

                var publicacoesDoUsuario = db.Publicacoes
                    .Where(p => p.User_id == user.Id)
                    .SelectMany(p => p.Artes)
                    .Select(a => new
                    {
                        Id = a.Id,
                        Name = a.Arte,
                        Path = "http://" + Request.Host.ToString()+"/" + a.Path,
                        Price = a.Valor,
                        Tipo = a.Tipo,
                        Quantidade = a.Quantidade,
                        CategoriaId = a.categoria_id,
                        PublicacaoId = a.publi_id,
                        UserId = a.Publicacao.User_id,
                    })
                    .ToList();

               
                var chatsAsUser1 = loggedInUser.ChatsAsUser1
                    .Where(chat => chat.user_two != loggedInUser.Id) // Filtra chats onde o user_two não é o userId
                    .Select(chat => new
                    {
                        chat_id = chat.chat_id,
                        user_chat = new
                        {
                            id = chat.user_two,
                            name = db.Users.Where(u => u.Id == chat.user_two).Select(u => u.Name).FirstOrDefault(),
                            pic = "http://" + Request.Host.ToString() + "/" + db.Users.Where(u => u.Id == chat.user_two).Select(u => u.Pic).FirstOrDefault()
                        },
                        time = chat.time,
                        status = chat.status,
                        messages = chat.Messages.Select(msg => new
                        {
                            user_from = msg.user_id_from,
                            user_to = msg.user_id_to,
                            id = msg.id,
                            msg_content = msg.msg_content,
                            user_id_from = msg.user_id_from,
                            user_id_to = msg.user_id_to,
                            dataHora = msg.dataHora,
                            visu_status = msg.visu_status,
                            chat_id = msg.chat_id
                        })
                    });

                // Filtra os ChatsAsUser2 para o usuário
                var chatsAsUser2 = loggedInUser.ChatsAsUser2
                    .Where(chat => chat.user_one != loggedInUser.Id) // Filtra chats onde o user_one não é o userId
                    .Select(chat => new
                    {
                        chat_id = chat.chat_id,
                        user_chat = new
                        {
                            id = chat.user_one,
                            name = db.Users.Where(u => u.Id == chat.user_one).Select(u => u.Name).FirstOrDefault(),
                            pic = "http://" + Request.Host.ToString() + "/" + db.Users.Where(u => u.Id == chat.user_one).Select(u => u.Pic).FirstOrDefault()
                        },
                        time = chat.time,
                        status = chat.status,
                        messages = chat.Messages.Select(msg => new
                        {
                            user_from = msg.user_id_from,
                            user_to = msg.user_id_to,
                            id = msg.id,
                            msg_content = msg.msg_content,
                            user_id_from = msg.user_id_from,
                            user_id_to = msg.user_id_to,
                            dataHora = msg.dataHora,
                            visu_status = msg.visu_status,
                            chat_id = msg.chat_id
                        })
                    });

                // Combina os resultados de chatsAsUser1 e chatsAsUser2 em uma única lista
                var allChats = chatsAsUser1.Concat(chatsAsUser2);

                var NewMessages = 0;
                
                foreach (var chat in allChats)
                {
                    foreach(var message in chat.messages)
                    {
                        if(message.user_id_from != loggedInUser.Id)
                        {
                            if(message.visu_status == 0)
                            {
                                NewMessages++;
                            }
                        }
                    }
                }
                var userInfo = new
                {
                    Id = loggedInUser.Id,
                    Name = loggedInUser.Name,
                    Email = loggedInUser.Email,
                    Pic = "http://" + Request.Host.ToString() + "/" + loggedInUser.Pic,
                    Saldo = loggedInUser.Saldo,
                    Plano = loggedInUser.plano_id,
                    Favoritos = favoritosDoUsuario,
                    Pedidos = pedidosDoUsuario,
                    Carrinho = produtosCarrinhoDoUsuario,
                    Publicacoes = publicacoesDoUsuario,
                    Chats = allChats,
                    NewMessages = NewMessages
                };

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("api/mobile/GetUser/{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var publicacoesDoUsuario = db.Publicacoes
                .Where(p => p.User_id == id)
                .SelectMany(p => p.Artes)
                .Select(a => new
                {
                    Id = a.Id,
                    Name = a.Arte,
                    Path = "http://" + Request.Host.ToString() + "/" + a.Path,
                    Price = a.Valor,
                    Tipo = a.Tipo,
                    Quantidade = a.Quantidade,
                    CategoriaId = a.categoria_id,
                    PublicacaoId = a.publi_id,
                    User = a.Publicacao.User_id,
                })
                .ToList();

                var user = db.Users
                    .Where(a => a.Id == id)
                    .Select(a => new
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Path = "http://" + Request.Host.ToString() + "/" + a.Pic,
                        Publicacoes = publicacoesDoUsuario,
                    })
                    .FirstOrDefault();

                if (id != null)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound("Usuário não encontrado");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost("api/mobile/register")]
        public async Task<IActionResult> MobileRegister([FromForm] VMRegistro registro)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(registro.Email);

                if (existingUser != null)
                {
                    return Json(new { error = "Email já cadastrado" });
                }

                var newUser = new IdentityUser
                {
                    UserName = registro.Email,
                    Email = registro.Email
                };

                var createUserResult = await _userManager.CreateAsync(newUser, registro.Password);

                if (createUserResult.Succeeded)
                {
                    var path = "";
                    var name = "";

                    if (registro.File != null)
                    {
                        path = Functions.WriteFilePerfil(registro.File);
                        var fileName = Path.GetFileName(path);
                        name = "images/" + fileName;
                    }

                    if (name == "")
                    {
                        name = "images/user.jpg";
                    }

                    var user = new User
                    {
                        Name = registro.Nome,
                        Email = registro.Email,
                        Pic = name,
                        plano_id = 1,
                        Saldo = 0,
                        status = (User.tipo)1
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    await _userManager.AddToRoleAsync(newUser, "Artista");
                    await _signInManager.SignInAsync(newUser, isPersistent: false);

                    return Ok(new { success = "Usuário registrado com sucesso" });
                }
                else
                {
                    return Json(new { error = "Erro ao registrar usuário" });
                }
            }

            return Json(new { error = "Por favor preencha todos os campos" });
        }




    }
}
