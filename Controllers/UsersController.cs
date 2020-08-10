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
        [HttpGet("{id}")]
        public async Task<ActionResult<UserCl>> GetUserCl(int id)
        {
            var userCl = await _context.UserCl.FindAsync(id);

            if (userCl == null)
            {
                return NotFound();
            }

            return userCl;
        }

        // PUT: api/Users/5
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPut("{id}")]
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
        [Route("api/PhoneAuth")]
        [HttpPost]
        public async Task<ActionResult<UserCl>> PhoneAuth(string phone)
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

            return user;
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
                userCl.role = _context.UserRolesCls.First(r => r.UserRoleName == "User").UserRolesId;

                _context.UserCl.Add(userCl);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetUserCl", new { id = userCl.UserId }, userCl);
            }
        }

        private bool UserClExists(int id)
        {
            return _context.UserCl.Any(e => e.UserId == id);
        }
    }
}
