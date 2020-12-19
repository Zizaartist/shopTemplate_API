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
using ApiClick.Models.EnumModels;

namespace ApiClick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        ClickContext _context;

        public PaymentMethodsController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/Category
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethodCl>>> GetCategoryCl()
        {
            return await _context.PaymentMethodCl.ToListAsync();
        }
    }
}

