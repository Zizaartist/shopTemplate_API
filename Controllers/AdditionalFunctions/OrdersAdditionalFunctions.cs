using ApiClick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers.AdditionalFunctions
{
    public class OrdersAdditionalFunctions
    {
        ClickContext _context;

        public OrdersAdditionalFunctions(ClickContext _context) 
        {
            this._context = _context;
        }

        /// <summary>
        /// Возвращает заказы без окончательного статуса
        /// </summary>
        /// <returns></returns>
        public List<OrdersCl> getAllUnfinishedOrders() 
        {
            return _context.OrdersCl.Where(e => 
                e.StatusId != _context.OrderStatusCl.First(e => 
                    e.OrderStatusName == "Завершено").OrderStatusId).ToList();
        }

        public async Task orderExpired(int id) 
        {
            var order = await _context.OrdersCl.FindAsync(id);

        }

        /// <summary>
        /// Сценарий 1 
        /// Жалоба клиента была ложной
        /// </summary>
        public async void Scenario1(OrdersCl order)
        {

        }

        /// <summary>
        /// Сценарий 2 
        /// Жалоба владельца бренда была ложной
        /// </summary>
        public async void Scenario2(OrdersCl order)
        {

        }

        /// <summary>
        /// Сценарий 3 
        /// Жалоба клиента является правдивой
        /// </summary>
        public async void Scenario3(OrdersCl order)
        {

        }

        /// <summary>
        /// Сценарий 4
        /// Жалоба владельца бренда является правдивой
        /// </summary>
        public async void Scenario4(OrdersCl order)
        {

        }

        /// <summary>
        /// Сценарий 5 
        /// Отсутствие жалоб с обеих сторон, обе стороны установили статус "Завершено"
        /// </summary>
        public async void Scenario5(OrdersCl order)
        {

        }

        /// <summary>
        /// Сценарий 6 
        /// Отсутствие жалоб с обеих сторон, обе стороны не установили статус "Завершено"
        /// </summary>
        public async void Scenario6(OrdersCl order)
        {

        }

        /// <summary>
        /// Сценарий 7 
        /// Отсутствие жалоб с обеих сторон, только клиент установил статус "Завершено"
        /// </summary>
        public async void Scenario7(OrdersCl order)
        {

        }

        /// <summary>
        /// Сценарий 8 
        /// Отсутствие жалоб с обеих сторон, только владелец бренда установил статус "Завершено"
        /// </summary>
        public async void Scenario8(OrdersCl order)
        {

        }
    }
}
