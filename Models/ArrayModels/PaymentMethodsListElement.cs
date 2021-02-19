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
        public PaymentMethod PaymentMethod { get; set; }
        public int? BrandId { get; set; } //Не обязательно для предотвращения капризов

        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
    }
}
