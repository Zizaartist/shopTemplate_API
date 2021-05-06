using ApiClick.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : Controller
    {
        private readonly ShopContext _context;

        public StatisticsController(ShopContext _context)
        {
            this._context = _context;
        }

        /// <summary>
        /// Создает запись с датой открытия приложения
        /// </summary>
        // GET: api/Statistics/AppOpened/
        [Route("AppOpened")]
        [HttpGet]
        public ActionResult AppOpened() 
        {
            _context.SessionRecord.Add(new SessionRecord { CreatedDate = DateTime.UtcNow.Date });
            _context.SaveChanges();

            return Ok();
        }
    }
}
