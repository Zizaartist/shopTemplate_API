using ApiClick.Controllers.FrequentlyUsed;
using ApiClick.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorReportsController : ControllerBase
    {
        ClickContext _context;

        public ErrorReportsController(ClickContext _context)
        {
            this._context = _context;
        }

        //Пока без серьезных ограничений, но может быть слабым местом для спама
        // POST: api/ErrorReport
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult> Post(ErrorReport _errorReport)
        {
            if (!ErrorReport.ModelIsValid(_errorReport)) 
            {
                return BadRequest();
            }

            _errorReport.UserId = new Functions().identityToUser(User.Identity, _context).UserId;

            _context.ErrorReports.Add(_errorReport);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
