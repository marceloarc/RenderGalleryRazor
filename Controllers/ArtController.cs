using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RenderGalleyRazor.Models;
using RenderGallery.Util;

namespace RenderGallery.Controllers
{
    

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

        public JsonResult LikeDeslike(int user_id,int art_id, bool isLike, bool isDeslike)
        {
            User user = db.Users.Where(x => x.Id == user_id).FirstOrDefault();

            if (user != null) {
                Art art = db.Arts.Where(x => x.Id == art_id).FirstOrDefault();

                if (art != null)
                {
                    LikesDeslikes like = db.LikeDeslikes.Where(x => x.user_id == user_id && x.art_id == art_id).FirstOrDefault(); 
                    
                    if(like != null)
                    {
                        if (isLike)
                        {
                            like.isLike = true;
                            like.isDeslike = false;
                        }
                        
                        if(isDeslike)
                        {
                            like.isDeslike = true;
                            like.isLike = false;
                        }
                    }
                    else
                    {
                        like = new LikesDeslikes();

                        like.user_id = user_id;
                        like.art_id = art_id;
                        like.isDeslike = isDeslike;
                        like.isLike = isLike;
                        db.LikeDeslikes.Add(like);
                    }

                    db.SaveChanges();
                    TempData["s"] = "s";
                }

            }
            int likes = db.LikeDeslikes.Where(x => x.art_id == art_id && x.isLike == true).Count();
            int deslikes = db.LikeDeslikes.Where(x => x.art_id == art_id && x.isDeslike == true).Count();

            TempData["likes"] = likes;
            TempData["deslikes"] = deslikes;

            return Json(TempData);
        }

    }
}
