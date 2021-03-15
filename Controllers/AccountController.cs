using ApiClick.Controllers.FrequentlyUsed;
using ApiClick.Models;
using ApiClick.Models.EnumModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class AccountController : Controller
    {
        private ClickContext _context;
        private IMemoryCache _cache;
        Functions funcs = new Functions();

        public AccountController(IMemoryCache memoryCache, ClickContext _context)
        {
            _cache = memoryCache;
            this._context = _context;
        }

        [Route("api/UserToken")]
        [HttpPost]
        public IActionResult UserToken(string phone)
        {
            //Поскольку вызов является автономным не вижу смысла конвертировать телефон, ибо берется из кэша, нежели пользовательского ввода
            var identity = GetIdentity(phone);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid phone number." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromDays(AuthOptions.LIFETIME)),
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

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromDays(AuthOptions.LIFETIME)),
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
        public async Task<IActionResult> SmsCheck(string phone)
        {
            string PhoneLoc = funcs.convertNormalPhoneNumber(phone);
            Random rand = new Random();
            string generatedCode = rand.Next(1000, 9999).ToString();
            if (phone != null)
            {
                if (funcs.IsPhoneNumber(PhoneLoc))
                {
                    //Позволяет получать ip отправителя, можно добавить к запросу sms api для фильтрации спаммеров
                    var senderIp = Request.HttpContext.Connection.RemoteIpAddress;
                    string moreReadable = senderIp.ToString();

                    HttpClient client = HttpClientSingleton.HttpClient;
                    HttpResponseMessage response = await client.GetAsync($"https://sms.ru/sms/send?api_id=0F4D5813-DEF7-5914-2D97-42D0FBA75865&to={PhoneLoc}&msg={generatedCode}&json=1");
                    if (response.IsSuccessStatusCode)
                    {
                        //Добавляем код в кэш на 5 минут
                        _cache.Set(funcs.convertNormalPhoneNumber(phone), generatedCode, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                        });
                    }
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
            if (code == _cache.Get(funcs.convertNormalPhoneNumber(phone)).ToString())
            {
                return Ok();
            }

            return BadRequest();
        }

        [Authorize(Roles = "SuperAdmin")]
        [Route("api/CacheCheck")]
        [HttpPost]
        public async Task<ActionResult<string>> CacheCheck(string phone)
        {
            try
            {
                string value = _cache.Get<string>(funcs.convertNormalPhoneNumber(phone));
                return value;
            }
            catch (Exception) 
            {
                return BadRequest();
            }
        }

        //identity with user rights
        private ClaimsIdentity GetIdentity(string phone)
        {
            User user = _context.Users.FirstOrDefault(x => x.Phone == phone);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Phone),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, UserRoleDictionaries.GetStringFromUserRole[UserRole.User])
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
            User user = _context.Users.Where(e => e.Login == login).FirstOrDefault(e => e.Password == password);

            //if user wasn't found or his role is user = ignore
            if (user != null && user.UserRole != Models.EnumModels.UserRole.User)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Phone),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, UserRoleDictionaries.GetStringFromUserRole[user.UserRole]) //probably not safe but eh works fine by me
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
