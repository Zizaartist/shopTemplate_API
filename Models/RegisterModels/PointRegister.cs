using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models.RegisterModels
{
    /// <summary>
    /// Учет абсолютно всех операций с баллами, 
    /// хранится только до тех пор пока у заказа не будет достигнут финальный статус
    /// после чего - удаляется
    /// </summary>
    public class PointRegister
    {
        public int PointRegisterId { get; set; }
        public int? OrderId { get; set; }
        public int? UserId { get; set; }
        /// <summary>
        /// Определяет в какую сторону изменяется счет: true + | false -
        /// </summary>
        public bool ValueChange { get; set; }
        public Decimal Sum { get; set; }

        public virtual OrdersCl Order { get; set; }
        public virtual UserCl User { get; set; } 
    }
}
