using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderGallery.Models;

namespace RenderGallery.Controllers
{
    [ApiController]
    [Route("chat")]
    public class ChatController : Controller
    {
        private readonly DatabaseContext db;

        public ChatController(DatabaseContext db) 
        {
            this.db = db;
        }

        [HttpGet("[action]/{id?}")]
        public async Task<IActionResult> Chat(int Id)
        {
            if (Id > 0)
            {
                var userDetails = db.Users.Where(x => x.Id == Id).FirstOrDefault();

                List<Chat> conversas_query = db.Chats.Where(x => x.user_one == Id || x.user_two == Id).Include(u => u.User1).Include(u => u.User2).ToList();

                var conversas = conversas_query;

                foreach (var conversa in conversas)
                {
                    if (conversa.User1.Id != userDetails.Id)
                    {
                        conversa.User2 = conversa.User1;
                        conversa.User1 = userDetails;
                    }
                }

                if (conversas_query != null)
                {
                    TempData["chat"] = conversas;

                }
                if (userDetails != null)
                {
                    TempData["user"] = userDetails;
                   
                }
            }
            return Json(TempData);
        }

        [HttpPost("[action]")]
        public IActionResult Enviar(Message message)
        {
            if(ModelState.IsValid)
            {
                var id = message.user_id_from;
                var usuario = db.Users.Where(x => x.Id == id).FirstOrDefault();
                Message mensagem = new Message();

                var conversa = new Chat();

                if (message.chat_id > 0)
                {
                    conversa = db.Chats.Where(x => x.chat_id == message.chat_id).FirstOrDefault();
                }
                else
                {
                    conversa = db.Chats.Where(x => x.user_one == id && x.user_two == message.user_id_to || x.user_one == message.user_id_to && x.user_two == id).FirstOrDefault();
                }

                if (conversa == null)
                {
                    Chat new_conversa = new Chat();
                    new_conversa.user_one = id;
                    new_conversa.user_two = message.user_id_to;
                    new_conversa.time = DateTime.Now;
                    new_conversa.status = 0;
                    db.Chats.Add(new_conversa);
                    db.SaveChanges();
                    message.chat_id = new_conversa.chat_id;
                }
                else
                {
                    message.chat_id = conversa.chat_id;
                }

                mensagem.user_id_from = id;
                mensagem.msg_content = message.msg_content;
                mensagem.chat_id = message.chat_id;
                mensagem.dataHora = DateTime.Now;
                mensagem.visu_status = 0;
                mensagem.user_id_to = message.user_id_to;
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
