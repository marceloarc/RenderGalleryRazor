using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGallery.Models
{
    public class Files
    { 
        public IFormFile? File { get; set; }
        

        public string? Arte { get; set; }


    }
}
