﻿using ApiClick.Models.EnumModels;
using ApiClick.Models.RegisterModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiClick.StaticValues;
using ApiClick.Models.ArrayModels;
using System.Linq;
using System.Diagnostics;

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
        [Required]
        public int UserId { get; set; }
        [Required]
        public Category Category { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        [Required]
        public OrderStatus OrderStatus { get; set; }
        [Required]
        public bool PointsUsed { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string Phone { get; set; }
        [Required]
        public bool Delivery { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Orderer { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Street { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_MIN)]
        public string House { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }

        //Nullable
        /// <summary>
        /// Хранит id владельца бренда просто для логгирования
        /// </summary>
        public int? BrandOwnerId { get; set; }
        [MaxLength(ModelLengths.LENGTH_MAX)]
        public string Commentary { get; set; }
        public int? Padik { get; set; }
        public int? Etash { get; set; }
        public int? Kv { get; set; }
        public int? PointRegisterId { get; set; }
        public Banknote? Banknote { get; set; }

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

        /// <summary>
        /// Проверяет валидность модели, полученной от клиента
        /// </summary>
        public static bool ModelIsValid(Order _order)
        {
            try
            {
                if (_order == null ||
                    //Collections
                    _order.OrderDetails == null ||
                    !_order.OrderDetails.Any() ||
                    //Required
                    string.IsNullOrEmpty(_order.Street) ||
                    string.IsNullOrEmpty(_order.House))
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex)
            {
                Debug.WriteLine($"Ошибка при проверке данных заказа - {_ex}");
                return false;
            }
        }
    }
}
