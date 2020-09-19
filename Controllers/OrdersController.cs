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
            }

            return orders;
        }

        // GET: api/GetMyTasks
        [Route("api/GetMyTasks")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersCl>>> GetMyTasks()
        {
            var orders = _context.OrdersCl.Where(e => e.BrandOwnerId == identityToUser(User.Identity).UserId).ToList();

            if (orders == null)
            {
                return NotFound();
            }

            foreach (OrdersCl order in orders)
            {
                List<OrderDetailCl> relatedOrderDetails = _context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList();
                order.OrderDetails = relatedOrderDetails;
            }

            return orders;
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

            List<OrderDetailCl> locallyStoredValues = ordersCl.OrderDetails.ToList();
            //upload details to DB
            foreach (OrderDetailCl orderDetail in locallyStoredValues) 
            {   
                _context.OrderDetailCl.Add(new OrderDetailCl() 
                {
                    OrderId = orderDetail.OrderId,
                    ProductId = orderDetail.ProductId,
                    Price = (await _context.ProductCl.FindAsync(orderDetail.ProductId)).Price
                });
                await _context.SaveChangesAsync();
            }

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
