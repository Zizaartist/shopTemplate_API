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

namespace ApiClick.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        ClickContext _context;
        Functions funcs = new Functions();

        public ProductsController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/Products
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();

            foreach (Product product in products)
            {
                product.Image = await _context.Images.FindAsync(product.ImgId);
            }

            return products;
        }

        // GET: api/GetProductsByMenu/5
        //Возвращает список продуктов принадлежащих меню с указаным id
        [Route("api/GetProductsByMenu/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByMenu(int id)
        {
            var products = await _context.Products.Where(p => p.BrandMenuId == id).ToListAsync();

            if (products == null)
            {
                return NotFound();
            }

            foreach (Product product in products) 
            {
                product.Image = await _context.Images.FindAsync(product.ImgId);
            }

            return products;
        }

        // GET: api/GetProductsByMenu/5
        //Возвращает список продуктов принадлежащих меню с указаным id
        [Route("api/GetVodaProductsByCategory/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetVodaProductsByCategory(int id)
        {
            var result = new List<Product>();
            switch (id) 
            {
                case 2:
                    result.Add(funcs.getCleanModel(await _context.Products.FindAsync(Constants.PRODUCT_ID_BOTTLED_WATER)));
                    result.Add(funcs.getCleanModel(await _context.Products.FindAsync(Constants.PRODUCT_ID_CONTAINER)));
                    break;
                case 3:
                    result.Add(funcs.getCleanModel(await _context.Products.FindAsync(Constants.PRODUCT_ID_WATER)));
                    break;
                default: return BadRequest();
            }

            foreach (Product product in result)
            {
                product.Image = funcs.getCleanModel(await _context.Images.FindAsync(product.ImgId));
            }

            return result;
        }

        // GET: api/Products/5
        //Возвращает продукт по id
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<Product>> GetProducts(int id)
        {
            var productCl = await _context.Products.FindAsync(id);

            if (productCl == null)
            {
                return NotFound();
            }

            productCl.Image = await _context.Images.FindAsync(productCl.ImgId);

            return productCl;
        }

        // PUT: api/Products/5
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public async Task<IActionResult> PutProducts(Product productCl)
        {
            if (productCl == null)
            {
                return BadRequest();
            }

            if (IsItMyProduct(funcs.identityToUser(User.Identity, _context), productCl))
            {
                var existingProduct = await _context.Products.FindAsync(productCl.ProductId);

                existingProduct.Description = productCl.Description;
                existingProduct.ImgId = productCl.ImgId;
                existingProduct.Price = productCl.Price;
                existingProduct.PriceDiscount = productCl.PriceDiscount;
                existingProduct.ProductName = productCl.ProductName;

                _context.SaveChanges();
            }
            else 
            {
                return NotFound();
            }

            return Ok();
        }

        // POST: api/Products
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProducts(Product productCl)
        {
            if (productCl == null) 
            {
                return BadRequest();
            }

            productCl.CreatedDate = DateTime.Now;

            _context.Products.Add(productCl);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Products/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
        public async Task<ActionResult<Product>> DeleteProducts(int id)
        {
            var productCl = await _context.Products.FindAsync(id);

            if (!IsItMyProduct(funcs.identityToUser(User.Identity, _context), productCl)) 
            {
                return BadRequest();
            }

            _context.Products.Remove(productCl);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private User identityToUser(IIdentity identity)
        {
            return _context.Users.FirstOrDefault(u => u.Phone == identity.Name);
        }

        /// <summary>
        /// Проверяет является ли продукт собственностью этого пользователя
        /// </summary>
        private bool IsItMyProduct(User user, Product product)
        {
            var productBuffer = _context.Products.Find(product.ProductId);
            if (productBuffer == null)
            {
                return false;
            }

            var menuBuffer = _context.BrandMenus.Find(productBuffer.BrandMenuId);
            if (menuBuffer == null) 
            {
                return false;
            }

            var brandBuffer = _context.Brands.Find(menuBuffer.BrandId);
            if ((brandBuffer == null) || (brandBuffer.UserId != funcs.identityToUser(User.Identity, _context).UserId))
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

        /// <summary>
        /// Находит продукт с таким же title
        /// </summary>
        private bool SameNameProduct(Product product)
        {
            return _context.Products.Where(m => m.BrandMenuId == product.BrandMenuId)
                                     .Any(m => m.ProductName == product.ProductName);
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
