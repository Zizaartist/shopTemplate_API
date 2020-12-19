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
        public async Task<ActionResult<IEnumerable<BrandMenuCl>>> GetBrandMenuCl()
        {
            var menus = await _context.BrandMenuCl.ToListAsync();

            foreach (BrandMenuCl menu in menus)
            {
                menu.Image = await _context.ImageCl.FindAsync(menu.ImgId);
            }

            return menus;
        }

        // GET: api/GetMenusByBrand/5
        //Возвращает список меню по id бренда
        [Route("api/GetMenusByBrand/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandMenuCl>>> GetMenusByBrand(int id)
        {
            var brandMenus = await _context.BrandMenuCl.Where(p => p.BrandId == id).ToListAsync();

            if (brandMenus == null)
            {
                return NotFound();
            }

            foreach (BrandMenuCl menu in brandMenus) 
            {
                menu.Image = await _context.ImageCl.FindAsync(menu.ImgId);
            }

            return brandMenus;
        }

        // GET: api/BrandsMenu/5
        //Возвращает меню по id
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<BrandMenuCl>> GetBrandMenuCl(int id)
        {
            var brandMenuCl = await _context.BrandMenuCl.FindAsync(id);

            if (brandMenuCl == null)
            {
                return NotFound();
            }

            brandMenuCl.Image = await _context.ImageCl.FindAsync(brandMenuCl.ImgId);

            return brandMenuCl;
        }

        // PUT: api/BrandsMenu/5
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public async Task<IActionResult> PutBrandMenuCl(BrandMenuCl brandMenuCl)
        {
            if (brandMenuCl == null)
            {
                return BadRequest();
            }

            if (IsItMyMenu(funcs.identityToUser(User.Identity, _context), brandMenuCl))
            {
                var existingMenu = await _context.BrandMenuCl.FindAsync(brandMenuCl.BrandMenuId);

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
        public async Task<ActionResult<BrandMenuCl>> PostBrandMenuCl(BrandMenuCl brandMenuCl)
        {
            if (brandMenuCl == null || SameNameMenu(brandMenuCl)) 
            {
                return BadRequest();
            }

            brandMenuCl.CreatedDate = DateTime.Now;

            _context.BrandMenuCl.Add(brandMenuCl);
            await _context.SaveChangesAsync();

            return brandMenuCl;
        }

        // DELETE: api/BrandsMenu/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
        public async Task<ActionResult<BrandMenuCl>> DeleteBrandMenuCl(int id)
        {
            var brandMenuCl = await _context.BrandMenuCl.FindAsync(id);

            //if menu belongs to user - allow removal
            if (IsItMyMenu(funcs.identityToUser(User.Identity, _context), brandMenuCl))
            {
                var products = _context.ProductCl.Where(e => e.BrandMenuId == brandMenuCl.BrandMenuId);
                foreach (ProductCl product in products)
                {
                    _context.ProductCl.Remove(product);
                }
                _context.BrandMenuCl.Remove(brandMenuCl);

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
        private bool IsItMyMenu(UserCl user, BrandMenuCl menu)
        {
            var menuBuffer = _context.BrandMenuCl.Find(menu.BrandMenuId);
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
        /// Находит меню с таким же именем среди меню бренда
        /// </summary>
        private bool SameNameMenu(BrandMenuCl menu) 
        {
            return _context.BrandMenuCl.Where(m => m.BrandId == menu.BrandId).Any(m => m.BrandMenuName == menu.BrandMenuName);
        }

        private bool BrandMenuClExists(int id)
        {
            return _context.BrandMenuCl.Any(e => e.BrandMenuId == id);
        }
    }
}
