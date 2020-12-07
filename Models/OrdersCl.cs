﻿using ApiClick.Models.EnumModels;
using ApiClick.Models.RegisterModels;
using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, содержащая данные заказа
    /// </summary>
    public partial class OrdersCl
    {
        public OrdersCl() 
        {
            OrderDetails = new List<OrderDetailCl>();
        }

        //Not nullable
        public int OrdersId { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
        public int CategoryId { get; set; }
        public int PaymentMethodId { get; set; }
        public bool PointsUsed { get; set; }
        public string Phone { get; set; }

        //Nullable
        /// <summary>
        /// Хранит id владельца бренда просто для логгирования
        /// </summary>
        public int? BrandOwnerId { get; set; }
        public string Commentary { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public int? Padik { get; set; }
        public int? Etash { get; set; }
        public int? Kv { get; set; }
        public int? PointRegisterId { get; set; }
        public int? BanknoteId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual CategoryCl Category { get; set; }
        public virtual UserCl User { get; set; }
        public virtual UserCl BrandOwner { get; set; }
        public virtual OrderStatusCl OrderStatus { get; set; }
        public virtual PaymentMethodCl PaymentMethod { get; set; }
        public virtual ICollection<OrderDetailCl> OrderDetails { get; set; }
        public virtual PointRegister PointRegister { get; set; }
        public virtual BanknoteCl Banknote { get; set; }
    }
}
