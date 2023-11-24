using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RenderGallery.Models;
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
            List<Categoria> categorias = db.Categorias.ToList();
            ViewBag.Categorias = categorias;
            
            List<Art> arts = db.Arts.Where(x => x.categoria_id == id).ToList();
            Categoria cat = db.Categorias.Where(x=> x.Id == id).FirstOrDefault();
            
            ViewBag.Arts = arts;
            ViewBag.Title = cat.Nome;
      
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}