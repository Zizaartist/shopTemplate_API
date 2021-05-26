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
using Microsoft.EntityFrameworkCore.Internal;
using ApiClick.Models.EnumModels;
using ApiClick.StaticValues;
using System.ComponentModel;
using ApiClick.Controllers.FrequentlyUsed;
using Microsoft.Extensions.Logging;
using ShopAdminAPI.Configurations;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ShopContext _context, ILogger<OrdersController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        /// <summary>
        /// Возвращает заказы клиента
        /// </summary>
        // GET: api/Orders/GetMyOrders/3
        [Route("GetMyOrders/{_page}")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetMyOrders(int _page)
        {
            IQueryable<Order> ordersFound = _context.Order.Include(order => order.OrderInfo)
                                            .Where(order => order.UserId == Functions.identityToUser(User.Identity, _context, false).UserId)
                                            .OrderBy(order => order.OrderStatus == OrderStatus.delivered) //Сперва false, потом true
                                                .ThenByDescending(order => order.CreatedDate); //Сперва true, потом false

            ordersFound = Functions.GetPageRange(ordersFound, _page, PageLengths.ORDER_LENGTH);

            if (!ordersFound.Any())
            {
                return NotFound();
            }

            var result = ordersFound.ToList();

            return result;
        }

        /// <summary>
        /// Возвращает детали заказа
        /// </summary>
        /// <param name="id">Id заказа</param>
        // GET: api/Orders/2
        [Route("{id}")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<OrderDetail>> GetDetails(int id) 
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);

            var order = _context.Order.Include(order => order.OrderDetails)
                                            .ThenInclude(detail => detail.Product)
                                        .FirstOrDefault(order => order.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }
            else if (order.UserId != mySelf.UserId) 
            {
                return Forbid();
            }

            return order.OrderDetails.ToList();
        }

        /// <summary>
        /// Добавляет новый обычный заказ
        /// </summary>
        /// <param name="_order">Данные нового заказа</param>
        // POST: api/Orders
        [Authorize]
        [HttpPost]
        public ActionResult Post(Order _order)
        {
            if (!IsOrderValid(_order))
            {
                return BadRequest();
            }
            _order.OrderStatus = OrderStatus.sent;

            var mySelf = Functions.identityToUser(User.Identity, _context, true);

            _order.OrderDetails = _order.OrderDetails.Select(detail =>
                new OrderDetail()
                {
                    ProductId = detail.ProductId,
                    Count = detail.Count
                }
            ).ToList();

            foreach (var detail in _order.OrderDetails)
            {
                var product = _context.Product.Find(detail.ProductId);
                detail.Price = product.Price;
                detail.Discount = product.Discount;

                //Если на складе недостаточно товара - отменить заказ, иначе - уменьшить счетчик
                if (detail.Count > product.InStorage)
                {
                    return NotFound(); //Недостаточно товара!
                }
                else
                {
                    product.InStorage -= detail.Count;
                }
            }

            _order.CreatedDate = DateTime.UtcNow;
            _order.PaymentMethod = PaymentMethod.card;
            _order.UserId = mySelf.UserId;
            _order.OrderInfo.Phone = mySelf.Phone;

            PointsController pointsController = new PointsController(_context);

            var orderSum = pointsController.GetDetailsSum(_order.OrderDetails);

            _order.DeliveryPrice = null;
            if (_order.Delivery.Value)
            {
                //Если стоимость заказа не преодолела показатель минимальной цены - присвоить указанную стоимость доставки
                if (orderSum < ShopConfiguration.MinimalDeliveryPrice)
                {
                    _order.DeliveryPrice = ShopConfiguration.DeliveryPrice;
                }
                else
                {
                    _order.DeliveryPrice = 0m;
                }
            }

            if (mySelf.Points <= 0) 
            {
                _order.PointsUsed = false;
            }

            if (_order.PointsUsed)
            {
                PointRegister register;
                if (!pointsController.StartTransaction(pointsController.GetMaxPayment(mySelf.Points, orderSum), mySelf, true, _order, out register))
                {
                    return BadRequest();
                }
            }

            _order.Sum = pointsController.CalculatePointless(_order) + (_order.DeliveryPrice ?? 0);

            _context.Order.Add(_order);
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Валидация получаемых данных метода POST
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsOrderValid(Order _order)
        {
            try
            {
                if (_order == null ||
                    _order.OrderInfo == null ||
                    string.IsNullOrEmpty(_order.OrderInfo.OrdererName) ||
                    !_order.OrderDetails.Any() ||
                    !AreOrderDetailsValid(_order.OrderDetails))
                {
                    return false;
                }
                if (_order.Delivery.Value)
                {
                    if (string.IsNullOrEmpty(_order.OrderInfo.Street) ||
                        string.IsNullOrEmpty(_order.OrderInfo.House))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации Order модели POST метода - {_ex}");
                return false;
            }
        }

        private bool AreOrderDetailsValid(IEnumerable<OrderDetail> _orderDetails)
        {
            foreach (var detail in _orderDetails)
            {
                if (detail.Count < 1 ||
                    detail.ProductId <= 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
