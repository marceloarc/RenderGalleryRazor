using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int User_id { get; set; }

        [ForeignKey("User_id")]
        public virtual User? User { get; set; }

        public virtual List<ProdutoCarrinho>? Produtos { get; set; }

    }
}
