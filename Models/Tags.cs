using System.ComponentModel.DataAnnotations;

namespace RenderGalleyRazor.Models
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
