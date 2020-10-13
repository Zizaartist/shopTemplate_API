using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiClick;
using ApiClick.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore.Internal;
using ApiClick.Models.EnumModels;
using ApiClick.Models.RegisterModels;

namespace ApiClick.Controllers
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        ClickContext _context = new ClickContext();
        Functions funcs = new Functions();

        // GET: api/Orders
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersCl>>> GetOrdersCl()
        {
            return await _context.OrdersCl.ToListAsync();
        }

        // GET: api/Orders/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<OrdersCl>> GetOrdersCl(int id)
        {
            var ordersCl = await _context.OrdersCl.FindAsync(id);

            if (ordersCl == null)
            {
                return NotFound();
            }

            if (ordersCl.BrandOwnerId != funcs.identityToUser(User.Identity, _context).UserId) 
            {
                return Forbid();
            }

            List<OrderDetailCl> relatedOrderDetails = _context.OrderDetailCl.Where(d => d.OrderId == ordersCl.OrdersId).ToList();
            ordersCl.OrderDetails = relatedOrderDetails;

            return ordersCl;
        }

        // GET: api/GetMyOrders
        [Route("api/GetMyOrders")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersCl>>> GetMyOrders()
        {
            //Находит заказы принадлежащие пользователю и отсеивает заказы со статусом "Завершенный"
            var ordersFound = _context.OrdersCl.Where(e => e.UserId == funcs.identityToUser(User.Identity, _context).UserId &&
                                                        e.StatusId != _context.OrderStatusCl.First(e => e.OrderStatusName == "Завершено").OrderStatusId).ToList();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (OrdersCl order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList());
                foreach (OrderDetailCl detail in order.OrderDetails) 
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.ProductCl.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.ImageCl.FindAsync(detail.Product.ImgId));
                    } 
                }
                var ownerBuffer = await _context.UserCl.FindAsync(order.BrandOwnerId);
                //Если бренд не "мокрый"
                if (ownerBuffer != null)
                {
                    order.BrandOwner = new UserCl();
                    order.BrandOwner.Brands = funcs.getCleanListOfModels(new List<BrandCl>() { _context.BrandCl.First(e => e.UserId == order.BrandOwnerId) });
                    order.BrandOwner.Brands.First().ImgBanner = funcs.getCleanModel(await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgBannerId));
                    order.BrandOwner.Brands.First().ImgLogo = funcs.getCleanModel(await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgLogoId));
                }
                order.OrderStatus = funcs.getCleanModel(await _context.OrderStatusCl.FindAsync(order.StatusId));
                order.PaymentMethod = await _context.PaymentMethodCl.FindAsync(order.PaymentMethodId);
                if (order.PointsUsed)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisterCl.FindAsync(order.PointRegisterId));
                }
            }

            return orders;
        }

        // GET: api/GetMyTasks
        [Route("api/GetMyTasks")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersCl>>> GetMyTasks()
        {
            //Получаем заказы, где владелец токена обозначен как владелец бренда
            //+ статус не является завершенным
            var ordersFound = _context.OrdersCl.Where(e => e.BrandOwnerId == funcs.identityToUser(User.Identity, _context).UserId)
                                          .Where(e => e.OrderStatus.OrderStatusId != _context.OrderStatusCl.First(e => e.OrderStatusName == "Завершено").OrderStatusId).ToList();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (OrdersCl order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList());
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.ProductCl.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.ImageCl.FindAsync(detail.Product.ImgId));
                    }
                }
                order.User = funcs.getCleanUser(await _context.UserCl.FindAsync(order.UserId));
                var ownerBuffer = await _context.UserCl.FindAsync(order.BrandOwnerId);
                order.OrderStatus = funcs.getCleanModel(await _context.OrderStatusCl.FindAsync(order.StatusId));
                order.PaymentMethod = await _context.PaymentMethodCl.FindAsync(order.PaymentMethodId);
                if (order.PointsUsed)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisterCl.FindAsync(order.PointRegisterId));
                }
            }

            return orders;
        }
        
        // GET: api/GetMyHistory
        [Route("api/GetMyHistory")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersCl>>> GetMyHistory()
        {
            //Получаем заказы, где владелец токена обозначен как владелец бренда
            //+ статус должен быть завершенным
            var ordersFound = _context.OrdersCl.Where(e => e.BrandOwnerId == funcs.identityToUser(User.Identity, _context).UserId)
                                          .Where(e => e.OrderStatus.OrderStatusId == _context.OrderStatusCl.First(e => e.OrderStatusName == "Завершено").OrderStatusId).ToList();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (OrdersCl order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList());
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.ProductCl.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.ImageCl.FindAsync(detail.Product.ImgId));
                    }
                }
                order.User = funcs.getCleanUser(await _context.UserCl.FindAsync(order.UserId));
                var ownerBuffer = await _context.UserCl.FindAsync(order.BrandOwnerId);
                order.OrderStatus = funcs.getCleanModel(await _context.OrderStatusCl.FindAsync(order.StatusId));
                order.PaymentMethod = await _context.PaymentMethodCl.FindAsync(order.PaymentMethodId);
                if (order.PointsUsed)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisterCl.FindAsync(order.PointRegisterId));
                }
            }

            return orders;
        }

        // PUT: api/Orders
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPut]
        public async Task<ActionResult> PutOrdersCl(int id)
        {
            //Сперва проверяем на физическую возможность смены статуса
            var order = await _context.OrdersCl.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = await _context.OrderStatusCl.FindAsync(order.StatusId);
            if (order.OrderStatus.OrderStatusName == "Завершено")
            {
                return Forbid();
            }

            //Только пользователь и владелец бренда имеют доступ к смене статуса
            var identity = funcs.identityToUser(User.Identity, _context);
            if (!(order.UserId == identity.UserId || order.BrandOwnerId == identity.UserId)) 
            {
                return Forbid();
            }

            //Затем проверяем права на смену статуса
            int userRole = funcs.identityToUser(User.Identity, _context).Role;
            int futureStatusId = order.StatusId + 1;
            OrderStatusCl futureOrderStatusCl = await _context.OrderStatusCl.FindAsync(futureStatusId);

            //Изменить статус могут лишь указанная роль или суперАдмин
            if (userRole == futureOrderStatusCl.MasterRoleId || 
                userRole == _context.UserRolesCl.First(e => e.UserRoleName == "SuperAdmin").UserRoleId)
            {
                order.StatusId++;
                order.OrderStatus = futureOrderStatusCl;
            }
            else
            {
                return Unauthorized();
            }

            if (order.OrderStatus.OrderStatusName == "Завершено") 
            {
                PointsController pointsController = new PointsController();
                if (order.PointsUsed)
                {
                    order.PointRegister = await _context.PointRegisterCl.FindAsync(order.PointRegisterId);
                    order.OrderDetails = _context.OrderDetailCl.Where(e => e.OrderId == order.OrdersId).ToList();
                    pointsController.RemovePoints(order);
                    pointsController.GetPoints(order, (await _context.PointRegisterCl.FindAsync(order.PointRegisterId)).Points);
                }
                else
                {
                    pointsController.GetPoints(order, 0);
                }
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Orders
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<OrdersCl>> PostOrdersCl(OrdersCl ordersCl)
        {
            if (ordersCl == null || ordersCl.OrderDetails == null || ordersCl.OrderDetails.Count < 1)
            {
                return BadRequest();
            }

            foreach (OrderDetailCl detail in ordersCl.OrderDetails) 
            {
                if (detail.Count < 1)
                {
                    return BadRequest();
                }
            }

            var responsibleBrandOwnerId = await _context.BrandCl.FindAsync(
                                                  (await  _context.BrandMenuCl.FindAsync(
                                                       (await _context.ProductCl.FindAsync(
                                                            ordersCl.OrderDetails.First().ProductId)
                                                       ).BrandMenuId)
                                                  ).BrandId);

            //filling blanks and sending to DB
            ordersCl.CreatedDate = DateTime.Now;
            ordersCl.StatusId = _context.OrderStatusCl.First(e => e.OrderStatusName == "Отправлено").OrderStatusId;
            ordersCl.OrderStatus = await _context.OrderStatusCl.FindAsync(ordersCl.StatusId);
            ordersCl.UserId = funcs.identityToUser(User.Identity, _context).UserId;
            ordersCl.User = await _context.UserCl.FindAsync(ordersCl.UserId);
            ordersCl.Phone = ordersCl.User.Phone;
            ordersCl.BrandOwnerId = responsibleBrandOwnerId.UserId;

            _context.OrdersCl.Add(ordersCl);
            await _context.SaveChangesAsync();
            if (ordersCl.PointsUsed)
            {

                PointsController pointsController = new PointsController();
                var register = await pointsController.CreatePointRegister(ordersCl.User, ordersCl);
                if (register == null) 
                {
                    return BadRequest();
                }
                ordersCl.PointRegisterId = register.PointRegisterId;
            }

            ordersCl.BrandOwner = await _context.UserCl.FindAsync(ordersCl.BrandOwnerId);
            await _context.SaveChangesAsync();
            NotificationsController notificationsController = new NotificationsController();
            await notificationsController.ToSendNotificationAsync(ordersCl.BrandOwner.DeviceType, "У Вас новый заказ", ordersCl.BrandOwner.NotificationRegistration);

            return Ok();
        }

        // DELETE: api/Orders/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpDelete]
        public async Task<ActionResult<OrdersCl>> DeleteOrdersCl(int id)
        {
            var ordersCl = await _context.OrdersCl.FindAsync(id);
            if (ordersCl == null)
            {
                return NotFound();
            }

            _context.OrdersCl.Remove(ordersCl);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private UserCl identityToUser(IIdentity identity)
        {
            return _context.UserCl.FirstOrDefault(u => u.Phone == identity.Name);
        }

        private bool OrdersClExists(int id)
        {
            return _context.OrdersCl.Any(e => e.OrdersId == id);
        }

        [Route("api/GetOrdersByCategory/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersCl>>> GetOrdersByCategory(int id)
        {
            var identity = funcs.identityToUser(User.Identity, _context);
            var ordersFound = await _context.OrdersCl.Where(p => p.CategoryId == id && (p.BrandOwnerId == null || p.BrandOwnerId == identity.UserId)).ToListAsync();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (OrdersCl order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList());
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.ProductCl.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.ImageCl.FindAsync(detail.Product.ImgId));
                    }
                }
                order.User = funcs.getCleanUser(await _context.UserCl.FindAsync(order.UserId));
                var ownerBuffer = await _context.UserCl.FindAsync(order.BrandOwnerId);
                order.OrderStatus = funcs.getCleanModel(await _context.OrderStatusCl.FindAsync(order.StatusId));
                order.PaymentMethod = await _context.PaymentMethodCl.FindAsync(order.PaymentMethodId);
                if(order.PointsUsed)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisterCl.FindAsync(order.PointRegisterId));
                }
                if (order.BrandOwnerId != null)
                {
                    order.BrandOwner = funcs.getCleanUser(identity); //Не равен null если заказ взят отправившим запрос
                }
            }

            return orders;
        }

        [Route("api/PostVodaOrders")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<OrdersCl>> PostVodaOrdersCl(OrdersCl ordersCl)
        {
            if (ordersCl == null || ordersCl.OrderDetails == null || ordersCl.OrderDetails.Count < 1)
            {
                return BadRequest();
            }

            foreach (OrderDetailCl detail in ordersCl.OrderDetails)
            {
                if (detail.Count < 1)
                {
                    return BadRequest();
                }
            }

            //filling blanks and sending to DB
            ordersCl.CreatedDate = DateTime.Now;
            ordersCl.StatusId = _context.OrderStatusCl.First(e => e.OrderStatusName == "Отправлено").OrderStatusId;
            ordersCl.OrderStatus = await _context.OrderStatusCl.FindAsync(ordersCl.StatusId);
            ordersCl.UserId = funcs.identityToUser(User.Identity, _context).UserId;
            ordersCl.User = await _context.UserCl.FindAsync(ordersCl.UserId);
            ordersCl.PaymentMethodId = 1;

            if (ordersCl.PointsUsed)
            {
                PointsController pointsController = new PointsController();
                var register = await pointsController.CreatePointRegister(ordersCl.User, ordersCl);
                if (register == null)
                {
                    return BadRequest();
                }
                ordersCl.PointRegisterId = register.PointRegisterId;
            }

            _context.OrdersCl.Add(ordersCl);
            await _context.SaveChangesAsync(); //вроде как рефрешит объект ordersCl

            //NotificationsController notificationsController = new NotificationsController();
            //await notificationsController.ToSendNotificationAsync(ordersCl.BrandOwner.DeviceType, "У Вас новый заказ", ordersCl.BrandOwner.NotificationRegistration);

            return Ok();
        }

        [Route("api/PutVodaOrders/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPut]
        public async Task<ActionResult> PutVodaOrdersCl(int id)
        {
            //Сперва проверяем на физическую возможность смены статуса
            var order = await _context.OrdersCl.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            //Только пользователь и владелец бренда имеют доступ к смене статуса
            var identity = funcs.identityToUser(User.Identity, _context);
            if (!(order.UserId == identity.UserId || order.BrandOwnerId == identity.UserId))
            {
                return Forbid();
            }

            if (order.BrandOwner == null)
            {
                order.BrandOwnerId = identity.UserId;
                order.BrandOwner = await _context.UserCl.FindAsync(identity.UserId);
            }
            else
            {
                return Forbid();
            }

            //Затем проверяем права на смену статуса
            int userRole = funcs.identityToUser(User.Identity, _context).Role;
            int futureStatusId = order.StatusId + 1;
            OrderStatusCl futureOrderStatusCl = await _context.OrderStatusCl.FindAsync(futureStatusId);

            //Изменить статус могут лишь указанная роль или суперАдмин
            if (userRole == futureOrderStatusCl.MasterRoleId ||
                userRole == _context.UserRolesCl.First(e => e.UserRoleName == "SuperAdmin").UserRoleId)
            {
                order.StatusId++;
                order.OrderStatus = futureOrderStatusCl;
            }
            else
            {
                return Unauthorized();
            }

            await _context.SaveChangesAsync();

            NotificationsController notificationsController = new NotificationsController();
            await notificationsController.ToSendNotificationAsync(order.User.DeviceType, "Ваш заказ приняли", order.User.NotificationRegistration);

            return Ok();
        }
    }
}
