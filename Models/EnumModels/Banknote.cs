using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models.EnumModels
{
    /// <summary>
    /// Модель, хранящая номиналы банкнот, сдачу для которых должен иметь при себе курьер
    /// </summary>
    public class Banknote
    {
        [Key]
        public int BanknoteId { get; set; }
        public decimal Value { get; set; }
    }
}
