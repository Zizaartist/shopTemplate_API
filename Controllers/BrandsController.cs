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

        // GET: api/Brands
        //Debug
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrand()
        {
            var brands = await _context.Brands.ToListAsync();

            return brands;
        }

        // POST: api/GetBrandsByFilter/0/3?name=blahbla&openNow=true
        [Route("GetBrandsByFilter/{_category}/{_page}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<List<Brand>>> GetBrandsByFilter(Kind _kind, int _page, [FromBody]List<int> HashTags = null, bool openNow = false)
        {
            var brands = _context.Brands.Where(p => p.Kind == _kind);

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

            var resultBrands = brands.ToList();

            //Чет десерилайзеру похуй на мои действия, он все равно присылает лишние данные
            return resultBrands;
        }


        // GET: api/GetBrandsByName/5?name=blahbla
        [Route("GetBrandsByName/{category}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<Brand>>> GetBrandsByName(Kind category, string name = null)
        {
            var brands = _context.Brands.Where(p => p.Kind == category);

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

            //Чет десерилайзеру похуй на мои действия, он все равно присылает лишние данные
            return resultBrands;
        }


        // GET: api/GetBrandsByKind/5
        //Получить список брендов категории id
        [Route("GetBrandsByKind/{category}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<Brand>>> GetBrandsByKind(Kind category)
        {
            var brands = _context.Brands.Where(p => p.Kind == category);

            if (!brands.Any())
            {
                return NotFound();
            }

            var resultBrands = await brands.ToListAsync();

            //Чет десерилайзеру похуй на мои действия, он все равно присылает лишние данные
            return resultBrands;
        }

        // GET: api/Brands/5
        //Получить бренд по id
        [Route("{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        // GET: api/GetMyBrands
        //Получить список своих брендов
        [Authorize(Roles = "SuperAdmin, Admin")]
        [Route("GetMyBrands")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetMyBrands() //хз пока как маршрутизировать
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);

            var brands = _context.Brands.Where(p => p.ExecutorId == mySelf.Executor.ExecutorId);

            if (brands == null)
            {
                return NotFound();
            }

            var resultBrands = await brands.ToListAsync();

            return resultBrands;
        }

        // PUT: api/Brands/5
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public async Task<IActionResult> PutBrand(Brand _brandData)
        {

            if (!IsPutModelValid(_brandData)) //дублирующиеся дни
            {
                return BadRequest();
            }

            var brand = _context.Brands.Find(_brandData.BrandId);

            if (brand == null) 
            {
                return NotFound();
            }

            var mySelf = Functions.identityToUser(User.Identity, _context);

            //отправитель - не владелец?
            if (brand.ExecutorId != mySelf.Executor.ExecutorId) 
            {
                return Forbid();
            }

            _context.BrandHashtags.RemoveRange(brand.BrandHashtags);
            brand.BrandHashtags = _brandData.BrandHashtags;

            _context.BrandPaymentMethods.RemoveRange(brand.BrandPaymentMethods);
            brand.BrandPaymentMethods = _brandData.BrandPaymentMethods;
            
            _context.ScheduleListElements.RemoveRange(brand.ScheduleListElements);
            brand.ScheduleListElements = _brandData.ScheduleListElements;

            brand.BrandName = _brandData.BrandName;
            brand.BrandInfo.Address = _brandData.BrandInfo.Address;
            brand.BrandInfo.Contact = _brandData.BrandInfo.Contact;
            brand.BrandInfo.Description = _brandData.BrandInfo.Description;
            if (_brandData.BrandInfo.Banner != null) brand.BrandInfo.Banner = _brandData.BrandInfo.Banner;
            if (_brandData.BrandInfo.Logo != null) brand.BrandInfo.Logo = _brandData.BrandInfo.Banner;
            brand.BrandInfo.DeliveryTime = _brandData.BrandInfo.DeliveryTime;
            brand.BrandInfo.Conditions = _brandData.BrandInfo.Conditions;

            if (_brandData.WaterBrand != null) 
            {
                brand.WaterBrand.WaterPrice = _brandData.WaterBrand.WaterPrice;
                brand.WaterBrand.ContainerPrice = _brandData.WaterBrand.ContainerPrice;
                if (_brandData.WaterBrand.Certificate != null) brand.WaterBrand.Certificate = _brandData.WaterBrand.Certificate;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

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
            if (_context.Brands.Any(a => a.BrandName == _brand.BrandName)) 
            {
                return Forbid();
            }

            //Заполняем пробелы
            _brand.CreatedDate = DateTime.UtcNow.Date;
            if (_brand.BrandInfo == null) _brand.BrandInfo = new BrandInfo();
            if (_brand.Kind == Kind.bottledWater || _brand.Kind == Kind.water) 
            {
                _brand.WaterBrand = new WaterBrand();
            }

            _context.SaveChanges();

            return Ok();
        }

        // DELETE: api/Brands/5
        [Route("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public ActionResult<Brand> DeleteBrand(int id)
        {
            var brand = _context.Brands.Find(id);

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
                    !_brand.ScheduleListElements.Any() ||
                    (_brand.BrandHashtags.Any() && !AreHashtagsValid(_brand.BrandHashtags.Select(tag => tag.HashtagId), _brand.Kind))
                    )
                {
                    return false;
                }

                var owner = _context.Users.Find(_brand.ExecutorId);
                if (owner == null) return false;

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
            if (_context.Hashtags.All(tag => _ids.Contains(tag.HashTagId) && tag.Kind == _kind)) 
            {
                return true;
            }
            return false;
        }

        private bool IsBrandNameTaken(string _suggestedName) 
        {
            var caps = _suggestedName.ToUpper();
            if (_context.Brands.Any(brand => brand.BrandName.ToUpper() == caps)) 
            {
                return true;
            }
            return false;
        }

        //private bool IsBrandOpen(List<ScheduleListElement> _schedule) 
        //{
        //    var currentYakutskTime = new DateTimeOffset(DateTime.UtcNow, Constants.YAKUTSK_OFFSET);
        //    var match = _schedule.FirstOrDefault(e => e.DayOfWeek == currentYakutskTime.DayOfWeek);
        //    if (match != null) 
        //    {
        //        //Попадаем ли мы во временной промежуток сейчас
        //        var closeEarlierThanOpen = match.OpenTime > match.CloseTime;
        //        var laterThanOpen = match.OpenTime <= currentYakutskTime.TimeOfDay;
        //        var earlierThanClose = match.CloseTime >= currentYakutskTime.TimeOfDay;
        //        if (
        //                (
        //                    closeEarlierThanOpen
        //                    || 
        //                    (laterThanOpen && earlierThanClose)
        //                ) 
        //            &&
        //                (
        //                    !closeEarlierThanOpen 
        //                    || 
        //                    (laterThanOpen || earlierThanClose)
        //                )
        //            ) 
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
