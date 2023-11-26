using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    public class Publicacao
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string? Nome { get; set; }
        [StringLength(250)]
        public string? Descricao { get; set; }

        public DateTime dataHora { get; set; }
        public int? User_id { get; set; }

        [ForeignKey("User_id")]
        public virtual User? User { get; set; }

        public virtual List<Art>? Artes { get; set; }

        [NotMapped]
        public string? Error { get; set; }
    }
}
