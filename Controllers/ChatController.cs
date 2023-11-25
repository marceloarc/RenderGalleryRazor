using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderGalleyRazor.Models;

namespace RenderGallery.Controllers
{

    public class ChatController : Controller
    {
        private readonly DatabaseContext db;

        public ChatController(DatabaseContext db) 
        {
            this.db = db;
        }


        public IActionResult Chat([FromQuery(Name = "to")] int to, int Id)
        {
            Console.WriteLine(to + " " + Id);
            List<Categoria> categorias = db.Categorias.ToList();
            ViewBag.Categorias = categorias;
            if (Id > 0)
            {
                var userDetails = db.Users.Where(x => x.Id == Id).FirstOrDefault();

                

                Chat conversa = db.Chats.Where((x => x.user_one == Id && x.user_two == to)).FirstOrDefault();

                if(conversa == null)
                {
                    conversa = db.Chats.Where((x => x.user_one == to && x.user_two == Id)).FirstOrDefault();
                    if (conversa == null)
                    {
                        conversa = new Chat();
                        conversa.user_one = Id;
                        conversa.user_two = to;
                        conversa.time = DateTime.Now;
                        conversa.status = 0;
                        db.Chats.Add(conversa);
                        db.SaveChanges();

                    }
                }

                if (userDetails != null)
                {
                    if(userDetails.Id == Id)
                    {
                        conversa.user_one = Id;
                        conversa.user_two = to;
                    }
                    else
                    {
                        conversa.user_one = to;
                        conversa.user_two = Id;
                    }
                }


                ViewBag.Chat = conversa;
                ViewBag.from = Id;
                ViewBag.to = to;
            }
          
            return View();
        }


        public JsonResult Enviar(int from, int? to, string msg, int? cid)
        {
            if (ModelState.IsValid)
            {
                var id = from;
                var usuario = db.Users.Where(x => x.Id == id).FirstOrDefault();
                Message mensagem = new Message();

                var conversa = new Chat();

                if (cid > 0)
                {
                    conversa = db.Chats.Where(x => x.chat_id == cid).FirstOrDefault();
                }
                else
                {
                    conversa = db.Chats.Where(x => x.user_one == id && x.user_two == to || x.user_one == to && x.user_two == id).FirstOrDefault();
                }

                if (conversa == null)
                {
                    Chat new_conversa = new Chat();
                    new_conversa.user_one = id;
                    new_conversa.user_two = to;
                    new_conversa.time = DateTime.Now;
                    new_conversa.status = 0;
                    db.Chats.Add(new_conversa);
                    db.SaveChanges();
                    cid = new_conversa.chat_id;
                }
                else
                {
                    cid = conversa.chat_id;
                }

                mensagem.user_id_from = id;
                mensagem.msg_content = msg;
                mensagem.chat_id = cid;
                mensagem.dataHora = DateTime.Now;
                mensagem.visu_status = 0;
                mensagem.user_id_to = to;
                db.Messages.Add(mensagem);
                db.SaveChanges();
                TempData["success"] = "sim";
                return Json(TempData);

            }
            else
            {
                TempData["error"] = "Algum dado está incorreto ou faltando!";
                return Json(TempData);
            }
        }
    }
}
