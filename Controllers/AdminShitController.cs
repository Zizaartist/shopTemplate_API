using ApiClick.Controllers.FrequentlyUsed;
using ApiClick.Models;
using ApiClick.Models.EnumModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminShitController : ControllerBase
    {
        private ClickContext _context;
        private string SUPER_SECRET_PASSWORD = "Cheese macaroni";

        public AdminShitController(ClickContext _context)
        {
            this._context = _context;
        }

        [Route("CreateSuperUser")]
        [HttpPost]
        public ActionResult CreateSuperUser(string super, string phone, string login, string password)
        {
            if (super != SUPER_SECRET_PASSWORD) return Forbid();

            var newSuperUser = new User()
            {
                Phone = phone,
                UserRole = UserRole.SuperAdmin,
                UserInfo = new UserInfo(),
                Executor = new Executor()
                {
                    Login = login,
                    Password = Functions.GetHashFromString(password)
                }
            };

            _context.User.Add(newSuperUser);
            _context.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "SuperAdmin")]
        [Route("PromoteUser/{id}")]
        [HttpPost]
        public ActionResult PromoteUser(int id, Executor credentials) 
        {
            var user = _context.User.Find(id);

            if (user == null || 
                credentials == null ||
                string.IsNullOrEmpty(credentials.Login) ||
                string.IsNullOrEmpty(credentials.Password)) 
            {
                return NotFound();
            }

            if (_context.Executor.Any(exe => exe.Login == credentials.Login)) 
            {
                return Forbid();
            }

            user.UserRole = UserRole.Admin;
            user.Executor = new Executor() 
            {
                Login = credentials.Login,
                Password = Functions.GetHashFromString(credentials.Password)
            };

            _context.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "SuperAdmin")]
        [Route("DemoteUser/{id}")]
        [HttpDelete]
        public ActionResult DemoteUser(int id)
        {
            var user = _context.User.Include(user => user.Executor).FirstOrDefault(user => user.UserId == id);

            if (user == null) 
            {
                return NotFound();
            }

            _context.Executor.Remove(user.Executor);
            _context.SaveChanges();

            return Ok();
        }

        //[Route("api/DeleteAllOrders")]
        //[Authorize(Roles = "SuperAdmin")]
        //[HttpDelete]
        //public async Task<ActionResult> DeleteAllOrders()
        //{
        //    foreach (var entity in _context.RequestDetails)
        //    {
        //        _context.RequestDetails.Remove(entity);
        //    }
        //    foreach (var entity in _context.WaterRequests)
        //    {
        //        _context.WaterRequests.Remove(entity);
        //    }
        //    _context.SaveChanges();

        //    foreach (var entity in _context.PointRegisters)
        //    {
        //        _context.PointRegisters.Remove(entity);
        //    }
        //    _context.SaveChanges();
        //    foreach (var entity in _context.OrderDetails) 
        //    {
        //        _context.OrderDetails.Remove(entity);
        //    }
        //    foreach (var entity in _context.Orders)
        //    {
        //        _context.Orders.Remove(entity);
        //    }
        //    _context.SaveChanges();
        //    return Ok();
        //}

        //[Route("api/DeleteAllUselessImages")]
        //[Authorize(Roles = "SuperAdmin")]
        //[HttpDelete]
        //public async Task<ActionResult> DeleteAllUselessImages()
        //{
        //    var images = _context.Images.ToList();

        //    var imagesToDelete = new List<Image>();
        //    //Сперва получаем список записей, которые не имеют зависимостей
        //    foreach (Image image in images) 
        //    {
        //        if ((_context.Brands.FirstOrDefault(e => e.ImgBannerId == image.ImageId || e.ImgBannerId == image.ImageId) != null) || 
        //            (_context.Categories.FirstOrDefault(e => e.ImgId == image.ImageId) != null) || 
        //            (_context.Products.FirstOrDefault(e => e.ImgId == image.ImageId) != null))
        //        {
        //            continue;
        //        }
        //        else 
        //        {
        //            imagesToDelete.Add(image);
        //        }
        //    }
        //    //Затем их удаляем
        //    foreach (Image image in imagesToDelete) 
        //    {
        //        _context.Images.Remove(image);
        //    }
        //    _context.SaveChanges();
        //    return Ok();
        //}

        //[Route("api/SendNotification")]
        //[Authorize(Roles = "SuperAdmin")]
        //[HttpPost]
        //public async Task<ActionResult> SendNotification(string phone, string text)
        //{
        //    string correctPhone = new Functions().convertNormalPhoneNumber(phone);
        //    var user = _context.Users.First(e => e.Phone == correctPhone);
        //    if (user.NotificationsEnabled)
        //    {
        //        await new NotificationsController().ToSendNotificationAsync(user.DeviceType, text, user.NotificationRegistration);
        //    }

        //    return Ok();
        //}
    }
}
