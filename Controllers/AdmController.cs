using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using RenderGalleyRazor.Models;

namespace RenderGalleyRazor.Controllers
{
    public class AdmController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly DatabaseContext db;
        public AdmController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, DatabaseContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this.db = db;
        }

        public IActionResult Index()
        {
            int user_id = 0;
            List<User> users = db.Users.ToList();
            ViewBag.Users = users;

            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                user_id = user.Id;
            }
            ViewBag.user_id = user_id;
            List<Art> arts = db.Arts.ToList();
            ViewBag.Arts = arts;

            return View();
        }

        [HttpPost]
        public IActionResult editUser(VMEditar editar)
        {
            if (ModelState.IsValid)
            {
                // Obter usuário pelo Id

                var user = db.Users.FirstOrDefault(u => u.Id == editar.id_user);


                if (user != null)
                {
                    // Atualizar os dados do usuário com os novos valores do formulário
                    user.Name = editar.Nome;
                    user.Email = editar.Email;
                    user.Telefone = editar.Telefone;
                    user.plano_id = editar.plano_id;

                    // Salvar as alterações no banco de dados
                    db.SaveChanges();

                    ViewBag.success = true;
                    return View("Index");
                }
            }

            return View("Index");
        }


        [HttpPost]
        public IActionResult removeUser(VMEditar editar)
        {
            var userId = editar.id_user;

            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
           
            var identityUser = _userManager.FindByEmailAsync(user.Email).Result;

            var result = _userManager.DeleteAsync(identityUser).Result;
            if (!result.Succeeded)
            {
                // Lidar com o erro, se necessário
                return RedirectToAction("Error");
            }

            // Encontre e remova as mensagens associadas aos chats do usuário
            var messagesToRemove = db.Messages.Where(m => m.Chat.user_one == userId || m.Chat.user_two == userId);
            db.Messages.RemoveRange(messagesToRemove);

            // Encontre e remova os chats associados ao usuário
            var chatsToRemove = db.Chats.Where(c => c.user_one == userId || c.user_two == userId);
            db.Chats.RemoveRange(chatsToRemove);

            db.Users.Remove(user);
            db.SaveChanges();

            return View("Index");
        }


        [HttpPost]
        public IActionResult EditOrRemove(VMEditar editar)
        {
            var userId = editar.id_user;

            if (editar.action == "edit")
            {
                return editUser(editar);
            }
            else if (editar.action == "remove")
            {
                return removeUser(editar);
            }

            return View("Index");
        }


    }
}
