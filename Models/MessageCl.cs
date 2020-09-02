using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель отзыва, оставленного пользователем касательно бренда
    /// </summary>
    public partial class MessageCl
    {
        //Not nullable
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int BrandId { get; set; }
        public int Likes { get; set; } //at least 0
        public int Dislikes { get; set; } //at least 0
        public int Rating { get; set; }
        public int Views { get; set; } //at least 0

        //Nullable
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual UserCl User { get; set; }
        public virtual BrandCl Brand { get; set; }
    }
}
