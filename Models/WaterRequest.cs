using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    public class WaterRequest
    {
        [Key]
        public int WaterRequestId { get; set; }
        public int OrderId { get; set; }
        public int BrandId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }

        /// <summary>
        /// Cодержат предлагаемые цены на соответствующие товары
        /// </summary>
        [InverseProperty("Request")]
        public virtual ICollection<RequestDetail> Suggestions { get; set; }
    }
}
