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
using ApiClick.Controllers.FrequentlyUsed;
using Microsoft.Extensions.Logging;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ShopContext _context, ILogger<CategoriesController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        /// <summary>
        /// Возвращает список категорий
        /// </summary>
        // GET: api/Categories/
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Category>> Get()
        {
            IQueryable<Category> categories = _context.Category;

            if (!categories.Any()) 
            {
                return NotFound();
            }

            return categories.ToList();
        }
    }
}
