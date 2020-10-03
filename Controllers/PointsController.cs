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
        ClickContext _context = new ClickContext();
        const decimal pointsCoef = 0.05m;

        /// <summary>
        /// Снятие баллов со счета клиента и создание записи в регистре
        /// </summary>
        /// <returns>Остаток, который нужно оплатить другими средствами</returns>
        public async Task<PointRegister> CreatePointRegister(UserCl user, OrdersCl order)
        {
            decimal pointsSum = order.OrderDetails.Sum(s => Convert.ToInt32(s.Price) * s.Count);
            decimal points = pointsSum;
            if (user.Points < points)
            {
                points = user.Points;
            }
            PointRegister register = new PointRegister
            {
                OwnerId = user.UserId,
                OrderId = order.OrdersId,
                Points = points,
                TransactionCompleted = false
            };
            _context.PointRegisterCl.Add(register);
            try
            {
                await _context.SaveChangesAsync();
                order.User.Points -= points;
            }
            catch 
            {
                return null;
            }

            return register;
        }

        /// <summary>
        /// Один из возможных исходов процесса выполнения заказа
        /// передача баллов владельцу бренда, начисление клиенту баллов, если была произведена оплата другими средствами
        /// </summary>
        public async void RemovePoints(PointRegister pointRegister)
        {
            pointRegister.Order.BrandOwner.Points += pointRegister.Points;
            decimal sum = pointRegister.Order.OrderDetails.Sum(s => Convert.ToInt32(s.Price) * s.Count);
            decimal moneySum = sum - pointRegister.Points;
            pointRegister.Owner.Points += Convert.ToInt32(moneySum * pointsCoef);
            
            var register = await _context.PointRegisterCl.FindAsync(pointRegister.PointRegisterId);
            register.TransactionCompleted = true;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Один из возможных исходов процесса выполнения заказа
        /// возврат баллов клиенту
        /// </summary>
        public async void ReturnPoints(PointRegister pointRegister)
        {
            pointRegister.Owner.Points += pointRegister.Points;
            var register = await _context.PointRegisterCl.FindAsync(pointRegister.PointRegisterId);
            register.TransactionCompleted = true;
            await _context.SaveChangesAsync();
        }
    }
}
