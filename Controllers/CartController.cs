using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderGalleyRazor.Models;

namespace RenderGallery.Controllers
{
    public class CartController : Controller
    {
        private readonly DatabaseContext db;

        public CartController(DatabaseContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public IActionResult Carrinho()
        {

            return View();
        }

        public JsonResult AddItem(int art_id, int quantidade, string? plan)
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
                    TempData["e"] = "Voçê precisa estar logado para relizar esta ação";
                }

                if (user_id > 0)
                {
                    Art art = db.Arts.Where(x=> x.Id == art_id).FirstOrDefault();

                    if(art!= null)
                    {


                        ProdutoCarrinho produto = new ProdutoCarrinho();

                        produto = db.Produtos.Where(x => x.art_id == art_id && x.User_id == user_id).FirstOrDefault();

                        if(produto == null)
                        {
                            produto = new ProdutoCarrinho();
                            produto.art_id = art_id;
                            produto.publi_id = art.publi_id;
                            produto.User_id = user_id;
                            produto.Quantidade = quantidade;
                            db.Produtos.Add(produto);
                            db.SaveChanges();
                            TempData["s"] = "sucesso";
                        }
                        else
                        {
                            TempData["e"] = "Produto já se encontra em seu carrinho!"; 
                        }



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

            List<ProdutoCarrinho> produtos = db.Produtos.Where(x => x.User_id == user_id).ToList();

            if(produtos.Count > 0)
            {
                ViewBag.produtos = produtos;


            }

         
            return View(); 
        
        
        }

 
        public JsonResult RemoveItem(int product_id)
        {
           if(product_id > 0)
            {

               db.Produtos.Where(x => x.Id == product_id).ExecuteDelete();
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
