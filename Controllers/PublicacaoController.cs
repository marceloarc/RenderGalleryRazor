using Microsoft.AspNetCore.Mvc;
using RenderGallery.Util;
using RenderGalleyRazor.Models;

namespace RenderGallery.Controllers
{ 
    public class PublicacaoController : Controller
{

    private readonly DatabaseContext db;

    public PublicacaoController(DatabaseContext rendergalleryContext)
    {
        db = rendergalleryContext;
    }

 
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

        [HttpGet]
        public IActionResult Create()
    {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
    }

        [HttpPost]
        public IActionResult Create(Publicacao publi)
    {

            if (ModelState.IsValid)
            {
                User user = db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                publi.User_id = user.Id;
                publi.User = user;
                foreach(Art art in publi.Artes)
                {
                    var path = Functions.WriteFile(art.File);
                    var fileName = Path.GetFileName(path);
                    var name = "images/" + fileName;

                    art.Path = name;

                }
                db.Publicacoes.Add(publi);
                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("Error", "Todos os campos são obrigatórios");
            }

            return View();
    }

        [HttpGet]
        public IActionResult Arte(int id)
    {
            ViewBag.id = id;
            return View();
    }

      
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
} }

