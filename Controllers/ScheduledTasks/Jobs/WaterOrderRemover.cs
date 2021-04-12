using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers.ScheduledTasks
{
    public class WaterOrderRemover : IJob
    {
        public static string WATER_GROUP_NAME = "WaterOrderTasks";
        public static TimeSpan ORDER_LIFETIME = TimeSpan.FromHours(2);

        private readonly IServiceScopeFactory _scopeFactory;

        public WaterOrderRemover(IServiceScopeFactory scopeFactory)
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
                    var order = _context.Order.Find(int.Parse(context.JobDetail.Key.Name)); //В ключе содержится id
                    _context.Order.Remove(order);
                    await _context.SaveChangesAsync();
                }
                catch (Exception _ex)
                {
                    Debug.WriteLine($"Произошла ошибка при запуске WaterOrderRemover - {_ex}");
                }
            }
        }

        public async static void Add(DateTime _triggerTime, int _orderId) 
        {
            var actualTriggerTime = new DateTimeOffset(_triggerTime.AddTicks(ORDER_LIFETIME.Ticks), TimeSpan.Zero);
            var debtTime = new DateTimeOffset(DateTime.UtcNow.AddMinutes(1), TimeSpan.Zero);

            IJobDetail waterJob = JobBuilder.Create<WaterOrderRemover>()
                .WithIdentity(_orderId.ToString(), WATER_GROUP_NAME)
                .Build();

            //Если работа выполнена до старта scheduler - выполнить через минуту

            ITrigger waterTrigger = TriggerBuilder.Create()
                .StartAt(actualTriggerTime < DateTime.UtcNow ? debtTime : actualTriggerTime)
                .Build();

            await Scheduler.scheduler.ScheduleJob(waterJob, waterTrigger);
        }

        public async static void Remove(int _orderId)
        {
            await Scheduler.scheduler.DeleteJob(new JobKey(_orderId.ToString(), WATER_GROUP_NAME));
        }
    }
}
