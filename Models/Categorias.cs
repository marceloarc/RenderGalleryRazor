using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        
        [StringLength(250)]
        public string? Nome { get; set; }
        
        public string? Image { get; set; }

        public string? Descricao { get; set; }

        public virtual List<Tags>? Tags { get; set; }

    }

    public class VMCategory
    {
        public int? categoryId { get; set; }
        public string? searchText { get; set; }

        public int? userId { get; set; }
    }
}
