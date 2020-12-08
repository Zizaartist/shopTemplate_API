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
    [Route("api/[controller]")]
    [ApiController]
    public class BanknoteController : ControllerBase
    {
        ClickContext _context = new ClickContext();

        // GET: api/Category
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BanknoteCl>>> GetBanknoteCl()
        {
            return await _context.BanknoteCl.ToListAsync();
        }
    }
}
