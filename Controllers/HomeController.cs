using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RenderGalleyRazor.Models;
using System.Data.Entity;
using System.Diagnostics;

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
            Categoria cat = db.Categorias.Where(x=> x.Id == id).FirstOrDefault();
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
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}