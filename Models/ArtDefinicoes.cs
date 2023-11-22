using System.ComponentModel.DataAnnotations;
using static RenderGallery.Models.ArtDefinicoes;

namespace RenderGallery.Models
{
    public class ArtDefinicoes
    {
        [Key] 
        public int Id { get; set; }
        [EnumDataType(typeof(categorias))]
        [Required]
        public categorias categoriaProduct { get; set; }
        public enum categorias { digital = 0, fisico = 1 }
        [EnumDataType(typeof(tipo))]
        [Required]
        public tipo tipoProduct { get; set; }
        public enum tipo { unico = 0, direito = 1 }
    }
}
