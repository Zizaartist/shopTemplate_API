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

        // GET: api/Orders
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Order.ToListAsync();
        }

        // GET: api/Orders/5
        [Route("{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public ActionResult<Order> GetOrders(int id)
        {
            var order = _context.Order.Include(order => order.Brand)
                                        .FirstOrDefault(order => order.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var brand = _context.Brand.Include(brand => brand.Executor)
                                        .FirstOrDefault(brand => brand.BrandId == order.BrandId);

            //Является ли отправитель исполнителем?
            if (brand == null || order.Brand.ExecutorId != Functions.identityToUser(User.Identity, _context).Executor.ExecutorId)
            {
                return Forbid();
            }

            return order;
        }

        // GET: api/GetMyOrders
        [Route("GetMyOrders")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetMyOrders()
        {
            //Находит заказы принадлежащие пользователю и отсеивает заказы со статусом "Завершенный"
            var ordersFound = _context.Order.Where(e => e.OrdererId == Functions.identityToUser(User.Identity, _context).UserId);

            if (!ordersFound.Any())
            {
                return NotFound();
            }

            var result = ordersFound.ToList();

            return result;
        }

        // GET: api/GetMyTasks
        [Route("GetMyTasks")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetMyTasks()
        {
            var myBrand = Functions.identityToUser(User.Identity, _context).Executor.Brand;

            if (myBrand == null)
            {
                return NotFound();
            }

            var ordersFound = _context.Order.Where(order => order.BrandId == myBrand.BrandId &&
                                                            order.OrderStatus < OrderStatus.delivered);

            if (!ordersFound.Any())
            {
                return NotFound();
            }

            var result = ordersFound.ToList();

            return result;
        }

        // GET: api/GetMyHistory
        [Route("GetMyHistory")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetMyHistory()
        {
            var myBrand = Functions.identityToUser(User.Identity, _context).Executor.Brand;

            if (myBrand == null)
            {
                return NotFound();
            }

            var ordersFound = _context.Order.Where(order => order.BrandId == myBrand.BrandId &&
                                                            order.OrderStatus >= OrderStatus.delivered);

            if (!ordersFound.Any())
            {
                return NotFound();
            }

            var result = ordersFound.ToList();

            return result;
        }

        // PUT: api/Orders
        [Route("ChangeStatus/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPut]
        public async Task<ActionResult> PutOrders(int id, OrderStatus? _status = null)
        {
            //Сперва проверяем на физическую возможность смены статуса
            var order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var initialStatus = order.OrderStatus;

            if (order.OrderStatus == OrderStatus.completed) return Forbid();

            //Только пользователь и владелец бренда имеют доступ к смене статуса
            var mySelf = Functions.identityToUser(User.Identity, _context);
            var isUser = order.OrdererId == mySelf.UserId;
            var isBrandOwner = mySelf.Executor != null && order.BrandId == mySelf.Executor.Brand.BrandId;
            if (!(isUser || isBrandOwner))
            {
                return Forbid();
            }

            if (isBrandOwner && order.OrderStatus == OrderStatus.delivered) return Forbid();

            //Затем проверяем права на смену статуса
            UserRole? userRole = null;
            if (isUser) userRole = UserRole.User;
            else if (isBrandOwner) userRole = UserRole.Admin;

            OrderStatus futureStatus;
            if (_status != null)
            {
                futureStatus = _status ?? default;
            }
            else
            {
                futureStatus = order.OrderStatus + 1;
            }

            //Изменить статус могут лишь указанная роль или суперАдмин
            if (userRole == OrderStatusDictionaries.GetMasterRoleFromOrderStatus[futureStatus] ||
                userRole == UserRole.SuperAdmin)
            {
                order.OrderStatus = futureStatus;
            }
            else
            {
                return Unauthorized();
            }

            if (order.OrderStatus == OrderStatus.completed)
            {
                PointsController pointsController = new PointsController(_context);

                if (order.PointsUsed ?? false)
                {
                    if (!pointsController.CompleteTransaction(order.PointRegister))
                    {
                        return BadRequest("Не удалось завершить транзакцию");
                    }
                }

                //Переводим кэшбэк
                Order orderTemp = new Order() { OrderDetails = Functions.getCleanListOfModels(_context.OrderDetail.Where(e => e.OrderId == order.OrderId).ToList()) };
                PointRegister cashbackRegister;
                //Если любой из процессов кэшбэка даст сбой
                if (!pointsController.StartTransaction(pointsController.CalculateCashback(order), mySelf.UserId, order, out cashbackRegister) ||
                    !pointsController.CompleteTransaction(cashbackRegister))
                {
                    return BadRequest("Не удалось произвести кэшбэк");
                }
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Orders
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Order>>> PostOrders(Order _order)
        {
            if (!IsNormalOrderValid(_order))
            {
                return BadRequest();
            }

            var brand = _context.Brand.Include(brand => brand.Executor)
                                            .ThenInclude(exe => exe.User)
                                        .First(brand => brand.BrandId == _order.BrandId);
            var mySelf = Functions.identityToUser(User.Identity, _context);

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

            if (_order.PointsUsed ?? false)
            {
                PointsController pointsController = new PointsController(_context);
                PointRegister register;
                if (!pointsController.StartTransaction(pointsController.GetMaxPayment(mySelf.Points, _order), _order.OrdererId, _order, out register, mySelf))
                {
                    return BadRequest();
                }
            }

            _context.Order.Add(_order);
            await _context.SaveChangesAsync();

            if (mySelf.NotificationsEnabled)
            {
                await new NotificationsController().ToSendNotificationAsync(brand.Executor.User.DeviceType, "У вас новый заказ!", brand.Executor.User.NotificationRegistration);
            }

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

        // DELETE: api/Orders/5
        [Route("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete]
        public async Task<ActionResult<Order>> DeleteOrders(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Получение списка запросов на выбранный заказ
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <returns>Список запросов</returns>
        [Route("GetRequestsByOrder/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public ActionResult<IEnumerable<WaterRequest>> GetRequestsByOrder(int id)
        {
            var mySelf = Functions.identityToUser(User.Identity, _context);
            var order = _context.Order.Find(id);

            if (order == null || order.OrdererId != mySelf.UserId)
            {
                return BadRequest();
            }

            var requests = _context.WaterRequest.Where(request => request.WaterOrderId == order.WaterOrder.WaterOrderId);

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
        [Route("SelectVodaBrand/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPut]
        public ActionResult SelectVodaBrand(int id)
        {
            var request = _context.WaterRequest.Find(id);

            if (request == null)
            {
                return NotFound();
            }

            var mySelf = Functions.identityToUser(User.Identity, _context);
            var order = _context.WaterOrder.Include(wo => wo.Order)
                                            .FirstOrDefault(wo => wo.WaterOrderId == request.WaterOrderId);
            var brand = _context.Brand.Include(brand => brand.Executor)
                                        .FirstOrDefault(brand => brand.WaterBrand.WaterBrandId == request.WaterBrandId);

            //Если клиент не является владельцем заказа - посылать подальше
            if (order.Order.OrdererId != brand.Executor.UserId)
            {
                return Forbid();
            }

            order.Order.BrandId = brand.BrandId;
            order.Order.OrderStatus = OrderStatus.received;

            _context.SaveChanges();

            WaterOrderRemover.Remove(order.OrderId);

            if (order.Order.Orderer.NotificationsEnabled)
            {
                Task.Run(() => new NotificationsController().ToSendNotificationAsync(order.Order.Orderer.DeviceType, "Ваш запрос на доставку был принят!", order.Order.Orderer.NotificationRegistration));
            }

            return Ok();
        }

        /// <summary>
        /// Получение свободных "мокрых" заказов
        /// </summary>
        /// <param name="id">Id категории</param>
        /// <returns>Список заказов</returns>
        [Route("GetOpenVodaOrders/{category}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public ActionResult<IEnumerable<WaterOrder>> GetOpenVodaOrders(Kind category) //category id
        {
            var mySelf = Functions.identityToUser(User.Identity, _context).Executor;
            var myBrand = _context.Brand.First(brand => brand.ExecutorId == mySelf.ExecutorId);
            var myRequests = _context.WaterRequest.Where(wr => wr.WaterBrandId == myBrand.WaterBrand.WaterBrandId);

            //Категория совпадает с указанной
            //Владелец бренда еще не привязан
            //В "мокрых" запросах нет записи с id заказа текущей итерации
            var allOrdersFound = _context.WaterOrder.Include(wr => wr.Order)
                                                        .Where(wr => wr.Order.Kind == category &&
                                                                    wr.Order.BrandId == null);
            //найти все те заказы, в которых отсутствует связь с "моими запросами"
            var ordersFound = allOrdersFound.Where(order => !myRequests.Any(request => order.WaterOrderId == request.WaterOrderId));

            if (!ordersFound.Any())
            {
                return NotFound();
            }

            var result = ordersFound.ToList();

            return result;
        }

        //[Route("GetOrdersByKind/{category}")]
        //[Authorize(Roles = "SuperAdmin, Admin")]
        //[HttpGet]
        //public async Task<ActionResult<List<Order>>> GetOrdersByKind(Kind category)
        //{
        //    var mySelf = Functions.identityToUser(User.Identity, _context);
        //    //Возвращать только те заказы, где ты являешься исполнителем, и статус не является завершенным
        //    var ordersFound = await _context.Orders.Where(p => p.Kind == category &&
        //                                                    p.BrandOwnerId == identity.UserId &&
        //                                                    p.OrderStatus < OrderStatus.completed).ToListAsync();

        //    if (ordersFound == null)
        //    {
        //        return NotFound();
        //    }

        //    var orders = Functions.getCleanListOfModels(ordersFound);

        //    foreach (Order order in orders)
        //    {
        //        order.OrderDetails = Functions.getCleanListOfModels(_context.OrderDetails.Where(d => d.OrderId == order.OrderId).ToList());
        //        foreach (OrderDetail detail in order.OrderDetails)
        //        {
        //            if (detail.ProductId != null) //Если продукта больше не существует
        //            {
        //                detail.Product = Functions.getCleanModel(await _context.Products.FindAsync(detail.ProductId));
        //                detail.Product.Image = Functions.getCleanModel(await _context.Images.FindAsync(detail.Product.ImgId));
        //            }
        //        }
        //        order.User = Functions.getCleanUser(await _context.Users.FindAsync(order.UserId));
        //        var ownerBuffer = await _context.Users.FindAsync(order.BrandOwnerId);
        //        if (order.PointsUsed && order.PointRegisterId != null)
        //        {
        //            order.PointRegister = Functions.getCleanModel(await _context.PointRegisters.FindAsync(order.PointRegisterId));
        //        }
        //        if (order.BrandOwnerId != null)
        //        {
        //            order.BrandOwner = Functions.getCleanUser(identity); //Не равен null если заказ взят отправившим запрос
        //        }
        //    }

        //    return orders;
        //}

        /// <summary>
        /// Создает запрос на исполнение заказа
        /// </summary>
        /// <param name="waterRequest">Неполная модель с указанием предлагаемой цены</param>
        /// <param name="id">Id заказа, на который претендует отправитель</param>
        [Route("PostVodaRequest")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public async Task<ActionResult> PostVodaRequest(WaterRequest _waterRequestData)
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
            var myBrand = _context.Brand.FirstOrDefault(brand => brand.ExecutorId == mySelf.ExecutorId);

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
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Route("PostVodaOrders")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public ActionResult PostVodaOrders(Order _order)
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

            _context.Order.Add(_order);
            _context.SaveChanges();

            WaterOrderRemover.Add(_order.CreatedDate, _order.OrderId); //Удалит заказ через 2 часа если job не будет удален до триггера

            return Ok();
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
                    (_order.Kind != Kind.food || _order.Kind != Kind.flowers) ||
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
                var brand = _context.Brand.Find(_order.BrandId);
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
                    (_order.Kind != Kind.bottledWater || _order.Kind != Kind.water) ||
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
                    var brand = _context.Brand.Find(_order.BrandId);
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
