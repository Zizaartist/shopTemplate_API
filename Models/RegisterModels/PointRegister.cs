using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models.RegisterModels
{
    /// <summary>
    /// Производит учет трат баллов для возможности возврата
    /// </summary>
    public class PointRegister
    {
        //Non-nullable
        [Key]
        public int PointRegisterId { get; set; }
        public int OrderId { get; set; }
        public int OwnerId { get; set; }
        public Decimal Points { get; set; }
        /// <summary>
        /// Указывает на завершенность траты баллов, true - лишь в случае успешного выполнения
        /// </summary>
        public bool TransactionCompleted { get; set; }

        //Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [ForeignKey("OwnerId")]
        public virtual User Owner { get; set; }
    }
}
