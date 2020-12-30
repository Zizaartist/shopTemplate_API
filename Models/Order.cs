using ApiClick.Models.EnumModels;
using ApiClick.Models.RegisterModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, содержащая данные заказа
    /// </summary>
    public partial class Order
    {
        public Order() 
        {
            OrderDetails = new List<OrderDetail>();
        }

        //Not nullable
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
        public int CategoryId { get; set; }
        public int PaymentMethodId { get; set; }
        public bool PointsUsed { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string Phone { get; set; }

        //Nullable
        /// <summary>
        /// Хранит id владельца бренда просто для логгирования
        /// </summary>
        public int? BrandOwnerId { get; set; }
        [MaxLength(ModelLengths.LENGTH_MAX)]
        public string Commentary { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Street { get; set; }
        [MaxLength(ModelLengths.LENGTH_MIN)]
        public string House { get; set; }
        public int? Padik { get; set; }
        public int? Etash { get; set; }
        public int? Kv { get; set; }
        public int? PointRegisterId { get; set; }
        public int? BanknoteId { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("BrandOwnerId")]
        public virtual User BrandOwner { get; set; }
        [ForeignKey("StatusId")]
        public virtual OrderStatus OrderStatus { get; set; }
        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod PaymentMethod { get; set; }
        [ForeignKey("PointRegisterId")]
        public virtual PointRegister PointRegister { get; set; }
        [ForeignKey("BanknoteId")]
        public virtual Banknote Banknote { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
