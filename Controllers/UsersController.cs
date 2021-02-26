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

namespace ApiClick.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        ClickContext _context;
        Functions funcs = new Functions();
        IMemoryCache _cache;

        public UsersController(IMemoryCache memoryCache, ClickContext _context)
        {
            _cache = memoryCache;
            this._context = _context;
        }

        // GET: api/Users
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users
        [Route("api/GetMyPoints")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<decimal>> GetMyPoints()
        {
            var user = funcs.identityToUser(User.Identity, _context);
            if (user == null)
            {
                return NotFound();
            }
            else 
            {
                return user.Points;
            }
        }

        // GET: api/Users/5PhoneAuth
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [Route("api/GetMyData")]
        [HttpGet]
        public async Task<ActionResult<User>> GetMyData()
        {
            var userCl = funcs.getCleanModel(funcs.identityToUser(User.Identity, _context));

            if (userCl == null)
            {
                return NotFound();
            }
            userCl.Login = null;
            userCl.Password = null;
            userCl.UserRole = Models.EnumModels.UserRole.User;

            return userCl;
        }

        // PUT: api/Users/5
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [Route("api/[controller]/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutUsers(int id, User userCl)
        {
            if (id != userCl.UserId)
            {
                return BadRequest();
            }

            _context.Entry(userCl).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [Route("api/ChangeNumber")]
        [HttpPut]
        public async Task<ActionResult<string>> ChangeUserNumber(string newPhoneNumber, string code)
        {
            if (newPhoneNumber == null || code == null)
            {
                return BadRequest();
            }

            var userCl = funcs.identityToUser(User.Identity, _context);
            var newPhoneNum = funcs.convertNormalPhoneNumber(newPhoneNumber);
            if (!funcs.IsPhoneNumber(newPhoneNum))
            {
                return BadRequest();
            }

            if (userCl == null || userCl.Phone == newPhoneNum)
            {
                return BadRequest();
            }

            //Такой номер уже занят
            if (_context.Users.FirstOrDefault(e => e.Phone == newPhoneNum) != null) 
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
                userCl.Phone = newPhoneNum;
            }
            else
            {
                return BadRequest();
            }

            _context.Entry(userCl).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(userCl.UserId))
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
        [Route("api/PhoneCheck")]
        [HttpPost]
        public async Task<ActionResult<User>> PhoneCheck(string phone)
        {
            var user = _context.Users.FirstOrDefault(u => u.Phone == phone);

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

        [Route("api/PhoneIsRegistered")]
        [HttpPost]
        public async Task<ActionResult<string>> PhoneIsRegistered(string phone)
        {
            string correctPhone = funcs.convertNormalPhoneNumber(phone);
            if (!funcs.phoneIsRegistered(correctPhone, _context)) 
            {
                return NotFound();
            }
            return correctPhone;
        }



        [Route("api/Ping")]
        [HttpGet]
        public async Task<ActionResult> Ping()
        {
            return Ok();
        }

        //returns ok if admin token is still valid
        [Authorize(Roles = "SuperAdmin, Admin")]
        [Route("api/AdminCheck")]
        [HttpPost]
        public async Task<ActionResult<User>> AdminCheck()
        {
            return Ok();
        }

        // Регистрация пользователей
        // POST: api/Users
        [Route("api/[controller]")]
        [HttpPost]
        public async Task<ActionResult<User>> PostUsers(User userCl, string code)
        {
            if (userCl == null)
            {
                return BadRequest();
            }

            userCl.Phone = funcs.convertNormalPhoneNumber(userCl.Phone);
            if (!funcs.IsPhoneNumber(userCl.Phone))
            {
                return BadRequest(new { errorText = "Не является номером телефона." });
            }

            string localCode;
            try
            {
                localCode = _cache.Get<string>(userCl.Phone);
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

            if (_context.Users.Any(x => x.Phone == userCl.Phone))
            {
                return BadRequest(new { errorText = "Такой номер уже зарегистрирован" });
            }
            else
            {
                userCl.CreatedDate = DateTime.Now;
                userCl.UserRole = Models.EnumModels.UserRole.User;
                userCl.Points = 0;

                _context.Users.Add(userCl);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception) 
                {
                    return Forbid();
                }
                return userCl; //без очистки, чтобы заполнить поля в приложении
            }
        }

        //ДОБАВЛЕНИЕ СУПЕРЮЗЕРА. УДАЛИТЬ ПО ВО ВРЕМЯ РЕЛИЗА
        [Route("api/AddSuperAdminTemp")]
        [HttpPost]
        public async Task<ActionResult<User>> AddSuperAdminTemp(User userCl) 
        {
            if (userCl == null)
            {
                return BadRequest();
            }
            else
            {
                userCl.CreatedDate = DateTime.Now;
                userCl.UserRole = UserRole.User;

                _context.Users.Add(userCl);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
