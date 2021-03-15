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
                                        .Select(e => new { TriggerTime = e.CreatedDate, OrderId = e.OrderId });

            int i = 0;
            foreach (var order in orders)
            {
                WaterOrderRemover.Add(order.TriggerTime, order.OrderId);
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

            await TestJob.AddNewJob(TimeSpan.FromSeconds(20));
            await TestJob.AddNewJob(TimeSpan.FromSeconds(30));
            await TestJob.AddNewJob(TimeSpan.FromSeconds(30));
            await TestJob.AddNewJob(TimeSpan.FromSeconds(30));

            #endregion

        }
    }
}
