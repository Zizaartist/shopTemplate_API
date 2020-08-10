using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    public partial class OrdersCl
    {
        public int OrdersId { get; set; }
        public int? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public virtual UserCl User { get; set; }
        public virtual ICollection<OrderDetailCl> orderDetails { get; set; }
    }
}
