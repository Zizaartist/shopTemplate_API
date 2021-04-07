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
    public class CategoriesController : ControllerBase
    {
        private readonly ClickContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ClickContext _context, ILogger<CategoriesController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        // GET: api/BrandsMenu
        //Debug
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var menus = await _context.Category.ToListAsync();

            return menus;
        }

        // GET: api/GetMenusByBrand/5
        //Возвращает список меню по id бренда
        [Route("GetMenusByBrand/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetMenusByBrand(int id)
        {
            var categories = _context.Category.Where(p => p.BrandId == id);

            if (!categories.Any())
            {
                return NotFound();
            }

            var result = categories.ToList();

            return result;
        }

        // GET: api/BrandsMenu/5
        //Возвращает меню по id
        [Route("{id}")]
        [Authorize(Roles = "SuperAdmin, Admin, User")]
        [HttpGet]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/BrandsMenu/5
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPut]
        public async Task<IActionResult> PutCategories(Category _categoryData)
        {
            if (!IsPutModelValid(_categoryData))
            {
                return BadRequest();
            }

            var category = _context.Category.Find(_categoryData.CategoryId);

            if (category == null) 
            {
                return NotFound();
            }

            if (!IsOwner(category)) 
            {
                return Forbid();
            }

            category.CategoryName = _categoryData.CategoryName;
            if (!string.IsNullOrEmpty(_categoryData.Image)) category.Image = _categoryData.Image;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/BrandsMenu
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public ActionResult<Category> PostCategory(Category _category)
        {
            if (!IsPostModelValid(_category)) 
            {
                return BadRequest();
            }

            if (!IsOwner(_category))
            {
                return Forbid();
            }

            _category.CreatedDate = DateTime.UtcNow.Date;

            _context.Category.Add(_category);
            _context.SaveChanges();

            return Ok();
        }

        // DELETE: api/BrandsMenu/5
        [Route("api/[controller]/{id}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpDelete]
        public ActionResult<Category> DeleteCategories(int id)
        {
            var category = _context.Category.Find(id);

            if (category == null) 
            {
                return NotFound();
            }

            if (!IsOwner(category))
            {
                return Forbid();
            }

            _context.Category.Remove(category);
            _context.SaveChanges();

            return Ok();
        }

        private bool IsOwner(Category _categoryData) 
        {
            var mySelf = Functions.identityToUser(User.Identity, _context).Executor;
            var brand = _context.Brand.Find(_categoryData.BrandId);

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
        private bool IsPutModelValid(Category _category)
        {
            try
            {
                if (_category == null ||
                    _category.CategoryId <= 0 ||
                    string.IsNullOrEmpty(_category.CategoryName) ||
                    IsCategoryNameTaken(_category.CategoryName, _category.BrandId))
                {
                    return false;
                }

                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации Category модели PUT метода - {_ex}");
                return false;
            }
        }

        /// <summary>
        /// Валидация получаемых данных
        /// </summary>
        /// <returns>Полученные данные являются допустимыми</returns>
        private bool IsPostModelValid(Category _category)
        {
            try
            {
                if (_category == null ||
                    _category.BrandId <= 0 ||
                    string.IsNullOrEmpty(_category.CategoryName) ||
                    IsCategoryNameTaken(_category.CategoryName, _category.BrandId))
                {
                    return false;
                }

                return true;
            }
            catch (Exception _ex)
            {
                _logger.LogWarning($"Ошибка при валидации Category модели POST метода - {_ex}");
                return false;
            }
        }

        private bool IsCategoryNameTaken(string _suggestedName, int _brandId)
        {
            var caps = _suggestedName.ToUpper();
            if (_context.Category.Any(cat => cat.CategoryName.ToUpper() == caps && cat.BrandId == _brandId))
            {
                return true;
            }
            return false;
        }
    }
}
