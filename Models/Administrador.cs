using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGallery.Models
{
    public class Administrador
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime dataHora { get; set; }
        public int User_Id { get; set; }

        [ForeignKey("User_Id")]
        public virtual User User { get; set; }
    }
}
