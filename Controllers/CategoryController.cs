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
        ClickContext _context = new ClickContext();

        // GET: api/Category
        [Authorize(Roles = "SupeAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryCl>>> GetCategoryCl()
        {
            return await _context.CategoryCl.ToListAsync();
        }

        // GET: api/Category/5
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryCl>> GetCategoryCl(int id)
        {
            var categoryCl = await _context.CategoryCl.FindAsync(id);

            if (categoryCl == null)
            {
                return NotFound();
            }

            return categoryCl;
        }

        private bool CategoryClExists(int id)
        {
            return _context.CategoryCl.Any(e => e.CategoryId == id);
        }
    }
}
