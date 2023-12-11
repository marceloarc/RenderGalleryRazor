using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderGalleyRazor.Models;
using System.Globalization;

namespace RenderGallery.Controllers
{
    [Authorize]
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

        public IActionResult Checkout()
        {
            int user_id = 0;
            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                user_id = user.Id;

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            List<ProdutoCarrinho> produtos = db.Produtos.Where(x => x.User_id == user_id).ToList();

            if (produtos.Count > 0)
            {
                float total = 0;

                foreach(ProdutoCarrinho produto in produtos)
                {
                    if (produto.Quantidade > 0)
                    {
                        total += produto.Arte.Valor * produto.Quantidade;
                        produto.Valor = produto.Arte.Valor.ToString("C", CultureInfo.CurrentCulture);
                    }
                    ViewBag.esgotou = true;
                    ViewBag.nome = produto.Arte.Arte;
                }
                ViewBag.Total = total.ToString("C", CultureInfo.CurrentCulture);
                ViewBag.Produtos = produtos;
            }
            else
            {
                return RedirectToAction("Login", "Account");
                         
            }

            ViewBag.Title = "Checkout";
            return View();
        }

        public JsonResult Finalizar()
        {
            int user_id = 0;
            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                user_id = user.Id;
            }
            else
            {
                TempData["erro"] = "Usuário não encontrado!";
            }

            if(user_id != 0)
            {
                Pedido pedido = new Pedido();
                float total = 0;
                List<ProdutoCarrinho> produtos = db.Produtos.Where(x => x.User_id == user_id).ToList();
                List<ProdutoPedido> produtoPedidos = new List<ProdutoPedido>();
                if (produtos.Count() > 0)
                {
                    foreach (ProdutoCarrinho produto in produtos)
                    {
                        total += (produto.Arte.Valor * produto.Quantidade);
                        ProdutoPedido produtoPedido = new ProdutoPedido();

                        produtoPedido.art_id = produto.Arte.Id;
                        produtoPedido.publi_id = produto.Arte.publi_id;
                        produtoPedido.Quantidade = produto.Quantidade;
                        produtoPedido.User_id = user_id;

                        produto.Arte.Publicacao.User.Saldo += (produto.Arte.Valor * produto.Quantidade);

                        produtoPedidos.Add(produtoPedido);

                        produto.Arte.Quantidade -= produto.Quantidade;
                    }

                }

                pedido.User_id = user_id;
                pedido.Produtos = produtoPedidos;
                pedido.total = total;
                pedido.sub_total = total;
                pedido.Status = 1;
                db.Pedidos.Add(pedido);
                db.Produtos.RemoveRange(produtos);
                db.SaveChanges();
                TempData["sucesso"] = "Pedido Relizado com sucesso!";

            }

            return Json(TempData);
        }

        public IActionResult Produtos() {
            int user_id = 0;
            float total = 0;
            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                user_id = user.Id;

            }

            List<ProdutoCarrinho> produtos = db.Produtos.Where(x => x.User_id == user_id).ToList();

            if(produtos.Count > 0)
            {
                foreach(ProdutoCarrinho produto in produtos)
                {
                    total += produto.Arte.Valor * produto.Quantidade;
                    produto.Valor = produto.Arte.Valor.ToString("C", CultureInfo.CurrentCulture);
                }

                ViewBag.produtos = produtos;
                ViewBag.Total = total.ToString("C", CultureInfo.CurrentCulture);
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
