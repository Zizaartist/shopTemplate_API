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
using Click.Models;

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
                brand.Hashtags = await _context.HashtagsListElements.Where(e => e.BrandId == brand.BrandId)
                                                                    .Select(e => e.Hashtag).ToListAsync();
                brand.PaymentMethods = await _context.PaymentMethodsListElements.Where(e => e.BrandId == brand.BrandId)
                                                                                .Select(e => e.PaymentMethod).ToListAsync();
            }

            return brands;
        }

        // POST: api/GetBrandsByFilter/5
        [Route("api/GetMyBrandsByFilter/{categoryId}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<List<Brand>>> GetBrandsByFilter(int categoryId, List<int?> HashTags)
        {
            var brands = _context.Brands.Where(p => p.CategoryId == categoryId);
            //Урезаем выборку по критерию наличия хештега в списке
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
                brand.Hashtags = await _context.HashtagsListElements.Where(e => e.BrandId == brand.BrandId)
                    .Select(e => e.Hashtag).ToListAsync();
                brand.PaymentMethods = await _context.PaymentMethodsListElements.Where(e => e.BrandId == brand.BrandId)
                    .Select(e => e.PaymentMethod).ToListAsync();
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
                brand.Hashtags = await _context.HashtagsListElements.Where(e => e.BrandId == brand.BrandId)
                    .Select(e => e.Hashtag).ToListAsync();
                brand.PaymentMethods = await _context.PaymentMethodsListElements.Where(e => e.BrandId == brand.BrandId)
                    .Select(e => e.PaymentMethod).ToListAsync();
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
            brand.Hashtags = await _context.HashtagsListElements.Where(e => e.BrandId == brand.BrandId)
                .Select(e => e.Hashtag).ToListAsync();
            brand.PaymentMethods = await _context.PaymentMethodsListElements.Where(e => e.BrandId == brand.BrandId)
                .Select(e => e.PaymentMethod).ToListAsync();

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
                brand.Hashtags = await _context.HashtagsListElements.Where(e => e.BrandId == brand.BrandId)
                    .Select(e => e.Hashtag).ToListAsync();
                brand.PaymentMethods = await _context.PaymentMethodsListElements.Where(e => e.BrandId == brand.BrandId)
                    .Select(e => e.PaymentMethod).ToListAsync();

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
        public async Task<IActionResult> PutBrand(Brand brand)
        {
            if (brand == null || !brand.PaymentMethods.Any())
            {
                return BadRequest();
            }
            
            var existingBrand = await _context.Brands.FindAsync(brand.BrandId);
            brand.UserId = existingBrand.UserId;
            
            if (IsItMyBrand(funcs.identityToUser(User.Identity, _context), brand))
            {
                int TAGS_COUNT = 3;

                brand.Hashtags = brand.Hashtags.Distinct() //Удаляем дубликаты
                    .OrderBy(e => e.HashTagId) //Сортируем
                    .Where(e => _context.Hashtags.Find(e.HashTagId) != null) //Проверяем на валидность
                    .Take(TAGS_COUNT) //Оставляем первые TAGS_COUNT
                    .ToList();
                brand.PaymentMethods = brand.PaymentMethods.Distinct() //Удаляем дубликаты
                    .Where(e => _context.PaymentMethods.Find(e.PaymentMethodId) != null) //Проверяем на валидность
                    .ToList();

                //Выводим 2 списка: новых элементов и исчезнувших

                existingBrand.Hashtags = _context.HashtagsListElements.Where(e => e.BrandId == existingBrand.BrandId).Select(e => e.Hashtag).ToList();

                //Находит элементы первого списка, отсутствующие во 2м
                var addHashtags = brand.Hashtags.Where(e => existingBrand.Hashtags.All(x => x.HashTagId != e.HashTagId)); //если в старой коллекции нет хэштегов с таким же id, то он новый
                var subHashtags = existingBrand.Hashtags.Where(e => brand.Hashtags.All(x => x.HashTagId != e.HashTagId)); //если в новой коллекции нет хэштегов с таким же id, то это устаревший

                var addtest = addHashtags.ToList();
                var subtest = subHashtags.ToList();
                
                //Те, что нужно добавить
                foreach (var hashtag in addHashtags)
                {
                    _context.HashtagsListElements.Add(new HashtagsListElement()
                    {
                        HashtagId = hashtag.HashTagId,
                        BrandId = existingBrand.BrandId
                    });
                }

                //Те, что нужно удалить
                foreach (var hashtag in subHashtags)
                {
                    //Находить обязательно по 2м критериям
                    var sacrifice = _context.HashtagsListElements.FirstOrDefault(e =>
                                                                                                e.HashtagId == hashtag.HashTagId &&
                                                                                                e.BrandId == existingBrand.BrandId);
                    if (sacrifice != null)
                    {
                        _context.HashtagsListElements.Remove(sacrifice);
                    }
                }

                await _context.SaveChangesAsync();

                //Выводим 2 списка: новых элементов и исчезнувших

                existingBrand.PaymentMethods = _context.PaymentMethodsListElements.Where(e => e.BrandId == existingBrand.BrandId).Select(e => e.PaymentMethod).ToList();

                //Находит элементы первого списка, отсутствующие во 2м
                var addPaymentMethods = brand.PaymentMethods.Where(e => existingBrand.PaymentMethods.All(x => x.PaymentMethodId != e.PaymentMethodId));
                var subPaymentMethods = existingBrand.PaymentMethods.Where(e => brand.PaymentMethods.All(x => x.PaymentMethodId != e.PaymentMethodId));

                var addasd = addPaymentMethods.ToList();
                var subasd = subPaymentMethods.ToList();
                
                //Те, что нужно добавить
                foreach (var paymentMethod in addPaymentMethods)
                {
                    _context.PaymentMethodsListElements.Add(new PaymentMethodsListElement()
                    {
                        PaymentMethodId = paymentMethod.PaymentMethodId,
                        BrandId = existingBrand.BrandId
                    });
                }

                //Те, что нужно удалить
                foreach (var paymentMethod in subPaymentMethods)
                {
                    //Находить обязательно по 2м критериям
                    var sacrifice = _context.PaymentMethodsListElements.FirstOrDefault(e =>
                        e.PaymentMethodId == paymentMethod.PaymentMethodId &&
                        e.BrandId == existingBrand.BrandId);
                    if (sacrifice != null)
                    {
                        _context.PaymentMethodsListElements.Remove(sacrifice);
                    }
                }
                
                await _context.SaveChangesAsync();

                existingBrand.Address = brand.Address;
                existingBrand.BrandName = brand.BrandName;
                existingBrand.Contact = brand.Contact;
                existingBrand.Description = brand.Description;
                existingBrand.DescriptionMax = brand.DescriptionMax;
                existingBrand.ImgBannerId = brand.ImgBannerId;
                existingBrand.ImgLogoId = brand.ImgLogoId;
                existingBrand.Phone = brand.Phone;
                existingBrand.WorkTime = brand.WorkTime;

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
        public async Task<ActionResult<Brand>> PostBrand(Brand brand)
        {
            if (brand == null || !brand.PaymentMethods.Any()) 
            {
                return BadRequest();
            }

            //Не позволять создавать бренды с уже имеющимся именем
            if (_context.Brands.Any(a => a.BrandName == brand.BrandName)) 
            {
                return Forbid();
            }

            //Заполняем пробелы
            brand.User = funcs.identityToUser(User.Identity, _context);
            brand.UserId = brand.User.UserId;
            brand.CreatedDate = DateTime.Now;

            int TAGS_COUNT = 3;
            
            brand.Hashtags = brand.Hashtags.Distinct() //Удаляем дубликаты
                                            .OrderBy(e => e.HashTagId) //Сортируем
                                            .Where(e => _context.Hashtags.Find(e.HashTagId) != null) //Проверяем на валидность
                                            .Take(TAGS_COUNT) //Оставляем первые TAGS_COUNT
                                            .ToList();
            brand.PaymentMethods = brand.PaymentMethods.Distinct() //Удаляем дубликаты
                                                        .Where(e => _context.PaymentMethods.Find(e.PaymentMethodId) != null) //Проверяем на валидность
                                                        .ToList();
            _context.Brands.Add(brand);

            await _context.SaveChangesAsync();

            foreach (var hashtag in brand.Hashtags)
            {
                _context.HashtagsListElements.Add(new HashtagsListElement()
                {
                    HashtagId = hashtag.HashTagId,
                    BrandId = brand.BrandId
                });
            }
            
            await _context.SaveChangesAsync();

            foreach (var paymentMethod in brand.PaymentMethods)
            {
                _context.PaymentMethodsListElements.Add(new PaymentMethodsListElement()
                {
                    PaymentMethodId = paymentMethod.PaymentMethodId,
                    BrandId = brand.BrandId
                });
            }

            await _context.SaveChangesAsync();

            return brand;
        }

        // DELETE: api/Brands/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
        public async Task<ActionResult<Brand>> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (IsItMyBrand(funcs.identityToUser(User.Identity, _context), brand))
            {
                var menus = _context.BrandMenus.Where(e => e.BrandId == brand.BrandId);
                var menusNoLazyLoading = menus;
                foreach (BrandMenu menu in menusNoLazyLoading) 
                {
                    var products = _context.Products.Where(e => e.BrandMenuId == menu.BrandMenuId);
                    foreach (Product product in products) 
                    {
                        _context.Products.Remove(product);
                    }
                    _context.BrandMenus.Remove(menu);
                }

                var hashtagListElements = _context.HashtagsListElements.Where(e => e.BrandId == brand.BrandId);
                foreach (var hashtagElement in hashtagListElements)
                {
                    _context.HashtagsListElements.Remove(hashtagElement);
                }
                
                var paymentMethodListElements = _context.PaymentMethodsListElements.Where(e => e.BrandId == brand.BrandId);
                foreach (var paymentMethodElement in paymentMethodListElements)
                {
                    _context.PaymentMethodsListElements.Remove(paymentMethodElement);
                }

                _context.Brands.Remove(brand);

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
