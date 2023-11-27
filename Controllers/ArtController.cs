﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RenderGalleyRazor.Models;
using RenderGallery.Util;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Search(string search)
        {
            int user_id = 0;
            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                user_id = user.Id;
            }
            List<Art> arts = db.Arts.Where(x => EF.Functions.Like(x.Arte, "%"+search+"%") || EF.Functions.Like(x.Categoria.Nome, "%" + search + "%")).ToList();
            ViewBag.user_id = user_id;
            ViewBag.Arts = arts;
            ViewBag.Title = "Pesquisando por "+search;

            return View();
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
                        bool like2 = false;
                        bool deslike2 = false;
                        if (isLike && !like.isLike)
                        {
                            like2 = true;
                            deslike2 = false;
                        }
                        
                        if(isDeslike && !like.isDeslike)
                        {
                            like2 = false;
                            deslike2 = true;
                        }

                        if(isLike && like.isLike)
                        {

                            like2 = false;
                            deslike2 = false;
                        }


                        if (isDeslike && like.isDeslike)
                        {

                            like2 = false;
                            deslike2 = false;
                        }

                        like.isLike = like2;
                        like.isDeslike = deslike2;

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
