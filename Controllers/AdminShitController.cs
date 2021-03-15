using ApiClick.Controllers.FrequentlyUsed;
using ApiClick.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    [ApiController]
    public class AdminShitController : ControllerBase
    {
        private ClickContext _context;

        public AdminShitController(ClickContext _context)
        {
            this._context = _context;
        }

        [Route("api/DeleteAllOrders")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<ActionResult> DeleteAllOrders()
        {
            foreach (var entity in _context.RequestDetails)
            {
                _context.RequestDetails.Remove(entity);
            }
            foreach (var entity in _context.WaterRequests)
            {
                _context.WaterRequests.Remove(entity);
            }
            _context.SaveChanges();

            foreach (var entity in _context.PointRegisters)
            {
                _context.PointRegisters.Remove(entity);
            }
            _context.SaveChanges();
            foreach (var entity in _context.OrderDetails) 
            {
                _context.OrderDetails.Remove(entity);
            }
            foreach (var entity in _context.Orders)
            {
                _context.Orders.Remove(entity);
            }
            _context.SaveChanges();
            return Ok();
        }

        [Route("api/DeleteAllUselessImages")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<ActionResult> DeleteAllUselessImages()
        {
            var images = _context.Images.ToList();

            var imagesToDelete = new List<Image>();
            //Сперва получаем список записей, которые не имеют зависимостей
            foreach (Image image in images) 
            {
                if ((_context.Brands.FirstOrDefault(e => e.ImgBannerId == image.ImageId || e.ImgBannerId == image.ImageId) != null) || 
                    (_context.BrandMenus.FirstOrDefault(e => e.ImgId == image.ImageId) != null) || 
                    (_context.Products.FirstOrDefault(e => e.ImgId == image.ImageId) != null))
                {
                    continue;
                }
                else 
                {
                    imagesToDelete.Add(image);
                }
            }
            //Затем их удаляем
            foreach (Image image in imagesToDelete) 
            {
                _context.Images.Remove(image);
            }
            _context.SaveChanges();
            return Ok();
        }

        [Route("api/SendNotification")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult> SendNotification(string phone, string text)
        {
            string correctPhone = new Functions().convertNormalPhoneNumber(phone);
            var user = _context.Users.First(e => e.Phone == correctPhone);
            if (user.NotificationsEnabled)
            {
                await new NotificationsController().ToSendNotificationAsync(user.DeviceType, text, user.NotificationRegistration);
            }
            
            return Ok();
        }
    }
}
