﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RenderGallery.Models;
using RenderGalleyRazor.Models;

namespace RenderGalleyRazor.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(VMRegistro registro)
        {
            if (ModelState.IsValid)
            {
                //Copia os dados do VMRegistro para o IdentityUser
                var user = new IdentityUser
                {
                    UserName = registro.Email,
                    Email = registro.Email
                };

                //Armazena os dados do usuário na tabela AspNetUsers
                var result = await _userManager.CreateAsync(user, registro.Password);

                //Se o usuário foi criado com sucesso, faz o login atravez do signInManager
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "home");
                }

                //Se houver erros entrão inclui no ModelState
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("Index");
        }



        //Login

        [HttpPost]
        public async Task<IActionResult> Login(VMRegistroLogin login)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(login.login.Email, login.login.Password, login.login.RememberMe, false);

                if(result.Succeeded)
                {
                    return RedirectToAction("Index", "home");
                }

                ModelState.AddModelError(string.Empty, "Login Inválido");
            }

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "home");
        }
    }
}
