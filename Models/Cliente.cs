using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RenderGalleyRazor.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string document { get; set; }
        [Required]
        public DateTime dataHora { get; set; }

        public int User_Id { get; set; }

        [ForeignKey("User_Id")]
        public virtual User User { get; set; }

        public virtual List<Favoritos>? Favoritos { get; set; }
        public virtual List<Tags>? Tags { get; set; }
    }
}
