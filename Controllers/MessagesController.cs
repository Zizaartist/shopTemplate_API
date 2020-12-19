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
        
        public MessagesController(ClickContext _context)
        {
            this._context = _context;
        }

        // POST: api/Messages
        /// <summary>
        /// Отправляет id сообщения, возвращает успешность операции
        /// </summary>
        [Route("api/LikeMessage")]
        [Authorize]
        [HttpPost("{id}")]
        public async Task<ActionResult<MessageCl>> LikeMessage(int id)
        {

            bool opinionValue = true;
            int userId = funcs.identityToUser(User.Identity, _context).UserId;
            var opinion = _context.MessageOpinionCl.FirstOrDefault(u => u.UserId == userId && u.MessageId == id);

            if (opinion != null)
            {
                if (opinion.Opinion == opinionValue)
                {
                    return BadRequest(); //wrong code
                }
                else //change dislike to like
                {
                    var message = _context.MessageCl.Find(id);
                    _context.Entry(opinion).State = EntityState.Modified;
                    _context.Entry(message).State = EntityState.Modified;
                    opinion.Opinion = !opinionValue;
                    message.Likes++;
                    message.Dislikes--;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MessageClExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return Ok();
                }
            }

            //if opinion is not present - create one
            opinion = new MessageOpinionCl()
            {
                MessageId = id,
                UserId = userId,
                Opinion = opinionValue
            };
            _context.MessageOpinionCl.Add(opinion);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Messages
        [Route("api/DislikeMessage")]
        [Authorize]
        [HttpPost("{id}")]
        public async Task<ActionResult<MessageCl>> DislikeMessage(int id)
        {
            bool opinionValue = false;
            int userId = funcs.identityToUser(User.Identity, _context).UserId;
            var opinion = _context.MessageOpinionCl.FirstOrDefault(u => u.UserId == userId && u.MessageId == id);

            if (opinion != null)
            {
                if (opinion.Opinion == opinionValue)
                {
                    return BadRequest(); //wrong code
                }
                else //changing like to a dislike
                {
                    var message = _context.MessageCl.Find(id);
                    _context.Entry(opinion).State = EntityState.Modified;
                    _context.Entry(message).State = EntityState.Modified;
                    opinion.Opinion = !opinionValue;
                    message.Likes--;
                    message.Dislikes++;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MessageClExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return Ok();
                }
            }

            //if opinion is not present - create one
            opinion = new MessageOpinionCl()
            {
                MessageId = id,
                UserId = userId,
                Opinion = opinionValue
            };
            _context.MessageOpinionCl.Add(opinion);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET: api/Messages
        [Route("api/[controller]")]
        [Authorize(Roles = "SupeAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageCl>>> GetMessageCl()
        {
            return await _context.MessageCl.ToListAsync();
        }

        // GET: api/Messages/5
        [Route("api/[controller]")]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<MessageCl>> GetMessageCl(int id)
        {
            var messageCl = await _context.MessageCl.FindAsync(id);

            if (messageCl == null)
            {
                return NotFound();
            }

            return messageCl;
        }

        // PUT: api/Messages/5
        [Route("api/[controller]")]
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessageCl(int id, MessageCl messageCl)
        {
            if (id != messageCl.MessageId)
            {
                return BadRequest();
            }

            _context.Entry(messageCl).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageClExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Messages
        [Route("api/[controller]")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<MessageCl>> PostMessageCl(MessageCl messageCl)
        {
            if (messageCl == null) 
            {
                return BadRequest();
            }

            _context.MessageCl.Add(messageCl);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Messages/5
        [Route("api/[controller]")]
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<MessageCl>> DeleteMessageCl(int id)
        {
            var messageCl = await _context.MessageCl.FindAsync(id);
            if (messageCl == null)
            {
                return NotFound();
            }

            _context.MessageCl.Remove(messageCl);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool MessageClExists(int id)
        {
            return _context.MessageCl.Any(e => e.MessageId == id);
        }
    }
}
