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

namespace ApiClick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        ClickContext _context = new ClickContext();

        // GET: api/Orders
        [Authorize(Roles = "SupeAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersCl>>> GetOrdersCl()
        {
            return await _context.OrdersCl.ToListAsync();
        }

        // GET: api/Orders/5
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersCl>> GetOrdersCl(int id)
        {
            var ordersCl = await _context.OrdersCl.FindAsync(id);

            if (ordersCl == null)
            {
                return NotFound();
            }

            List<OrderDetailCl> relatedOrderDetails = _context.OrderDetailCl.Where(d => d.OrderId == ordersCl.OrdersId).ToList();
            ordersCl.orderDetails = relatedOrderDetails;

            return ordersCl;
        }

        // POST: api/Orders
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<OrdersCl>> PostOrdersCl(OrdersCl ordersCl)
        {
            if (ordersCl == null || ordersCl.orderDetails == null || ordersCl.User == null)
            {
                return BadRequest();
            }

            //filling blanks and sending to DB
            ordersCl.CreatedDate = DateTime.Now;
            ordersCl.Status = _context.UserRolesCls.First(r => r.UserRoleName == "SomeKindOfStatus").UserRolesId;
            ordersCl.User = _context.UserCl.First(u => u.Phone == ordersCl.User.Phone);
            ordersCl.UserId = ordersCl.User.UserId;

            _context.OrdersCl.Add(ordersCl);
            await _context.SaveChangesAsync(); //вроде как рефрешит объект ordersCl

            List<OrderDetailCl> locallyStoredValues = ordersCl.orderDetails.ToList();
            //upload details to DB
            foreach (OrderDetailCl orderDetail in locallyStoredValues) 
            {   
                _context.OrderDetailCl.Add(new OrderDetailCl() 
                {
                    OrderId = orderDetail.OrderId,
                    ProductId = orderDetail.ProductId,
                    price = _context.ProductCl.First(p => p.ProductId == orderDetail.ProductId).Price,
                    product = _context.ProductCl.FirstOrDefault(p => p.ProductId == orderDetail.ProductId),
                    order = orderDetail.order
                    //order = _context.OrdersCl.
                });
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetOrdersCl", new { id = ordersCl.OrdersId }, ordersCl);
        }

        // DELETE: api/Orders/5
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<OrdersCl>> DeleteOrdersCl(int id)
        {
            var ordersCl = await _context.OrdersCl.FindAsync(id);
            if (ordersCl == null)
            {
                return NotFound();
            }

            _context.OrdersCl.Remove(ordersCl);
            await _context.SaveChangesAsync();

            return ordersCl;
        }

        private bool OrdersClExists(int id)
        {
            return _context.OrdersCl.Any(e => e.OrdersId == id);
        }
    }
}
