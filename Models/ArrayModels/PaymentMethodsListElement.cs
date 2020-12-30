using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ApiClick.Models.EnumModels;
using Click.Models;

namespace ApiClick.Models.ArrayModels
{
    public class PaymentMethodsListElement
    {
        [Key]
        public int PaymentMethodListElementId { get; set; }
        public int PaymentMethodId { get; set; }
        public int BrandId { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod PaymentMethod { get; set; } //Хранимое значение
    }
}
