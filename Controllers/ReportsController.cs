﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApiClick;
using ApiClick.Models;
using Microsoft.AspNetCore.Authorization;
using ApiClick.Models.EnumModels;
using ApiClick.Controllers.FrequentlyUsed;
using Microsoft.Extensions.Logging;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ClickContext _context;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ClickContext _context, ILogger<ReportsController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        /// <summary>
        /// Возвращает отчеты за определенный период
        /// </summary>
        /// <param name="datePeriod">Временной период (неделя, месяц, год)</param>
        // GET: api/Reports/30/
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("{datePeriod}")]
        public ActionResult<IEnumerable<Report>> Get(DatePeriod datePeriod)
        {
            var mySelf = Functions.identityToUser(User.Identity, _context).Executor;

            //Вычитаем период для получения дня отчета. Получаем результаты от startingDay по текущий
            var startingDay = DateTime.UtcNow.Date.AddDays((int)datePeriod * -1); 

            var myBrand = _context.Brand.FirstOrDefault(brand => brand.ExecutorId == mySelf.ExecutorId);

            var reports = _context.Report.Where(rep => rep.BrandId == myBrand.BrandId && (rep.CreatedDate >= startingDay));

            if (!reports.Any()) 
            {
                return NotFound();
            }

            var result = reports.ToList();

            return result;
        }

        /// <summary>
        /// Возвращает количество и сумму за все время
        /// </summary>
        /// <returns>Количество, сумма</returns>
        // GET: api/Reports/AllTimeStats
        [Route("AllTimeStats")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public ActionResult<(int, decimal)> GetAllTimeStats()
        {
            var mySelf = Functions.identityToUser(User.Identity, _context).Executor;

            var myBrand = _context.Brand.FirstOrDefault(brand => brand.ExecutorId == mySelf.ExecutorId);

            var reports = _context.Report.Where(report => report.BrandId == myBrand.BrandId);

            if (!reports.Any()) 
            {
                return (0, 0m);
            }

            return (reports.Count(), reports.Sum(report => report.Sum));
        }
    }
}
