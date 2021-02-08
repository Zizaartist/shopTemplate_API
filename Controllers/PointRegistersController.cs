using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiClick;
using ApiClick.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore.Internal;
using ApiClick.Models.EnumModels;
using ApiClick.Models.RegisterModels;
using ApiClick.StaticValues;
using System.ComponentModel;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PointRegistersController : ControllerBase
    {
        ClickContext _context;
        Functions funcs = new Functions();

        public PointRegistersController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/PointRegisters
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointRegister>>> GetPointRegisters()
        {
            var user = funcs.identityToUser(User.Identity, _context);
            if (user == null) 
            {
                return NotFound();
            }
            return _context.PointRegisters.Where(e => e.ReceiverId == user.UserId || e.SenderId == user.UserId).ToList();
        }
    }
}
