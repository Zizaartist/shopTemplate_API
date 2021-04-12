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

        /// <summary>
        /// Получаем количество баллов отправителя запроса
        /// </summary>
        // GET: api/Users/GetMyPoints
        [Route("GetMyPoints")]
        [Authorize]
        [HttpGet]
        public ActionResult<decimal> GetMyPoints()
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);
            return mySelf.Points;
        }

        /// <summary>
        /// Получаем пользовательскую информацию отправителя
        /// </summary>
        // GET: api/Users/GetMyData
        [Authorize]
        [Route("GetMyData")]
        [HttpGet]
        public ActionResult<User> GetMyData()
        {
            var mySelf = _context.User.Include(user => user.UserInfo)
                                        .FirstOrDefault(user => user.Phone == User.Identity.Name);

            return mySelf;
        }

        /// <summary>
        /// Обновляет незначительные пользовательские данные
        /// </summary>
        /// <param name="_userData">Новые пользовательские данные</param>
        // PUT: api/Users
        [Authorize]
        [HttpPut]
        public ActionResult PutUsers(User _userData)
        {
            if (!IsPutModelValid(_userData)) 
            {
                return BadRequest();
            }

            var mySelf = _context.User.Include(user => user.UserInfo)
                                        .FirstOrDefault(user => user.Phone == User.Identity.Name);

            //Вторичные данные
            mySelf.UserInfo.Name = _userData.UserInfo.Name;
            mySelf.UserInfo.Street = _userData.UserInfo.Street;
            mySelf.UserInfo.House = _userData.UserInfo.House;
            mySelf.UserInfo.Entrance = _userData.UserInfo.Entrance;
            mySelf.UserInfo.Floor = _userData.UserInfo.Floor;
            mySelf.UserInfo.Apartment = _userData.UserInfo.Apartment;

            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Изменяет текущий номер телефона
        /// </summary>
        /// <param name="newPhoneNumber">Новый номер</param>
        /// <param name="code">СМС код</param>
        /// <returns>Новый номер, отформатированный</returns>
        // PUT: api/Users/ChangeNumber/?newPhoneNumber=88005553535&code=3344
        [Authorize]
        [Route("ChangeNumber")]
        [HttpPut]
        public ActionResult<string> ChangeNumber(string newPhoneNumber, string code)
        {
            if (newPhoneNumber == null || code == null)
            {
                return BadRequest();
            }

            var mySelf = _context.User.FirstOrDefault(user => user.Phone == User.Identity.Name);
            var newPhoneNum = Functions.convertNormalPhoneNumber(newPhoneNumber);
            if (!Functions.IsPhoneNumber(newPhoneNum))
            {
                return BadRequest("Указан неправильный номер.");
            }

            if (mySelf.Phone == newPhoneNum)
            {
                return BadRequest("Указан текущий номер.");
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
                return BadRequest("Код устарел.");
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

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="_user">Данные предоставленные клиентом</param>
        /// <param name="code">СМС код</param>
        // POST: api/Users/?code=3366
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
