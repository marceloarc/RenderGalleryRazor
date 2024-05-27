using System.ComponentModel.DataAnnotations;
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

    public class AdicionarItemCarrinhoModel
    {
        [Required(ErrorMessage = "O ID do usuário é obrigatório")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "O ID da arte é obrigatório")]
        public int ArtId { get; set; }

        public int? Quantidade { get; set; }
    }


}
