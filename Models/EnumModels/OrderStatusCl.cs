using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models.EnumModels
{
    /// <summary>
    /// Enum модель, хранящая строковые значения статусов
    /// </summary>
    public class OrderStatusCl
    {

        public int OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }

    }
}
