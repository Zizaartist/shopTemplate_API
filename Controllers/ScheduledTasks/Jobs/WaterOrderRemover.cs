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
                    var order = _context.Orders.Find(int.Parse(context.JobDetail.Key.Name)); //В ключе содержится id
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                }
                catch (Exception _ex)
                {
                    Debug.WriteLine($"Произошла ошибка при запуске WaterOrderRemover - {_ex}");
                }
            }
        }
    }
}
