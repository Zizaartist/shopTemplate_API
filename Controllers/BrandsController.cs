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
using System.Reflection;
using ApiClick.StaticValues;

namespace ApiClick.Controllers
{
    [ApiController]
    public class BrandsController : ControllerBase
    {
        ClickContext _context = new ClickContext();
        Functions funcs = new Functions();

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
                                            hashTagId == p.Hashtag3Id).ToList();
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
                    attachDefaultMenuToVodaBrand(brand);
                }
            }

            foreach (BrandCl brand in brands)
            {
                brand.ImgLogo = await _context.ImageCl.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.ImageCl.FindAsync(brand.ImgBannerId);
                brand.HashTag1 = await _context.HashtagCl.FindAsync(brand.Hashtag1Id);
                brand.HashTag2 = await _context.HashtagCl.FindAsync(brand.Hashtag2Id);
                brand.HashTag3 = await _context.HashtagCl.FindAsync(brand.Hashtag3Id);
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
                    attachDefaultMenuToVodaBrand(brand);
                }
            }

            foreach (BrandCl brand in brands) 
            {
                brand.ImgLogo = await _context.ImageCl.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.ImageCl.FindAsync(brand.ImgBannerId);
                brand.HashTag1 = await _context.HashtagCl.FindAsync(brand.Hashtag1Id);
                brand.HashTag2 = await _context.HashtagCl.FindAsync(brand.Hashtag2Id);
                brand.HashTag3 = await _context.HashtagCl.FindAsync(brand.Hashtag3Id);
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
            var brands = await _context.BrandCl.Where(p => p.UserId == funcs.identityToUser(User.Identity, _context).UserId).ToListAsync();

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
                switch (brand.CategoryId)
                {
                    case 2:
                    case 3:
                        attachDefaultMenuToVodaBrand(brand);
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

            if (IsItMyBrand(funcs.identityToUser(User.Identity, _context), brandCl))
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
            brandCl.User = funcs.identityToUser(User.Identity, _context);
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

            if (IsItMyBrand(funcs.identityToUser(User.Identity, _context), brandCl))
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
            if ((brandBuffer == null) || (brandBuffer.UserId != funcs.identityToUser(User.Identity, _context).UserId))
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
            int TAGS_COUNT = 3;

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

            for (int i = 0; i < TAGS_COUNT; i++)
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

        /// <summary>
        /// Присоединяет к отправляемому бренду единственное меню с соответствующими категории товарами
        /// </summary>
        /// <param name="brand">Бренд, к которому будут привязаны данные</param>
        private void attachDefaultMenuToVodaBrand(BrandCl brand)
        {
            var ListOfProducts = new List<ProductCl>();

            switch (brand.CategoryId)
            {
                //Бутилированная вода
                case 2:
                    ListOfProducts.Add(funcs.getCleanModel(_context.ProductCl.Find(Constants.PRODUCT_ID_BOTTLED_WATER)));
                    ListOfProducts.Add(funcs.getCleanModel(_context.ProductCl.Find(Constants.PRODUCT_ID_CONTAINER)));
                    break;
                //Водовоз
                case 3:
                    ListOfProducts.Add(funcs.getCleanModel(_context.ProductCl.Find(Constants.PRODUCT_ID_WATER)));
                    break;
                //Error
                default: return;
            }

            brand.BrandMenus = new List<BrandMenuCl>() { new BrandMenuCl() };
            brand.BrandMenus.First().Products = ListOfProducts;
        }
    }
}
