using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    /// <summary>
    /// Элемент Order-а, ссылающийся на товар и сам заказ плюс запись стоимости товара на случай каскада
    /// </summary>
    public class OrderDetailCl
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? BrandId { get; set; }
        public int price { get; set; }
        public int count { get; set; }

        [ForeignKey("ProductId")]
        public ProductCl product { get; set; }
        [ForeignKey("OrderId")]
        public OrdersCl order { get; set; }
    }
}
