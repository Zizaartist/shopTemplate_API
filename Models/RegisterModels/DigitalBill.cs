using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models.RegisterModels
{
    /// <summary>
    /// Производит учет трат баллов для возможности возврата
    /// </summary>
    public class DigitalBill
    {
        //Non-nullable
        public int DigitalBillId { get; set; }
        public int? OrderId { get; set; }
        public int? OwnerId { get; set; }
        public Decimal Points { get; set; }
        public Decimal Money { get; set; }
        /// <summary>
        /// Указывает на завершенность оплаты, true - лишь в случае успешного выполнения
        /// </summary>
        public bool TransactionCompleted { get; set; }

        //Navigation properties
        public virtual OrdersCl Order { get; set; }
        public virtual UserCl Owner { get; set; }
    }
}
