namespace RenderGallery.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Message")]
    public class Message
    {
        [Key]

        public int id { get; set; }

        [Column(TypeName = "text")]
        public string msg_content { get; set; }
        public int? user_id_from { get; set; }

        [ForeignKey("user_id_from")]
        public virtual User? User_from { get; set; }

        public int? user_id_to { get; set; }

        [ForeignKey("user_id_to")]
        public virtual User? User_to { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? dataHora { get; set; }
        public int? visu_status { get; set; }
   
        public int? chat_id { get; set; }

        [ForeignKey("chat_id")]
        public virtual Chat? Chat { get; set; }

    }
}

