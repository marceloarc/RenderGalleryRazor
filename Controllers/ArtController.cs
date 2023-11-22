using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RenderGallery.Models;
using RenderGallery.Util;

namespace RenderGallery.Controllers
{
    [ApiController]
    [Route("art")]
    public class ArtController : Controller
    {
        private readonly DatabaseContext db;
        private string caminhoServidor;

        public ArtController(DatabaseContext db)
        {
            this.db = db;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload([FromForm]Files arte)
        {

         string caminhoArquivo = Functions.WriteFile(arte.File);

            if (string.IsNullOrEmpty(caminhoArquivo))
            {
                return BadRequest("Erro ao fazer o upload da imagem");
            }


            TempData["sucesso"] = "Upload realizado com sucesso!";
            TempData["path"] = caminhoArquivo;
            return Ok(TempData);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SaveArt(Art arte)
        {

            arte.dataHora = DateTime.Now;

            db.Arts.Add(arte);
            db.SaveChanges();

            TempData["sucesso"] = "Arte Cadastrada com sucesso!";
            return Ok(TempData);
        }

    }
}
