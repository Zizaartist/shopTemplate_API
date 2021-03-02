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

        // GET: api/GetHashtagsByCategory/0
        [Route("api/GetHashtagsByCategory/{category}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<Hashtag>>> GetHashtagsByCategory(Category category)
        {
            return _context.Hashtags.Where(e => e.Category == category).ToList();
        }

        // GET: api/HashtagsTooltip
        //Возвращает хэштеги в виде tooltip в зависимости от написанного фрагмента
        [Route("api/HashtagsTooltip")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<Hashtag>>> GetTooltipHashtags(string hashtagText)
        {
            var inputCaps = hashtagText.ToUpper();
            return await _context.Hashtags.Where(e => e.HashTagName.ToUpper().Contains(inputCaps)).ToListAsync();
        }
    }
}
