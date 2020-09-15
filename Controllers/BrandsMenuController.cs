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
        ClickContext _context = new ClickContext();

        // GET: api/BrandsMenu
        //Debug
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandMenuCl>>> GetBrandMenuCl()
        {
            return await _context.BrandMenuCl.ToListAsync();
        }

        // GET: api/GetMenusByBrand/5
        //Возвращает список меню по id бренда
        [Route("api/GetMenusByBrand/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandMenuCl>>> GetMenusByBrand(int id)
        {
            var brandMenuCl = await _context.BrandMenuCl.Where(p => p.BrandId == id).ToListAsync();

            if (brandMenuCl == null)
            {
                return NotFound();
            }

            return brandMenuCl;
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

            return brandMenuCl;
        }

        // PUT: api/BrandsMenu/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public async Task<IActionResult> PutBrandMenuCl(int id, BrandMenuCl brandMenuCl)
        {
            if (id != brandMenuCl.BrandMenuId || brandMenuCl == null)
            {
                return BadRequest();
            }

            _context.Entry(brandMenuCl).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandMenuClExists(id))
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

            return Ok();
        }

        // DELETE: api/BrandsMenu/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
        public async Task<ActionResult<BrandMenuCl>> DeleteBrandMenuCl(int id)
        {
            var brandMenuCl = await _context.BrandMenuCl.FindAsync(id);

            if (brandMenuCl == null)
            {
                return NotFound();
            }

            //if menu belongs to user - allow removal
            if (!IsItMyMenu(identityToUser(User.Identity), brandMenuCl))
            {
                return BadRequest(); //idk maybe wrong return code but who cares
            }

            _context.BrandMenuCl.Remove(brandMenuCl);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private UserCl identityToUser(IIdentity identity)
        {
            return _context.UserCl.FirstOrDefault(u => u.Phone == identity.Name);
        }

        /// <summary>
        /// Проверяет является ли меню собственностью этого пользователя
        /// </summary>
        private bool IsItMyMenu(UserCl user, BrandMenuCl menu) 
        {
            var brand = _context.BrandCl.FirstOrDefault(b => b.BrandId == menu.BrandId);

            if ((brand == null) || (brand.UserId != user.UserId))
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
            return _context.BrandMenuCl.Where(m => m.BrandId == menu.BrandId).Any(m => m.Description == menu.Description);
        }

        private bool BrandMenuClExists(int id)
        {
            return _context.BrandMenuCl.Any(e => e.BrandMenuId == id);
        }
    }
}
