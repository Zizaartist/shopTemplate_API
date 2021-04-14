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
using ApiClick.Controllers.FrequentlyUsed;
using Microsoft.Extensions.Logging;
using ApiClick.Models.EnumModels;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ClickContext _context;
        private readonly ILogger<ReviewsController> _logger;
        
        public ReviewsController(ClickContext _context, ILogger<ReviewsController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        /// <summary>
        /// Создает отзыв к заказу
        /// </summary>
        /// <param name="_review">Данные отзыва</param>
        // POST: api/Reviews
        [Authorize]
        [HttpPost]
        public ActionResult<Review> PostReviews(Review _review)
        {
            if (!IsPostModelValid(_review)) 
            {
                return BadRequest();
            }

            var order = _context.Order.Find(_review.OrderId);

            if (order == null) 
            {
                return BadRequest("Ошибка при получении данных о заказе");
            }

            if (order.OrderStatus < OrderStatus.completed) 
            {
                return Forbid();
            }

            var user = Functions.identityToUser(User.Identity, _context);
            _review.SenderId = user.UserId;

            //Проверяем существование отзыва с таким же orderId
            if (_context.Review.Any(e => e.OrderId == _review.OrderId))
            {
                return Forbid();
            }

            _review.CreatedDate = DateTime.UtcNow;
            _context.Review.Add(_review); 
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Валидация получаемых данных
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsPostModelValid(Review _review)
        {
            try
            {
                if (_review == null ||
                    _review.Rating > 5 ||
                    _review.Rating < 0 ||
                    _review.OrderId <= 0 ||
                    !DoesOrderIdExist(_review.OrderId ?? default))
                {
                    return false;
                }

                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации Review модели POST метода - {_ex}");
                return false;
            }
        }

        private bool DoesOrderIdExist(int _orderId)
        {
            if (_context.Order.Find(_orderId) == null) 
            {
                return false;
            }
            return true;
        }
    }
}
