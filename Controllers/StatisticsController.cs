using ApiClick.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

            var commandText = $"INSERT INTO SessionRecord(CreatedDate)VALUES(@DataTime)";
            var name = new SqlParameter("@DataTime", DateTime.UtcNow.Date);
            _context.Database.ExecuteSqlRaw(commandText, name);

            return Ok();
        }
    }
}
