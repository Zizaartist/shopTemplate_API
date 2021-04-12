using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiClick.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HashtagsController : ControllerBase
    {
        private readonly ClickContext _context;

        public HashtagsController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/Hashtags/0
        [Route("{category}")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Hashtag>> GetHashtagsByKind(Kind category)
        {
            return _context.Hashtag.Where(e => e.Kind == category).ToList();
        }

        //// GET: api/HashtagsTooltip
        ////Возвращает хэштеги в виде tooltip в зависимости от написанного фрагмента
        //[Route("HashtagsTooltip")]
        //[Authorize(Roles = "SuperAdmin, Admin")]
        //[HttpGet]
        //public async Task<ActionResult<List<Hashtag>>> GetTooltipHashtags(string hashtagText)
        //{
        //    var inputCaps = hashtagText.ToUpper();
        //    return await _context.Hashtags.Where(e => e.HashTagName.ToUpper().Contains(inputCaps)).ToListAsync();
        //}
    }
}
