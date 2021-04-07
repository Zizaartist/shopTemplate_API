using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiClick.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiClick.Controllers.FrequentlyUsed
{
    public class PointsController
    {
        ClickContext _context;
        const decimal pointsCoef = 0.05m;
        const decimal pointsMax = 0.3m; //30%

        public PointsController(ClickContext _context)
        {
            this._context = _context;
        }

        public decimal GetMaxPayment(decimal _userPoints, Order _order) 
        {
            var sumCost = _order.OrderDetails.Sum(e => e.Price * e.Count);
            var costInPoints = sumCost * pointsMax; //Пока статичные 30%
            if (_userPoints > costInPoints)
            {
                return costInPoints;
            }
            else 
            {
                return _userPoints;
            }
        }

        /// <summary>
        /// Выполняет изменение значения текущих баллов
        /// затем документирует изменение в виде регистра
        /// </summary>
        /// <returns>Успешность операции</returns>
        public bool StartTransaction(decimal _points, int _receiverId, Order _order, out PointRegister register, User _sender = null) 
        {
            User receiver;
            bool hasSender = _sender != null;
            register = null;

            try
            {
                receiver = _context.User.Find(_receiverId); //Просто подтверждаем факт наличия
            }
            catch 
            {
                return false; //пользователь или заказ не найден
            }

            if (hasSender) //Отправитель - человек
            {
                if (_sender.Points < _points)
                {
                    return false; //недостаточно средств для действия
                }

                //Изымаем средства от отправителя
                try
                {
                    _sender.Points -= _points;
                }
                catch
                {
                    return false;
                }
            }

            try
            {
                //Документируем
                _order.PointRegister = new PointRegister()
                {
                    TransactionCompleted = false,
                    SenderId = _sender?.UserId,
                    ReceiverId = _receiverId,
                    OrderId = _order.OrderId,
                    Points = _points,
                    CreatedDate = DateTime.UtcNow
                };
            }
            catch 
            {
                return false;
            }
            return true;
        }

        public bool CompleteTransaction(PointRegister _pointRegister)
        {
            User receiver;
            try
            {
                receiver = _context.User.Find(_pointRegister.ReceiverId);
            }
            catch
            {
                return false;
            }

            //Совершаем изменение
            try
            {
                if (!_pointRegister.TransactionCompleted)
                {
                    receiver.Points += _pointRegister.Points;
                    _pointRegister.TransactionCompleted = true;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool CancelTransaction(PointRegister _pointRegister)
        {
            User sender;
            try
            {
                sender = _context.User.Find(_pointRegister.SenderId);
            }
            catch
            {
                return false;
            }

            //Совершаем изменение
            try
            {
                sender.Points += _pointRegister.Points;
                _context.PointRegister.Remove(_pointRegister);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static decimal CalculateSum(Order _order)
        {
            if (!_order.OrderDetails.Any() || _order.OrderDetails.Any(e => e.Price == default || e.Count == default)) throw new Exception("Ошибка при вычислении кэшбэка");
            return _order.OrderDetails.Sum(e => e.Price * e.Count);
        }

        public decimal CalculateCashback(Order _order) 
        {
            var pointlessSum = CalculatePointless(_order);
            return pointlessSum * pointsCoef;
        }

        public decimal CalculatePointless(Order _order) 
        {
            var sum = CalculateSum(_order);
            try
            {
                var points = _order.PointRegisterId != null ? _context.PointRegister.Find(_order.PointRegisterId).Points : 0;
                return sum - points;
            }
            catch (Exception _ex) 
            {
                throw _ex;
            }
        }
    }
}
