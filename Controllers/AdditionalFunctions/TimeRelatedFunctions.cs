using ApiClick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiClick.Controllers.AdditionalFunctions
{
    public class ScheduleFunctions
    {
        ClickContext _context;
        OrdersAdditionalFunctions ordersFuncs;
        List<OrderTask> orderTasks;

        public ScheduleFunctions() 
        {
            _context = new ClickContext();
            ordersFuncs = new OrdersAdditionalFunctions(_context);
            orderTasks = new List<OrderTask>();
        }

        public async Task<List<OrderTask>> FormSchedules(System.Threading.Timer timer)
        {
            var orders = ordersFuncs.getAllUnfinishedOrders().OrderBy(e => e.ExpiresAt).ToList();
            var orderTaskList = new List<OrderTask>();

            orders.ForEach(order => orderTaskList.Add(new OrderTask()
            {
                ExprirationTime = order.ExpiresAt,
                OrderId = order.OrdersId
            }));

            //var timeNow = DateTime.Now;
            //TimeSpan timeLeft = newFirstTask.ExprirationTime.TimeOfDay - timeNow.TimeOfDay;

            //timer = new System.Threading.Timer(x =>
            //{
            //    //Удаление устаревшего задания, запуск последствий окончания задания, установка нового задания 
            //    replaceAction();
            //}, null, timeLeft, Timeout.InfiniteTimeSpan);

            return orderTaskList;
        }

        /// <summary>
        /// Запускается при достижении таймером времени текущего задания
        /// </summary>
        public async Task OrderTaskExecuted(Action replaceAction, System.Threading.Timer sender)
        {
            var timeNow = DateTime.Now;
            TimeSpan timeLeft = newFirstTask.ExprirationTime.TimeOfDay - timeNow.TimeOfDay;

            sender = new System.Threading.Timer(x =>
            {
                //Удаление устаревшего задания, запуск последствий окончания задания, установка нового задания 
                
            }, null, timeLeft, Timeout.InfiniteTimeSpan);
        }

    }

    public class OrderTask 
    {
        public int OrderId { get; set; }
        public DateTime ExprirationTime { get; set; }
    }
}
