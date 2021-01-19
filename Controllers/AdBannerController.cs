using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiClick.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiClick.Controllers
{
    [ApiController]
    public class AdBannerController : ControllerBase
    {
        ClickContext _context;
        Functions funcs = new Functions();

        public AdBannerController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/AdBanners
        //Получение рекламных баннеров, пока крайне неэффективная функция
        [Route("api/[controller]/{categoryId}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet("{categoryId}")]
        public async Task<ActionResult<IEnumerable<int>>> GetAdBanners(int categoryId) //IEnumerable<AdBanner>
        {
            if (await _context.Categories.FindAsync(categoryId) == null)
            {
                return BadRequest("Такой категории не найдено");
            }
            
            //Удаляем просроченные баннеры
            var expiredBanners = _context.AdBanners.Where(e => e.ViewCount <= 0);
            _context.RemoveRange(expiredBanners);
            await _context.SaveChangesAsync();

            //Получаем баннеры по нужной категории
            var allBanners = _context.AdBanners.Where(e => e.CategoryId == categoryId).ToList();
            
            //Низкоуровневый код 😱
            //От initialCount зависят шансы на отображение, чем больше просмотров должно было быть у баннера - тем чаще от будет попадать в результирующий список
            List<AdBanner> resultBanners = new List<AdBanner>();
            for (int adsAmount = 3; adsAmount > 0; adsAmount--)
            {
                int sum = allBanners.Sum(e => e.InitialCount); //Суммарное количество "потенциальных просмотров"
                int rNumber = new Random().Next(sum); //Получаем случайное число в диапазоне 0-sum
                int currentNumber = 0;
                foreach (var banner in allBanners)
                {
                    var nextNumber = currentNumber + banner.InitialCount;
                    if (rNumber >= currentNumber && rNumber < nextNumber) //Если случайное число указывает на баннер - добавить его в результат и завершиить цикл
                    {
                        resultBanners.Add(banner);
                        break;
                    }
                    currentNumber += nextNumber;
                }
                allBanners.Remove(resultBanners.Last());
            }
            
            //Успех! Измени значения текущих просмотров для элементов списка
            foreach (var banner in resultBanners)
            {
                (await _context.AdBanners.FindAsync(banner.AdBannerId)).ViewCount--;
            }
            await _context.SaveChangesAsync();
            
            foreach (var banner in resultBanners)
            {
                banner.Image = funcs.getCleanModel(await _context.Images.FindAsync(banner.ImgId));
            }

            return resultBanners.Select(e => e.InitialCount).ToList();
        }
        
        // POST: api/AdBanners
        //Установка баннера, предположительно суперадмином
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult> PostAdBanner(AdBanner adBanner)
        {
            if (adBanner == null)
            {
                return BadRequest();
            }

            adBanner.ViewCount = adBanner.InitialCount;

            _context.AdBanners.Add(adBanner);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
