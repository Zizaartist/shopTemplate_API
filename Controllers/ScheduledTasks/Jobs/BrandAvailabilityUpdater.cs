using ApiClick.Models;
using ApiClick.StaticValues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers.ScheduledTasks.Jobs
{
    /// <summary>
    /// Задача, обновляющая состояние открытости брендов
    /// Выполняется в начале часа, повторяясь каждый час
    /// Задача создается при запуске API 
    /// </summary>
    public class BrandAvailabilityUpdater : IJob
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BrandAvailabilityUpdater(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ClickContext>();

                    //Будем надеятся что не будет переполнения оперативки
                    var allBrands = _context.Brand.Include(brand => brand.ScheduleListElements);

                    foreach (var brand in allBrands)
                    {
                        brand.Available = IsBrandOpen(brand.ScheduleListElements);
                    }

                    //Момент истины pls don't crash
                    await _context.SaveChangesAsync();

                }
                catch (Exception _ex)
                {
                    Debug.WriteLine($"Произошла ошибка при запуске BrandAvailabilityUpdater - {_ex}");
                }
            }
        }
    
        private bool IsBrandOpen(IEnumerable<ScheduleListElement> _schedule)
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
