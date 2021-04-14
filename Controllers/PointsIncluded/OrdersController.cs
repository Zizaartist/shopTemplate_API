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
using ApiClick.Controllers.ScheduledTasks;
using ApiClick.Controllers.FrequentlyUsed;
using Microsoft.Extensions.Logging;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ClickContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ClickContext _context, ILogger<OrdersController> _logger)
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
            var mySelf = User.Identity.Name;

            var ordersFound = _context.Order.Include(order => order.OrderInfo)
                                            .Include(order => order.PointRegisters)
                                            .Include(order => order.OrderDetails)
                                            .Where(order => order.Orderer == mySelf);

            if (!ordersFound.Any())
            {
                return NotFound();
            }

            var result = ordersFound.ToList();

            //Расчет суммы каждого заказа
            foreach (var order in result)
            {
                order.Sum = order.OrderDetails.Sum(detail => detail.Count * detail.Price) +
                            (order.DeliveryPrice ?? 0) -
                            (order.PointRegister?.Points ?? 0);
            }

            return result;
        }

        /// <summary>
        /// Получение бонусов от завершенного заказа
        /// </summary>
        /// <param name="id">Id заказа</param>
        // PUT: api/Orders/ClaimPoints/{id}
        [Route("ClaimPoints/{id}")]
        [Authorize]
        [HttpPut]
        public ActionResult PutOrders(int id)
        {
            var mySelf = User.Identity.Name;

            //Сперва проверяем на физическую возможность смены статуса
            var order = _context.Order.Include(order => order.PointRegisters)
                                        .FirstOrDefault(order => order.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            //Заказ должен быть строго на предпоследнем статусе и отправитель запроса должен быть orderer
            if (order.OrderStatus != OrderStatus.delivered || order.Orderer == mySelf)
            {
                return Forbid();
            }

            PointsController pointsController = new PointsController(_context);

            if (order.PointsUsed ?? false)
            {
                if (!pointsController.CompleteTransaction(order.PointRegister))
                {
                    return BadRequest("Не удалось завершить транзакцию");
                }
            }

            //Переводим кэшбэк
            PointRegister cashbackRegister;
            //Если любой из процессов кэшбэка даст сбой
            if (!pointsController.StartTransaction(pointsController.CalculateCashback(order), mySelf, order, out cashbackRegister) ||
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
        public ActionResult PostNormalOrder(Order _order)
        {
            if (!IsNormalOrderValid(_order))
            {
                return BadRequest();
            }

            var brand = _context.Brand.Include(brand => brand.Executor)
                                            .ThenInclude(exe => exe.User)
                                        .First(brand => brand.BrandId == _order.BrandId);
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
            _order.OrdererId = mySelf.UserId;
            _order.OrderInfo.Phone = mySelf.Phone;

            var orderSum = _order.OrderDetails.Sum(e => CalcSumPrice(e.ProductId, e.Count));

            _order.DeliveryPrice = 0m;
            if (_order.Delivery ?? false)
            {
                //Если стоимость заказа не преодолела показатель минимальной цены - присвоить указанную стоимость доставки
                if (orderSum < brand.MinimalPrice)
                {
                    _order.DeliveryPrice = brand.DeliveryPrice;
                }
            }

            if (mySelf.Points <= 0) 
            {
                _order.PointsUsed = false;
            }

            if (_order.PointsUsed ?? false)
            {
                PointsController pointsController = new PointsController(_context);
                PointRegister register;
                if (!pointsController.StartTransaction(pointsController.GetMaxPayment(mySelf.Points, _order, brand.PointsPercentage), brand.Executor.UserId, _order, out register, mySelf))
                {
                    return BadRequest();
                }
            }

            _context.Order.Add(_order);
            _context.SaveChanges();

            if (mySelf.NotificationsEnabled)
            {
                Task.Run(() => new NotificationsController().ToSendNotificationAsync(brand.Executor.User.DeviceType, "У вас новый заказ!", brand.Executor.User.NotificationRegistration));
            }

            return Ok();
        }

        /// <summary>
        /// Добавляет новый водный заказ
        /// </summary>
        /// <param name="_order">Данные заказа</param>
        // POST: api/Orders/PostWaterOrder
        [Route("PostWaterOrder")]
        [Authorize]
        [HttpPost]
        public ActionResult PostWaterOrder(Order _order)
        {
            if (!IsWaterOrderValid(_order))
            {
                return BadRequest();
            }

            var mySelf = Functions.identityToUser(User.Identity, _context);

            //filling blanks and sending to DB
            _order.CreatedDate = DateTime.UtcNow;
            _order.OrderStatus = OrderStatus.sent;
            _order.OrdererId = mySelf.UserId;
            _order.OrderInfo.Phone = mySelf.Phone;

            if (!_order.WaterOrder.Express)
            {
                var brand = _context.WaterBrand.FirstOrDefault(brand => brand.BrandId == _order.BrandId);
                _order.WaterOrder.Price = brand.WaterPrice;
            }

            _context.Order.Add(_order);
            _context.SaveChanges();

            if (_order.WaterOrder.Express)
            {
                WaterOrderRemover.Add(_order.CreatedDate, _order.OrderId); //Удалит заказ через 2 часа если job не будет удален до триггера
            }

            return Ok();
        }

        /// <summary>
        /// Удаляет выбранный заказ
        /// </summary>
        // DELETE: api/Orders/5
        [Route("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public ActionResult<Order> DeleteOrders(int id)
        {
            var order = _context.Order.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Получение списка запросов на выбранный заказ
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <returns>Список запросов</returns>
        // POST: api/Orders/GetRequestsByOrder/2
        [Route("GetRequestsByOrder/{id}")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<WaterRequest>> GetRequestsByOrder(int id)
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);
            var order = _context.WaterOrder.AsNoTracking()
                                            .Include(wo => wo.Order)
                                            .FirstOrDefault(wo => wo.WaterOrderId == id);

            if (order == null || order.Order.OrdererId != mySelf.UserId)
            {
                return BadRequest();
            }

            var requests = _context.WaterRequest.Include(wr => wr.WaterBrand)
                                                    .ThenInclude(wb => wb.Brand)
                                                        .ThenInclude(brand => brand.BrandInfo)
                                                .Where(request => request.WaterOrderId == order.WaterOrderId);

            if (!requests.Any())
            {
                return NotFound();
            }

            var result = requests.ToList();

            return result;
        }

        /// <summary>
        /// Выбирает запрос и назначает бренд 
        /// </summary>
        /// <param name="id">Id запроса на прием заказа</param>
        /// <param name="pointsUsed">Были ли использованы баллы для оплаты</param>
        // POST: api/Orders/SelectWaterBrand/3
        [Route("SelectWaterBrand/{id}")]
        [Authorize]
        [HttpPut]
        public ActionResult SelectWaterBrand(int id)
        {
            var request = _context.WaterRequest.Include(wr => wr.WaterBrand)
                                                .FirstOrDefault(wr => wr.WaterRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            var mySelf = Functions.identityToUser(User.Identity, _context);
            var order = _context.WaterOrder.Include(wo => wo.Order)
                                            .FirstOrDefault(wo => wo.WaterOrderId == request.WaterOrderId);

            //Если клиент не является владельцем заказа - посылать подальше
            if (order.Order.OrdererId != mySelf.UserId)
            {
                return Forbid();
            }

            order.Order.BrandId = request.WaterBrand.BrandId;
            order.Order.OrderStatus = OrderStatus.received;

            //Находим и удаляем запросы связанные с заказом
            var allRelatedRequests = _context.WaterRequest.Where(wr => wr.WaterOrderId == order.WaterOrderId);
            _context.RemoveRange(allRelatedRequests);

            _context.SaveChanges();

            WaterOrderRemover.Remove(order.OrderId);

            var executor = _context.Brand.Include(brand => brand.Executor)
                                                .ThenInclude(exe => exe.User)
                                            .FirstOrDefault(brand => brand.BrandId == order.Order.BrandId)
                                            .Executor;

            if (executor.User.NotificationsEnabled)
            {
                Task.Run(() => new NotificationsController().ToSendNotificationAsync(executor.User.DeviceType, "Ваш запрос на доставку был принят!", executor.User.NotificationRegistration));
            }

            return Ok();
        }

        /// <summary>
        /// Создает запрос на исполнение заказа
        /// </summary>
        /// <param name="waterRequest">Неполная модель с указанием предлагаемой цены</param>
        /// <param name="id">Id заказа, на который претендует отправитель</param>
        // POST: api/Orders/PostWaterRequest
        [Route("PostWaterRequest")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public ActionResult PostWaterRequest(WaterRequest _waterRequestData)
        {
            if (!IsWaterRequestValid(_waterRequestData))
            {
                return BadRequest();
            }

            var order = _context.WaterOrder.Include(wo => wo.Order)
                                            .First(wo => wo.WaterOrderId == _waterRequestData.WaterOrderId);

            //Если заказ занят - запретить
            if (order.Order.BrandId != null)
            {
                return Forbid();
            }

            //получаем первый бренд отправителя
            var mySelf = Functions.identityToUser(User.Identity, _context).Executor;
            var myBrand = _context.Brand.Include(brand => brand.WaterBrand)
                                        .FirstOrDefault(brand => brand.ExecutorId == mySelf.ExecutorId);

            //... и проверяем наличие хотя бы одной записи с id этого бренда, подавляем попытку создать дубликат
            if (_context.WaterRequest.Where(wr => wr.WaterOrderId == order.WaterOrderId)
                                        .Any(e => e.WaterBrandId == myBrand.WaterBrand.WaterBrandId))
            {
                return Forbid();
            }

            var request = new WaterRequest()
            {
                WaterBrandId = myBrand.WaterBrand.WaterBrandId,
                WaterOrderId = order.WaterOrderId
            };

            _context.WaterRequest.Add(request);
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
        private bool IsNormalOrderValid(Order _order)
        {
            try
            {
                if (_order == null ||
                    (_order.Kind != Kind.food && _order.Kind != Kind.flowers) ||
                    _order.OrderInfo == null ||
                    _order.BrandId == null ||
                    _order.PointsUsed == null ||
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
                var brand = _context.Brand.Include(brand => brand.BrandPaymentMethods)
                                            .FirstOrDefault(brand => brand.BrandId == _order.BrandId);
                if (!brand.BrandPaymentMethods.Any(pm => pm.PaymentMethod == _order.PaymentMethod) ||
                    brand.Kind != _order.Kind)
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации Order модели POST метода - {_ex}");
                return false;
            }
        }

        /// <summary>
        /// Валидация получаемых данных метода POST
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsWaterOrderValid(Order _order)
        {
            try
            {
                if (_order == null ||
                    (_order.Kind != Kind.bottledWater && _order.Kind != Kind.water) ||
                    _order.OrderInfo == null ||
                    string.IsNullOrEmpty(_order.OrderInfo.Street) ||
                    string.IsNullOrEmpty(_order.OrderInfo.House) ||
                    _order.WaterOrder == null ||
                    _order.WaterOrder.Amount <= 0 ||
                    (_order.Kind == Kind.bottledWater && _order.WaterOrder.Container == null))
                {
                    return false;
                }
                if (!_order.WaterOrder.Express) 
                {
                    if (_order.BrandId == null) 
                    {
                        return false;
                    }
                    var brand = _context.Brand.Include(brand => brand.BrandPaymentMethods)
                                                .FirstOrDefault(brand => brand.BrandId == _order.BrandId);
                    if (!brand.BrandPaymentMethods.Any(pm => pm.PaymentMethod == _order.PaymentMethod) ||
                        brand.Kind != _order.Kind)
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

        private bool IsWaterRequestValid(WaterRequest _waterRequest)
        {
            try
            {
                if (_waterRequest == null ||
                    _waterRequest.WaterOrderId <= 0)
                {
                    return false;
                }
                var waterOrder = _context.WaterOrder.Find(_waterRequest.WaterOrderId);
                if (waterOrder == null) 
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации WaterRequest модели POST метода - {_ex}");
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
