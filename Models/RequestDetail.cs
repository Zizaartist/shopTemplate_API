using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    /// <summary>
    /// Предлагаемая цена по конкретному продукту
    /// </summary>
    public class RequestDetail
    {
        public int RequestDetailId { get; set; }
        public int RequestId { get; set; }
        public int ProductId { get; set; }
        public decimal SuggestedPrice { get; set; }

        public virtual WaterRequest Request { get; set; }
        public virtual ProductCl Product { get; set; }
    }
}
