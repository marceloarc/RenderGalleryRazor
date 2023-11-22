namespace RenderGallery.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Table("Chat")]
    public class Chat
    {
        [Key]
        public int chat_id { get; set; }
        public int? user_one { get; set; }
        public int? user_two { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? time { get; set; }
        public int? status { get; set; }

        [ForeignKey("user_one")]
        public virtual User User1 { get; set; }

        [ForeignKey("user_two")]
        public virtual User User2 { get; set; }
        public virtual List<Message>? Messages { get; set; }

    }
}
