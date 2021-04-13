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
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ClickContext _context;
        private readonly IMemoryCache _cache;

        public AccountController(IMemoryCache memoryCache, ClickContext _context)
        {
            _cache = memoryCache;
            this._context = _context;
        }

        /// <summary>
        /// Получение пользовательского access токена
        /// </summary>
        /// <returns>Сериализированный токен</returns>
        // POST: api/Account/UserToken/?phone=79991745473
        [Route("UserToken")]
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

        /// <summary>
        /// Получение access токена с правами исполнителя
        /// </summary>
        /// <returns>Сериализированный токен</returns>
        // POST: api/Account/AdminToken/?login=Zizi&password=zaza
        [Route("AdminToken")]
        [HttpPost]
        public IActionResult AdminToken(string login, string password)
        {
            var identity = GetIdentity(login, Functions.GetHashFromString(password));
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

        /// <summary>
        /// Отправляет СМС код на указанный номер и создает временный кэш с кодом для проверки
        /// </summary>
        /// <param name="phone">Неотформатированный номер</param>
        // POST: api/Account/SmsCheck/?phone=79991745473
        [Route("SmsCheck")]
        [HttpPost]
        public async Task<IActionResult> SmsCheck(string phone)
        {
            string PhoneLoc = Functions.convertNormalPhoneNumber(phone);
            Random rand = new Random();
            string generatedCode = rand.Next(1000, 9999).ToString();
            if (phone != null)
            {
                if (Functions.IsPhoneNumber(PhoneLoc))
                {
                    //Позволяет получать ip отправителя, можно добавить к запросу sms api для фильтрации спаммеров
                    var senderIp = Request.HttpContext.Connection.RemoteIpAddress;
                    string moreReadable = senderIp.ToString();

                    HttpClient client = HttpClientSingleton.HttpClient;
                    HttpResponseMessage response = await client.GetAsync($"https://sms.ru/sms/send?api_id=0F4D5813-DEF7-5914-2D97-42D0FBA75865&to={PhoneLoc}&msg={generatedCode}&json=1");
                    if (response.IsSuccessStatusCode)
                    {
                        //Добавляем код в кэш на 5 минут
                        _cache.Set(Functions.convertNormalPhoneNumber(phone), generatedCode, new MemoryCacheEntryOptions
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

        /// <summary>
        /// Проверяет активность (сущ.) кода
        /// </summary>
        /// <param name="code">СМС код</param>
        /// <param name="phone">Номер получателя</param>
        // POST: api/Account/CodeCheck/?code=3344&phone=79991745473
        [Route("CodeCheck")]
        [HttpPost]
        public IActionResult CodeCheck(string code, string phone)
        {
            if (code == _cache.Get(Functions.convertNormalPhoneNumber(phone)).ToString())
            {
                return Ok();
            }

            return BadRequest();
        }

        /// <summary>
        /// Возвращает СМС код напрямую
        /// </summary>
        /// <param name="phone">Номер получателя</param>
        /// <returns>СМС код</returns>
        // POST: api/Account/CacheCheck/?phone=79991745473
        [Authorize(Roles = "SuperAdmin")]
        [Route("CacheCheck")]
        [HttpPost]
        public ActionResult<string> CacheCheck(string phone)
        {
            try
            {
                string value = _cache.Get<string>(Functions.convertNormalPhoneNumber(phone));
                return value;
            }
            catch (Exception) 
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Проверка на наличие регистрации и валидности токена
        /// </summary>
        /// <param name="phone">Отформатированный номер</param>
        // POST: api/Account/PhoneCheck/?phone=79991745473
        [Route("PhoneCheck")]
        [HttpPost]
        public ActionResult PhoneCheck(string phone)
        {
            var foundUser = _context.User.Any(user => user.Phone == phone);

            if (!foundUser)
            {
                return NotFound(); //"Номер не зарегистрирован"
            }

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); //"Токен недействителен или отсутствует"
            }

            return Ok();
        }

        /// <summary>
        /// Возвращает отформатированный номер
        /// </summary>
        /// <param name="phone">Неотформатированный номер</param>
        /// <returns>Отформатированный номер</returns>
        // POST: api/Account/GetValidPhone/?phone=79991745473
        [Route("GetValidPhone")]
        [HttpPost]
        public ActionResult<string> GetValidPhone(string phone)
        {
            string correctPhone = Functions.convertNormalPhoneNumber(phone);
            return correctPhone;
        }

        //identity with user rights
        private ClaimsIdentity GetIdentity(string phone)
        {
            User user = _context.User.FirstOrDefault(x => x.Phone == phone);

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
            var executor = _context.User.FirstOrDefault(exe => (exe.Executor.Login == login) && (exe.Executor.Password == password));

            //if user wasn't found or his role is user = ignore
            if (executor != null && executor.UserRole != UserRole.User)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, executor.Phone),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, UserRoleDictionaries.GetStringFromUserRole[executor.UserRole]) //probably not safe but eh works fine by me
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
