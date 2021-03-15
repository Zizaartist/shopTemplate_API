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

namespace ApiClick.Controllers
{
    [ApiController]
    public class BrandsMenuController : ControllerBase
    {
        ClickContext _context;
        Functions funcs = new Functions();

        public BrandsMenuController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/BrandsMenu
        //Debug
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandMenu>>> GetBrandMenus()
        {
            var menus = await _context.BrandMenus.ToListAsync();

            foreach (BrandMenu menu in menus)
            {
                menu.Image = await _context.Images.FindAsync(menu.ImgId);
            }

            return menus;
        }

        // GET: api/GetMenusByBrand/5
        //Возвращает список меню по id бренда
        [Route("api/GetMenusByBrand/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandMenu>>> GetMenusByBrand(int id)
        {
            var brandMenus = await _context.BrandMenus.Where(p => p.BrandId == id).ToListAsync();

            if (brandMenus == null)
            {
                return NotFound();
            }

            foreach (BrandMenu menu in brandMenus) 
            {
                menu.Image = await _context.Images.FindAsync(menu.ImgId);
            }

            return brandMenus;
        }

        // GET: api/BrandsMenu/5
        //Возвращает меню по id
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<BrandMenu>> GetBrandMenus(int id)
        {
            var brandMenuCl = await _context.BrandMenus.FindAsync(id);

            if (brandMenuCl == null)
            {
                return NotFound();
            }

            brandMenuCl.Image = await _context.Images.FindAsync(brandMenuCl.ImgId);

            return brandMenuCl;
        }

        // PUT: api/BrandsMenu/5
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public async Task<IActionResult> PutBrandMenus(BrandMenu brandMenuCl)
        {
            if (!BrandMenu.ModelIsValid(brandMenuCl))
            {
                return BadRequest();
            }

            if (IsItMyMenu(funcs.identityToUser(User.Identity, _context), brandMenuCl))
            {
                var existingMenu = await _context.BrandMenus.FindAsync(brandMenuCl.BrandMenuId);

                existingMenu.BrandMenuName = brandMenuCl.BrandMenuName;
                existingMenu.ImgId = brandMenuCl.ImgId;

                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }

        // POST: api/BrandsMenu
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public async Task<ActionResult<BrandMenu>> PostBrandMenus(BrandMenu brandMenuCl)
        {
            if (!BrandMenu.ModelIsValid(brandMenuCl) || SameNameMenu(brandMenuCl)) 
            {
                return BadRequest();
            }

            brandMenuCl.CreatedDate = DateTime.UtcNow;

            _context.BrandMenus.Add(brandMenuCl);
            await _context.SaveChangesAsync();

            return brandMenuCl;
        }

        // DELETE: api/BrandsMenu/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
        public async Task<ActionResult<BrandMenu>> DeleteBrandMenus(int id)
        {
            var brandMenuCl = await _context.BrandMenus.FindAsync(id);

            //if menu belongs to user - allow removal
            if (IsItMyMenu(funcs.identityToUser(User.Identity, _context), brandMenuCl))
            {
                var products = _context.Products.Where(e => e.BrandMenuId == brandMenuCl.BrandMenuId);
                foreach (Product product in products)
                {
                    _context.Products.Remove(product);
                }
                _context.BrandMenus.Remove(brandMenuCl);

                await _context.SaveChangesAsync();
            }
            else 
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// Проверяет является ли меню собственностью этого пользователя
        /// </summary>
        private bool IsItMyMenu(User user, BrandMenu menu)
        {
            var menuBuffer = _context.BrandMenus.Find(menu.BrandMenuId);
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
        /// Находит меню с таким же именем среди меню бренда
        /// </summary>
        private bool SameNameMenu(BrandMenu menu) 
        {
            return _context.BrandMenus.Where(m => m.BrandId == menu.BrandId).Any(m => m.BrandMenuName == menu.BrandMenuName);
        }

        private bool BrandMenusExists(int id)
        {
            return _context.BrandMenus.Any(e => e.BrandMenuId == id);
        }
    }
}
