using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    public class Favoritos
    {
        [Key]
        public int Id { get; set; }

        public int? user_id { get; set; }

        public int? art_id { get; set; }       

        [ForeignKey("user_id")]
        public virtual User? User { get; set; }


        [ForeignKey("art_id")]
        public virtual Art? Art { get; set; }

    }
}
