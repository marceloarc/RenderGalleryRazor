using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using RenderGallery.Util;
using RenderGalleyRazor.Models;
using System.Globalization;

namespace RenderGallery.Controllers
{
    [Authorize]
    public class PedidosController : Controller
	{

		private readonly DatabaseContext db;

			public PedidosController(DatabaseContext rendergalleryContext)
		{
			db = rendergalleryContext;
		}

			public IActionResult Index() {
				int user_id = 0;
				if (User.Identity.IsAuthenticated)
				{
					User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
					user_id = user.Id;
				}

				if(user_id> 0)
				{ 
					List<Pedido> pedidos = db.Pedidos.Where(x => x.User_id == user_id).ToList();

					foreach(Pedido pedido in pedidos)
					{
						float total = (float)pedido.total;
						pedido.total_formatted = total.ToString("C", CultureInfo.CurrentCulture);
              
					}

					if(pedidos.Count>0)
					{
						ViewBag.pedidos = pedidos;
					}
				}

				ViewBag.Title = "Pedidos";
				return View();
        
        
			}

			public IActionResult Details(int id)
			{
				int user_id = 0;
				if (User.Identity.IsAuthenticated)
				{
					User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
					user_id = user.Id;
				}

				if (user_id > 0)
				{
					List<ProdutoPedido> produtos = db.ProdutosPedido.Where(x => x.User_id == user_id && x.Pedido.Id == id).ToList();

					if(produtos.Count > 0)
					{
						foreach (ProdutoPedido produto in produtos)
						{
							produto.Valor = produto.Arte.Valor.ToString("C", CultureInfo.CurrentCulture);

                     
						}

				

						ViewBag.produtos = produtos;

					}
					else
					{
						return RedirectToAction("Index", "Home");
					}


				}

           

				ViewBag.Title = "Produtos do pedido #"+id;
				return View();


			}

			public IActionResult Vendas()
			{
				int user_id = 0;
				User user;
	
				float saldo = 0;
				if (User.Identity.IsAuthenticated)
				{
					user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
			
					user_id = user.Id;
					saldo = (float)user.Saldo;

					ViewBag.saldo = saldo.ToString("C", CultureInfo.CurrentCulture); ;
				}

				if (user_id > 0)
				{
					ViewBag.Total = 0;
					ViewBag.Sell = 0;
					List<ProdutoPedido> produtos = db.ProdutosPedido.Where(x => x.Arte.Publicacao.User_id == user_id).ToList();

					if (produtos.Count > 0)
					{
						foreach (ProdutoPedido produto in produtos)
						{
							produto.Valor = produto.Arte.Valor.ToString("C", CultureInfo.CurrentCulture);

							ViewBag.Sell += produto.Quantidade;

							ViewBag.Total += (produto.Arte.Valor * produto.Quantidade);
						}

						ViewBag.Total = ViewBag.Total.ToString("C", CultureInfo.CurrentCulture);
						ViewBag.produtos = produtos;
					
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}


				}



				ViewBag.Title = "Minhas Vendas";
				return View();


			}

	} 

}

