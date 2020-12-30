using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    /// <summary>
    /// Предлагаемая цена по конкретному продукту
    /// </summary>
    public class RequestDetail
    {
        [Key]
        public int RequestDetailId { get; set; }
        public int RequestId { get; set; }
        public int ProductId { get; set; }
        public decimal SuggestedPrice { get; set; }

        [ForeignKey("RequestId")]
        public virtual WaterRequest Request { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
