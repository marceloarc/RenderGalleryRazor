using Microsoft.AspNetCore.Mvc;
using RenderGallery.Util;
using RenderGalleyRazor.Models;

namespace RenderGallery.Controllers
{ 
    public class PedidosController : Controller
{

    private readonly DatabaseContext db;

        public PedidosController(DatabaseContext rendergalleryContext)
    {
        db = rendergalleryContext;
    }

        public IActionResult Index() { 
            
            
            return View();
        
        
        }
  
} 

}

