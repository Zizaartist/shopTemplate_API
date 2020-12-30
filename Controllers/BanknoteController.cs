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
        private ClickContext _context;
        
        public BanknoteController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/Category
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Banknote>>> GetBanknotes()
        {
            return await _context.Banknotes.ToListAsync();
        }
    }
}
