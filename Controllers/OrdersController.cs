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
            var orders = _context.OrdersCl.Where(e => e.UserId == identityToUser(User.Identity).UserId).ToList();

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
                ownerBuffer = new UserCl()
                {
                    Brands = _context.BrandCl.Where(e => e.UserId == ownerBuffer.UserId).ToList()
                };
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //FIX THIS SHIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                order.BrandOwner = ownerBuffer;
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

            //Затем проверяем права на смену статуса
            // if(order.OrderStatus.)
            int userRole = identityToUser(User.Identity).Role;
            int statusId = order.StatusId;
            statusId++;
            OrderStatusCl orderStatusCl = await _context.OrderStatusCl.FindAsync(statusId);

            if (orderStatusCl.MasterRoleId == userRole /*|| userRole == 3*/)
            {
                order.StatusId++;
                order.OrderStatus = orderStatusCl;
            }
            else
            {
                return Unauthorized();
            }
            await _context.SaveChangesAsync();

            return Ok();
            ////Получаем заказы, где владелец токена обозначен как владелец бренда
            ////+ статус должен быть завершенным
            //var orders = _context.OrdersCl.Where(e => e.BrandOwnerId == identityToUser(User.Identity).UserId)
            //                              .Where(e => e.OrderStatus.OrderStatusId == _context.OrderStatusCl.First(e => e.OrderStatusName == "Завершено").OrderStatusId).ToList();

            //if (orders == null)
            //{
            //    return NotFound();
            //}

            //foreach (OrdersCl order in orders)
            //{
            //    List<OrderDetailCl> relatedOrderDetails = _context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList();
            //    order.OrderDetails = relatedOrderDetails;
            //    order.BrandOwner = await _context.UserCl.FindAsync(order.BrandOwnerId);
            //    order.BrandOwner.Brands = _context.BrandCl.Where(e => e.UserId == order.BrandOwnerId).ToList();
            //    order.BrandOwner.Brands.First().ImgLogo = await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgLogoId);
            //    order.BrandOwner.Brands.First().ImgBanner = await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgBannerId);
            //    order.OrderStatus = await _context.OrderStatusCl.FindAsync(order.StatusId);
            //}

            //return orders;
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
            ordersCl.BrandOwnerId = responsibleBrandOwnerId.UserId;

            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!
            //TODO!!!!!!!!!!!!!!!!!

            _context.OrdersCl.Add(ordersCl);
            await _context.SaveChangesAsync(); //вроде как рефрешит объект ordersCl

            if (ordersCl.PointsUsed)
            {
                PointsController pointsController = new PointsController();
                pointsController.CreatePointRegister(ordersCl.User, ordersCl);
            }

            ordersCl.BrandOwner = await _context.UserCl.FindAsync(ordersCl.BrandOwnerId);
            NotificationsController notificationsController = new NotificationsController();
            await notificationsController.ToSendNotificationAsync(ordersCl.BrandOwner.DeviceType, "У Вас новый заказ", ordersCl.BrandOwner.NotificationRegistration);

            //List<OrderDetailCl> locallyStoredValues = ordersCl.OrderDetails.ToList();
            ////upload details to DB
            //foreach (OrderDetailCl orderDetail in locallyStoredValues) 
            //{   
            //    _context.OrderDetailCl.Add(new OrderDetailCl() 
            //    {
            //        OrderId = orderDetail.OrderId,
            //        ProductId = orderDetail.ProductId,
            //        Price = (await _context.ProductCl.FindAsync(orderDetail.ProductId)).Price,
            //        Count = orderDetail.Count
            //    });
            //    await _context.SaveChangesAsync();
            //}
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
    }
}
