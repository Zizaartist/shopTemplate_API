using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ApiClick.StaticValues;

namespace ApiClick.Models.EnumModels
{
    /// <summary>
    /// Enum модель, хранящая строковые значения статусов
    /// </summary>
    public class OrderStatus
    {
        [Key]
        public int OrderStatusId { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string OrderStatusName { get; set; }
        public int MasterRoleId { get; set; }

        /// <summary>
        /// Роль, которая имеет права сменить статус на текущий
        /// </summary>
        [ForeignKey("MasterRoleId")]
        public virtual UserRole MasterRole { get; set; }

    }
}
