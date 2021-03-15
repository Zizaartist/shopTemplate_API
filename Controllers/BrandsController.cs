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
        public static int PAGE_SIZE = 5;
        
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

        // POST: api/GetBrandsByFilter/0/3?name=blahbla&openNow=true
        [Route("api/GetBrandsByFilter/{category}/{_page}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<List<Brand>>> GetBrandsByFilter(Category category, int _page, [FromBody]List<int> HashTags = null, bool openNow = false)
        {
            var brands = _context.Brands.Where(p => p.Category == category && p.Available);

            //Урезаем выборку по критерию наличия хештега в списке
            if (HashTags != null)
            {
                foreach (int hashTagId in HashTags)
                {
                    //Оставляет те бренды, в которых имеется текущий хэштег итерации
                    brands = brands.Where(e => e.HashtagsListElements.Any(x => x.HashtagId == hashTagId));
                }
            }

            //Урезаем выборку по критерию доступности бренда
            if (openNow)
            {
                brands = brands.Where(e => IsBrandOpen(_context.ScheduleListElements.Where(x => x.BrandId == e.BrandId).ToList()));
            }

            //Урезаем по критерию страницы
            brands = funcs.GetPageRange(brands, _page, PAGE_SIZE);

            if (!brands.Any())
            {
                return NotFound();
            }

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
                brand.ScheduleListElements = await _context.ScheduleListElements.Where(e => e.BrandId == brand.BrandId).ToListAsync();
            }

            //Чет десерилайзеру похуй на мои действия, он все равно присылает лишние данные
            return resultBrands;
        }


        // GET: api/GetBrandsByName/5?name=blahbla
        [Route("api/GetBrandsByName/{category}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<Brand>>> GetBrandsByName(Category category, string name = null)
        {
            var brands = _context.Brands.Where(p => p.Category == category);

            //Урезаем выборку по критерию наличия строки в имени бренда
            if (name != null)
            {
                //Сводим обе строки к капсу и проверяем содержание одной в другой
                brands = brands.Where(e => e.BrandName.ToUpper().Contains(name.ToUpper()));
            }

            if (!brands.Any())
            {
                return NotFound();
            }

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
                brand.ScheduleListElements = await _context.ScheduleListElements.Where(e => e.BrandId == brand.BrandId).ToListAsync();
            }

            //Чет десерилайзеру похуй на мои действия, он все равно присылает лишние данные
            return resultBrands;
        }


        // GET: api/GetBrandsByCategory/5
        //Получить список брендов категории id
        [Route("api/GetBrandsByCategory/{category}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<Brand>>> GetBrandsByCategory(Category category)
        {
            var brands = _context.Brands.Where(p => p.Category == category);

            if (!brands.Any())
            {
                return NotFound();
            }

            var resultBrands = await brands.ToListAsync();

            //Если категория "мокрая" - добавить экстра инфы
            if (category == Category.bottledWater || 
                category == Category.water) 
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
                brand.ScheduleListElements = await _context.ScheduleListElements.Where(e => e.BrandId == brand.BrandId).ToListAsync();
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

                switch (brand.Category)
                {
                    case Category.bottledWater:
                    case Category.water:
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
            if (!Brand.ModelIsValid(brand)) //дублирующиеся дни
            {
                return BadRequest();
            }
            
            var existingBrand = await _context.Brands.FindAsync(brand.BrandId);
            brand.UserId = existingBrand.UserId;
            
            if (IsItMyBrand(funcs.identityToUser(User.Identity, _context), brand))
            {
                int TAGS_COUNT = 3;

                brand.Hashtags = brand.Hashtags.GroupBy(e => e.HashTagName).Select(e => e.First()) //Удаляем дубликаты
                    .OrderBy(e => e.HashTagId) //Сортируем
                    .Take(TAGS_COUNT) //Оставляем первые TAGS_COUNT
                    .ToList();
                brand.PaymentMethods = brand.PaymentMethods.Distinct() //Удаляем дубликаты
                    .ToList();

                #region hashtagTrash

                //Проверяем, есть ли несуществующие теги
                List<(Hashtag, Hashtag)> newHashtags = new List<(Hashtag, Hashtag)>(); //item1 - пустые теги, item2 - свежесозданные
                foreach (Hashtag _tag in brand.Hashtags)
                {
                    //Если присланный хэштэг не имеет id - пытаемся найти по тексту
                    if (_tag.HashTagId == default)
                    {
                        var tagTextCaps = _tag.HashTagName.ToUpper();
                        var tryingToFindTag = _context.Hashtags.FirstOrDefault(e => e.HashTagName.ToUpper() == tagTextCaps);
                        if (tryingToFindTag != null)
                        {
                            _tag.HashTagId = tryingToFindTag.HashTagId;
                        }
                        else
                        {
                            if (_tag.HashTagName.Length > 2)
                            {
                                //Создаем новый тег
                                var newHashtag = new Hashtag()
                                {
                                    HashTagName = _tag.HashTagName.ToLower(),
                                    Category = brand.Category
                                };
                                newHashtags.Add((_tag, newHashtag));
                                _context.Hashtags.Add(newHashtag);
                            }
                        }
                    }
                }

                if (newHashtags.Any())
                {
                    await _context.SaveChangesAsync(); //Сохраняем новые хэштеги
                    newHashtags.ForEach(e => e.Item1.HashTagId = e.Item2.HashTagId);
                }

                #endregion

                #region hashtags

                //Выводим 2 списка: новых элементов и исчезнувших

                existingBrand.Hashtags = _context.HashtagsListElements.Where(e => e.BrandId == existingBrand.BrandId).Select(e => e.Hashtag).ToList();

                //Находит элементы первого списка, отсутствующие во 2м
                var addHashtags = brand.Hashtags.Where(e => existingBrand.Hashtags.All(x => x.HashTagId != e.HashTagId)); //если в старой коллекции нет хэштегов с таким же id, то он новый
                var subHashtags = existingBrand.Hashtags.Where(e => brand.Hashtags.All(x => x.HashTagId != e.HashTagId)); //если в новой коллекции нет хэштегов с таким же id, то это устаревший

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
                    var sacrifice = _context.HashtagsListElements.FirstOrDefault(e => e.HashtagId == hashtag.HashTagId &&
                                                                                        e.BrandId == existingBrand.BrandId);
                    if (sacrifice != null)
                    {
                        _context.HashtagsListElements.Remove(sacrifice);
                    }
                }

                await _context.SaveChangesAsync();

                //Удаляем неиспользуемые хэштеги
                var tagsToRemove = _context.Hashtags.Where(e => !_context.HashtagsListElements.Any(x => x.HashtagId == e.HashTagId));
                if (tagsToRemove.Any()) 
                {
                    foreach (Hashtag _tag in tagsToRemove)
                    {
                        _context.Hashtags.Remove(_tag);
                    }
                    await _context.SaveChangesAsync();
                }

#endregion

                #region paymentMethods

                //Выводим 2 списка: новых элементов и исчезнувших

                existingBrand.PaymentMethods = _context.PaymentMethodsListElements.Where(e => e.BrandId == existingBrand.BrandId).Select(e => e.PaymentMethod).ToList();

                //Находит элементы первого списка, отсутствующие во 2м
                var addPaymentMethods = brand.PaymentMethods.Where(e => existingBrand.PaymentMethods.All(x => x != e));
                var subPaymentMethods = existingBrand.PaymentMethods.Where(e => brand.PaymentMethods.All(x => x != e));

                //Те, что нужно добавить
                foreach (var paymentMethod in addPaymentMethods)
                {
                    _context.PaymentMethodsListElements.Add(new PaymentMethodsListElement()
                    {
                        PaymentMethod = paymentMethod,
                        BrandId = existingBrand.BrandId
                    });
                }

                //Те, что нужно удалить
                foreach (var paymentMethod in subPaymentMethods)
                {
                    //Находить обязательно по 2м критериям
                    var sacrifice = _context.PaymentMethodsListElements.FirstOrDefault(e =>
                        e.PaymentMethod == paymentMethod &&
                        e.BrandId == existingBrand.BrandId);
                    if (sacrifice != null)
                    {
                        _context.PaymentMethodsListElements.Remove(sacrifice);
                    }
                }

#endregion

                #region schedule

                //Выводим 2 списка: новых элементов и исчезнувших

                existingBrand.ScheduleListElements = _context.ScheduleListElements.Where(e => e.BrandId == existingBrand.BrandId).ToList();

                //Находит элементы первого списка, отсутствующие во 2м
                var addScheduleListElement = brand.ScheduleListElements.Where(e => existingBrand.ScheduleListElements.All(x => x != e));
                var subScheduleListElement = existingBrand.ScheduleListElements.Where(e => brand.ScheduleListElements.All(x => x != e));

                //Те, что нужно добавить
                foreach (var scheduleElement in addScheduleListElement)
                {
                    _context.ScheduleListElements.Add(new ScheduleListElement()
                    {
                        OpenTime = scheduleElement.OpenTime,
                        CloseTime = scheduleElement.CloseTime,
                        DayOfWeek = scheduleElement.DayOfWeek,
                        BrandId = existingBrand.BrandId
                    });
                }

                //Те, что нужно удалить
                foreach (var scheduleElement in subScheduleListElement)
                {
                    //Находить обязательно по 2м критериям
                    var sacrifice = _context.ScheduleListElements.FirstOrDefault(e =>
                        e.DayOfWeek == scheduleElement.DayOfWeek &&
                        e.BrandId == existingBrand.BrandId);
                    if (sacrifice != null)
                    {
                        _context.ScheduleListElements.Remove(sacrifice);
                    }
                }

                #endregion

                await _context.SaveChangesAsync();

                existingBrand.Address = brand.Address;
                existingBrand.BrandName = brand.BrandName;
                existingBrand.Contact = brand.Contact;
                existingBrand.Description = brand.Description;
                existingBrand.ImgBannerId = brand.ImgBannerId;
                existingBrand.ImgLogoId = brand.ImgLogoId;

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
            if (!Brand.ModelIsValid(brand)) 
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
            brand.CreatedDate = DateTime.UtcNow;

            int TAGS_COUNT = 3;

            #region hashtagTrash

            brand.Hashtags = brand.Hashtags.GroupBy(e => e.HashTagName).Select(e => e.First()) //Удаляем дубликаты
                                            .OrderBy(e => e.HashTagId) //Сортируем
                                            .Take(TAGS_COUNT) //Оставляем первые TAGS_COUNT
                                            .ToList();
            //Проверяем, есть ли несуществующие теги
            List<(Hashtag, Hashtag)> newHashtags = new List<(Hashtag, Hashtag)>(); //item1 - пустые теги, item2 - свежесозданные
            foreach (Hashtag _tag in brand.Hashtags) 
            {
                //Если присланный хэштэг не имеет id - пытаемся найти по тексту
                if (_tag.HashTagId == default)
                {
                    var tagTextCaps = _tag.HashTagName.ToUpper();
                    var tryingToFindTag = _context.Hashtags.FirstOrDefault(e => e.HashTagName.ToUpper() == tagTextCaps);
                    if (tryingToFindTag != null)
                    {
                        _tag.HashTagId = tryingToFindTag.HashTagId;
                    }
                    else 
                    {
                        if (_tag.HashTagName.Length > 2)
                        {
                            //Создаем новый тег
                            var newHashtag = new Hashtag()
                            {
                                HashTagName = _tag.HashTagName.ToLower(),
                                Category = brand.Category
                            };
                            newHashtags.Add((_tag, newHashtag));
                            _context.Hashtags.Add(newHashtag);
                        }
                    }
                }
            }

            if (newHashtags.Any()) 
            {
                await _context.SaveChangesAsync(); //Сохраняем новые хэштеги
                newHashtags.ForEach(e => e.Item1.HashTagId = e.Item2.HashTagId);
            }

            #endregion

            brand.PaymentMethods = brand.PaymentMethods.Distinct() //Удаляем дубликаты
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
                    PaymentMethod = paymentMethod,
                    BrandId = brand.BrandId
                });
            }

            await _context.SaveChangesAsync();

            foreach (var scheduleElement in brand.ScheduleListElements)
            {
                _context.ScheduleListElements.Add(new ScheduleListElement()
                {
                    OpenTime = scheduleElement.OpenTime,
                    CloseTime = scheduleElement.CloseTime,
                    DayOfWeek = scheduleElement.DayOfWeek,
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

            switch (brand.Category)
            {
                //Бутилированная вода
                case Category.bottledWater:
                    ListOfProducts.Add(funcs.getCleanModel(_context.Products.Find(Constants.PRODUCT_ID_BOTTLED_WATER)));
                    ListOfProducts.Add(funcs.getCleanModel(_context.Products.Find(Constants.PRODUCT_ID_CONTAINER)));
                    break;
                //Водовоз
                case Category.water:
                    ListOfProducts.Add(funcs.getCleanModel(_context.Products.Find(Constants.PRODUCT_ID_WATER)));
                    break;
                //Error
                default: return;
            }

            brand.BrandMenus = new List<BrandMenu>() { new BrandMenu() };
            brand.BrandMenus.First().Products = ListOfProducts;
        }

        private bool IsBrandOpen(List<ScheduleListElement> _schedule) 
        {
            var currentYakutskTime = new DateTimeOffset(DateTime.UtcNow, Constants.YAKUTSK_OFFSET);
            var match = _schedule.FirstOrDefault(e => e.DayOfWeek == currentYakutskTime.DayOfWeek);
            if (match != null) 
            {
                //Попадаем ли мы во временной промежуток сейчас
                var closeEarlierThanOpen = match.OpenTime > match.CloseTime;
                var laterThanOpen = match.OpenTime <= currentYakutskTime.TimeOfDay;
                var earlierThanClose = match.CloseTime >= currentYakutskTime.TimeOfDay;
                if (
                        (
                            closeEarlierThanOpen
                            || 
                            (laterThanOpen && earlierThanClose)
                        ) 
                    &&
                        (
                            !closeEarlierThanOpen 
                            || 
                            (laterThanOpen || earlierThanClose)
                        )
                    ) 
                {
                    return true;
                }
            }
            return false;
        }
    }
}
