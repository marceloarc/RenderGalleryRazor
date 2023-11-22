using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGallery.Models
{
    [JsonObject(IsReference = true)]
    public class Artista
    {

        public int Id { get; set; }

        public int User_Id { get; set; }

        public DateTime dataHora { get; set; }

        [ForeignKey("User_Id")]
        public virtual User User { get; set; }
    }
}
