using ApiClick.Models;
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
    /// Совершает ежедневное (00:00) создание отчетов для каждого бренда
    /// Также производит проверку на случай отсутствия вчерашних отчетов
    /// </summary>
    public class ReportSender : IJob
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ReportSender(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("Job executed!");
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ClickContext>();
                    var yesterday = DateTime.UtcNow.AddDays(-2).Date;
                    var today = DateTime.UtcNow.AddDays(-1).Date;

                    //Проверка пропущенных отчетов
                    //Проверка производится только на предыдущий день, большее количество проверок уже достигает абсурда

                    var brandsWithNoReportsYesterday = _context.Brands.Where(brand => 
                        _context.Reports.Any(report => 
                            report.BrandId == brand.BrandId &&
                            report.CreatedDate.Date == yesterday));

                    //Отправка недостающих отчетов
                    foreach (var brand in brandsWithNoReportsYesterday) 
                    {
                        AddingReports(brand, _context, yesterday);
                    }

                    //Отправка отчетов производится после полуночи по UTC
                    foreach (var brand in _context.Brands)
                    {
                        AddingReports(brand, _context, today);
                    }

                    //Момент истины pls don't crash
                    await _context.SaveChangesAsync();
                }
                catch (Exception _ex)
                {
                    Debug.WriteLine($"Произошла ошибка при запуске WaterOrderRemover - {_ex}");
                }
            }
        }

        private void AddingReports(Brand _brand, ClickContext _context, DateTime _day)
        {
            var ordersFromDay = _context.Orders.Where(order =>
                order.BrandOwnerId == _brand.UserId &&
                order.CreatedDate.Date == _day).ToList();

            var totatSum = ordersFromDay.Sum(order => PointsController.CalculateSum(order));

            Report result = new Report() 
            {
                OrderCount = ordersFromDay.Count,
                Sum = totatSum,
                CreatedDate = _day
            };

            if (ordersFromDay.Any())
            {
                //Выравниваем, создавая большой список orderDetails и группируем по критерию id продукции
                var productsByGroup = ordersFromDay.SelectMany(e => e.OrderDetails)
                        .Where(e => e.ProductId != null)
                        .GroupBy(e => e.ProductId);

                var countsPerProductType = productsByGroup.Select(e => new { ProductId = e.Key, Count = e.Sum(x => x.Count) });

                var productOfDay = countsPerProductType.Aggregate((max, next) => max.Count > next.Count ? max : next);

                result.ProductOfDayId = productOfDay.ProductId;
                result.ProductOfDayCount = productOfDay.Count;
                //Находим группу по id и из первого элемента вытаскиваем цену
                result.ProductOfDaySum = productOfDay.Count * productsByGroup.First(e => e.Key == productOfDay.ProductId).First().Price; //пока без проверок безопасности
            }

            _context.Reports.Add(result);
        }
    }
}
