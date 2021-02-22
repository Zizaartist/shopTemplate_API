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
        public int? OrderId { get; set; }
        public int? SenderId { get; set; }
        public int ReceiverId { get; set; }
        public Decimal Points { get; set; }
        /// <summary>
        /// Указывает на завершенность траты баллов, true - лишь в случае успешного выполнения
        /// </summary>
        public bool TransactionCompleted { get; set; }
        //Дата создания
        public DateTime CreatedDate { get; set; }

        //Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }
    }
}
