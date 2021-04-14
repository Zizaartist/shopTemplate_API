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
        public const string ISSUER = "ShopTokenSource"; // издатель токена
        public const string AUDIENCE = "ShopTokenReceiver"; // потребитель токена
        const string KEY = "iamalongasskeyyoustupididiotswillnevercrackmelmao";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
