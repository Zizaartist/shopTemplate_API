using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClick
{
    public class AuthOptions
    {
        public const string ISSUER = "ClickAuthController"; // издатель токена
        public const string AUDIENCE = "ClickClient"; // потребитель токена
        public const int LIFETIME = 1; // дней
        const string KEY = "imtirediwanttodiepleaseendme";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
