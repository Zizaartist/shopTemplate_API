﻿using System;
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
using ApiClick.StaticValues;
using ApiClick.Controllers.FrequentlyUsed;
using Microsoft.Extensions.Logging;

namespace ApiClick.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ClickContext _context;
        private readonly ILogger<ProductsController> _logger;
        public static int PAGE_SIZE = 5;

        public ProductsController(ClickContext _context, ILogger<ProductsController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        /// <summary>
        /// Возвращает продукцию из указанной категории
        /// </summary>
        /// <param name="id">Id категории</param>
        /// <param name="_page">Страница</param>
        /// <returns></returns>
        // GET: api/Products/ByMenu/5/1
        [Route("ByMenu/{id}/{_page}")]
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProductsByMenu(int id, int _page)
        {
            var products = _context.Product.Where(p => p.CategoryId == id); 
            
            products = Functions.GetPageRange(products, _page, PAGE_SIZE);

            if (!products.Any())
            {
                return NotFound();
            }

            var result = products.ToList();

            return result;
        }

        /// <summary>
        /// Изменение данных продукции
        /// </summary>
        /// <param name="_productData">Новые данные продукции</param>
        // PUT: api/Products
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public ActionResult PutProducts(Product _productData)
        {
            if (!IsPutModelValid(_productData))
            {
                return BadRequest();
            }

            var product = _context.Product.Find(_productData.ProductId);

            if (product == null) 
            {
                return NotFound();
            }

            if (!IsOwner(product)) 
            {
                return Forbid();
            }

            product.Description = _productData.Description;
            if (!string.IsNullOrEmpty(_productData.Image)) product.Image = _productData.Image;
            product.Price = _productData.Price;
            product.PriceDiscount = _productData.PriceDiscount;
            product.ProductName = _productData.ProductName;

            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Создает новую продукцию
        /// </summary>
        /// <param name="_product">Данные новой продукции</param>
        // POST: api/Products
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public ActionResult PostProducts(Product _product)
        {
            if (!IsPostModelValid(_product)) 
            {
                return BadRequest();
            }

            if (!IsOwner(_product)) 
            {
                return Forbid();
            }

            _product.CreatedDate = DateTime.UtcNow;

            //preventing exploitations
            _product.Category = null;
            _product.Reports = null;
            _product.OrderDetails = null;

            _context.Product.Add(_product);
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Удаляет выбранную продукцию
        /// </summary>
        // DELETE: api/Products/5
        [Route("{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
        public ActionResult<Product> DeleteProducts(int id)
        {
            var product = _context.Product.Find(id);

            if (product == null) 
            {
                return BadRequest();
            }

            if (!IsOwner(product)) 
            {
                return Forbid();
            }

            _context.Product.Remove(product);
            _context.SaveChanges();

            return Ok();
        }

        private bool IsOwner(Product _product)
        {
            var mySelf = Functions.identityToUser(User.Identity, _context).Executor;
            var category = _context.Category.Find(_product.CategoryId);

            if (category == null) 
            {
                return false;
            }

            var brand = _context.Brand.Find(category.BrandId);

            if (brand == null || brand.ExecutorId != mySelf.ExecutorId)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Валидация получаемых данных
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsPutModelValid(Product _product)
        {
            try
            {
                if (_product == null ||
                    _product.Price <= 0 ||
                    string.IsNullOrEmpty(_product.ProductName) ||
                    (_product.PriceDiscount != null && _product.PriceDiscount <= 0))
                {
                    return false;
                }
                var product = _context.Product.Find(_product.ProductId);
                if (product == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации Product модели PUT метода - {_ex}");
                return false;
            }
        }

        /// <summary>
        /// Валидация получаемых данных
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsPostModelValid(Product _product)
        {
            try
            {
                if (_product == null ||
                    _product.Price <= 0 ||
                    string.IsNullOrEmpty(_product.ProductName) ||
                    (_product.PriceDiscount != null && _product.PriceDiscount <= 0))
                {
                    return false;
                }

                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации Product модели PUT метода - {_ex}");
                return false;
            }
        }
    }
}
