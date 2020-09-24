using ApiClick.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    public class AccountController : Controller
    {

        private IMemoryCache cache;

        public AccountController(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }

        [Route("api/UserToken")]
        [HttpPost]
        public IActionResult UserToken(string phone)
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

        [Route("api/AdminToken")]
        [HttpPost]
        public IActionResult AdminToken(string login, string password)
        {
            var identity = GetIdentity(login, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid credentials." });
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

        [Route("api/SmsCheck")]
        [HttpPost]
        public IActionResult SmsCheck(string phone)
        {
            Random rand = new Random();
            string randomNumber = rand.Next(1000, 9999).ToString();
            string PhoneLoc = phone;
            if (phone != null)
            {
                if (IsPhoneNumber(PhoneLoc))
                {
                    //HttpClient client = new HttpClient();
                    //HttpResponseMessage response = await client.GetAsync("https://smsc.ru/sys/send.php?login=syberia&psw=K1e2s3k4i5l6&phones=" + PhoneLoc + "&mes=" + randomNumer);
                    //await response.Content.ReadAsStringAsync();
                    cache.Set(phone, randomNumber, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    });
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok();
        }

        [Route("api/CodeCheck")]
        [HttpPost]
        public IActionResult CodeCheck(string code, string phone)
        {
            if (code == cache.Get(phone).ToString())
            {
                return Ok();
            }


            return BadRequest();
        }

        public static bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^(\+*[0-9]{11})$").Success;
        }

        //identity with user rights
        private ClaimsIdentity GetIdentity(string phone)
        {
            ClickContext _context = new ClickContext();
            UserCl user = _context.UserCl.FirstOrDefault(x => x.Phone == phone);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Phone),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
        
        //identity with admin rights
        private ClaimsIdentity GetIdentity(string login, string password)
        {
            ClickContext _context = new ClickContext();
            UserCl user = _context.UserCl.Where(e => e.Login == login).FirstOrDefault(e => e.Password == password);

            //if user wasn't found or his role is user = ignore
            if (user != null && user.Role != _context.UserRolesCl.FirstOrDefault(e => e.UserRoleName == "User").UserRoleId)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Phone),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, _context.UserRolesCl.Find(user.Role).UserRoleName) //probably not safe but eh works fine by me
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
