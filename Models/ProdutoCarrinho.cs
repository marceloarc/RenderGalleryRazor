using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    public class ProdutoCarrinho
    {
        public int Id { get; set; }

        public int Quantidade { get; set; }

        public  int? User_id { get; set; }   
        
        public int? publi_id { get; set; }

        [ForeignKey("publi_id")]
        public virtual Publicacao? Publicacao { get; set; }

        [ForeignKey("User_id")]
        public virtual User? User { get; set; }

        public int? art_id { get; set; }

        [ForeignKey("art_id")]
        public virtual Art? Arte { get; set; }


        [NotMapped]
        public string? Valor { get; set; }


    }
}
