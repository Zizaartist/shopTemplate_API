using ApiClick.Controllers.FrequentlyUsed;
using ApiClick.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ClickContext _context;
        private readonly ILogger<ErrorReportsController> _logger;

        public ErrorReportsController(ClickContext _context, ILogger<ErrorReportsController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        //Пока без серьезных ограничений, но может быть слабым местом для спама
        // POST: api/ErrorReport
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult> Post(ErrorReport _errorReport)
        {
            if (!IsPostModelValid(_errorReport)) 
            {
                return BadRequest();
            }

            _errorReport.UserId = Functions.identityToUser(User.Identity, _context).UserId;

            _context.ErrorReports.Add(_errorReport);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Валидация получаемых данных
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsPostModelValid(ErrorReport _errorReport)
        {
            try
            {
                if (_errorReport == null ||
                    string.IsNullOrEmpty(_errorReport.Text))
                {
                    return false;
                }

                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации ErrorReport модели POST метода - {_ex}");
                return false;
            }
        }
    }
}
