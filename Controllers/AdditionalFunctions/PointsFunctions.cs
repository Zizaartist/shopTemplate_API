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
    /// <summary>
    /// Функции, связанные с переводами баллов
    /// Занимается моделями PointRegister, DigitalBill
    /// </summary>
    public class PointsFunctions
    {
        ClickContext registersContext;
        ClickContext outsideContext;
        const decimal pointsCoef = 0.05m;

        //PUBLIC

        public PointsFunctions(ClickContext _context)
        {
            registersContext = new ClickContext(); //Новый, чтобы не портить работу уже имеющихся
            outsideContext = _context;
        }

        /// <summary>
        /// Добавляет баллы пользователю
        /// </summary>
        /// <param name="context">Заказ, являющийся контекстом перевода</param>
        /// <param name="target">Целевой пользователь, получающий сумму</param>
        /// <param name="sum">Размер перевода</param>
        /// <returns>Успешность операции</returns>
        public bool AddPoints(OrdersCl context, UserCl target, decimal sum)
        {
            bool valueChange = true;

            return ChangePoints(context, target, sum, valueChange);
        }

        /// <summary>
        /// Отнимает баллы у пользователя
        /// </summary>
        /// <param name="context">Заказ, являющийся контекстом перевода</param>
        /// <param name="target">Целевой пользователь, получающий сумму</param>
        /// <param name="sum">Размер перевода</param>
        /// <returns>Успешность операции</returns>
        public bool RemovePoints(OrdersCl context, UserCl target, decimal sum)
        {
            bool valueChange = false;

            return ChangePoints(context, target, sum, valueChange);
        }

        //PRIVATE

        private bool ChangePoints(OrdersCl context, UserCl target, decimal sum, bool valueChange)
        {
            if (sum <= 0 ||
                   context == null ||
                   target == null ||
                   context.OrdersId <= 0 ||
                   target.UserId <= 0)
            {
                return false;
            }

            var newRegister = AddNewRegister(context, target, sum, valueChange);
            if (newRegister == null)
            {
                return false;
            }

            if (ChangeTargetPoints(target, sum, valueChange))
            {
                return true;
            }
            else
            {
                RemoveRegister(newRegister); //Если не выполнится, то это станет заботой сис. админа
                return false;
            }
        }

        private PointRegister AddNewRegister(OrdersCl context, UserCl target, decimal sum, bool valueChange)
        {
            var newRegister = new PointRegister()
            {
                OrderId = context.OrdersId,
                UserId = context.UserId,
                Sum = sum,
                ValueChange = true
            };

            registersContext.PointRegisters.Add(newRegister);

            try
            {
                registersContext.SaveChanges();
                return newRegister;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void RemoveRegister(PointRegister newRegister) 
        {
            registersContext.PointRegisters.Remove(newRegister);
            registersContext.SaveChanges();
        }

        private bool ChangeTargetPoints(UserCl target, decimal sum, bool valueChange) 
        {
            if (valueChange)
            {
                target.Points += sum;
            }
            else
            {
                target.Points -= sum;
            }

            try
            {
                outsideContext.SaveChanges();
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }
    }
}
