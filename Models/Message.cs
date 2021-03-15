using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
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
        [Required]
        public int UserId { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public int? OrderId { get; set; }
        [Required]
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

        /// <summary>
        /// Проверяет валидность модели, полученной от клиента
        /// </summary>
        public static bool ModelIsValid(Message _message)
        {
            try
            {
                if (_message == null ||
                    //Required
                    _message.OrderId == default ||
                    _message.Rating > 5 || 
                    _message.Rating < 1)
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex)
            {
                Debug.WriteLine($"Ошибка при проверке данных отзыва - {_ex}");
                return false;
            }
        }
    }
}
