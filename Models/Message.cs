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
        public Message()
        {
            OrderedProducts = new List<Product>();
        }

        //Not nullable
        [Key]
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int BrandId { get; set; }
        public int? OrderId { get; set; }
        public int Rating { get; set; }

        //Nullable
        [MaxLength(ModelLengths.LENGTH_MAX)]
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [NotMapped]
        public List<Product> OrderedProducts { get; set; } //Первые 3 заказанных продукта
    }
}
