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
using ApiClick.StaticValues;
using ApiClick.Controllers.FrequentlyUsed;
using Microsoft.Extensions.Logging;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly ILogger<ProductsController> _logger;
        public static int PAGE_SIZE = 5;

        public ProductsController(ShopContext _context, ILogger<ProductsController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        /// <summary>
        /// Возвращает продукцию из указанной категории
        /// </summary>
        /// <param name="id">Id категории</param>
        /// <param name="_page">Страница</param>
        /// <returns></returns>
        // GET: api/Products/ByMenu/5/1
        [Route("ByMenu/{id}/{_page}")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProductsByMenu(int id, int _page)
        {
            var products = _context.Product.Where(p => p.CategoryId == id); 
            
            products = Functions.GetPageRange(products, _page, PAGE_SIZE);

            if (!products.Any())
            {
                return NotFound();
            }

            var result = products.ToList();

            return result;
        }
    }
}
