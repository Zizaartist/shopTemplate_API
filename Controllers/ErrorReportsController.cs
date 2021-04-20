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
        private readonly ShopContext _context;
        private readonly ILogger<ErrorReportsController> _logger;

        public ErrorReportsController(ShopContext _context, ILogger<ErrorReportsController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        /// <summary>
        /// Добавляет отчет об ошибке
        /// </summary>
        /// <param name="_errorReport">Данные отчета</param>
        // POST: api/ErrorReport
        [Authorize]
        [HttpPost]
        public ActionResult Post(ErrorReport _errorReport)
        {
            if (!IsPostModelValid(_errorReport)) 
            {
                return BadRequest();
            }

            _errorReport.UserId = Functions.identityToUser(User.Identity, _context).UserId;

            _context.ErrorReport.Add(_errorReport);
            _context.SaveChanges();

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
