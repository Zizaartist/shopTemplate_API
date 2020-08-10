using ApiClick.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    public class AccountController : Controller
    {
        [Route("api/Token")]
        [HttpPost]
        public IActionResult Token(string phone)
        {
            var identity = GetIdentity(phone);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid phone number." });
            }

            var now = DateTime.Now;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string phone)
        {
            ClickContext _context = new ClickContext();
            UserCl user = _context.UserCl.FirstOrDefault(x => x.Phone == phone);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Phone),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, _context.UserRolesCls.First(r => r.UserRolesId == user.role).UserRoleName)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}
