using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    public class Planos
    {
        [Key]
        public int Id { get; set; }
        [StringLength(250)]
        public string Nome { get; set; }
        public float Preco { get; set; }

        [NotMapped]
        public string Preco_formatted { get; set; } 

        public int LimitePublicacoes { get; set; }
        public float taxa { get; set; }
        public virtual ICollection<Vantagens> Vantagens { get; set; }
        public string cor { get; set; }

    }
}