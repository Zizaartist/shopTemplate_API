using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models.EnumModels
{
    /// <summary>
    /// Модель, отвечающая за хранение методов оплаты
    /// </summary>
    public partial class PaymentMethodCl
    {
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
    }
}
