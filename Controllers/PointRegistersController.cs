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
using ApiClick.StaticValues;
using System.ComponentModel;
using ApiClick.Controllers.FrequentlyUsed;
using Microsoft.Extensions.Logging;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PointRegistersController : ControllerBase
    {
        private readonly ClickContext _context;
        private readonly ILogger<PointRegistersController> _logger;

        public PointRegistersController(ClickContext _context, ILogger<PointRegistersController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        // GET: api/PointRegisters
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<PointRegister>> GetPointRegisters()
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);
            
            var result = _context.PointRegister.Where(e => e.ReceiverId == mySelf.UserId || e.SenderId == mySelf.UserId);

            if (!result.Any()) 
            {
                return NotFound();
            }

            var resultList = result.ToList();

            return resultList;
        }
    }
}
