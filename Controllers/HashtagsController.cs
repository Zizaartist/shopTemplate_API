using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiClick.Models;
using Click.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiClick.Controllers
{
    [ApiController]
    public class HashtagsController : ControllerBase
    {
        ClickContext _context;
        
        public HashtagsController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/BrandsMenu
        //Debug
        [Route("api/GetHashtagsByCategory/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<HashTagCl>>> GetHashtagsByCategory(int id)
        {
            return _context.HashtagCl.Where(e => e.CategoryId == id).ToList();
        }
    }
}
