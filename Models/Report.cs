using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        public int? OrderId { get; set; }
        public int SenderId { get; set; }
        public string Text { get; set; }

        public virtual OrdersCl Order { get; set; }
        public virtual UserCl Sender { get; set; }
    }
}
