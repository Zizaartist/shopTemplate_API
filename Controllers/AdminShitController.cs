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
    }
}
