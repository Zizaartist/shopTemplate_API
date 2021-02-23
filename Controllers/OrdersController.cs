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
using ApiClick.Models.RegisterModels;
using ApiClick.StaticValues;
using System.ComponentModel;

namespace ApiClick.Controllers
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        ClickContext _context;
        Functions funcs = new Functions();

        public OrdersController(ClickContext _context)
        {
            this._context = _context;
        }

        // GET: api/Orders
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<Order>> GetOrders(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.BrandOwnerId != funcs.identityToUser(User.Identity, _context).UserId)
            {
                return Forbid();
            }

            List<OrderDetail> relatedOrderDetails = _context.OrderDetails.Where(d => d.OrderId == order.OrderId).ToList();
            order.OrderDetails = relatedOrderDetails;

            return order;
        }

        // GET: api/GetMyOrders
        [Route("api/GetMyOrders")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetMyOrders()
        {
            //Находит заказы принадлежащие пользователю и отсеивает заказы со статусом "Завершенный"
            var ordersFound = _context.Orders.Where(e => e.UserId == funcs.identityToUser(User.Identity, _context).UserId).ToList();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (Order order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetails.Where(d => d.OrderId == order.OrderId).ToList());
                foreach (OrderDetail detail in order.OrderDetails)
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.Products.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.Images.FindAsync(detail.Product.ImgId));
                    }
                }
                var ownerBuffer = await _context.Users.FindAsync(order.BrandOwnerId);
                //Если бренд не "мокрый"
                if (ownerBuffer != null)
                {
                    order.BrandOwner = new User();
                    order.BrandOwner.Brands = funcs.getCleanListOfModels(new List<Brand>() { _context.Brands.First(e => e.UserId == order.BrandOwnerId) });
                    order.BrandOwner.Brands.First().ImgBanner = funcs.getCleanModel(await _context.Images.FindAsync(order.BrandOwner.Brands.First().ImgBannerId));
                    order.BrandOwner.Brands.First().ImgLogo = funcs.getCleanModel(await _context.Images.FindAsync(order.BrandOwner.Brands.First().ImgLogoId));
                }
                if (order.PointsUsed && order.PointRegisterId != null)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisters.FindAsync(order.PointRegisterId));
                }
            }

            return orders;
        }

        // GET: api/GetMyTasks
        [Route("api/GetMyTasks")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetMyTasks()
        {
            //Получаем заказы, где владелец токена обозначен как владелец бренда
            //+ статус не является завершенным
            var ordersFound = _context.Orders.Where(e => e.BrandOwnerId == funcs.identityToUser(User.Identity, _context).UserId)
                                          .Where(e => e.OrderStatus < OrderStatus.delivered).ToList();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (Order order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetails.Where(d => d.OrderId == order.OrderId).ToList());
                foreach (OrderDetail detail in order.OrderDetails)
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.Products.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.Images.FindAsync(detail.Product.ImgId));
                    }
                }
                order.User = funcs.getCleanUser(await _context.Users.FindAsync(order.UserId));
                var ownerBuffer = await _context.Users.FindAsync(order.BrandOwnerId);
                if (order.PointsUsed && order.PointRegisterId != null)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisters.FindAsync(order.PointRegisterId));
                }
            }

            return orders;
        }

        // GET: api/GetMyHistory
        [Route("api/GetMyHistory")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetMyHistory()
        {
            //Получаем заказы, где владелец токена обозначен как владелец бренда
            //+ статус должен быть завершенным
            var ordersFound = _context.Orders.Where(e => e.BrandOwnerId == funcs.identityToUser(User.Identity, _context).UserId)
                                          .Where(e => e.OrderStatus >= OrderStatus.delivered).ToList();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (Order order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetails.Where(d => d.OrderId == order.OrderId).ToList());
                foreach (OrderDetail detail in order.OrderDetails)
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.Products.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.Images.FindAsync(detail.Product.ImgId));
                    }
                }
                order.User = funcs.getCleanUser(await _context.Users.FindAsync(order.UserId));
                var ownerBuffer = await _context.Users.FindAsync(order.BrandOwnerId);
                if (order.PointsUsed && order.PointRegisterId != null)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisters.FindAsync(order.PointRegisterId));
                }
            }

            return orders;
        }

        // PUT: api/Orders
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPut]
        public async Task<ActionResult> PutOrders(int id, OrderStatus? _status = null)
        {
            //Сперва проверяем на физическую возможность смены статуса
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var initialStatus = order.OrderStatus;

            if (order.OrderStatus == OrderStatus.completed) return Forbid();

            //Только пользователь и владелец бренда имеют доступ к смене статуса
            var identity = funcs.identityToUser(User.Identity, _context);
            var isUser = order.UserId == identity.UserId;
            var isBrandOwner = order.BrandOwnerId == identity.UserId;
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
                await _context.SaveChangesAsync();
                PointsController pointsController = new PointsController(_context);

                if (order.PointsUsed)
                {
                    if (!pointsController.CompleteTransaction(order.PointRegisterId ?? default))
                    {
                        order.OrderStatus = initialStatus;
                        await _context.SaveChangesAsync();
                        return BadRequest("Не удалось завершить транзакцию");
                    }
                }

                //Переводим кэшбэк
                Order orderTemp = new Order() { OrderDetails = funcs.getCleanListOfModels(_context.OrderDetails.Where(e => e.OrderId == order.OrderId).ToList()) };
                PointRegister cashbackRegister;
                //Если любой из процессов кэшбэка даст сбой - вернуть изначальный статус
                if (!pointsController.StartTransaction(pointsController.CalculateCashback(order), null, order.UserId, order.OrderId, out cashbackRegister) ||
                    !pointsController.CompleteTransaction(cashbackRegister.PointRegisterId)) 
                {
                    order.OrderStatus = initialStatus;
                    await _context.SaveChangesAsync();
                    return BadRequest("Не удалось произвести кэшбэк");
                }
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Orders
        [Route("api/[controller]")]

        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<List<Order>>> PostOrders(Order order)
        {
            if (order == null ||
                order.OrderDetails == null ||
                order.OrderDetails.Count < 1)
            {
                return BadRequest();
            }

            var responsibleBrand = await _context.Brands.FindAsync(
                                                (await _context.BrandMenus.FindAsync(
                                                    (await _context.Products.FindAsync(
                                                            order.OrderDetails.First().ProductId)
                                                    ).BrandMenuId)
                                                ).BrandId);

            order.OrderDetails = funcs.getCleanListOfModels(order.OrderDetails.ToList());
            order.Category = responsibleBrand.Category;
            order.CreatedDate = DateTime.Now;
            order.OrderStatus = OrderStatus.received;
            order.User = funcs.identityToUser(User.Identity, _context);
            order.UserId = order.User.UserId;
            order.Phone = order.User.Phone;
            order.BrandOwnerId = responsibleBrand.UserId;
            order.BrandOwner = await _context.Users.FindAsync(order.BrandOwnerId);

            //filling blanks and sending to DB
            if (order.PointsUsed)
            {
                var orderSum = order.OrderDetails.Sum(e => CalcSumPrice(e.ProductId, e.Count));
                //Если заказ бесплатный - убрать связь с банкнотами
                if (orderSum <= order.User.Points)
                {
                    order.Banknote = null;
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            if (order.User.NotificationsEnabled)
            {
                await new NotificationsController().ToSendNotificationAsync(order.BrandOwner.DeviceType, "У вас новый заказ!", order.BrandOwner.NotificationRegistration);
            }

            if (order.PointsUsed)
            {
                PointsController pointsController = new PointsController(_context);
                PointRegister register;
                if (pointsController.StartTransaction(pointsController.GetMaxPayment(order.User.Points, order), order.UserId, order.BrandOwnerId??default, order.OrderId, out register))
                {
                    order.PointRegisterId = register.PointRegisterId;
                }
                else
                {
                    order.PointsUsed = false;
                    await _context.SaveChangesAsync();
                    return BadRequest();
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        private decimal CalcSumPrice(int? _productId, int _count)
        {
            Product product;
            try
            {
                product = _context.Products.Find(_productId);
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
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpDelete]
        public async Task<ActionResult<Order>> DeleteOrders(int id)
        {
            var ordersCl = await _context.Orders.FindAsync(id);
            if (ordersCl == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(ordersCl);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Получение списка запросов на выбранный заказ
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <returns>Список запросов</returns>
        [Route("api/GetRequestsByOrder/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<WaterRequest>>> GetRequestsByOrder(int id)
        {
            var identity = funcs.identityToUser(User.Identity, _context);
            var order = await _context.Orders.FindAsync(id);

            if (order == null || id <= 0) 
            {
                return BadRequest();
            }

            if (order.UserId != identity.UserId) 
            {
                return Forbid();
            }

            var requests = funcs.getCleanListOfModels(_context.WaterRequests.Where(e => e.OrderId == order.OrderId).ToList());
            foreach (WaterRequest request in requests) 
            {
                request.Suggestions = funcs.getCleanListOfModels(_context.RequestDetails.Where(e => e.RequestId == request.WaterRequestId).ToList());
                request.Brand = funcs.getCleanModel(_context.Brands.Find(request.BrandId));
                request.Brand.ImgBanner = funcs.getCleanModel(_context.Images.Find(request.Brand.ImgBannerId));
                request.Brand.ImgLogo = funcs.getCleanModel(_context.Images.Find(request.Brand.ImgLogoId));
            }
            return requests;
        }

        /// <summary>
        /// Выбирает запрос и назначает бренд 
        /// </summary>
        /// <param name="id">Id запроса на прием заказа</param>
        /// <param name="pointsUsed">Были ли использованы баллы для оплаты</param>
        [Route("api/SelectVodaBrand/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPut]
        public async Task<ActionResult> SelectVodaBrand(int id, bool pointsUsed)
        {
            var request = await _context.WaterRequests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            var identity = funcs.identityToUser(User.Identity, _context);
            var order = await _context.Orders.FindAsync(request.OrderId);
            var brand = await _context.Brands.FindAsync(request.BrandId);

            //Если клиент не является владельцем заказа - посылать подальше
            if (order.UserId != identity.UserId)
            {
                return Forbid();
            }

            order.User = await _context.Users.FindAsync(order.UserId);
            order.BrandOwnerId = brand.UserId;
            order.BrandOwner = await _context.Users.FindAsync(order.BrandOwnerId);
            order.OrderDetails = _context.OrderDetails.Where(e => e.OrderId == order.OrderId).ToList();
            order.Phone = order.User.Phone;
            order.PointsUsed = false; //!!!!!!!!!!!!!!!!!!!!!!!!temp default!!!!!!!!!!!!!!!!!!!
            request.Suggestions = _context.RequestDetails.Where(e => e.RequestId == request.WaterRequestId).ToList();

            //Для каждой продукции найти пару и присвоить стоимость указанную в запросе
            try
            {
                foreach (OrderDetail detail in order.OrderDetails)
                {
                    detail.Price = request.Suggestions.First(e => e.ProductId == detail.ProductId).SuggestedPrice;
                }
            }
            catch 
            {
                //Полученый запрос не имеет соответствующего списка деталей
                return BadRequest();
            }

            //if (pointsUsed)
            //{
            //    order.PointsUsed = true;
            //    await _context.SaveChangesAsync();
            //    PointsController pointsController = new PointsController(_context);
            //    PointRegister register;
            //    if (pointsController.StartTransaction(pointsController.GetMaxPayment(order.User.Points, order), order.UserId, order.BrandOwnerId ?? default, order.OrderId, out register))
            //    {
            //        order.PointRegisterId = register.PointRegisterId;
            //    }
            //    else
            //    {
            //        //Хотя бы предотвратит потери
            //        order.PointsUsed = false;
            //        await _context.SaveChangesAsync();
            //        return BadRequest("Не удалось создать регистр учета баллов");
            //    }
            //}

            await _context.SaveChangesAsync();

            if (order.User.NotificationsEnabled)
            {
                await new NotificationsController().ToSendNotificationAsync(order.BrandOwner.DeviceType, "Ваш запрос на доставку был принят!", order.BrandOwner.NotificationRegistration);
            }

            return Ok();
        }

        /// <summary>
        /// Получение свободных "мокрых" заказов
        /// </summary>
        /// <param name="id">Id категории</param>
        /// <returns>Список заказов</returns>
        [Route("api/GetOpenVodaOrders/{category}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOpenVodaOrders(Category category) //category id
        {
            var identity = funcs.identityToUser(User.Identity, _context);
            var myFirstBrand = _context.Brands.First(e => e.UserId == identity.UserId);
            var myRequests = _context.WaterRequests.Where(e => e.BrandId == myFirstBrand.BrandId);
            
            //Категория совпадает с указанной
            //Владелец бренда еще не привязан
            //В "мокрых" запросах нет записи с id заказа текущей итерации
            var allOrdersFound = _context.Orders.Where(p => p.Category == category && 
                                                                    p.BrandOwnerId == null);
            //найти все те заказы, в которых отсутствует связь с "моими запросами"
            var ordersFound = allOrdersFound.Where(p => !myRequests.Any(e => p.OrderId == e.OrderId));

            if (!ordersFound.Any())
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound.ToList());

            foreach (Order order in orders)
            {
                //Нифига, кроме пользовательских данных неизвестно
                order.User = funcs.getCleanUser(await _context.Users.FindAsync(order.UserId));
                order.OrderDetails = funcs.getCleanListOfModels(await _context.OrderDetails.Where(e => e.OrderId == order.OrderId).ToListAsync());
                foreach (OrderDetail detail in order.OrderDetails) 
                {
                    detail.Product = funcs.getCleanModel(await _context.Products.FindAsync(detail.ProductId));
                    detail.Product.Image = funcs.getCleanModel(await _context.Images.FindAsync(detail.Product.ImgId));
                }
            }

            return orders;
        }

        [Route("api/GetOrdersByCategory/{category}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrdersByCategory(Category category)
        {
            var identity = funcs.identityToUser(User.Identity, _context);
            //Возвращать только те заказы, где ты являешься исполнителем, и статус не является завершенным
            var ordersFound = await _context.Orders.Where(p => p.Category == category && 
                                                            p.BrandOwnerId == identity.UserId && 
                                                            p.OrderStatus < OrderStatus.completed).ToListAsync();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (Order order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetails.Where(d => d.OrderId == order.OrderId).ToList());
                foreach (OrderDetail detail in order.OrderDetails)
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.Products.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.Images.FindAsync(detail.Product.ImgId));
                    }
                }
                order.User = funcs.getCleanUser(await _context.Users.FindAsync(order.UserId));
                var ownerBuffer = await _context.Users.FindAsync(order.BrandOwnerId);
                if(order.PointsUsed && order.PointRegisterId != null)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisters.FindAsync(order.PointRegisterId));
                }
                if (order.BrandOwnerId != null)
                {
                    order.BrandOwner = funcs.getCleanUser(identity); //Не равен null если заказ взят отправившим запрос
                }
            }

            return orders;
        }

        /// <summary>
        /// Создает запрос на исполнение заказа
        /// </summary>
        /// <param name="waterRequest">Неполная модель с указанием предлагаемой цены</param>
        /// <param name="id">Id заказа, на который претендует отправитель</param>
        [Route("api/PostVodaRequest")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public async Task<ActionResult> PostVodaRequest(WaterRequest waterRequest)
        {
            if (waterRequest == null || waterRequest.Suggestions == null || waterRequest.Suggestions.Count == 0) 
            {
                return BadRequest();
            }

            Order order;

            try
            {
                order = await _context.Orders.FindAsync(waterRequest.OrderId);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            //Если заказ занят - запретить
            if (order.BrandOwnerId != null) 
            {
                return Forbid();
            }

            order.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == order.OrderId).ToListAsync();

            //Если количество предложений не соответствует
            if (order.OrderDetails.Count != waterRequest.Suggestions.Count)
            {
                return BadRequest();
            }
            else 
            {
                var listOfSuggestions = new List<RequestDetail>(waterRequest.Suggestions);
                //Проверка на соответствие соответствие каждой детали заказа с деталью запроса
                //Если не найдено детали запроса для соответствующей детали заказа - badrequest
                foreach (OrderDetail detail in order.OrderDetails) 
                {
                    if (!listOfSuggestions.Any(e => e.ProductId == detail.ProductId))
                        return BadRequest();
                }
            }

            //получаем первый бренд отправителя
            var user = funcs.identityToUser(User.Identity, _context);
            var brands = _context.Brands.Where(e => e.UserId == user.UserId);
            var brand = brands.First();

            //... и проверяем наличие хотя бы одной записи с id этого бренда, подавляем попытку создать дубликат
            if (_context.WaterRequests.Where(e => e.OrderId == order.OrderId).Any(e => e.BrandId == brand.BrandId)) 
            {
                return Forbid();
            }

            var request = new WaterRequest()
            {
                BrandId = brand.BrandId,
                OrderId = order.OrderId
            };
            _context.WaterRequests.Add(request);
            await _context.SaveChangesAsync();

            foreach (RequestDetail detail in waterRequest.Suggestions) 
            {
                detail.RequestId = request.WaterRequestId;
                _context.RequestDetails.Add(detail);
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Route("api/PostVodaOrders")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<Order>> PostVodaOrders(Order ordersCl)
        {
            if (ordersCl == null || ordersCl.OrderDetails == null || ordersCl.OrderDetails.Count < 1)
            {
                return BadRequest();
            }

            foreach (OrderDetail detail in ordersCl.OrderDetails)
            {
                if (detail.Count < 1)
                {
                    return BadRequest();
                }
            }

            //filling blanks and sending to DB
            ordersCl.CreatedDate = DateTime.Now;
            ordersCl.OrderStatus = OrderStatus.sent;
            ordersCl.UserId = funcs.identityToUser(User.Identity, _context).UserId;
            ordersCl.User = await _context.Users.FindAsync(ordersCl.UserId);
            ordersCl.PaymentMethod = PaymentMethod.cash; //Только налик

            _context.Orders.Add(ordersCl);
            await _context.SaveChangesAsync(); //вроде как рефрешит объект ordersCl

            return Ok();
        }

        //[Route("api/PutVodaOrders/{id}")]
        //[Authorize(Roles = "SuperAdmin, Admin")]
        //[HttpPut]
        //public async Task<ActionResult> PutVodaOrders(int id)
        //{
        //    //Сперва проверяем на физическую возможность смены статуса
        //    var order = await _context.Orders.FindAsync(id);

        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    //Только пользователь и владелец бренда имеют доступ к смене статуса
        //    var identity = funcs.identityToUser(User.Identity, _context);

        //    //Если заказ занят - посылать куда подальше
        //    if (order.BrandOwnerId != null)
        //    {
        //        return Forbid();
        //    }

        //    order.BrandOwnerId = identity.UserId;
        //    order.BrandOwner = await _context.Users.FindAsync(identity.UserId);
        //    order.User = await _context.Users.FindAsync(order.UserId);

        //    await _context.SaveChangesAsync();

        //    if (order.User.NotificationsEnabled)
        //    {
        //        await new NotificationsController().ToSendNotificationAsync(order.User.DeviceType, "Статус вашего заказа обновлен!", order.User.NotificationRegistration);
        //    }

        //    return Ok();
        //}
    }
}
