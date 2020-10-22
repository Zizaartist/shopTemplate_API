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
        ClickContext _context = new ClickContext();
        Functions funcs = new Functions();

        // GET: api/Products
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCl>>> GetProductCl()
        {
            var products = await _context.ProductCl.ToListAsync();

            foreach (ProductCl product in products)
            {
                product.Image = await _context.ImageCl.FindAsync(product.ImgId);
            }

            return products;
        }

        // GET: api/GetProductsByMenu/5
        //Возвращает список продуктов принадлежащих меню с указаным id
        [Route("api/GetProductsByMenu/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCl>>> GetProductsByMenu(int id)
        {
            var products = await _context.ProductCl.Where(p => p.BrandMenuId == id).ToListAsync();

            if (products == null)
            {
                return NotFound();
            }

            foreach (ProductCl product in products) 
            {
                product.Image = await _context.ImageCl.FindAsync(product.ImgId);
            }

            return products;
        }

        // GET: api/GetProductsByMenu/5
        //Возвращает список продуктов принадлежащих меню с указаным id
        [Route("api/GetVodaProductsByCategory/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCl>>> GetVodaProductsByCategory(int id)
        {
            var result = new List<ProductCl>();
            switch (id) 
            {
                case 2:
                    result.Add(funcs.getCleanModel(await _context.ProductCl.FindAsync(Constants.PRODUCT_ID_BOTTLED_WATER)));
                    result.Add(funcs.getCleanModel(await _context.ProductCl.FindAsync(Constants.PRODUCT_ID_CONTAINER)));
                    break;
                case 3:
                    result.Add(funcs.getCleanModel(await _context.ProductCl.FindAsync(Constants.PRODUCT_ID_WATER)));
                    break;
                default: return BadRequest();
            }

            foreach (ProductCl product in result)
            {
                product.Image = funcs.getCleanModel(await _context.ImageCl.FindAsync(product.ImgId));
            }

            return result;
        }

        // GET: api/Products/5
        //Возвращает продукт по id
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<ProductCl>> GetProductCl(int id)
        {
            var productCl = await _context.ProductCl.FindAsync(id);

            if (productCl == null)
            {
                return NotFound();
            }

            productCl.Image = await _context.ImageCl.FindAsync(productCl.ImgId);

            return productCl;
        }

        // PUT: api/Products/5
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public async Task<IActionResult> PutProductCl(ProductCl productCl)
        {
            if (productCl == null)
            {
                return BadRequest();
            }

            if (IsItMyProduct(funcs.identityToUser(User.Identity, _context), productCl))
            {
                var existingProduct = await _context.ProductCl.FindAsync(productCl.ProductId);

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
        public async Task<ActionResult<ProductCl>> PostProductCl(ProductCl productCl)
        {
            if (productCl == null) 
            {
                return BadRequest();
            }

            productCl.CreatedDate = DateTime.Now;

            _context.ProductCl.Add(productCl);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Products/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
        public async Task<ActionResult<ProductCl>> DeleteProductCl(int id)
        {
            var productCl = await _context.ProductCl.FindAsync(id);

            if (!IsItMyProduct(funcs.identityToUser(User.Identity, _context), productCl)) 
            {
                return BadRequest();
            }

            _context.ProductCl.Remove(productCl);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private UserCl identityToUser(IIdentity identity)
        {
            return _context.UserCl.FirstOrDefault(u => u.Phone == identity.Name);
        }

        /// <summary>
        /// Проверяет является ли продукт собственностью этого пользователя
        /// </summary>
        private bool IsItMyProduct(UserCl user, ProductCl product)
        {
            var productBuffer = _context.ProductCl.Find(product.ProductId);
            if (productBuffer == null)
            {
                return false;
            }

            var menuBuffer = _context.BrandMenuCl.Find(productBuffer.BrandMenuId);
            if (menuBuffer == null) 
            {
                return false;
            }

            var brandBuffer = _context.BrandCl.Find(menuBuffer.BrandId);
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
        private bool SameNameProduct(ProductCl product)
        {
            return _context.ProductCl.Where(m => m.BrandMenuId == product.BrandMenuId)
                                     .Any(m => m.ProductName == product.ProductName);
        }

        private bool ProductClExists(int id)
        {
            return _context.ProductCl.Any(e => e.ProductId == id);
        }
    }
}
