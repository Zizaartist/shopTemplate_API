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
        // GET: api/Orders/GetMyOrders
        [Route("GetMyOrders")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetMyOrders()
        {
            var ordersFound = _context.Order.Include(order => order.OrderInfo)
                                            .Include(order => order.PointRegisters)
                                            .Include(order => order.OrderDetails)
                                            .Where(e => e.UserId == Functions.identityToUser(User.Identity, _context, false).UserId);

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
        /// Изменяет статус заказа
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <param name="_status">Опциональный статус, при отсутствии выбирается следующий по очереди</param>
        // PUT: api/Orders/ClaimPoints/4
        [Route("ClaimPoints/{id}")]
        [Authorize]
        [HttpPut]
        public ActionResult ClaimPoints(int id)
        {
            //Сперва проверяем на физическую возможность смены статуса
            var order = _context.Order.Include(order => order.PointRegisters)
                                        .FirstOrDefault(order => order.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            //Только на предпоследнем статусе клиенту дозволено сменить на последний
            if (order.OrderStatus != OrderStatus.delivered)
            {
                return Forbid();
            }

            var mySelf = Functions.identityToUser(User.Identity, _context, true);

            PointsController pointsController = new PointsController(_context);

            if (order.PointsUsed)
            {
                //Завершаем перевод баллов от клиента магазину
                if (!pointsController.CompleteTransaction(order.PointRegister))
                {
                    return BadRequest("Не удалось завершить транзакцию");
                }
            }

            //Переводим кэшбэк
            Order orderTemp = new Order() { OrderDetails = Functions.getCleanListOfModels(_context.OrderDetail.Where(e => e.OrderId == order.OrderId).ToList()) };
            PointRegister cashbackRegister;
            //Если любой из процессов кэшбэка даст сбой
            if (!pointsController.StartTransaction(pointsController.CalculateCashback(order), mySelf, false, order, out cashbackRegister) ||
                !pointsController.CompleteTransaction(cashbackRegister))
                {
                    return BadRequest("Не удалось произвести кэшбэк");
                }

            _context.SaveChanges();

            return Ok();
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

            try
            {
                _order.OrderDetails = _order.OrderDetails.Select(detail =>
                    new OrderDetail()
                    {
                        ProductId = detail.ProductId,
                        Count = detail.Count,
                        Price = _context.Product.Find(detail.ProductId).Price
                    }
                ).ToList();
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при получении продукта - {_ex}");
                return NotFound();
            }

            _order.CreatedDate = DateTime.UtcNow;
            _order.OrderStatus = OrderStatus.received;
            _order.UserId = mySelf.UserId;
            _order.OrderInfo.Phone = mySelf.Phone;

            var orderSum = _order.OrderDetails.Sum(e => CalcSumPrice(e.ProductId, e.Count));

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
                    _order.Delivery == null ||
                    string.IsNullOrEmpty(_order.OrderInfo.OrdererName) ||
                    !_order.OrderDetails.Any() ||
                    !AreOrderDetailsValid(_order.OrderDetails))
                {
                    return false;
                }
                if (_order.Delivery ?? false)
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
