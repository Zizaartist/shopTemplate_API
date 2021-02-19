using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    public class Report
    {
        //required

        [Key]
        public int ReportId { get; set; }
        public int OrderCount { get; set; }
        public Decimal Sum { get; set; }
        public int BrandId { get; set; }
        public DateTime CreatedDate { get; set; }

        //non-required
        public int? ProductOfDayId { get; set; }

        [ForeignKey("ProductOfDayId")]
        public Product ProductOfDay { get; set; }
        [ForeignKey("BrandId")]
        public Product Brand { get; set; }
    }
}
