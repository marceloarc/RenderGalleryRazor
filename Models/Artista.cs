using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    [JsonObject(IsReference = true)]
    public class Artista
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int User_Id { get; set; }
        public DateTime dataHora { get; set; }

        [ForeignKey("User_Id")]
        public virtual User User { get; set; }

        public virtual List<Favoritos>? Favoritos { get; set; }
        public virtual List<Tags>? Tags { get; set; }
    }
}
