using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    public class Favoritos
    {
        [Key]
        public int Id { get; set; }

        public int? cli_id { get; set; }

        public int? publi_id { get; set; }       

        [ForeignKey("cli_id")]
        public virtual Cliente? cliente { get; set; }


        [ForeignKey("publi_id")]
        public virtual Publicacao? Publicacao { get; set; }

    }
}
