using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApiClick.StaticValues;

namespace ApiClick.Models.EnumModels
{
    /// <summary>
    /// Enum модель, содержащая строковое значение роли
    /// </summary>
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string UserRoleName { get; set; }
    }
}
