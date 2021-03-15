using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers.ScheduledTasks.Jobs
{
    public class TestJob : IJob
    {
        private static int counter = 0;
        public static int Counter { get => counter++; }

        private readonly IServiceScopeFactory _scopeFactory;

        public TestJob(IServiceScopeFactory scopeFactory)
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
                    Debug.WriteLine($"Success! Result: {(await _context.Images.FindAsync(2)).Path} - {context.JobDetail.Key.Name}");
                }
                catch (Exception _ex)
                {
                    Debug.WriteLine($"Произошла ошибка при запуске WaterOrderRemover - {_ex}");
                }
            }
        }

        public async static Task AddNewJob(TimeSpan _executeTime)
        {
            IJobDetail testJob = JobBuilder.Create<TestJob>()
                .WithIdentity(TestJob.Counter.ToString(), "TestGroup")
                .Build();

            var activationTime = new DateTimeOffset(DateTime.UtcNow.AddTicks(_executeTime.Ticks), TimeSpan.Zero);

            ITrigger testTrigger = TriggerBuilder.Create()
                .StartAt(activationTime)
                .Build();

            await Scheduler.scheduler.ScheduleJob(testJob, testTrigger);
        }

        public async static Task RemoveJob(int id) 
        {
            await Scheduler.scheduler.DeleteJob(new JobKey(id.ToString(), "TestGroup"));
        }
    }
}
