using ApiClick.Controllers.ScheduledTasks.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers.ScheduledTasks
{
    public class Scheduler
    {

        #region properies & fields

        public static TimeSpan ORDER_LIFETIME = TimeSpan.FromHours(2);
        public static string WATER_GROUP_NAME = "WaterOrderTasks";
        public static string REPORT_GROUP_NAME = "ReportTask";

        public static IScheduler scheduler { get; private set; }

        #endregion

        public static async void Start(IServiceProvider serviceProvider)
        {
            scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            scheduler.JobFactory = serviceProvider.GetService(typeof(JobFactory)) as JobFactory;
            await scheduler.Start();

            var _context = serviceProvider.GetService(typeof(ClickContext)) as ClickContext;

            #region water orders removal tasks

            //Создаем список задач
            var orders = _context.Orders.Where(e => e.Category == Models.Category.bottledWater || e.Category == Models.Category.water) //Водные заказы
                                        .Where(e => e.BrandOwnerId == null) //Не оккупированные
                                        .Select(e => new { TriggerTime = e.CreatedDate.AddTicks(ORDER_LIFETIME.Ticks), OrderId = e.OrderId });

            int i = 0;
            foreach (var order in orders)
            {
                IJobDetail waterJob = JobBuilder.Create<WaterOrderRemover>()
                    .WithIdentity(order.OrderId.ToString(), WATER_GROUP_NAME)
                    .Build();

                ITrigger waterTrigger = TriggerBuilder.Create()
                    .StartAt(new DateTimeOffset(order.TriggerTime, TimeSpan.Zero))
                    .Build();                               

                await scheduler.ScheduleJob(waterJob, waterTrigger);

                //Если работа выполнена до старта scheduler - выполнить вручную
                if (DateTime.UtcNow >= order.TriggerTime)
                {
                    await scheduler.TriggerJob(new JobKey(order.OrderId.ToString()));
                }
            }

            #endregion

            #region reports tasks

            IJobDetail reportJob = JobBuilder.Create<ReportSender>()
                .WithIdentity("1", REPORT_GROUP_NAME)
                .Build();

            var todayAtMidnight = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(1), TimeSpan.Zero);

            ITrigger reportTrigger = TriggerBuilder.Create()
                .StartAt(todayAtMidnight)
                .Build();

            await scheduler.ScheduleJob(reportJob, reportTrigger);

            #endregion

            #region test tasks

            IJobDetail testJob = JobBuilder.Create<TestJob>()
                .WithIdentity("1", "TestGroup")
                .Build();

            var activationTime = new DateTimeOffset(DateTime.UtcNow.AddSeconds(10), TimeSpan.Zero);

            ITrigger testTrigger = TriggerBuilder.Create()
                .StartAt(activationTime)
                .Build();

            await scheduler.ScheduleJob(testJob, testTrigger);

            #endregion

        }
    }
}
