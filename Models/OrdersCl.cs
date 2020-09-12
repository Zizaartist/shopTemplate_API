using ApiClick.Models.EnumModels;
using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, содержащая данные заказа
    /// </summary>
    public partial class OrdersCl
    {
        //Not nullable
        public int OrdersId { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }

        /// <summary>
        /// Хранит id владельца бренда просто для логгирования
        /// </summary>
        public int BrandOwnerId { get; set; }

        //Nullable
        public DateTime CreatedDate { get; set; }

        public virtual UserCl User { get; set; }
        public virtual UserCl BrandOwner { get; set; }
        public virtual OrderStatusCl OrderStatus { get; set; }
        public virtual ICollection<OrderDetailCl> OrderDetails { get; set; }
    }
}
