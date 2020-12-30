using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    /// <summary>
    /// Элемент Order-а, ссылающийся на товар и сам заказ плюс запись стоимости товара на случай каскада
    /// </summary>
    public class OrderDetail
    {
        //Not nullable
        [Key]
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int Count { get; set; }

        //Nullable
        public int? ProductId { get; set; }
        public Decimal Price { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
