using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    [JsonObject(IsReference = true)]
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
        public virtual ICollection<Chat> Chats { get; set; }
        [InverseProperty("User1")]
        public virtual ICollection<Chat> ChatsAsUser1 { get; set; }

        [InverseProperty("User2")]
        public virtual ICollection<Chat> ChatsAsUser2 { get; set; }
        public virtual ICollection<Favoritos> Favoritos { get; set; }
        public virtual ICollection<LikesDeslikes> LikesDeslikes { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
        public virtual ICollection<ProdutoCarrinho> ProdutosCarrinho { get; set; }
        public virtual ICollection<ProdutoPedido> ProdutosPedido { get; set; }
        public virtual ICollection<Publicacao> Publicacoes { get; set; }



    }
}
