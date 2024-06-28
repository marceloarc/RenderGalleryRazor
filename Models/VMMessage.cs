using System.ComponentModel.DataAnnotations;

namespace RenderGalleyRazor.Models
{
    public class VMMessage
    {
        [Required(ErrorMessage = "User de Envio obrigatório")]
        public int? from { get; set; }

        [Required(ErrorMessage = "User de Recebimento obrigatório")]
        public int? to { get; set; }
        public string msg { get; set; }
        [Required(ErrorMessage = "Chat Id obrigatório")]
        public int? cid { get; set; }
    }
}
