using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApiClick.Models.ArrayModels;
using ApiClick.StaticValues;

namespace ApiClick.Models.EnumModels
{
    /// <summary>
    /// Модель, отвечающая за хранение методов оплаты
    /// </summary>
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodId { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string PaymentMethodName { get; set; }

        public virtual ICollection<PaymentMethodsListElement> PaymentMethodsListElements { get; set; }
    }
}
