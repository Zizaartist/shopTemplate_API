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
        ClickContext _context = new ClickContext();
        Functions funcs = new Functions();

        // GET: api/Orders
        [Route("api/[controller]")]
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersCl>>> GetOrdersCl()
        {
            return await _context.OrdersCl.ToListAsync();
        }

        // GET: api/Orders/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<OrdersCl>> GetOrdersCl(int id)
        {
            var ordersCl = await _context.OrdersCl.FindAsync(id);

            if (ordersCl == null)
            {
                return NotFound();
            }

            if (ordersCl.BrandOwnerId != funcs.identityToUser(User.Identity, _context).UserId) 
            {
                return Forbid();
            }

            List<OrderDetailCl> relatedOrderDetails = _context.OrderDetailCl.Where(d => d.OrderId == ordersCl.OrdersId).ToList();
            ordersCl.OrderDetails = relatedOrderDetails;

            return ordersCl;
        }

        // GET: api/GetMyOrders
        [Route("api/GetMyOrders")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersCl>>> GetMyOrders()
        {
            //Находит заказы принадлежащие пользователю и отсеивает заказы со статусом "Завершенный"
            var ordersFound = _context.OrdersCl.Where(e => e.UserId == funcs.identityToUser(User.Identity, _context).UserId &&
                                                        e.StatusId != _context.OrderStatusCl.First(e => e.OrderStatusName == "Завершено").OrderStatusId).ToList();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (OrdersCl order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList());
                foreach (OrderDetailCl detail in order.OrderDetails) 
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.ProductCl.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.ImageCl.FindAsync(detail.Product.ImgId));
                    } 
                }
                var ownerBuffer = await _context.UserCl.FindAsync(order.BrandOwnerId);
                //Если бренд не "мокрый"
                if (ownerBuffer != null)
                {
                    order.BrandOwner = new UserCl();
                    order.BrandOwner.Brands = funcs.getCleanListOfModels(new List<BrandCl>() { _context.BrandCl.First(e => e.UserId == order.BrandOwnerId) });
                    order.BrandOwner.Brands.First().ImgBanner = funcs.getCleanModel(await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgBannerId));
                    order.BrandOwner.Brands.First().ImgLogo = funcs.getCleanModel(await _context.ImageCl.FindAsync(order.BrandOwner.Brands.First().ImgLogoId));
                }
                order.OrderStatus = funcs.getCleanModel(await _context.OrderStatusCl.FindAsync(order.StatusId));
                order.PaymentMethod = await _context.PaymentMethodCl.FindAsync(order.PaymentMethodId);
                if (order.BanknoteId != null) 
                {
                    order.Banknote = await _context.BanknoteCl.FindAsync(order.BanknoteId);
                }
                if (order.PointsUsed)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisterCl.FindAsync(order.PointRegisterId));
                }
            }

            return orders;
        }

        // GET: api/GetMyTasks
        [Route("api/GetMyTasks")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersCl>>> GetMyTasks()
        {
            //Получаем заказы, где владелец токена обозначен как владелец бренда
            //+ статус не является завершенным
            var ordersFound = _context.OrdersCl.Where(e => e.BrandOwnerId == funcs.identityToUser(User.Identity, _context).UserId)
                                          .Where(e => e.OrderStatus.OrderStatusId != _context.OrderStatusCl.First(e => e.OrderStatusName == "Завершено").OrderStatusId).ToList();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (OrdersCl order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList());
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.ProductCl.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.ImageCl.FindAsync(detail.Product.ImgId));
                    }
                }
                order.User = funcs.getCleanUser(await _context.UserCl.FindAsync(order.UserId));
                var ownerBuffer = await _context.UserCl.FindAsync(order.BrandOwnerId);
                order.OrderStatus = funcs.getCleanModel(await _context.OrderStatusCl.FindAsync(order.StatusId));
                order.PaymentMethod = await _context.PaymentMethodCl.FindAsync(order.PaymentMethodId);
                if (order.PointsUsed)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisterCl.FindAsync(order.PointRegisterId));
                }
                if (order.BanknoteId != null)
                {
                    order.Banknote = await _context.BanknoteCl.FindAsync(order.BanknoteId);
                }
            }

            return orders;
        }
        
        // GET: api/GetMyHistory
        [Route("api/GetMyHistory")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersCl>>> GetMyHistory()
        {
            //Получаем заказы, где владелец токена обозначен как владелец бренда
            //+ статус должен быть завершенным
            var ordersFound = _context.OrdersCl.Where(e => e.BrandOwnerId == funcs.identityToUser(User.Identity, _context).UserId)
                                          .Where(e => e.OrderStatus.OrderStatusId == _context.OrderStatusCl.First(e => e.OrderStatusName == "Завершено").OrderStatusId).ToList();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (OrdersCl order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList());
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.ProductCl.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.ImageCl.FindAsync(detail.Product.ImgId));
                    }
                }
                order.User = funcs.getCleanUser(await _context.UserCl.FindAsync(order.UserId));
                var ownerBuffer = await _context.UserCl.FindAsync(order.BrandOwnerId);
                order.OrderStatus = funcs.getCleanModel(await _context.OrderStatusCl.FindAsync(order.StatusId));
                order.PaymentMethod = await _context.PaymentMethodCl.FindAsync(order.PaymentMethodId);
                if (order.PointsUsed)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisterCl.FindAsync(order.PointRegisterId));
                }
            }

            return orders;
        }

        // PUT: api/Orders
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPut]
        public async Task<ActionResult> PutOrdersCl(int id)
        {
            //Сперва проверяем на физическую возможность смены статуса
            var order = await _context.OrdersCl.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = await _context.OrderStatusCl.FindAsync(order.StatusId);
            if (order.OrderStatus.OrderStatusName == "Завершено")
            {
                return Forbid();
            }

            //Только пользователь и владелец бренда имеют доступ к смене статуса
            var identity = funcs.identityToUser(User.Identity, _context);
            if (!(order.UserId == identity.UserId || order.BrandOwnerId == identity.UserId)) 
            {
                return Forbid();
            }

            //Затем проверяем права на смену статуса
            int userRole = funcs.identityToUser(User.Identity, _context).Role;
            int futureStatusId = order.StatusId + 1;
            OrderStatusCl futureOrderStatusCl = await _context.OrderStatusCl.FindAsync(futureStatusId);

            //Изменить статус могут лишь указанная роль или суперАдмин
            if (userRole == futureOrderStatusCl.MasterRoleId || 
                userRole == _context.UserRolesCl.First(e => e.UserRoleName == "SuperAdmin").UserRoleId)
            {
                order.StatusId++;
                order.OrderStatus = futureOrderStatusCl;
            }
            else
            {
                return Unauthorized();
            }

            if (order.OrderStatus.OrderStatusName == "Завершено") 
            {
                PointsController pointsController = new PointsController();
                if (order.PointsUsed)
                {
                    order.PointRegister = await _context.PointRegisterCl.FindAsync(order.PointRegisterId);
                    order.OrderDetails = _context.OrderDetailCl.Where(e => e.OrderId == order.OrdersId).ToList();
                    pointsController.RemovePoints(order);
                    pointsController.GetPoints(order, (await _context.PointRegisterCl.FindAsync(order.PointRegisterId)).Points);
                }
                else
                {
                    pointsController.GetPoints(order, 0);
                }
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Orders
        [Route("api/[controller]")]

        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpPost]
        public async Task<ActionResult<List<OrdersCl>>> PostOrdersCl(OrdersCl orderContainer)
        {
            if (orderContainer == null || orderContainer.OrderDetails == null || orderContainer.OrderDetails.Count < 1)
            {
                return BadRequest();
            }

            //Формируем заказы по критерию принадлежности бренду
            List<List<OrderDetailCl>> brandOrders = new List<List<OrderDetailCl>>();
            List<OrdersCl> orders = new List<OrdersCl>();
            foreach (OrderDetailCl detail in orderContainer.OrderDetails)
            {
                detail.Product = _context.ProductCl.Find(detail.ProductId);
                detail.Product.BrandMenu = _context.BrandMenuCl.Find(detail.Product.BrandMenuId);
                detail.Price = detail.Product.Price;

                bool found = false;
                foreach (List<OrderDetailCl> brandOrder in brandOrders)
                {
                    //Если бренд первого элемента списка совпадает с брендом текущей детали - добавить в список и перейти на следущую деталь 
                    if (brandOrder[0].Product.BrandMenu.BrandId == detail.Product.BrandMenu.BrandId)
                    {
                        brandOrder.Add(detail);
                        found = true;
                        break;
                    }
                }
                //Если не найден во время циклов - добавить список
                if (!found)
                {
                    List<OrderDetailCl> buffer = new List<OrderDetailCl>();
                    buffer.Add(detail);
                    brandOrders.Add(buffer);
                }
            }

            //Создаем заказ из имеющихся списков деталей
            //foreach (List<OrderDetailCl> brandOrder in brandOrders)
            //{
            //    int categoryId = brandOrder.First().Product.BrandMenu.Brand.CategoryId;
            //    foreach (OrderDetailCl detail in brandOrder)
            //    {
            //        detail.Product = null;
            //    }
            //    OrdersCl order = new OrdersCl()
            //    {
            //        OrderDetails = brandOrder
            //    };
            //    orders.Add(order);
            //    orders.Last().CategoryId = categoryId;
            //}

            brandOrders.ForEach(listOfDetails => 
            {
                OrdersCl newOrder = new OrdersCl();
                newOrder.CategoryId = _context.BrandCl.Find(listOfDetails.First().Product.BrandMenu.BrandId).CategoryId;
                newOrder.UserId = funcs.identityToUser(User.Identity, _context).UserId;
                listOfDetails.ForEach(detail => newOrder.OrderDetails.Add(detail));
                orders.Add(newOrder);
            });

            //Determine which orders will be paid with points and which will not
            PointsController pointsController = new PointsController();
            List<PointsController.OrderExtended> ordersWithPoints = new List<PointsController.OrderExtended>();
            if (orderContainer.PointsUsed)
            {
                ordersWithPoints = pointsController.DistributePoints(orders);
            }

            //Заполни пробелы и 
            foreach (OrdersCl order in orders)
            {
                var responsibleBrandOwnerId = await _context.BrandCl.FindAsync(
                                                   (await _context.BrandMenuCl.FindAsync(
                                                        (await _context.ProductCl.FindAsync(
                                                             order.OrderDetails.First().ProductId)
                                                        ).BrandMenuId)
                                                   ).BrandId);

                //filling blanks and sending to DB
                decimal pointsInvested = default;
                order.BanknoteId = orderContainer.BanknoteId;
                if (orderContainer.PointsUsed) 
                {
                    var currectOrder = ordersWithPoints.Find(extendedOrder => extendedOrder.order.Equals(order));
                    pointsInvested = currectOrder.pointsInvested;
                    //Если заказ бесплатный - убрать связь с банкнотами
                    if (currectOrder.orderSum == currectOrder.pointsInvested) 
                    {
                        order.BanknoteId = null;
                    }
                } 
                order.Street = orderContainer.Street;
                order.House = orderContainer.House;
                order.Kv = orderContainer.Kv;
                order.Padik = orderContainer.Padik;
                order.Etash = orderContainer.Etash;
                order.PointsUsed = orderContainer.PointsUsed && (pointsInvested > 0);
                order.PaymentMethodId = orderContainer.PaymentMethodId;
                order.Commentary = orderContainer.Commentary;

                order.CreatedDate = DateTime.Now;
                order.StatusId = _context.OrderStatusCl.First(e => e.OrderStatusName == "Отправлено").OrderStatusId;
                order.OrderStatus = await _context.OrderStatusCl.FindAsync(order.StatusId);
                order.User = await _context.UserCl.FindAsync(order.UserId);
                order.Phone = order.User.Phone;
                order.BrandOwnerId = responsibleBrandOwnerId.UserId;

                _context.OrdersCl.Add(order);
                await _context.SaveChangesAsync();
                if (order.PointsUsed)
                {
                    var register = await pointsController.CreatePointRegister(order.User, order, pointsInvested);
                    if (register == null)
                    {
                        return BadRequest();
                    }
                    order.PointRegisterId = register.PointRegisterId;
                }

                order.BrandOwner = await _context.UserCl.FindAsync(order.BrandOwnerId);
                await _context.SaveChangesAsync();
                NotificationsController notificationsController = new NotificationsController();
                await notificationsController.ToSendNotificationAsync(order.BrandOwner.DeviceType, "У вас новый заказ!", order.BrandOwner.NotificationRegistration);
            }
            return Ok();
        }

        // DELETE: api/Orders/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpDelete]
        public async Task<ActionResult<OrdersCl>> DeleteOrdersCl(int id)
        {
            var ordersCl = await _context.OrdersCl.FindAsync(id);
            if (ordersCl == null)
            {
                return NotFound();
            }

            _context.OrdersCl.Remove(ordersCl);
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
            var order = await _context.OrdersCl.FindAsync(id);

            if (order == null || id <= 0) 
            {
                return BadRequest();
            }

            if (order.UserId != identity.UserId) 
            {
                return Forbid();
            }

            var requests = funcs.getCleanListOfModels(_context.WaterRequests.Where(e => e.OrderId == order.OrdersId).ToList());
            foreach (WaterRequest request in requests) 
            {
                request.Suggestions = funcs.getCleanListOfModels(_context.RequestDetails.Where(e => e.RequestId == request.WaterRequestId).ToList());
                request.Brand = funcs.getCleanModel(_context.BrandCl.Find(request.BrandId));
                request.Brand.ImgBanner = funcs.getCleanModel(_context.ImageCl.Find(request.Brand.ImgBannerId));
                request.Brand.ImgLogo = funcs.getCleanModel(_context.ImageCl.Find(request.Brand.ImgLogoId));
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
            var order = await _context.OrdersCl.FindAsync(request.OrderId);
            var brand = await _context.BrandCl.FindAsync(request.BrandId);

            //Если клиент не является владельцем заказа - посылать подальше
            if (order.UserId != identity.UserId)
            {
                return Forbid();
            }

            order.User = await _context.UserCl.FindAsync(order.UserId);
            order.BrandOwnerId = brand.UserId;
            order.BrandOwner = await _context.UserCl.FindAsync(order.BrandOwnerId);
            order.OrderDetails = _context.OrderDetailCl.Where(e => e.OrderId == order.OrdersId).ToList();
            request.Suggestions = _context.RequestDetails.Where(e => e.RequestId == request.WaterRequestId).ToList();

            //Для каждой продукции найти пару и присвоить стоимость указанную в запросе
            try
            {
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    detail.Price = request.Suggestions.First(e => e.ProductId == detail.ProductId).SuggestedPrice;
                }
            }
            catch 
            {
                //Полученый запрос не имеет соответствующего списка деталей
                return BadRequest(); 
            }

            if (pointsUsed) 
            {
                order.PointsUsed = true;
                PointsController pointsController = new PointsController();
                PointRegister register = await pointsController.CreatePointRegister(order.User, order);
                if (register == null) 
                {
                    return BadRequest();
                }
                order.PointRegisterId = register.PointRegisterId;
            }

            await _context.SaveChangesAsync();
            await new NotificationsController().ToSendNotificationAsync(order.BrandOwner.DeviceType, "Ваш запрос на доставку был принят!", order.BrandOwner.NotificationRegistration);

            //Удаление ненужных записей
            //foreach (RequestDetail detail in request.Suggestions)
            //{
            //    _context.RequestDetails.Remove(detail);
            //}
            //_context.WaterRequests.Remove(request);
            //await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Получение свободных "мокрых" заказов
        /// </summary>
        /// <param name="id">Id категории</param>
        /// <returns>Список заказов</returns>
        [Route("api/GetOpenVodaOrders/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersCl>>> GetOpenVodaOrders(int id) //category id
        {
            var identity = funcs.identityToUser(User.Identity, _context);
            var myFirstBrand = _context.BrandCl.First(e => e.UserId == identity.UserId);
            var myRequests = _context.WaterRequests.Where(e => e.BrandId == myFirstBrand.BrandId);
            
            //Категория совпадает с указанной
            //Владелец бренда еще не привязан
            //В "мокрых" запросах нет записи с id заказа текущей итерации
            var allOrdersFound = _context.OrdersCl.Where(p => p.CategoryId == id && 
                                                                    p.BrandOwnerId == null);
            var test = allOrdersFound.ToList();
            //найти все те заказы, в которых отсутствует связь с "моими запросами"
            var ordersFound = allOrdersFound.Where(p => !myRequests.Any(e => p.OrdersId == e.OrderId)).ToList();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (OrdersCl order in orders)
            {
                //Нифига, кроме пользовательских данных неизвестно
                order.User = funcs.getCleanUser(await _context.UserCl.FindAsync(order.UserId));
                order.OrderDetails = funcs.getCleanListOfModels(await _context.OrderDetailCl.Where(e => e.OrderId == order.OrdersId).ToListAsync());
                foreach (OrderDetailCl detail in order.OrderDetails) 
                {
                    detail.Product = funcs.getCleanModel(await _context.ProductCl.FindAsync(detail.ProductId));
                    detail.Product.Image = funcs.getCleanModel(await _context.ImageCl.FindAsync(detail.Product.ImgId));
                }
            }

            return orders;
        }

        [Route("api/GetOrdersByCategory/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrdersCl>>> GetOrdersByCategory(int id)
        {
            var identity = funcs.identityToUser(User.Identity, _context);
            //Возвращать только те заказы, где ты являешься исполнителем, и статус не является завершенным
            var ordersFound = await _context.OrdersCl.Where(p => p.CategoryId == id && 
                                                            p.BrandOwnerId == identity.UserId && 
                                                            p.StatusId != _context.OrderStatusCl.First(s => s.OrderStatusName == "Завершено").OrderStatusId).ToListAsync();

            if (ordersFound == null)
            {
                return NotFound();
            }

            var orders = funcs.getCleanListOfModels(ordersFound);

            foreach (OrdersCl order in orders)
            {
                order.OrderDetails = funcs.getCleanListOfModels(_context.OrderDetailCl.Where(d => d.OrderId == order.OrdersId).ToList());
                foreach (OrderDetailCl detail in order.OrderDetails)
                {
                    if (detail.ProductId != null) //Если продукта больше не существует
                    {
                        detail.Product = funcs.getCleanModel(await _context.ProductCl.FindAsync(detail.ProductId));
                        detail.Product.Image = funcs.getCleanModel(await _context.ImageCl.FindAsync(detail.Product.ImgId));
                    }
                }
                order.User = funcs.getCleanUser(await _context.UserCl.FindAsync(order.UserId));
                var ownerBuffer = await _context.UserCl.FindAsync(order.BrandOwnerId);
                order.OrderStatus = funcs.getCleanModel(await _context.OrderStatusCl.FindAsync(order.StatusId));
                order.PaymentMethod = await _context.PaymentMethodCl.FindAsync(order.PaymentMethodId);
                if(order.PointsUsed)
                {
                    order.PointRegister = funcs.getCleanModel(await _context.PointRegisterCl.FindAsync(order.PointRegisterId));
                }
                if (order.BrandOwnerId != null)
                {
                    order.BrandOwner = funcs.getCleanUser(identity); //Не равен null если заказ взят отправившим запрос
                }
                if (order.BanknoteId != null)
                {
                    order.Banknote = await _context.BanknoteCl.FindAsync(order.BanknoteId);
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

            OrdersCl order;

            try
            {
                order = await _context.OrdersCl.FindAsync(waterRequest.OrderId);
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

            order.OrderDetails = await _context.OrderDetailCl.Where(e => e.OrderId == order.OrdersId).ToListAsync();

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
                foreach (OrderDetailCl detail in order.OrderDetails) 
                {
                    if (!listOfSuggestions.Any(e => e.ProductId == detail.ProductId))
                        return BadRequest();
                }
            }

            //получаем первый бренд отправителя
            var user = funcs.identityToUser(User.Identity, _context);
            var brands = _context.BrandCl.Where(e => e.UserId == user.UserId);
            var brand = brands.First();

            //... и проверяем наличие хотя бы одной записи с id этого бренда, подавляем попытку создать дубликат
            if (_context.WaterRequests.Where(e => e.OrderId == order.OrdersId).Any(e => e.BrandId == brand.BrandId)) 
            {
                return Forbid();
            }

            var request = new WaterRequest()
            {
                BrandId = brand.BrandId,
                OrderId = order.OrdersId
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
        public async Task<ActionResult<OrdersCl>> PostVodaOrders(OrdersCl ordersCl)
        {
            if (ordersCl == null || ordersCl.OrderDetails == null || ordersCl.OrderDetails.Count < 1)
            {
                return BadRequest();
            }

            foreach (OrderDetailCl detail in ordersCl.OrderDetails)
            {
                if (detail.Count < 1)
                {
                    return BadRequest();
                }
            }

            //filling blanks and sending to DB
            ordersCl.CreatedDate = DateTime.Now;
            ordersCl.StatusId = _context.OrderStatusCl.First(e => e.OrderStatusName == "Отправлено").OrderStatusId;
            ordersCl.OrderStatus = await _context.OrderStatusCl.FindAsync(ordersCl.StatusId);
            ordersCl.UserId = funcs.identityToUser(User.Identity, _context).UserId;
            ordersCl.User = await _context.UserCl.FindAsync(ordersCl.UserId);
            ordersCl.PaymentMethodId = 1; //Только налик

            //foreach (OrderDetailCl detail in ordersCl.OrderDetails) 
            //{
            //    _context.OrderDetailCl.Add(detail);
            //}
            //await _context.SaveChangesAsync();

            _context.OrdersCl.Add(ordersCl);
            await _context.SaveChangesAsync(); //вроде как рефрешит объект ordersCl

            //NotificationsController notificationsController = new NotificationsController();
            //await notificationsController.ToSendNotificationAsync(ordersCl.BrandOwner.DeviceType, "У Вас новый заказ", ordersCl.BrandOwner.NotificationRegistration);

            return Ok();
        }

        [Route("api/PutVodaOrders/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public async Task<ActionResult> PutVodaOrdersCl(int id)
        {
            //Сперва проверяем на физическую возможность смены статуса
            var order = await _context.OrdersCl.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            //Только пользователь и владелец бренда имеют доступ к смене статуса
            var identity = funcs.identityToUser(User.Identity, _context);

            //Если заказ занят - посылать куда подальше
            if (order.BrandOwnerId != null)
            {
                return Forbid();
            }

            order.BrandOwnerId = identity.UserId;
            order.BrandOwner = await _context.UserCl.FindAsync(identity.UserId);
            order.User = await _context.UserCl.FindAsync(order.UserId);

            await _context.SaveChangesAsync();

            await new NotificationsController().ToSendNotificationAsync(order.User.DeviceType, "Статус вашего заказа обновлен!", order.User.NotificationRegistration);

            return Ok();
        }
    }
}
