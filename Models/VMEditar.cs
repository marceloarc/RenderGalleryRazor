using System.ComponentModel.DataAnnotations;

namespace RenderGalleyRazor.Models
{
    public class VMEditar
    {
        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string? Nome { get; set; }
        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress]
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        [Required(ErrorMessage = "A Senha é obrigatória")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        public string? RepeatePassword { get; set; }
        public IFormFile? File { get; set; }
        public string? Error { get; set; }
    }
}
