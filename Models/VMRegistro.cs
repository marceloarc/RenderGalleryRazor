using System.ComponentModel.DataAnnotations;

namespace RenderGalleyRazor.Models
{
    public class VMRegistro
    {
        [Required]
        public string? Nome { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
