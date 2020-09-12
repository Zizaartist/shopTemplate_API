using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiClick;
using ApiClick.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;

namespace ApiClick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        ClickContext _context = new ClickContext();

        // GET: api/Brands
        //Debug
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandCl>>> GetBrandCl()
        {
            return await _context.BrandCl.ToListAsync();
        }

        // GET: api/Brands/5
        //Получить список брендов категории id
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<BrandCl>>> GetBrandCl(int id)
        {
            var brandCl = await _context.BrandCl.Where(p => p.CategoryId == id).ToListAsync();

            if (brandCl == null)
            {
                return NotFound();
            }

            return brandCl;
        }

        // GET: api/Brands
        //Получить список своих брендов
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandCl>>> GetMyBrands() //хз пока как маршрутизировать
        {
            var brandCl = await _context.BrandCl.Where(p => p.UserId == identityToUser(User.Identity).UserId).ToListAsync();

            if (brandCl == null)
            {
                return NotFound();
            }

            return brandCl;
        }

        // PUT: api/Brands/5
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrandCl(int id, BrandCl brandCl)
        {
            if (id != brandCl.BrandId || brandCl == null)
            {
                return BadRequest();
            }

            _context.Entry(brandCl).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandClExists(id))
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

        // POST: api/Brands
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public async Task<ActionResult<BrandCl>> PostBrandCl(BrandCl brandCl)
        {
            if (brandCl == null) 
            {
                return BadRequest();
            }

            //Не позволять создавать бренды с уже имеющимся именем
            if (_context.BrandCl.Any(a => a.BrandName == brandCl.BrandName)) 
            {
                return Forbid();
            }

            //Заполняем пробелы
            brandCl.User = identityToUser(User.Identity);
            brandCl.UserId = brandCl.User.UserId;
            brandCl.CreatedDate = DateTime.Now;

            _context.BrandCl.Add(brandCl);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBrandCl", new { id = brandCl.BrandId }, brandCl);
        }

        // DELETE: api/Brands/5
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<BrandCl>> DeleteBrandCl(int id)
        {
            var brandCl = await _context.BrandCl.FindAsync(id);

            if (brandCl == null)
            {
                return NotFound();
            }

            if (brandCl.UserId != identityToUser(User.Identity).UserId) 
            {
                return BadRequest();
            }

            _context.BrandCl.Remove(brandCl);
            await _context.SaveChangesAsync();

            return brandCl;
        }

        private UserCl identityToUser(IIdentity identity) 
        {
            return _context.UserCl.FirstOrDefault(u => u.Phone == identity.Name);
        }

        private bool BrandClExists(int id)
        {
            return _context.BrandCl.Any(e => e.BrandId == id);
        }
    }
}
