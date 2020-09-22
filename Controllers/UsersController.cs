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

namespace ApiClick.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        ClickContext _context = new ClickContext();

        // GET: api/Users
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserCl>>> GetUserCl()
        {
            return await _context.UserCl.ToListAsync();
        }

        // GET: api/Users/5PhoneAuth
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [Route("api/[controller]/{id}")]
        [HttpGet]
        public async Task<ActionResult<UserCl>> GetUserCl(int id)
        {
            var userCl = await _context.UserCl.FindAsync(id);
            var userCl2 = _context.UserCl.FirstOrDefault(e => e.UserId == id);

            if (userCl == null)
            {
                return NotFound();
            }

            return userCl;
        }

        // PUT: api/Users/5
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [Route("api/[controller]/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutUserCl(int id, UserCl userCl)
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
                if (!UserClExists(id))
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

        // POST:
        // Авторизация с помощью номера телефона
        [Route("api/PhoneCheck")]
        [HttpPost]
        public async Task<ActionResult<UserCl>> PhoneCheck(string phone)
        {
            var user = _context.UserCl.FirstOrDefault(u => u.Phone == phone);

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

        //returns ok if admin token is still valid
        [Authorize(Roles = "SuperAdmin, Admin")]
        [Route("api/AdminCheck")]
        [HttpPost]
        public async Task<ActionResult<UserCl>> AdminCheck()
        {
            return Ok();
        }

        // POST: api/Users
        [Route("api/[controller]")]
        [HttpPost]
        public async Task<ActionResult<UserCl>> PostUserCl(UserCl userCl)
        {
            if (userCl == null)
            {
                return BadRequest();

            }

            if (_context.UserCl.Any(x => x.Phone == userCl.Phone))
            {
                return BadRequest(); //prob wrong code 
            }
            else
            {
                userCl.CreatedDate = DateTime.Now;
                userCl.Role = _context.UserRolesCl.First(r => r.UserRoleName == "User").UserRoleId;

                _context.UserCl.Add(userCl);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }

        //ДОБАВЛЕНИЕ СУПЕРЮЗЕРА. УДАЛИТЬ ПО ВО ВРЕМЯ РЕЛИЗА
        [Route("api/AddSuperAdminTemp")]
        [HttpPost]
        public async Task<ActionResult<UserCl>> AddSuperAdminTemp(UserCl userCl) 
        {
            if (userCl == null)
            {
                return BadRequest();
            }
            else
            {
                userCl.CreatedDate = DateTime.Now;
                userCl.Role = _context.UserRolesCl.First(r => r.UserRoleName == "User").UserRoleId;

                _context.UserCl.Add(userCl);
                await _context.SaveChangesAsync();
                return Ok();
            }
        }

        private bool UserClExists(int id)
        {
            return _context.UserCl.Any(e => e.UserId == id);
        }
    }
}
