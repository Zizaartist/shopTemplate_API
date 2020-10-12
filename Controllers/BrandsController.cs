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
using System.Reflection;

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
                brand.HashTag1 = await _context.HashtagCl.FindAsync(brand.Hashtag1Id);
                brand.HashTag2 = await _context.HashtagCl.FindAsync(brand.Hashtag2Id);
                brand.HashTag3 = await _context.HashtagCl.FindAsync(brand.Hashtag3Id);
                brand.HashTag4 = await _context.HashtagCl.FindAsync(brand.Hashtag4Id);
                brand.HashTag5 = await _context.HashtagCl.FindAsync(brand.Hashtag5Id);
            }

            return brands;
        }

        // POST: api/GetBrandsByFilter/5
        [Route("api/GetMyBrandsByFilter/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<List<BrandCl>>> GetBrandsByFilter(int id, List<int?> HashTags)
        {
            var brands = await _context.BrandCl.Where(p => p.CategoryId == id).ToListAsync();
            foreach (int? hashTagId in HashTags)
            {
                //Оставляет те бренды, в которых имеется текущий хэштег итерации
                brands = brands.Where(p => hashTagId == p.Hashtag1Id ||
                                            hashTagId == p.Hashtag2Id ||
                                            hashTagId == p.Hashtag3Id ||
                                            hashTagId == p.Hashtag4Id ||
                                            hashTagId == p.Hashtag5Id).ToList();
            }


            if (brands == null)
            {
                return NotFound();
            }

            //Если категория "мокрая" - добавить экстра инфы
            if (id == (await _context.CategoryCl.FirstAsync(e => e.CategoryName == "Бутылки")).CategoryId ||
                id == (await _context.CategoryCl.FirstAsync(e => e.CategoryName == "Водовоз")).CategoryId)
            {
                foreach (BrandCl brand in brands)
                {
                    //Если все идет по правильному сценарию, то у бренда будет не более 1 меню и 1 товара 
                    brand.BrandMenus = _context.BrandMenuCl.Where(e => e.BrandId == brand.BrandId).ToList();
                    if (brand.BrandMenus != null)
                    {
                        brand.BrandMenus.First().Image = null;
                        brand.BrandMenus.First().Products = _context.ProductCl.Where(e => e.BrandMenuId == brand.BrandMenus.First().BrandMenuId).ToList();
                        if (brand.BrandMenus.First().Products != null)
                        {
                            brand.BrandMenus.First().Products.First().Image = null;
                        }
                    }
                }
            }

            foreach (BrandCl brand in brands)
            {
                brand.ImgLogo = await _context.ImageCl.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.ImageCl.FindAsync(brand.ImgBannerId);
                brand.HashTag1 = await _context.HashtagCl.FindAsync(brand.Hashtag1Id);
                brand.HashTag2 = await _context.HashtagCl.FindAsync(brand.Hashtag2Id);
                brand.HashTag3 = await _context.HashtagCl.FindAsync(brand.Hashtag3Id);
                brand.HashTag4 = await _context.HashtagCl.FindAsync(brand.Hashtag4Id);
                brand.HashTag5 = await _context.HashtagCl.FindAsync(brand.Hashtag5Id);
            }

            //Чет десерилайзеру похуй на мои действия, он все равно присылает лишние данные
            return brands;
        }

        // GET: api/GetBrandsByCategory/5
        //Получить список брендов категории id
        [Route("api/GetBrandsByCategory/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<BrandCl>>> GetBrandsByCategory(int id)
        {
            var brands = await _context.BrandCl.Where(p => p.CategoryId == id).ToListAsync();

            if (brands == null)
            {
                return NotFound();
            }

            //Если категория "мокрая" - добавить экстра инфы
            if (id == (await _context.CategoryCl.FirstAsync(e => e.CategoryName == "Бутылки")).CategoryId || 
                id == (await _context.CategoryCl.FirstAsync(e => e.CategoryName == "Водовоз")).CategoryId) 
            {
                foreach (BrandCl brand in brands) 
                {
                    //Если все идет по правильному сценарию, то у бренда будет не более 1 меню и 1 товара 
                    brand.BrandMenus = _context.BrandMenuCl.Where(e => e.BrandId == brand.BrandId).ToList();
                    if (brand.BrandMenus != null)
                    {
                        brand.BrandMenus.First().Image = null;
                        brand.BrandMenus.First().Products = _context.ProductCl.Where(e => e.BrandMenuId == brand.BrandMenus.First().BrandMenuId).ToList();
                        if (brand.BrandMenus.First().Products != null)
                        {
                            brand.BrandMenus.First().Products.First().Image = null;
                        }
                    }
                }
            }

            foreach (BrandCl brand in brands) 
            {
                brand.ImgLogo = await _context.ImageCl.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.ImageCl.FindAsync(brand.ImgBannerId);
                brand.HashTag1 = await _context.HashtagCl.FindAsync(brand.Hashtag1Id);
                brand.HashTag2 = await _context.HashtagCl.FindAsync(brand.Hashtag2Id);
                brand.HashTag3 = await _context.HashtagCl.FindAsync(brand.Hashtag3Id);
                brand.HashTag4 = await _context.HashtagCl.FindAsync(brand.Hashtag4Id);
                brand.HashTag5 = await _context.HashtagCl.FindAsync(brand.Hashtag5Id);
            }

            //Чет десерилайзеру похуй на мои действия, он все равно присылает лишние данные
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
                brand.HashTag1 = await _context.HashtagCl.FindAsync(brand.Hashtag1Id);
                brand.HashTag2 = await _context.HashtagCl.FindAsync(brand.Hashtag2Id);
                brand.HashTag3 = await _context.HashtagCl.FindAsync(brand.Hashtag3Id);
                brand.HashTag4 = await _context.HashtagCl.FindAsync(brand.Hashtag4Id);
                brand.HashTag5 = await _context.HashtagCl.FindAsync(brand.Hashtag5Id);
                switch (brand.CategoryId)
                {
                    case 2:
                    case 3:
                        brand.BrandMenus = _context.BrandMenuCl.Where(e => e.BrandId == brand.BrandId).ToList();
                        brand.BrandMenus.First().Products = _context.ProductCl.Where(e => e.BrandMenuId == brand.BrandMenus.First().BrandMenuId).ToList();
                        break;
                    default: break;
                }
            }

            return brands;
        }

        // PUT: api/Brands/5
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public async Task<IActionResult> PutBrandCl(BrandCl brandCl)
        {
            if (brandCl == null)
            {
                return BadRequest();
            }

            if (IsItMyBrand(identityToUser(User.Identity), brandCl))
            {
                insertTagsCorrectly(brandCl);
                var existingBrand = await _context.BrandCl.FindAsync(brandCl.BrandId);

                existingBrand.Address = brandCl.Address;
                existingBrand.BrandName = brandCl.BrandName;
                existingBrand.Contact = brandCl.Contact;
                existingBrand.Description = brandCl.Description;
                existingBrand.DescriptionMax = brandCl.DescriptionMax;
                existingBrand.ImgBannerId = brandCl.ImgBannerId;
                existingBrand.ImgLogoId = brandCl.ImgLogoId;
                existingBrand.Phone = brandCl.Phone;
                existingBrand.WorkTime = brandCl.WorkTime;

                await _context.SaveChangesAsync();
            }
            else 
            {
                return NotFound();
            }

            return Ok();
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

            insertTagsCorrectly(brandCl);

            _context.BrandCl.Add(brandCl);
            await _context.SaveChangesAsync();

            return brandCl;
        }

        // DELETE: api/Brands/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
        public async Task<ActionResult<BrandCl>> DeleteBrandCl(int id)
        {
            var brandCl = await _context.BrandCl.FindAsync(id);

            if (IsItMyBrand(identityToUser(User.Identity), brandCl))
            {
                var menus = _context.BrandMenuCl.Where(e => e.BrandId == brandCl.BrandId);
                var menusNoLazyLoading = menus.ToList();
                foreach (BrandMenuCl menu in menusNoLazyLoading) 
                {
                    var products = _context.ProductCl.Where(e => e.BrandMenuId == menu.BrandMenuId);
                    foreach (ProductCl product in products) 
                    {
                        _context.ProductCl.Remove(product);
                    }
                    _context.BrandMenuCl.Remove(menu);
                }
                _context.BrandCl.Remove(brandCl);

                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }

        /// <summary>
        /// Проверяет является ли бренд собственностью этого пользователя
        /// </summary>
        private bool IsItMyBrand(UserCl user, BrandCl brand)
        {
            var brandBuffer = _context.BrandCl.Find(brand.BrandId);
            if ((brandBuffer == null) || (brandBuffer.UserId != identityToUser(User.Identity).UserId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void insertTagsCorrectly(BrandCl brand)
        {
            //Ставим теги в нужном порядке
            var tagList = new List<int?>();
            var propertyList = new List<System.Reflection.PropertyInfo>();

            propertyList = brand.GetType()
                                    .GetProperties()
                                    .Where(e => e.Name.Contains("Hashtag") && e.Name.Contains("Id"))
                                    .ToList();
            tagList = propertyList.Where(e => e.GetValue(brand) != null)
                                    .Select(e => (int?)e.GetValue(brand))
                                    .Distinct()
                                    .OrderBy(e => e.Value)
                                    .ToList();

            for (int i = 0; i < 5; i++)
            {
                //Если элемент существует - добавить в свойство
                if (i < tagList.Count)
                {
                    propertyList[i].SetValue(brand, tagList[i]);
                }
                else
                {
                    propertyList[i].SetValue(brand, null);
                }
            }
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
