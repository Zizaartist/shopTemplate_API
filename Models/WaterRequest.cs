﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    public class WaterRequest
    {
        public int WaterRequestId { get; set; }
        public int OrderId { get; set; }
        public int BrandId { get; set; }

        public virtual OrdersCl Order { get; set; }
        public virtual BrandCl Brand { get; set; }

        /// <summary>
        /// Cодержат предлагаемые цены на соответствующие товары
        /// </summary>
        public virtual ICollection<RequestDetail> Suggestions { get; set; }
    }
}
