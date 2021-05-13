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
                                            .Include(order => order.PointRegisters)
                                            .Include(order => order.OrderDetails)
                                            .Where(order => order.UserId == Functions.identityToUser(User.Identity, _context, false).UserId)
                                            .OrderBy(order => order.OrderStatus == OrderStatus.delivered) //Сперва false, потом true
                                                .ThenByDescending(order => order.CreatedDate); //Сперва true, потом false

            ordersFound = Functions.GetPageRange(ordersFound, _page, PageLengths.ORDER_LENGTH);

            if (!ordersFound.Any())
            {
                return NotFound();
            }

            var result = ordersFound.ToList();

            foreach (var order in result)
            {
                order.Sum = order.OrderDetails.Sum(detail => detail.Count * detail.Price) +
                            (order.DeliveryPrice ?? 0) -
                            (order.PointRegister?.Points ?? 0);
                order.OrderDetails = null;
            }

            return result;
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

                //Если на складе недостаточно товара - отменить заказ, иначе - уменьшить счетчик
                if (detail.Count > product.InStorage)
                {
                    return BadRequest("Недостаточно товара!");
                }
                else
                {
                    product.InStorage -= detail.Count;
                }
            }

            _order.CreatedDate = DateTime.UtcNow;
            _order.OrderStatus = OrderStatus.sent;
            _order.UserId = mySelf.UserId;
            _order.OrderInfo.Phone = mySelf.Phone;

            var orderSum = _order.OrderDetails.Sum(e => CalcSumPrice(e.ProductId, e.Count));

            _order.DeliveryPrice = null;
            if (_order.Delivery.Value)
            {
                //Если стоимость заказа не преодолела показатель минимальной цены - присвоить указанную стоимость доставки
                if (true)//(orderSum < Constants.MINIMAL_PRICE)
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
                PointsController pointsController = new PointsController(_context);
                PointRegister register;
                if (!pointsController.StartTransaction(pointsController.GetMaxPayment(mySelf.Points, _order), mySelf, true, _order, out register))
                {
                    return BadRequest();
                }
            }

            _context.Order.Add(_order);
            _context.SaveChanges();

            return Ok();
        }

        private decimal CalcSumPrice(int? _productId, int _count)
        {
            Product product;
            try
            {
                product = _context.Product.Find(_productId);
            }
            catch
            {
                throw new Exception("No product found");
            }

            if (_count <= 0)
            {
                throw new Exception("Unexpected value");
            }
            return product.Price * (decimal)_count;
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
