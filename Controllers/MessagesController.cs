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
using System.Security.Principal;

namespace ApiClick.Controllers
{
    [ApiController]
    public class MessagesController : ControllerBase
    {
        ClickContext _context;
        Functions funcs = new Functions();
        private static int pageSize = 10;
        
        public MessagesController(ClickContext _context)
        {
            this._context = _context;
        }

        //// POST: api/Messages
        ///// <summary>
        ///// Отправляет id сообщения, возвращает успешность операции
        ///// </summary>
        //[Route("api/LikeMessage")]
        //[Authorize]
        //[HttpPost("{id}")]
        //public async Task<ActionResult<Message>> LikeMessage(int id)
        //{

        //    bool opinionValue = true;
        //    int userId = funcs.identityToUser(User.Identity, _context).UserId;
        //    var opinion = _context.MessageOpinions.FirstOrDefault(u => u.UserId == userId && u.MessageId == id);

        //    if (opinion != null)
        //    {
        //        if (opinion.Opinion == opinionValue)
        //        {
        //            return BadRequest(); //wrong code
        //        }
        //        else //change dislike to like
        //        {
        //            var message = _context.Messages.Find(id);
        //            _context.Entry(opinion).State = EntityState.Modified;
        //            _context.Entry(message).State = EntityState.Modified;
        //            opinion.Opinion = !opinionValue;
        //            message.Likes++;
        //            message.Dislikes--;

        //            try
        //            {
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!MessagesExists(id))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }

        //            return Ok();
        //        }
        //    }

        //    //if opinion is not present - create one
        //    opinion = new MessageOpinion()
        //    {
        //        MessageId = id,
        //        UserId = userId,
        //        Opinion = opinionValue
        //    };
        //    _context.MessageOpinions.Add(opinion);
        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}

        //// POST: api/Messages
        //[Route("api/DislikeMessage")]
        //[Authorize]
        //[HttpPost("{id}")]
        //public async Task<ActionResult<Message>> DislikeMessage(int id)
        //{
        //    bool opinionValue = false;
        //    int userId = funcs.identityToUser(User.Identity, _context).UserId;
        //    var opinion = _context.MessageOpinions.FirstOrDefault(u => u.UserId == userId && u.MessageId == id);

        //    if (opinion != null)
        //    {
        //        if (opinion.Opinion == opinionValue)
        //        {
        //            return BadRequest(); //wrong code
        //        }
        //        else //changing like to a dislike
        //        {
        //            var message = _context.Messages.Find(id);
        //            _context.Entry(opinion).State = EntityState.Modified;
        //            _context.Entry(message).State = EntityState.Modified;
        //            opinion.Opinion = !opinionValue;
        //            message.Likes--;
        //            message.Dislikes++;

        //            try
        //            {
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!MessagesExists(id))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }

        //            return Ok();
        //        }
        //    }

        //    //if opinion is not present - create one
        //    opinion = new MessageOpinion()
        //    {
        //        MessageId = id,
        //        UserId = userId,
        //        Opinion = opinionValue
        //    };
        //    _context.MessageOpinions.Add(opinion);
        //    await _context.SaveChangesAsync();

        //    return Ok();
        //}

        // GET: api/Messages

        [Route("api/[controller]")]
        [Authorize(Roles = "SupeAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        // GET: api/Messages/5
        [Route("api/[controller]")]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessages(int id)
        {
            var messageCl = await _context.Messages.FindAsync(id);

            if (messageCl == null)
            {
                return NotFound();
            }

            return messageCl;
        }


        // GET: api/BrandReviews/5
        [Route("api/BrandReviews/{id}/{_page}")]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetBrandReviews(int id, int _page)
        {
            var messages = _context.Messages.Where(e => e.BrandId == id && !string.IsNullOrEmpty(e.Text));

            messages = funcs.GetPageRange(messages, _page, pageSize);

            if (!messages.Any()) 
            {
                return NotFound();
            }

            return await messages.ToListAsync();
        }

        // POST: api/Messages
        [Route("api/[controller]")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Message>> PostMessages(Message message)
        {
            if (message == null || message.Rating > 5 || message.Rating < 1) 
            {
                return BadRequest();
            }

            var order = _context.Orders.Find(message.OrderId);

            if (order == null) 
            {
                return BadRequest("Ошибка при получении данных о заказе");
            }

            try
            {
                message.BrandId = _context.Brands.FirstOrDefault(e => e.UserId == (order.BrandOwnerId ?? default)).BrandId;
            }
            catch 
            {
                return BadRequest("Ошибка при получении данных о бренде");
            }

            var user = funcs.identityToUser(User.Identity, _context);
            message.UserId = user.UserId;

            //Проверяем существование отзыва с таким же orderId
            if (_context.Messages.Any(e => e.OrderId == message.OrderId))
            {
                return Forbid();
            }

            var oldReviewCount = _context.Messages.Where(e => e.BrandId == message.BrandId).Count();
            message.CreatedDate = DateTime.Now;
            _context.Messages.Add(message); //Выдаст 500 если обязательные поля не заполнены

            await _context.SaveChangesAsync();

            //Изменяем рейтинг бренда
            var brand = _context.Brands.Find(message.BrandId);
            //formula https://stackoverflow.com/a/32631668, в разы лучше чем суммировать итерацией IMO
            brand.Rating = (((brand.Rating ?? 0f) * oldReviewCount) + (float)message.Rating) / (float)(oldReviewCount + 1);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <returns>Количество текстовых, количество всех</returns>
        // GET: api/BrandReviewCount/3
        [Route("api/BrandReviewCount/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<(int, int)>> GetReviewCount(int id)
        {
            var allReviews = _context.Messages.Where(e => e.BrandId == id);
            return (allReviews.Count(), allReviews.Where(e => string.IsNullOrEmpty(e.Text)).Count());
        }
    }
}
