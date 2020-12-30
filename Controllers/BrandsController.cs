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
using ApiClick.Models.ArrayModels;
using ApiClick.StaticValues;

namespace ApiClick.Controllers
{
    [ApiController]
    public class BrandsController : ControllerBase
    {
        ClickContext _context;
        Functions funcs = new Functions();
        
        public BrandsController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/Brands
        //Debug
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrand()
        {
            var brands = await _context.Brands.ToListAsync();

            foreach (Brand brand in brands) 
            {
                brand.ImgLogo = await _context.Images.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.Images.FindAsync(brand.ImgBannerId);
                brand.Hashtags = _context.Brands.Find(brand.BrandId).HashtagsListElements.Select(e => e.Hashtag).ToList();
            }

            return brands;
        }

        // POST: api/GetBrandsByFilter/5
        [Route("api/GetMyBrandsByFilter/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<List<Brand>>> GetBrandsByFilter(int id, List<int?> HashTags)
        {
            var brands = _context.Brands.Where(p => p.CategoryId == id);
            foreach (int? hashTagId in HashTags)
            {
                //Оставляет те бренды, в которых имеется текущий хэштег итерации
                brands = brands.Where(e => e.HashtagsListElements.Any(x => x.HashtagId == hashTagId));
            }

            if (!brands.Any())
            {
                return NotFound();
            }

            //У мокрых брендов не должно быть хэштегов
            //Если категория "мокрая" - добавить экстра инфы
            //if (id == (await _context.Categories.FirstAsync(e => e.CategoryName == "Бутылки")).CategoryId ||
            //    id == (await _context.Categories.FirstAsync(e => e.CategoryName == "Водовоз")).CategoryId)
            //{
            //    foreach (Brand brand in brands)
            //    {
            //        attachDefaultMenuToVodaBrand(brand);
            //    }
            //}

            var resultBrands = brands.ToList();
            
            foreach (Brand brand in resultBrands)
            {
                var remoteBrand = await _context.Brands.FindAsync(brand.BrandId);
                brand.ImgLogo = await _context.Images.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.Images.FindAsync(brand.ImgBannerId);
                brand.Hashtags = remoteBrand.HashtagsListElements.Select(e => e.Hashtag).ToList();
                brand.PaymentMethods = remoteBrand.PaymentMethodsListElements.Select(e => e.PaymentMethod).ToList();
            }

            //Чет десерилайзеру похуй на мои действия, он все равно присылает лишние данные
            return resultBrands;
        }

        // GET: api/GetBrandsByCategory/5
        //Получить список брендов категории id
        [Route("api/GetBrandsByCategory/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<Brand>>> GetBrandsByCategory(int id)
        {
            var brands = _context.Brands.Where(p => p.CategoryId == id);

            if (!brands.Any())
            {
                return NotFound();
            }

            var resultBrands = await brands.ToListAsync();

            //Если категория "мокрая" - добавить экстра инфы
            if (id == (await _context.Categories.FirstAsync(e => e.CategoryName == "Бутылки")).CategoryId || 
                id == (await _context.Categories.FirstAsync(e => e.CategoryName == "Водовоз")).CategoryId) 
            {
                foreach (Brand brand in brands) 
                {
                    attachDefaultMenuToVodaBrand(brand);
                }
            }

            foreach (Brand brand in resultBrands)
            {
                var remoteBrand = await _context.Brands.FindAsync(brand.BrandId);
                brand.ImgLogo = await _context.Images.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.Images.FindAsync(brand.ImgBannerId);
                brand.Hashtags = remoteBrand.HashtagsListElements.Select(e => e.Hashtag).ToList();
                brand.PaymentMethods = remoteBrand.PaymentMethodsListElements.Select(e => e.PaymentMethod).ToList();
            }

            //Чет десерилайзеру похуй на мои действия, он все равно присылает лишние данные
            return resultBrands;
        }

        // GET: api/Brands/5
        //Получить бренд по id
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            brand.ImgLogo = await _context.Images.FindAsync(brand.ImgLogoId);
            brand.ImgBanner = await _context.Images.FindAsync(brand.ImgBannerId);
            brand.Hashtags = brand.HashtagsListElements.Select(e => e.Hashtag).ToList();
            brand.PaymentMethods = brand.PaymentMethodsListElements.Select(e => e.PaymentMethod).ToList();

            return brand;
        }

        // GET: api/GetMyBrands
        //Получить список своих брендов
        [Authorize(Roles = "SuperAdmin, Admin")]
        [Route("api/GetMyBrands")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetMyBrands() //хз пока как маршрутизировать
        {
            var brands = _context.Brands.Where(p => p.UserId == funcs.identityToUser(User.Identity, _context).UserId);

            if (brands == null)
            {
                return NotFound();
            }

            var resultBrands = await brands.ToListAsync();

            foreach (Brand brand in resultBrands)
            {
                var remoteBrand = await _context.Brands.FindAsync(brand.BrandId);
                brand.ImgLogo = await _context.Images.FindAsync(brand.ImgLogoId);
                brand.ImgBanner = await _context.Images.FindAsync(brand.ImgBannerId);
                brand.Hashtags = remoteBrand.HashtagsListElements.Select(e => e.Hashtag).ToList();
                brand.PaymentMethods = remoteBrand.PaymentMethodsListElements.Select(e => e.PaymentMethod).ToList();

                switch (brand.CategoryId)
                {
                    case 2:
                    case 3:
                        attachDefaultMenuToVodaBrand(brand);
                        break;
                    default: break;
                }
            }

            return resultBrands;
        }

        // PUT: api/Brands/5
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public async Task<IActionResult> PutBrand(Brand brandCl)
        {
            if (brandCl == null)
            {
                return BadRequest();
            }

            if (IsItMyBrand(funcs.identityToUser(User.Identity, _context), brandCl))
            {
                insertTagsCorrectly(brandCl);
                var existingBrand = await _context.Brands.FindAsync(brandCl.BrandId);

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
        public async Task<ActionResult<Brand>> PostBrand(Brand brandCl)
        {
            if (brandCl == null) 
            {
                return BadRequest();
            }

            //Не позволять создавать бренды с уже имеющимся именем
            if (_context.Brands.Any(a => a.BrandName == brandCl.BrandName)) 
            {
                return Forbid();
            }

            //Заполняем пробелы
            brandCl.User = funcs.identityToUser(User.Identity, _context);
            brandCl.UserId = brandCl.User.UserId;
            brandCl.CreatedDate = DateTime.Now;

            insertTagsCorrectly(brandCl);
            _context.Brands.Add(brandCl);

            await _context.SaveChangesAsync();

            return brandCl;
        }

        // DELETE: api/Brands/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
        public async Task<ActionResult<Brand>> DeleteBrand(int id)
        {
            var brandCl = await _context.Brands.FindAsync(id);

            if (IsItMyBrand(funcs.identityToUser(User.Identity, _context), brandCl))
            {
                var menus = _context.BrandMenus.Where(e => e.BrandId == brandCl.BrandId);
                var menusNoLazyLoading = menus.ToList();
                foreach (BrandMenu menu in menusNoLazyLoading) 
                {
                    var products = _context.Products.Where(e => e.BrandMenuId == menu.BrandMenuId);
                    foreach (Product product in products) 
                    {
                        _context.Products.Remove(product);
                    }
                    _context.BrandMenus.Remove(menu);
                }
                _context.Brands.Remove(brandCl);

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
        private bool IsItMyBrand(User user, Brand brand)
        {
            var brandBuffer = _context.Brands.Find(brand.BrandId);
            if ((brandBuffer == null) || (brandBuffer.UserId != funcs.identityToUser(User.Identity, _context).UserId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void insertTagsCorrectly(Brand brand)
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
        private void attachDefaultMenuToVodaBrand(Brand brand)
        {
            var ListOfProducts = new List<Product>();

            switch (brand.CategoryId)
            {
                //Бутилированная вода
                case 2:
                    ListOfProducts.Add(funcs.getCleanModel(_context.Products.Find(Constants.PRODUCT_ID_BOTTLED_WATER)));
                    ListOfProducts.Add(funcs.getCleanModel(_context.Products.Find(Constants.PRODUCT_ID_CONTAINER)));
                    break;
                //Водовоз
                case 3:
                    ListOfProducts.Add(funcs.getCleanModel(_context.Products.Find(Constants.PRODUCT_ID_WATER)));
                    break;
                //Error
                default: return;
            }

            brand.BrandMenus = new List<BrandMenu>() { new BrandMenu() };
            brand.BrandMenus.First().Products = ListOfProducts;
        }
    }
}
