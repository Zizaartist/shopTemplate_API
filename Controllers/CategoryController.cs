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
    public class CategoryController : ControllerBase
    {
        ClickContext _context;
        
        public CategoryController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/Category
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryCl>>> GetCategoryCl()
        {
            return await _context.CategoryCl.ToListAsync();
        }
    }
}
