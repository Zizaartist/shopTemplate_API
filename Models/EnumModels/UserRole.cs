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
    public enum UserRole
    {
        User,
        Admin,
        SuperAdmin
    }

    public class UserRoleDictionaries
    {
        public static Dictionary<string, UserRole> GetUserRoleFromString = new Dictionary<string, UserRole>()
        {
            { "Пользователь", UserRole.User },
            { "Исполнитель", UserRole.Admin },
            { "Администратор", UserRole.SuperAdmin }
        };

        public static Dictionary<UserRole, string> GetStringFromUserRole = new Dictionary<UserRole, string>
        {
            { UserRole.User, "Пользователь" },
            { UserRole.Admin, "Исполнитель" },
            { UserRole.SuperAdmin, "Администратор" }
        };
    }
}
