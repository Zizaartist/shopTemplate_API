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
using ApiClick.StaticValues;
using ApiClick.Controllers.FrequentlyUsed;
using Microsoft.Extensions.Logging;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly ClickContext _context;
        private readonly ILogger<BrandsController> _logger;
        public static int PAGE_SIZE = 5;

        public BrandsController(ClickContext _context, ILogger<BrandsController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        /// <summary>
        /// Возвращает бренды соответствующие указанным критериям
        /// </summary>
        /// <param name="_kind">Вид товара</param>
        /// <param name="_page">Страница</param>
        /// <param name="HashTags">Хэштеги, которые должны быть в наличии у бренда</param>
        /// <param name="openNow">Фильтровать ли бренды по доступности на текущий момент</param>
        /// <returns>Бренды соответствующие критериям</returns>
        // POST: api/Brands/GetByFilter/0/3?openNow=true
        [Route("GetByFilter/{_kind}/{_page}")]
        [Authorize]
        [HttpPost]
        public ActionResult<IEnumerable<Brand>> GetBrandsByFilter(Kind _kind, int _page, [FromBody]List<int> HashTags = null, bool openNow = false)
        {
            var brands = _context.Brand.Include(brand => brand.BrandInfo)
                                        .Include(brand => brand.BrandHashtags)
                                            .ThenInclude(bh => bh.Hashtag)
                                        .Include(brand => brand.BrandPaymentMethods)
                                        .Where(brand => brand.Kind == _kind);

            //Урезаем выборку по критерию наличия хештега в списке
            if (HashTags != null)
            {
                foreach (int hashTagId in HashTags)
                {
                    //Оставляет те бренды, в которых имеется текущий хэштег итерации
                    brands = brands.Where(e => e.BrandHashtags.Any(x => x.HashtagId == hashTagId));
                }
            }

            //Урезаем выборку по критерию доступности бренда
            if (openNow)
            {
                brands = brands.Where(e => e.Available);
            }

            //Урезаем по критерию страницы
            brands = Functions.GetPageRange(brands, _page, PAGE_SIZE);

            if (!brands.Any())
            {
                return NotFound();
            }

            var result = brands.ToList();

            return result;
        }

        /// <summary>
        /// Возвращает бренды которые содержат указанную строку
        /// </summary>
        /// <param name="category">Вид товаров</param>
        /// <param name="name">Критерий поиска</param>
        /// <returns>Бренды содержащие указанную строку</returns>
        // GET: api/Brands/GetByName/5?name=blahbla
        [Route("GetByName/{_kind}")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Brand>> GetBrandsByName(Kind _kind, string name = null)
        {
            var brands = _context.Brand.Include(brand => brand.BrandInfo)
                                        .Include(brand => brand.BrandHashtags)
                                            .ThenInclude(bh => bh.Hashtag)
                                        .Include(brand => brand.BrandPaymentMethods)
                                        .Where(p => p.Kind == _kind);

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

            var result = brands.ToList();

            return result;
        }

        /// <summary>
        /// Возвращает водные бренды указанного типа
        /// </summary>
        /// <param name="category">Тип продукции (2 варианта)</param>
        /// <returns>Бренды указанного типа</returns>
        // GET: api/Brands/GetWaterBrands/5
        [Route("GetWaterBrands/{_kind}")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Brand>> GetWaterBrands(Kind _kind)
        {
            if (_kind == Kind.food || _kind == Kind.flowers) 
            {
                return BadRequest();
            }

            var brands = _context.Brand.Include(brand => brand.BrandInfo)
                                        .Include(brand => brand.BrandPaymentMethods)
                                        .Include(brand => brand.ScheduleListElements)
                                        .Include(brand => brand.WaterBrand)
                                        .Where(p => p.Kind == _kind);

            if (!brands.Any())
            {
                return NotFound();
            }

            var result = brands.ToList();

            var dayOfWeek = DateTime.UtcNow.Add(Constants.YAKUTSK_OFFSET).DayOfWeek;
            foreach (var brand in result) 
            {
                brand.ScheduleListElements = new List<ScheduleListElement>() { brand.ScheduleListElements.First(sle => sle.DayOfWeek == dayOfWeek) };
            }

            return result;
        }

        /// <summary>
        /// Получить данные определенного бренда
        /// </summary>
        /// <param name="id">Id бренда</param>
        /// <returns>Публичные данные бренда</returns>
        // GET: api/Brands/5
        [Route("{id}")]
        [Authorize]
        [HttpGet]
        public ActionResult<Brand> GetBrandData(int id)
        {
            var brand = _context.Brand.Include(brand => brand.BrandInfo)
                                        .Include(brand => brand.ScheduleListElements)
                                        .FirstOrDefault(brand => brand.BrandId == id);

            if (brand == null)
            {
                return NotFound();
            }

            switch (brand.Kind)
            {
                case Kind.food:
                case Kind.flowers:
                    _context.Entry(brand).Collection(brand => brand.BrandPaymentMethods).Load();
                    break;
                case Kind.bottledWater:
                case Kind.water:
                    _context.Entry(brand).Reference(brand => brand.WaterBrand).Load();
                    break;
            }

            return brand;
        }

        /// <summary>
        /// Возвращает подробную информацию о бренде владельцу
        /// </summary>
        /// <returns></returns>
        // GET: api/Brands/GetMyBrand
        [Authorize(Roles = "SuperAdmin, Admin")]
        [Route("GetMyBrand")]
        [HttpGet]
        public ActionResult<Brand> GetMyBrand()
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);

            var brand = _context.Brand.Include(brand => brand.BrandInfo)
                                        .Include(brand => brand.WaterBrand)
                                        .Include(brand => brand.BrandHashtags)
                                        .Include(brand => brand.BrandPaymentMethods)
                                        .Include(brand => brand.ScheduleListElements)
                                        .Include(brand => brand.AdBanners)
                                        .FirstOrDefault(brand => brand.ExecutorId == mySelf.Executor.ExecutorId);

            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        /// <summary>
        /// Изменяет документацию бренда
        /// </summary>
        // PUT: api/Brands/BrandDoc
        [Route("BrandDoc")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public ActionResult PutBrandDocumentation(BrandDoc _brandDocData)
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);

            var myBrandDoc = _context.BrandDoc.FirstOrDefault(doc => doc.Brand.ExecutorId == mySelf.Executor.ExecutorId);

            if (myBrandDoc == null)
            {
                return NotFound();
            }

            myBrandDoc.OfficialName = _brandDocData.OfficialName;
            myBrandDoc.Ogrn = _brandDocData.Ogrn;
            myBrandDoc.Inn = _brandDocData.Inn;
            myBrandDoc.LegalAddress = _brandDocData.LegalAddress;
            myBrandDoc.Executor = _brandDocData.Executor;

            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Возвращает документацию бренда
        /// </summary>
        // GET: api/Brands/BrandDoc
        [Route("BrandDoc")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public ActionResult<BrandDoc> GetBrandDocumentation()
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);

            var myBrandDoc = _context.BrandDoc.FirstOrDefault(doc => doc.Brand.ExecutorId == mySelf.Executor.ExecutorId);

            if (myBrandDoc == null)
            {
                return NotFound();
            }

            return myBrandDoc;
        }

        /// <summary>
        /// Изменяет данные бренда
        /// </summary>
        /// <param name="_brandData">Новые данные бренда</param>
        // PUT: api/Brands
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public IActionResult PutBrand(Brand _brandData)
        {
            if (!IsPutModelValid(_brandData))
            {
                return BadRequest();
            }

            var mySelf = Functions.identityToUser(User.Identity, _context);

            var brand = _context.Brand.Include(brand => brand.BrandInfo)
                                        .Include(brand => brand.WaterBrand)
                                        .Include(brand => brand.BrandHashtags)
                                        .Include(brand => brand.BrandPaymentMethods)
                                        .Include(brand => brand.ScheduleListElements)
                                        .FirstOrDefault(brand => brand.ExecutorId == mySelf.Executor.ExecutorId);

            if (brand == null)
            {
                return NotFound();
            }

            //Не будем ебаться с проверками - тупо заменяем

            _context.BrandHashtag.RemoveRange(brand.BrandHashtags);
            brand.BrandHashtags = _brandData.BrandHashtags;

            _context.BrandPaymentMethod.RemoveRange(brand.BrandPaymentMethods);
            brand.BrandPaymentMethods = _brandData.BrandPaymentMethods;

            _context.ScheduleListElement.RemoveRange(brand.ScheduleListElements);
            brand.ScheduleListElements = _brandData.ScheduleListElements;

            brand.BrandName = _brandData.BrandName;
            brand.DeliveryPrice = _brandData.DeliveryPrice;
            brand.MinimalPrice = _brandData.MinimalPrice;
            brand.BrandInfo.Address = _brandData.BrandInfo.Address;
            brand.BrandInfo.Contact = _brandData.BrandInfo.Contact;
            brand.BrandInfo.Description = _brandData.BrandInfo.Description;
            //Для картинок проверки, т.к. высокий шанс того что там будет null
            if (_brandData.BrandInfo.Banner != null) brand.BrandInfo.Banner = _brandData.BrandInfo.Banner;
            if (_brandData.BrandInfo.Logo != null) brand.BrandInfo.Logo = _brandData.BrandInfo.Banner;
            brand.BrandInfo.DeliveryTime = _brandData.BrandInfo.DeliveryTime;
            brand.BrandInfo.DeliveryTerms = _brandData.BrandInfo.DeliveryTerms;

            if (_brandData.WaterBrand != null)
            {
                brand.WaterBrand.WaterPrice = _brandData.WaterBrand.WaterPrice;
                brand.WaterBrand.ContainerPrice = _brandData.WaterBrand.ContainerPrice;
                if (_brandData.WaterBrand.Certificate != null) brand.WaterBrand.Certificate = _brandData.WaterBrand.Certificate;
            }

            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Создает новый бренд для исполнителя
        /// </summary>
        /// <param name="_brand">Данные нового бренда</param>
        /// <param name="_executorId">Id владельца бренда</param>
        // POST: api/Brands
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public ActionResult PostBrand(Brand _brand)
        {
            if (!IsPostModelValid(_brand)) 
            {
                return BadRequest();
            }

            //Не позволять создавать бренды с уже имеющимся именем
            if (_context.Brand.Any(brand => brand.BrandName == _brand.BrandName)) 
            {
                return Forbid();
            }

            //Заполняем пробелы
            _brand.CreatedDate = DateTime.UtcNow.Date;
            if (_brand.BrandInfo == null)
            {
                _brand.BrandInfo = new BrandInfo();
            }
            if (_brand.Kind == Kind.bottledWater || _brand.Kind == Kind.water) 
            {
                if (_brand.WaterBrand == null)
                {
                    _brand.WaterBrand = new WaterBrand();
                }
            }

            _context.Brand.Add(_brand);
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Удаляет бренд от лица супер-админа и все подчиненные сущности каскадом
        /// </summary>
        // DELETE: api/Brands/5
        [Route("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public ActionResult DeleteBrand(int id)
        {
            var brand = _context.Brand.Find(id);

            _context.Remove(brand);
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Валидация получаемых данных метода POST
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsPostModelValid(Brand _brand)
        {
            try
            {
                if (_brand == null ||
                    string.IsNullOrEmpty(_brand.BrandName) ||
                    _brand.BrandDoc == null ||
                    IsBrandNameTaken(_brand.BrandName) ||
                    string.IsNullOrEmpty(_brand.BrandDoc.OfficialName) ||
                    string.IsNullOrEmpty(_brand.BrandDoc.Ogrn) ||
                    string.IsNullOrEmpty(_brand.BrandDoc.Inn) ||
                    string.IsNullOrEmpty(_brand.BrandDoc.LegalAddress) ||
                    string.IsNullOrEmpty(_brand.BrandDoc.Executor) ||
                    !_brand.BrandPaymentMethods.Any() ||
                    (_brand.ScheduleListElements.Count < 7) ||
                    !AreAllDaysDistinct(_brand.ScheduleListElements) ||
                    (_brand.BrandHashtags.Any() && !AreHashtagsValid(_brand.BrandHashtags.Select(tag => tag.HashtagId), _brand.Kind))
                    )
                {
                    return false;
                }

                var executor = _context.Executor.Include(exe => exe.Brand)
                                                .FirstOrDefault(exe => exe.ExecutorId == _brand.ExecutorId);

                if (executor == null || executor.Brand != null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации Brand модели POST метода - {_ex}");
                return false;
            }
        }

        /// <summary>
        /// Валидация получаемых данных метода PUT
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsPutModelValid(Brand _brand) 
        {
            try
            {
                if (_brand == null ||
                    string.IsNullOrEmpty(_brand.BrandName) ||
                    IsBrandNameTaken(_brand.BrandName) ||
                    !_brand.BrandPaymentMethods.Any() ||
                    !_brand.ScheduleListElements.Any() ||
                    (_brand.BrandHashtags.Any() && !AreHashtagsValid(_brand.BrandHashtags.Select(tag => tag.HashtagId), _brand.Kind))) 
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex) 
            {
                _logger.LogWarning($"Ошибка при валидации Brand модели PUT метода - {_ex}");
                return false;
            }
        }

        private bool AreHashtagsValid(IEnumerable<int> _ids, Kind _kind) 
        {
            if (_context.Hashtag.All(tag => _ids.Contains(tag.HashTagId) && tag.Kind == _kind)) 
            {
                return true;
            }
            return false;
        }

        private bool AreAllDaysDistinct(IEnumerable<ScheduleListElement> _days) 
        {
            var accumulatedDays = new List<DayOfWeek>();
            foreach (var day in _days) 
            {
                if (!accumulatedDays.Contains(day.DayOfWeek)) 
                {
                    accumulatedDays.Add(day.DayOfWeek);
                }
            }
            return accumulatedDays.Count == 7;
        }

        private bool IsBrandNameTaken(string _suggestedName) 
        {
            var caps = _suggestedName.ToUpper();
            if (_context.Brand.Any(brand => brand.BrandName.ToUpper() == caps)) 
            {
                return true;
            }
            return false;
        }
    }
}
