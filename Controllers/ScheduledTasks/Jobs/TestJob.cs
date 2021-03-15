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
                    Debug.WriteLine($"Success! Result: {(await _context.Images.FindAsync(2)).Path}");
                }
                catch (Exception _ex)
                {
                    Debug.WriteLine($"Произошла ошибка при запуске WaterOrderRemover - {_ex}");
                }
            }
        }
    }
}
