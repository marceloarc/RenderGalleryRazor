using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderGalleyRazor.Models;

namespace RenderGallery.Controllers
{
    [ApiController]
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly DatabaseContext db;

        public CartController(DatabaseContext db)
        {
            this.db = db;
        }

        [HttpGet("[action]/{id?}")]
        public async Task<IActionResult> Cart(int Id)
        {
            if(Id != null)
            {
                User userDetails = db.Users.Where(x => x.Id == Id).FirstOrDefault();

                if(userDetails != null) {
                    Cart cart = db.Carts.Where(x => x.User_id == Id).FirstOrDefault();
                    TempData["cart"] = cart;
                }
            }
            return Json(TempData);
        }

        public async Task<IActionResult> AddItem(ProdutoCarrinho produto)
        {
           if(ModelState.IsValid)
            {
                if(produto.cart_id > 0)
                {
                    Cart cart = db.Carts.Where(x => x.Id == produto.cart_id).FirstOrDefault();

                    cart.Produtos.Add(produto);

                    db.SaveChanges();
                }
                else
                {
                    Cart cart = new Cart();

                    cart.User_id = (int)produto.User_id;
                    cart.Produtos.Add(produto);
                    db.Carts.Add(cart);
                    db.SaveChanges();
                }
                TempData["success"] = "Produto adicionado com sucesso!";

            }
            else
            {
                TempData["error"] = "Algum dado está faltando ou incorreto!";
                
            }
            return Json(TempData);
        }

        [HttpGet("[action]/{id?}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
           if(id>0)
            {

               db.Produtos.Where(x => x.Id == id).ExecuteDelete();
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
