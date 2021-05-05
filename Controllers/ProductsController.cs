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
        // GET: api/Products/ByCategory/5/1
        [Route("ByCategory/{id}/{_page}")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProductsByCategory(int id, int _page)
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

        /// <summary>
        /// Возвращает продукцию по критерию наличия входной строки в имени продукта 
        /// </summary>
        // GET: api/Products/Search/blabla
        [Route("Search/{_nameCriteria}")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProductsByName(string _nameCriteria)
        {
            var capsNameCriteria = _nameCriteria.ToUpper();

            var products = _context.Product.Where(product => product.ProductName.ToUpper().Contains(capsNameCriteria));

            if (!products.Any())
            {
                return NotFound();
            }

            var result = products.ToList();

            return result;
        }
    }
}
