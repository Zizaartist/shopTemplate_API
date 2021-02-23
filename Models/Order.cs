using ApiClick.Models.EnumModels;
using ApiClick.Models.RegisterModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiClick.StaticValues;
using ApiClick.Models.ArrayModels;

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
        public Category Category { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public bool PointsUsed { get; set; }
        [MaxLength(ModelLengths.LENGTH_SMALL)]
        public string Phone { get; set; }
        public bool Delivery { get; set; }

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
        public Banknote? Banknote { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("BrandOwnerId")]
        public virtual User BrandOwner { get; set; }
        [ForeignKey("PointRegisterId")]
        public virtual PointRegister PointRegister { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        //Registers, related to this order
        [NotMapped]
        public virtual ICollection<PointRegister> PointRegisters { get; set; }
    }
}
