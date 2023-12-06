using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        public string? Telefone { get; set; }

        public string? Pic { get; set; }

        public int? plano_id { get; set; }
        [ForeignKey("plano_id")]
        public virtual Planos? Plano { get; set; }
    }
}
