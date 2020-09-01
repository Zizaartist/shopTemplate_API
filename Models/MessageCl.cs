using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    public partial class MessageCl
    {
        public int MessageId { get; set; }
        public int? CategoryId { get; set; } //?
        public string Description { get; set; }
        public int? Likes { get; set; } //required
        public int? DisLike { get; set; } //to Dislikes/required
        public int? Rating { get; set; } //required
        public int? Views { get; set; } //required
        public int? BrandId { get; set; } //?
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }

        public virtual UserCl User { get; set; }
    }
}
