using System;
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

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private Functions funcs;
        private readonly ClickContext _context;

        public ReportsController(ClickContext context)
        {
            funcs = new Functions();
            _context = context;
        }

        // GET: api/ReportsController/30/
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet("{datePeriod}")]
        public async Task<ActionResult<List<Report>>> Get(DatePeriod datePeriod)
        {
            var user = funcs.identityToUser(User.Identity, _context);

            var brand = _context.Brands.First(e => e.UserId == user.UserId);

            var reports = _context.Reports.Where(e => e.BrandId == brand.BrandId).Take((int)datePeriod);

            if (!reports.Any()) 
            {
                return NotFound();
            }

            var result = await reports.ToListAsync();

            foreach (var report in result) 
            {
                if (report.ProductOfDayId != null) 
                {
                    report.ProductOfDay = funcs.getCleanModel(_context.Products.Find(report.ProductOfDayId));
                    if (report.ProductOfDay.ImgId != null) 
                    {
                        report.ProductOfDay.Image = funcs.getCleanModel(_context.Images.Find(report.ProductOfDay.ImgId));
                    }
                }
            }

            return result;
        }


        // GET: api/ReportsController/AllTimeStats
        [Route("AllTimeStats")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<(int, decimal)>> GetAllTimeStats()
        {
            var user = funcs.identityToUser(User.Identity, _context);

            var brand = _context.Brands.First(e => e.UserId == user.UserId);

            var reports = _context.Reports.Where(e => e.BrandId == brand.BrandId);

            if (!reports.Any()) 
            {
                return (0, 0m);
            }

            return (reports.Count(), reports.Sum(e => e.Sum));
        }
    }
}
