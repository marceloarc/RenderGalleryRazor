using System.ComponentModel.DataAnnotations;

namespace RenderGallery.Models
{
    public class Tags
    {
        [Key] 
        public string Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Title { get; set; }
    }
}
