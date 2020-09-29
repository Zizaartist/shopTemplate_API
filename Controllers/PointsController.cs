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

        public async void CreatePointRegister(UserCl user, OrdersCl order)
        {
            int points = order.OrderDetails.Sum(s => Convert.ToInt32(s.Price) * s.Count);
            if (user.Points < points)
            {
                points = user.Points;
            }
            var pointRegister = new PointRegister();
            pointRegister.Owner = user;
            pointRegister.Order = order;
            pointRegister.Points = points;
            _context.PointRegisterCl.Add(pointRegister);
            await _context.SaveChangesAsync();
        }

        public async void RemovePoints(PointRegister pointRegister)
        {
            pointRegister.Owner.Points -= pointRegister.Points;
            pointRegister.Order.BrandOwner.Points += pointRegister.Points;
            int summ = pointRegister.Order.OrderDetails.Sum(s => Convert.ToInt32(s.Price) * s.Count);
            float moneySumm = summ - pointRegister.Points;
            pointRegister.Owner.Points += Convert.ToInt32(moneySumm*0.05);
            var register = await _context.PointRegisterCl.FindAsync(pointRegister.PointRegisterId);
            if (register != null)
            {
                _context.PointRegisterCl.Remove(register);
            }
            await _context.SaveChangesAsync();
        }

        public async void ReturnPoints(PointRegister pointRegister)
        {
            pointRegister.Owner.Points += pointRegister.Points;
            var register = await _context.PointRegisterCl.FindAsync(pointRegister.PointRegisterId);
            if (register != null)
            {
                _context.PointRegisterCl.Remove(register);
            }
            await _context.SaveChangesAsync();
        }
    }
}
