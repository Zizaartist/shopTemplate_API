﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.StaticValues
{
    public static class Constants
    {
        public readonly static TimeSpan YAKUTSK_OFFSET = new TimeSpan(9, 0, 0); //GMT +9
        public readonly static decimal DELIVERY_PRICE = 300m;
        public readonly static decimal MINIMAL_PRICE = 1000m;
    }
}
