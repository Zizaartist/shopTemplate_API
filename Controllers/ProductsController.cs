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

namespace ApiClick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        ClickContext _context = new ClickContext();

        // GET: api/Products
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCl>>> GetProductCl()
        {
            return await _context.ProductCl.ToListAsync();
        }

        // GET: api/Products/5
        //Возвращает список продуктов принадлежащих меню с указаным id
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ProductCl>>> GetProductsByMenuCl(int id)
        {
            var productCl = await _context.ProductCl.Where(p => p.BrandMenuId == id).ToListAsync();

            if (productCl == null)
            {
                return NotFound();
            }

            return productCl;
        }

        // GET: api/Products/5
        //Возвращает продукт по id
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCl>> GetProductCl(int id)
        {
            var productCl = _context.ProductCl.FirstOrDefault(p => p.ProductId == id);

            if (productCl == null)
            {
                return NotFound();
            }

            return productCl;
        }

        // PUT: api/Products/5
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductCl(int id, ProductCl productCl)
        {
            if (id != productCl.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(productCl).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductClExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
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

            return CreatedAtAction("GetProductCl", new { id = productCl.ProductId }, productCl);
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductCl>> DeleteProductCl(int id)
        {
            var productCl = await _context.ProductCl.FindAsync(id);

            if (productCl == null)
            {
                return NotFound();
            }

            if (!IsItMyProduct(identityToUser(User.Identity), productCl)) 
            {
                return BadRequest();
            }

            _context.ProductCl.Remove(productCl);
            await _context.SaveChangesAsync();

            return productCl;
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
            var menu = _context.BrandMenuCl.FirstOrDefault(b => b.BrandMenuId == product.BrandMenuId);

            if (menu == null)
            {
                return false;
            }

            var brand = _context.BrandCl.FirstOrDefault(m => m.BrandId == menu.BrandId);

            if ((brand == null) || (brand.UserId != identityToUser(User.Identity).UserId))
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
