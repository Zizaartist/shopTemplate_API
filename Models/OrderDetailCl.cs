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
        //Not nullable
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }

        //Nullable
        public int? ProductId { get; set; }
        public int? Price { get; set; }

        public ProductCl Product { get; set; }
        public OrdersCl Order { get; set; }
    }
}
