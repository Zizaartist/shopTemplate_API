using ApiClick.Models.RegisterModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models.ArrayModels
{
    public class PointRegistersListElement
    {
        [Key]
        public int PointRegistersListElementId { get; set; }
        public int PointRegisterId { get; set; }
        public int? OrderId { get; set; } //Не обязательно для предотвращения капризов

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        [ForeignKey("PointRegisterId")]
        public virtual PointRegister PointRegister { get; set; } //Хранимое значение
    }
}
