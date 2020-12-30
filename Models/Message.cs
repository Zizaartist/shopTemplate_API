using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель отзыва, оставленного пользователем касательно бренда
    /// </summary>
    public partial class Message
    {
        //Not nullable
        [Key]
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int BrandId { get; set; }
        public int Likes { get; set; } //at least 0
        public int Dislikes { get; set; } //at least 0
        public int Rating { get; set; }
        public int Views { get; set; } //at least 0

        //Nullable
        [MaxLength(ModelLengths.LENGTH_MAX)]
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
    }
}
