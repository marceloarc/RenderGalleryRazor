using Microsoft.AspNetCore.Mvc;
using RenderGalleyRazor.Models;

namespace RenderGallery.Controllers
{
    [ApiController]
    [Route("publicacao")]
    public class PublicacaoController : Controller
    {

        private readonly DatabaseContext db;

        public PublicacaoController(DatabaseContext rendergalleryContext)
        {
            db = rendergalleryContext;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Criar(Publicacao publi)
        {
            if (ModelState.IsValid)
            {
                db.Publicacoes.Add(publi);
                db.SaveChanges();
                TempData["success"] = "Publicação criada!";

            }
            else
            {
                TempData["error"] = "Algum dado incorreto!";
            }
            return Json(TempData);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Editar(Publicacao publi)
        {
            if (ModelState.IsValid)
            {
                Publicacao publicacao = db.Publicacoes.Where(x => x.Id == publi.Id).FirstOrDefault();

                if (publicacao != null)
                {
                    publicacao = publi;
                    db.SaveChanges();
                    TempData["success"] = "Publicação Editada!";
                }
                else
                {
                    TempData["error"] = "Publicacao não encontrada!";
                }
            }
            else
            {
                TempData["error"] = "Algum dado incorreto!";
            }
            return Json(TempData);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Excluir([FromBody] int id)
        {
            if (id > 0)
            {
                Publicacao publicacao = db.Publicacoes.Where(x => x.Id == id).FirstOrDefault();

                if (publicacao != null)
                {
                    db.Publicacoes.Remove(publicacao);
                    db.SaveChanges();
                    TempData["success"] = "Publicação apagada!";
                }
                else
                {
                    TempData["error"] = "Publicacao não encontrada!";
                }
            }
            else
            {
                TempData["error"] = "id inválido!";
            }
            return Json(TempData);
        }
    }
}
