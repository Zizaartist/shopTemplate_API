using ApiClick.Controllers.FrequentlyUsed;
using ApiClick.Models;
using ApiClick.Models.EnumModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    /// <summary>
    /// Контроллер, функционал которого связан с авторизацией
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ClickContext _context;

        public AuthController(ClickContext _context)
        {
            this._context = _context;
        }

        /// <summary>
        /// Получение пользовательского access токена
        /// </summary>
        /// <returns>Сериализированный токен</returns>
        // POST: api/Auth/UserToken/?phone=79991745473
        [Route("UserToken")]
        [HttpPost]
        public IActionResult UserToken()
        {
            //Получаем новую "личность"
            var identity = GetNewIdentity();

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Json(response);
        }

        private ClaimsIdentity GetNewIdentity()
        {
            //Получаем из базы последнее значение числа пользователей
            int userNumber = GetUserCount() + 1;
            //Затем преобразуем в хэш чтобы нельзя было reverse-engineer-ить
            var userNumberHash = Functions.GetHashFromString(userNumber.ToString());

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userNumberHash)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }

        //TO-DO
        private int GetUserCount() 
        {
            return 1;
        }
    }
}
