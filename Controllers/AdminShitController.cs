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
        ClickContext _context = new ClickContext();

        [Route("api/DeleteAllOrders")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<ActionResult> DeleteAllOrders()
        {
            foreach (var entity in _context.OrderDetailCl) 
            {
                _context.OrderDetailCl.Remove(entity);
            }
            foreach (var entity in _context.OrdersCl)
            {
                _context.OrdersCl.Remove(entity);
            }
            _context.SaveChanges();
            return Ok();
        }

        [Route("api/DeleteAllUselessImages")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<ActionResult> DeleteAllUselessImages()
        {
            var images = _context.ImageCl.ToList();

            var imagesToDelete = new List<ImageCl>();
            //Сперва получаем список записей, которые не имеют зависимостей
            foreach (ImageCl image in images) 
            {
                if ((_context.BrandCl.FirstOrDefault(e => e.ImgBannerId == image.ImageId || e.ImgBannerId == image.ImageId) != null) || 
                    (_context.BrandMenuCl.FirstOrDefault(e => e.ImgId == image.ImageId) != null) || 
                    (_context.ProductCl.FirstOrDefault(e => e.ImgId == image.ImageId) != null))
                {
                    continue;
                }
                else 
                {
                    imagesToDelete.Add(image);
                }
            }
            //Затем их удаляем
            foreach (ImageCl image in imagesToDelete) 
            {
                _context.ImageCl.Remove(image);
            }
            _context.SaveChanges();
            return Ok();
        }


        [Route("api/DeleteAllRequests")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<ActionResult> DeleteAllRequests()
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
            return Ok();
        }

        [Route("api/SendNotification")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult> SendNotification(string phone, string text)
        {
            string correctPhone = new Functions().convertNormalPhoneNumber(phone);
            var user = _context.UserCl.First(e => e.Phone == correctPhone);
            await new NotificationsController().ToSendNotificationAsync(user.DeviceType, text, user.NotificationRegistration);
            
            return Ok();
        }
    }
}
