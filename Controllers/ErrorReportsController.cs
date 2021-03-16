using ApiClick.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    [Route("api/ErrorReport")]
    public class ErrorReportsController : Controller
    {
        ClickContext _context;

        public ErrorReportsController(ClickContext _context)
        {
            this._context = _context;
        }

        // POST: api/ErrorReport
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult> Post(ErrorReport _errorReport)
        {
            if (!ErrorReport.ModelIsValid(_errorReport)) 
            {
                return BadRequest();
            }

            _context.ErrorReports.Add(_errorReport);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
