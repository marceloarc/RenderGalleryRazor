using System.ComponentModel.DataAnnotations;

namespace RenderGalleyRazor.Models
{
    public class ArtDefinicoes
    {
        [Key] 
        public int Id { get; set; }
        [EnumDataType(typeof(categoriaTipo))]
        [Required]
        public categoriaTipo categoriaProduct { get; set; }
        public enum categoriaTipo { digital = 0, fisico = 1 }
        [EnumDataType(typeof(tipo))]
        [Required]
        public tipo tipoProduct { get; set; }
        public enum tipo { unico = 0, direito = 1 }
    }
}
