using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderGalleyRazor.Models;

namespace RenderGallery.Controllers
{
    [Authorize]
    public class FavoritosController : Controller
    {
        private readonly DatabaseContext db;

        public FavoritosController(DatabaseContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public IActionResult Favoritos()
        {

            return View();
        }

        public JsonResult AddItem(int art_id, string? plan)
        {
            if (ModelState.IsValid)
            {

                if (plan != null) 
                { 


                }
                int user_id = 0;
                if (User.Identity.IsAuthenticated)
                {
                    User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                    user_id = user.Id;
                }
                else
                {
                    TempData["e"] = "Você precisa estar logado para relizar esta ação";
                }

                if (user_id > 0)
                {
                    Art art = db.Arts.Where(x=> x.Id == art_id).FirstOrDefault();

                    if(art!= null)
                    {


                        Favoritos favorito = new Favoritos();

                        favorito = db.Favoritos.Where(x => x.art_id == art_id && x.user_id == user_id).FirstOrDefault();

                        if(favorito == null)
                        {
                            favorito = new Favoritos();
                            favorito.art_id = art_id;
                            favorito.user_id = user_id;
                            db.Favoritos.Add(favorito);

                            TempData["s"] = "sucesso";
                        }
                        else
                        {
                            
                            TempData["e"] = "Produto já se encontra em seus favoritos!";
                        }

                        db.SaveChanges();
                        return Json(TempData);

                    }
 
                
                }


            }
            return Json(TempData);
        }

        public IActionResult Produtos() {
            int user_id = 0;
            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                user_id = user.Id;

            }

            List<Favoritos> favoritos = db.Favoritos.Where(x => x.user_id == user_id).ToList();

            if(favoritos.Count > 0)
            {
                ViewBag.favoritos = favoritos;


            }
            ViewBag.user_id = user_id;


            return View(); 
        
        
        }

        public JsonResult AddAll(int user_id)
        {
            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

                user_id = user.Id;
                if(user != null)
                {

                    List<Favoritos> favoritos = db.Favoritos.Where(x =>x.user_id == user_id).ToList();

                    if(favoritos.Count() >0)
                    {
                      
                        foreach (Favoritos favorito in favoritos)
                        {
                            ProdutoCarrinho produto = new ProdutoCarrinho();

                            ProdutoCarrinho productExists = db.Produtos.Where(x => x.art_id == favorito.art_id && x.User_id == favorito.user_id).FirstOrDefault();

                            if(productExists == null)
                            {
                                produto.art_id = favorito.art_id;
                                produto.publi_id = favorito.Art.publi_id;
                                produto.Quantidade = 1;
                                produto.User_id = user_id;
                                db.Produtos.Add(produto);
                            }

                        }
                
                        db.SaveChanges();
                        TempData["s"] = "Todos produtos favoritados foram adicionados ao seu carrinho de compras!";

                    }
                    else
                    {
                        TempData["e"] = "Você não possui nenhum produto favorito!";
                    }

                }
            }
            return Json(TempData);
        }

        public JsonResult RemoveItem(int product_id)
        {
           if(product_id > 0)
            {

               db.Favoritos.Where(x => x.Id == product_id).ExecuteDelete();
               db.SaveChanges();
               TempData["success"] = "Produto deletado com sucesso!";
           
            }
            else
            {
                TempData["error"] = "produto não encontrado!";
         
            }
            return Json(TempData);
        }

    }
}
