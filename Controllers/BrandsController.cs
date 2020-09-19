﻿using System;
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
    [ApiController]
    public class BrandsController : ControllerBase
    {
        ClickContext _context = new ClickContext();

        // GET: api/Brands
        //Debug
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandCl>>> GetBrandCl()
        {
            var brands = await _context.BrandCl.ToListAsync();

            foreach (BrandCl brand in brands) 
            {
                brand.ImgLogo = await _context.ImageCl.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.ImageCl.FindAsync(brand.ImgBannerId);
            }

            return brands;
        }

        // GET: api/GetBrandsByCategory/5
        //Получить список брендов категории id
        [Route("api/GetBrandsByCategory/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandCl>>> GetBrandsByCategory(int id)
        {
            var brands = await _context.BrandCl.Where(p => p.CategoryId == id).ToListAsync();

            if (brands == null)
            {
                return NotFound();
            }

            foreach (BrandCl brand in brands) 
            {
                brand.ImgLogo = await _context.ImageCl.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.ImageCl.FindAsync(brand.ImgBannerId);
            }

            return brands;
        }

        // GET: api/Brands/5
        //Получить бренд по id
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<BrandCl>> GetBrandCl(int id)
        {
            var brandCl = await _context.BrandCl.FindAsync(id);

            if (brandCl == null)
            {
                return NotFound();
            }

            brandCl.ImgLogo = await _context.ImageCl.FindAsync(brandCl.ImgLogoId);
            brandCl.ImgBanner = await _context.ImageCl.FindAsync(brandCl.ImgBannerId);

            return brandCl;
        }

        // GET: api/GetMyBrands
        //Получить список своих брендов
        [Authorize(Roles = "SuperAdmin, Admin")]
        [Route("api/GetMyBrands")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandCl>>> GetMyBrands() //хз пока как маршрутизировать
        {
            var brands = await _context.BrandCl.Where(p => p.UserId == identityToUser(User.Identity).UserId).ToListAsync();

            if (brands == null)
            {
                return NotFound();
            }

            foreach (BrandCl brand in brands)
            {
                brand.ImgLogo = await _context.ImageCl.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.ImageCl.FindAsync(brand.ImgBannerId);
            }

            return brands;
        }

        // PUT: api/Brands/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
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

        [Route("api/[controller]")]
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

            return Ok();
        }

        // DELETE: api/Brands/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
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

            return Ok();
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
