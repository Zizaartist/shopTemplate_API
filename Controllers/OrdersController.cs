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

namespace ApiClick.Controllers
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        ClickContext _context = new ClickContext();

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
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<OrdersCl>> GetOrdersCl(int id)
        {
            var ordersCl = await _context.OrdersCl.FindAsync(id);

            if (ordersCl == null)
            {
                return NotFound();
            }

            if (ordersCl.BrandOwnerId != identityToUser(User.Identity).UserId) 
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
            var orders = _context.OrdersCl.Where(e => e.UserId == identityToUser(User.Identity).UserId &&
                                                        e.StatusId != _context.OrderStatusCl.First(e => e.OrderStatusName == "Завершено").OrderStatusId).ToList();

            if (orders == null)
            {
                return NotFound();
            }

            foreach (OrdersCl order in orders)
            {
                List<OrderDetailCl> relatedOrderDetails = _context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList();
                order.OrderDetails = relatedOrderDetails;
                foreach (OrderDetailCl detail in order.OrderDetails) 
                {
                    detail.Product = await _context.ProductCl.FindAsync(detail.ProductId);
                }
                var ownerBuffer = await _context.UserCl.FindAsync(order.BrandOwnerId);
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //Получаем список брендов которые ему принадлежат, а должен быть один единственный
                if (ownerBuffer != null)
                {
                    ICollection<BrandCl> brands = new List<BrandCl>();
                    brands.Add(_context.BrandCl.Where(e => e.UserId == ownerBuffer.UserId).First());
                    ownerBuffer = new UserCl()
                    {
                        Brands = brands
                    };
                    order.BrandOwner = ownerBuffer;
                    order.BrandOwner.Brands.First().ImgLogo = await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgLogoId);
                    order.BrandOwner.Brands.First().ImgBanner = await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgBannerId);
                    foreach (BrandCl brand in order.BrandOwner.Brands)
                    {
                        brand.User = null;
                        brand.ImgLogo.User = null;
                        brand.ImgBanner.User = null;
                    }
                    
                    foreach (OrderDetailCl detail in order.OrderDetails)
                    {
                        if (detail.Product != null)
                        {
                            detail.Product.Image = await _context.ImageCl.FindAsync(detail.Product.ImgId);
                            detail.Product.Image.User = null;
                        }
                    }
                }
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
               
                order.OrderStatus = await _context.OrderStatusCl.FindAsync(order.StatusId);
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                order.User = null;
                

                
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
            var orders = _context.OrdersCl.Where(e => e.BrandOwnerId == identityToUser(User.Identity).UserId)
                                          .Where(e => e.OrderStatus.OrderStatusId != _context.OrderStatusCl.First(e => e.OrderStatusName == "Завершено").OrderStatusId).ToList();

            if (orders == null)
            {
                return NotFound();
            }

            foreach (OrdersCl order in orders)
            {
                List<OrderDetailCl> relatedOrderDetails = _context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList();
                order.OrderDetails = relatedOrderDetails;
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    detail.Product = await _context.ProductCl.FindAsync(detail.ProductId);
                }
                order.BrandOwner = await _context.UserCl.FindAsync(order.BrandOwnerId);
                order.BrandOwner.Brands = _context.BrandCl.Where(e => e.UserId == order.BrandOwnerId).ToList();
                order.BrandOwner.Brands.First().ImgLogo = await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgLogoId);
                order.BrandOwner.Brands.First().ImgBanner = await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgBannerId);
                order.OrderStatus = await _context.OrderStatusCl.FindAsync(order.StatusId);
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                order.User = null;
                foreach (BrandCl brand in order.BrandOwner.Brands)
                {
                    brand.User = null;
                    brand.ImgLogo.User = null;
                    brand.ImgBanner.User = null;
                }
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    detail.Product.Image = await _context.ImageCl.FindAsync(detail.Product.ImgId);
                    detail.Product.Image.User = null;
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
            var orders = _context.OrdersCl.Where(e => e.BrandOwnerId == identityToUser(User.Identity).UserId)
                                          .Where(e => e.OrderStatus.OrderStatusId == _context.OrderStatusCl.First(e => e.OrderStatusName == "Завершено").OrderStatusId).ToList();

            if (orders == null)
            {
                return NotFound();
            }

            foreach (OrdersCl order in orders)
            {
                List<OrderDetailCl> relatedOrderDetails = _context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList();
                order.OrderDetails = relatedOrderDetails;
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    detail.Product = await _context.ProductCl.FindAsync(detail.ProductId);
                }
                order.BrandOwner = await _context.UserCl.FindAsync(order.BrandOwnerId);
                order.BrandOwner.Brands = _context.BrandCl.Where(e => e.UserId == order.BrandOwnerId).ToList();
                order.BrandOwner.Brands.First().ImgLogo = await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgLogoId);
                order.BrandOwner.Brands.First().ImgBanner = await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgBannerId);
                order.OrderStatus = await _context.OrderStatusCl.FindAsync(order.StatusId);
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                order.User = null;
                foreach (BrandCl brand in order.BrandOwner.Brands)
                {
                    brand.User = null;
                    brand.ImgLogo.User = null;
                    brand.ImgBanner.User = null;
                }
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    detail.Product.Image = await _context.ImageCl.FindAsync(detail.Product.ImgId);
                    detail.Product.Image.User = null;
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
            var identity = identityToUser(User.Identity);
            if (!(order.UserId == identity.UserId || order.BrandOwnerId == identity.UserId)) 
            {
                return Forbid();
            }

            //Затем проверяем права на смену статуса
            int userRole = identityToUser(User.Identity).Role;
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
            ordersCl.UserId = identityToUser(User.Identity).UserId;
            ordersCl.User = await _context.UserCl.FindAsync(ordersCl.UserId);
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

            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!

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
        public async Task<ActionResult<List<OrdersCl>>> GetBrandsByCategory(int id)
        {
            var identity = identityToUser(User.Identity);
            var orders = await _context.OrdersCl.Where(p => p.CategoryId == id && (p.BrandOwnerId == null || p.BrandOwnerId == identity.UserId)).ToListAsync();
            foreach (var item in orders)
            {
                item.OrderStatus = _context.OrderStatusCl.Find(item.StatusId);
            }

            if (orders == null)
            {
                return NotFound();
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
            ordersCl.UserId = identityToUser(User.Identity).UserId;
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
            var identity = identityToUser(User.Identity);
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
            int userRole = identityToUser(User.Identity).Role;
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
