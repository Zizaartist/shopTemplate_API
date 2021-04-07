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

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ClickContext _context;
        private readonly ILogger<ReviewsController> _logger;
        private static int PAGE_SIZE = 2;
        
        public ReviewsController(ClickContext _context, ILogger<ReviewsController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        [Authorize(Roles = "SupeAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            return await _context.Reviews.ToListAsync();
        }

        // GET: api/Reviews/5
        [Route("{id}")]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<Review>> GetReviews(int id)
        {
            var messageCl = await _context.Reviews.FindAsync(id);

            if (messageCl == null)
            {
                return NotFound();
            }

            return messageCl;
        }


        // GET: api/BrandReviews/5
        [Route("BrandReviews/{id}/{_page}")]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetBrandReviews(int id, int _page)
        {
            var messages = _context.Reviews.Where(e => e.BrandId == id && !string.IsNullOrEmpty(e.Text));

            messages = Functions.GetPageRange(messages, _page, PAGE_SIZE);

            if (!messages.Any()) 
            {
                return NotFound();
            }

            var result = await messages.ToListAsync();
            foreach (var message in result) 
            {
                //Только первые 3 в каждом review 
                message.Products = message.Order.OrderDetails.Select(detail => detail.Product.ProductName).Take(3).ToList();
            }

            return result;
        }

        // POST: api/Reviews
        [Authorize]
        [HttpPost]
        public ActionResult<Review> PostReviews(Review _review)
        {
            if (!IsPostModelValid(_review)) 
            {
                return BadRequest();
            }

            var order = _context.Orders.Find(_review.OrderId);

            if (order == null) 
            {
                return BadRequest("Ошибка при получении данных о заказе");
            }

            if (order.BrandId == null) 
            {
                return BadRequest("Ошибка при получении данных бренда");
            }

            var brandId = order.BrandId ?? -1;
            _review.BrandId = brandId;

            var user = Functions.identityToUser(User.Identity, _context);
            _review.SenderId = user.UserId;

            //Проверяем существование отзыва с таким же orderId
            if (_context.Reviews.Any(e => e.OrderId == _review.OrderId))
            {
                return Forbid();
            }

            var oldReviewCount = _context.Reviews.Where(e => e.BrandId == _review.BrandId).Count();
            _review.CreatedDate = DateTime.UtcNow;
            _context.Reviews.Add(_review); //Выдаст 500 если обязательные поля не заполнены

            _context.SaveChanges();

            //Изменяем рейтинг бренда
            var brand = _context.Brands.Find(_review.BrandId);
            //formula https://stackoverflow.com/a/32631668, в разы лучше чем суммировать итерацией IMO
            brand.Rating = (((brand.Rating ?? 0f) * oldReviewCount) + (float)_review.Rating) / (float)(oldReviewCount + 1);
            brand.ReviewCount++;
            _context.SaveChanges();

            return Ok();
        }

        /// <returns>Количество текстовых, количество всех</returns>
        // GET: api/BrandReviewCount/3
        [Route("BrandReviewCount/{id}")]
        [Authorize]
        [HttpGet]
        public ActionResult<(int, int)> GetReviewCount(int id)
        {
            var allReviews = _context.Reviews.Where(e => e.BrandId == id);
            return (allReviews.Count(), allReviews.Where(e => !string.IsNullOrEmpty(e.Text)).Count());
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
                    !DoesOrderIdExist(_review.OrderId))
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
            if (_context.Orders.Find(_orderId) == null) 
            {
                return false;
            }
            return true;
        }
    }
}
