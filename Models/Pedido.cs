using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public float? sub_total { get; set; }

        public float? total { get; set; }

        public  int? User_id { get; set; }   
        

        [ForeignKey("User_id")]
        public virtual User? User { get; set; }

        public virtual List<ProdutoPedido>? Produtos { get; set; }

       


    }
}
