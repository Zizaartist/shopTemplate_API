using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiClick;
using ApiClick.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Principal;
using ApiClick.Models.EnumModels;
using ApiClick.Controllers.ScheduledTasks;
using Quartz;
using ApiClick.Controllers.ScheduledTasks.Jobs;
using ApiClick.Controllers.FrequentlyUsed;
using Microsoft.Extensions.Logging;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ClickContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly IMemoryCache _cache;
        private object scheduler;

        public UsersController(IMemoryCache memoryCache, ClickContext _context, ILogger<UsersController> _logger)
        {
            _cache = memoryCache;
            this._context = _context;
            this._logger = _logger;
        }

        // GET: api/Users
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users
        [Route("GetMyPoints")]
        [Authorize]
        [HttpGet]
        public ActionResult<decimal> GetMyPoints()
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);
            return mySelf.Points;
        }

        // GET: api/Users/5PhoneAuth
        [Authorize]
        [Route("GetMyData")]
        [HttpGet]
        public ActionResult<User> GetMyData()
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);

            mySelf.Executor = null;

            return mySelf;
        }

        // PUT: api/Users/5
        //Успешный ответ должен заставить фронт получить новый токен с нового номера
        [Authorize]
        [HttpPut]
        public ActionResult PutUsers(User _userData)
        {
            if (!IsPutModelValid(_userData)) 
            {
                return BadRequest();
            }

            var user = Functions.identityToUser(User.Identity, _context);

            //Вторичные данные
            user.UserInfo.Name = _userData.UserInfo.Name;
            user.UserInfo.Street = _userData.UserInfo.Street;
            user.UserInfo.House = _userData.UserInfo.House;
            user.UserInfo.Entrance = _userData.UserInfo.Entrance;
            user.UserInfo.Floor = _userData.UserInfo.Floor;
            user.UserInfo.Apartment = _userData.UserInfo.Apartment;

            _context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [Route("ChangeNumber")]
        [HttpPut]
        public ActionResult<string> ChangeUserNumber(string newPhoneNumber, string code)
        {
            if (newPhoneNumber == null || code == null)
            {
                return BadRequest();
            }

            var mySelf = Functions.identityToUser(User.Identity, _context);
            var newPhoneNum = Functions.convertNormalPhoneNumber(newPhoneNumber);
            if (!Functions.IsPhoneNumber(newPhoneNum))
            {
                return BadRequest();
            }

            if (mySelf == null || mySelf.Phone == newPhoneNum)
            {
                return BadRequest();
            }

            //Такой номер уже занят
            if (_context.User.FirstOrDefault(e => e.Phone == newPhoneNum) != null) 
            {
                return Forbid();
            }

            string localCode = null;

            try
            {
                localCode = _cache.Get(newPhoneNum).ToString();
            }
            catch 
            {
                return BadRequest();
            } 

            if (localCode == code)
            {
                mySelf.Phone = newPhoneNum;
            }
            else
            {
                return BadRequest();
            }

            _context.Entry(mySelf).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(mySelf.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return newPhoneNum;
        }

        // POST:
        // Авторизация с помощью номера телефона
        [Route("PhoneCheck")]
        [HttpPost]
        public ActionResult PhoneCheck(string phone)
        {
            var user = _context.User.FirstOrDefault(u => u.Phone == phone);

            if (user == null)
            {
                return NotFound(); //"you should register"
            }

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); //"go grab token first"
            }

            return Ok();
        }

        [Route("PhoneIsRegistered")]
        [HttpPost]
        public ActionResult<string> PhoneIsRegistered(string phone)
        {
            string correctPhone = Functions.convertNormalPhoneNumber(phone);
            if (!Functions.phoneIsRegistered(correctPhone, _context)) 
            {
                return NotFound();
            }
            return correctPhone;
        }

        // Регистрация пользователей
        // POST: api/Users
        [HttpPost]
        public ActionResult PostUsers(User _user, string code)
        {
            if (!IsPostModelValid(_user))
            {
                return BadRequest();
            }

            _user.Phone = Functions.convertNormalPhoneNumber(_user.Phone);
            if (!Functions.IsPhoneNumber(_user.Phone))
            {
                return BadRequest(new { errorText = "Не является номером телефона." });
            }

            string localCode;
            try
            {
                localCode = _cache.Get<string>(_user.Phone);
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

            if (_context.User.Any(x => x.Phone == _user.Phone))
            {
                return BadRequest(new { errorText = "Такой номер уже зарегистрирован" });
            }
            else
            {
                _user.CreatedDate = DateTime.UtcNow;
                _user.UserRole = UserRole.User;
                _user.Points = 0;
                _user.NotificationsEnabled = false;
                _user.UserInfo = new UserInfo();

                _context.User.Add(_user);
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception) 
                {
                    return Forbid();
                }
                return Ok(); //без очистки, чтобы заполнить поля в приложении
            }
        }

        private bool UsersExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }

        /// <summary>
        /// Валидация получаемых данных
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsPutModelValid(User _user)
        {
            try
            {
                if(_user == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации User модели PUT метода - {_ex}");
                return false;
            }
        }

        /// <summary>
        /// Валидация получаемых данных
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsPostModelValid(User _user)
        {
            try
            {
                if(_user == null ||
                    string.IsNullOrEmpty(_user.Phone) ||
                    !Functions.IsPhoneNumber(_user.Phone))
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации User модели POST метода - {_ex}");
                return false;
            }
        }
    }
}
