using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RenderGalleyRazor.Models;
using System.Data.Entity;
using System.Diagnostics;
using System.Numerics;

namespace RenderGalleyRazor.Controllers
{
    public class HomeController : Controller
    {

        private readonly DatabaseContext db;
        public HomeController(DatabaseContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Render Gallery";
            List<Categoria> categorias = db.Categorias.ToList();
            ViewBag.Categorias = categorias;
            return View();
        }
        public IActionResult Galeria(int id)
        {
            int user_id = 0;
            List<Categoria> categorias = db.Categorias.ToList();
            ViewBag.Categorias = categorias;
            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                user_id = user.Id;
            }
            List<Art> arts = db.Arts.Where(x => x.categoria_id == id).ToList();
            Categoria cat = db.Categorias.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.user_id = user_id;
            ViewBag.Arts = arts;
            ViewBag.Title = cat.Nome;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Planos()
        {
            int user_id = 0;
            List<Planos> planos = db.Planos.ToList();
            ViewBag.Planos = planos;

            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                user_id = user.Id;
            }
            ViewBag.user_id = user_id;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Artistas()
        {
            int user_id = 0;
            List<User> users = db.Users.ToList();
            ViewBag.Users = users;

            if (User.Identity.IsAuthenticated)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                user_id = user.Id;
            }
            ViewBag.user_id = user_id;
            List<Art> arts = db.Arts.ToList();
            ViewBag.Arts = arts;

            return View();
        }

        [HttpPost("api/mobile/home")]
        public IActionResult GetAllArts([FromBody] VMCategory category)
        {
            try
            {
                var art = 0;
                if (category.categoryId != 0)
                {
                    if (category.searchText != null || category.searchText != "")
                    {
                        var arts = db.Arts.Where(a => a.categoria_id == category.categoryId).Where(a => EF.Functions.Like(a.Arte, "%" + category.searchText + "%") || EF.Functions.Like(a.Publicacao.Nome, "%" + category.searchText + "%")).Select(a => new
                        {
                            Id = a.Id,
                            Name = a.Arte,
                            Path = "http://192.168.0.13:5000/" + a.Path,
                            Price = a.Valor,
                            Tipo = a.Tipo,
                            Quantidade = a.Quantidade,
                            CategoriaId = a.categoria_id,
                            PublicacaoId = a.publi_id,
                            User = a.Publicacao.User_id,
                            Description = a.dataHora
                        })
                        .ToList();
                        return Ok(arts);
                    } 
                    else
                    {
                        var arts = db.Arts.Where(a => a.categoria_id == category.categoryId).Select(a => new
                        {
                            Id = a.Id,
                            Name = a.Arte,
                            Path = "http://192.168.0.13:5000/" + a.Path,
                            Price = a.Valor,
                            Tipo = a.Tipo,
                            Quantidade = a.Quantidade,
                            CategoriaId = a.categoria_id,
                            PublicacaoId = a.publi_id,
                            User = a.Publicacao.User_id,
                            Description = a.dataHora
                        })
                        .ToList();
                        return Ok(arts);
                    }

                }
                else
                {
                    if(category.searchText != null || category.searchText != "")
                    {
                        var arts = db.Arts.Where(a => EF.Functions.Like(a.Arte, "%" + category.searchText + "%") || EF.Functions.Like(a.Publicacao.Nome, "%" + category.searchText + "%")).Select(a => new
                        {
                            Id = a.Id,
                            Name = a.Arte,
                            Path = "http://192.168.0.13:5000/" + a.Path,
                            Price = a.Valor,
                            Tipo = a.Tipo,
                            Quantidade = a.Quantidade,
                            CategoriaId = a.categoria_id,
                            PublicacaoId = a.publi_id,
                            User = a.Publicacao.User_id,
                            Description = a.dataHora
                        })
                        .ToList();
                        return Ok(arts);
                    }
                    else
                    {
                        var arts = db.Arts
                        .Select(a => new
                        {
                            Id = a.Id,
                            Name = a.Arte,
                            Path = "http://192.168.0.13:5000/" + a.Path,
                            Price = a.Valor,
                            Tipo = a.Tipo,
                            Quantidade = a.Quantidade,
                            CategoriaId = a.categoria_id,
                            PublicacaoId = a.publi_id,
                            User = a.Publicacao.User_id,
                            Description = a.dataHora
                        })
                        .ToList();
                        return Ok(arts);
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpGet("api/mobile/product/{id}")]
        public IActionResult GetArt(int id)
        {
            try
            {
                var art = db.Arts
                    .Where(a => a.Id == id)
                    .Select(a => new
                    {
                        Id = a.Id,
                        Name = a.Arte,
                        Path = "http://192.168.0.13:5000/" + a.Path,
                        Price = a.Valor,
                        Tipo = a.Tipo,
                        Quantidade = a.Quantidade,
                        Categoria = new
                        {
                            Id = a.categoria_id,
                            Name = a.Categoria.Nome
                        },
                        PublicacaoId = a.publi_id,
                        User = new
                        {
                            Id = a.Publicacao.User.Id,
                            Name = a.Publicacao.User.Name,
                            Email = a.Publicacao.User.Email,
                            Pic = a.Publicacao.User.Pic,
                            Saldo = a.Publicacao.User.Saldo,
                            Plano = a.Publicacao.User.plano_id
                        },
                    })
                    .FirstOrDefault();

                if (art != null)
                {
                    return Ok(art);
                }
                else
                {
                    return NotFound("Arte não encontrada");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

    }
}