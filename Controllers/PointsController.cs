using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiClick.Models;
using ApiClick.Models.RegisterModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiClick.Controllers
{
    public class PointsController
    {
        ClickContext _context;
        const decimal pointsCoef = 0.05m;

        public PointsController(ClickContext _context)
        {
            this._context = _context;
        }

        /// <summary>
        /// Снятие баллов со счета клиента и создание записи в регистре
        /// </summary>
        /// <returns>Остаток, который нужно оплатить другими средствами</returns>
        public async Task<PointRegister> CreatePointRegister(User user, Order order)
        {
            decimal pointsSum = order.OrderDetails.Sum(s => Convert.ToInt32(s.Price) * s.Count);
            decimal points = pointsSum;
            if (user.Points < points)
            {
                points = user.Points;
            }
            //Нет возможности оплатить баллами, не учитывать
            PointRegister register = null;
            if (points != 0)
            {
                register = new PointRegister
                {
                    OwnerId = user.UserId,
                    OrderId = order.OrderId,
                    Points = points,
                    TransactionCompleted = false
                };
                _context.PointRegisters.Add(register);
                try
                {
                    var dbUser = _context.Users.Find(order.UserId);
                    dbUser.Points -= points;
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    return null;
                }
            }

            return register;
        }


        public async Task<PointRegister> CreatePointRegister(User user, Order order, decimal points)
        {
            //Нет возможности оплатить баллами, не учитывать
            PointRegister register = null;
            if (points != 0)
            {
                register = new PointRegister
                {
                    OwnerId = user.UserId,
                    OrderId = order.OrderId,
                    Points = points,
                    TransactionCompleted = false
                };
                _context.PointRegisters.Add(register);
                try
                {
                    var dbUser = _context.Users.Find(order.UserId);
                    dbUser.Points -= points;
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    return null;
                }
            }
            else 
            {
                return null;
            }

            return register;
        }

        /// <summary>
        /// Один из возможных исходов процесса выполнения заказа
        /// передача баллов владельцу бренда, начисление клиенту баллов, если была произведена оплата другими средствами
        /// </summary>
        public void RemovePoints(Order order)
        {
            //Начисляем баллы пользователя владельцу бренда
            var brandOwner = _context.Users.Find(order.BrandOwnerId);
            brandOwner.Points += order.PointRegister.Points;
            
            var register = _context.PointRegisters.Find(order.PointRegisterId);
            register.TransactionCompleted = true;
            _context.SaveChanges();
        }

        /// <summary>
        /// Вычисляет 5% от денежной суммы и переводит на счет пользователя баллами
        /// </summary>
        public void GetPoints(Order order, decimal points)
        {
            decimal sum = order.OrderDetails.Sum(s => Convert.ToInt32(s.Price) * s.Count);
            decimal moneySum = sum - points;
            var user = _context.Users.Find(order.UserId);
            user.Points += Convert.ToInt32(moneySum * pointsCoef);
            _context.SaveChanges();
        }

        /// <summary>
        /// Один из возможных исходов процесса выполнения заказа
        /// возврат баллов клиенту
        /// </summary>
        public async Task ReturnPoints(PointRegister pointRegister)
        {
            pointRegister.Owner.Points += pointRegister.Points;
            var register = await _context.PointRegisters.FindAsync(pointRegister.PointRegisterId);
            register.TransactionCompleted = true;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Распределеяет баллы равномерно по всем предъявленным заказам
        /// </summary>
        /// <returns>Список числовых значений баллов для каждого заказа</returns>
        public List<OrderExtended> DistributePoints(List<Order> orders)
        {
            decimal currentPoints = _context.Users.Find(orders.First().UserId).Points;
            //Включает в себя те, заказы, которые оплачены полностью
            List<OrderExtended> notYetPaidOrders = new List<OrderExtended>();
            orders.ForEach(e => 
            {
                OrderExtended oe = new OrderExtended();
                oe.order = e;
                oe.orderSum = e.OrderDetails.Sum(x => x.Price * x.Count);
                oe.pointsInvested = 0;
                notYetPaidOrders.Add(oe);
            });

            List<OrderExtended> results = new List<OrderExtended>();
            bool fairlyDistributed = false;

            //АЛГОРИТМ РАСПРЕДЕЛЕНИЯ
            //Каждую итерацию должна производиться оплата минимум 1 заказа
            while (!fairlyDistributed) 
            {
                OrderExtended paidOrder;
                //Сперва получаем неточную "среднюю" величину
                decimal notPreciseEvenSum = GetAverage(currentPoints, notYetPaidOrders.Count);
                //Если она может покрыть хоть 1 заказ - оплатить и запустить следующую итерацию
                paidOrder = notYetPaidOrders.FirstOrDefault(e => e.orderSum <= notPreciseEvenSum);
                if (paidOrder == null)
                {
                    //Оплачиваем среднюю величину для всех
                    while (notYetPaidOrders.Count > 0)
                    {
                        paidOrder = notYetPaidOrders.First();
                        notPreciseEvenSum = GetAverage(currentPoints, notYetPaidOrders.Count);
                        DistributePoints_paidOrder(ref currentPoints, paidOrder, results, notYetPaidOrders, notPreciseEvenSum);
                    }
                }
                else
                {
                    //Оплачиваем полную стоимость заказа
                    DistributePoints_paidOrder(ref currentPoints, paidOrder, results, notYetPaidOrders, paidOrder.orderSum);
                }
                if (notYetPaidOrders.Count == 0) 
                {
                    fairlyDistributed = true;
                }
            }

            return results;
        }

        //Действия при "оплате"
        private void DistributePoints_paidOrder(ref decimal currentPoints, OrderExtended paidOrder, List<OrderExtended> results, List<OrderExtended> notYetPaidOrders, decimal points)
        {
            currentPoints -= points;
            paidOrder.pointsInvested = points;
            results.Add(paidOrder);
            notYetPaidOrders.Remove(paidOrder);
        }

        private decimal GetAverage(decimal sumInput, int countInput) 
        {
            return Math.Round(sumInput / countInput, 2);
        }

        public class OrderExtended
        {
            public Order order { get; set; }
            public decimal orderSum { get; set; }
            public decimal pointsInvested { get; set; }
        }
    }
}
