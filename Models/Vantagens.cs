using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    public class Vantagens
    {
        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(250)]
        public string descricao { get; set; }
        public int PlanoId { get; set; }

        [ForeignKey("PlanoId")]
        public virtual Planos Plano { get; set; }

    }
}
