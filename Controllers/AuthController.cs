using ApiClick.Configurations;
using ApiClick.Controllers.FrequentlyUsed;
using ApiClick.Models;
using ApiClick.Models.EnumModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        private readonly ShopContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<OrdersController> _logger;

        public AuthController(IMemoryCache memoryCache, ShopContext _context, ILogger<OrdersController> _logger)
        {
            _cache = memoryCache;
            this._context = _context;
            this._logger = _logger;
        }

        /// <summary>
        /// Получение пользовательского access токена
        /// в случае отсутствия пользователя в бд, создается новый
        /// </summary>
        /// <returns>Сериализированный токен и корректный номер телефона</returns>
        // POST: api/Auth/UserToken/?phone=79991745473&code=3667
        [Route("UserToken")]
        [HttpPost]
        public IActionResult UserToken(string phone, string code)
        {
            if (!Functions.IsPhoneNumber(phone))
            {
                return BadRequest(new { errorText = "Invalid phone number." });
            }

            var formattedPhone = Functions.convertNormalPhoneNumber(phone);

            string localCode;
            try
            {
                localCode = _cache.Get<string>(formattedPhone);
            }
            catch (Exception)
            {
                return BadRequest(new { errorText = "Ошибка при извлечении из кэша." });
            }

            if (localCode == null)
            {
                return BadRequest(new { errorText = "Устаревший или отсутствующий код." });
            }
            else
            {
                if (localCode != code)
                {
                    return BadRequest(new { errorText = "Ошибка. Получен неверный код. Подтвердите номер еще раз." });
                }
            }

            ClaimsIdentity identity;
            try
            {
                identity = GetIdentity(formattedPhone);
            }
            catch (Exception _ex) 
            {
                _logger.LogWarning($"Ошибка при попытке получить identity - {_ex}");
                return BadRequest(new { errorText = "Unexpected error." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: ApiConfiguration.SHOP_ID,
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

        [Route("UserTokenDefault")]
        [HttpGet]
        public IActionResult UserTokenDefault()
        {
            ClaimsIdentity identity;
            identity = GetIdentityDefault();

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: ApiConfiguration.SHOP_ID,
                    notBefore: now,
                    claims: identity.Claims,
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            //Добавляем запись о создании токена
          
            var commandText = $"INSERT INTO TokenRecord(CreatedDate)VALUES(@DataTime)";
            var name = new SqlParameter("@DataTime", DateTime.UtcNow.Date);
            _context.Database.ExecuteSqlRaw(commandText, name);

            return Json(response);
        }

        /// <summary>
        /// Отправляет СМС код на указанный номер и создает временный кэш с кодом для проверки
        /// </summary>
        /// <param name="phone">Неотформатированный номер</param>
        // POST: api/Auth/SmsCheck/?phone=79991745473
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
                    HttpResponseMessage response = await client.GetAsync($"https://smsc.ru/sys/send.php?login=syberia&psw=K1e2s3k4i5l6&phones={PhoneLoc}&mes={generatedCode}");
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
        // POST: api/Auth/CodeCheck/?code=3344&phone=79991745473
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
        /// Подтверждает валидность токена
        /// </summary>
        // GET: api/Auth/ValidateToken
        [Route("ValidateToken")]
        [HttpGet]
        public ActionResult ValidateToken()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); //"Токен недействителен или отсутствует"
            }

            return Ok();
        }

        /// <summary>
        /// Получаем количество баллов отправителя запроса
        /// </summary>
        // GET: api/Auth/GetMyPoints
        [Route("GetMyPoints")]
        [Authorize]
        [HttpGet]
        public ActionResult<decimal> GetMyPoints()
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);
            return mySelf.Points;
        }

        //identity with user rights
        private ClaimsIdentity GetIdentity(string phone)
        {
            try
            {
                //Сперва находим существующего пользователя, либо создаем нового
                User user = _context.User.FirstOrDefault(x => x.Phone == phone);

                if (user == null)
                {
                    user = RegisterNewUser(phone);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Phone),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "Rolee")
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            catch (Exception _ex) 
            {
                throw _ex;
            }
        }

        private ClaimsIdentity GetIdentityDefault()
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, "DefaultName"),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "DefaultRole")
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            catch (Exception _ex)
            {
                throw _ex;
            }
        }

        private User RegisterNewUser(string _phone) 
        {
            try
            {
                var newUser = new User()
                {
                    Phone = _phone,
                    CreatedDate = DateTime.Now,
                    Points = 0m
                };

                _context.User.Add(newUser);
                _context.SaveChanges();

                return newUser;
            }
            catch (Exception _ex) 
            {
                throw _ex;
            }
        }
    }
}
