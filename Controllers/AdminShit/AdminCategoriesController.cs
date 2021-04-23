using ApiClick.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Controllers.AdminShit
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly ShopContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public AdminCategoriesController(ShopContext _context, ILogger<CategoriesController> _logger)
        {
            this._context = _context;
            this._logger = _logger;
        }

        [Route("{id}")]
        [HttpPost]


        [Route("{id}")]
        [HttpDelete]
        public ActionResult Remove(int id)
        {
            var category = _context.Category.Include(cat => cat.ChildCategories)
                                            .Include(cat => cat.Products)
                                            .FirstOrDefault(cat => cat.CategoryId == id);

            RecursiveRemove(category);

            _context.SaveChanges();

            return Ok();
        }

        private void RecursiveRemove(Category parentEntity)
        {
            if (parentEntity.ChildCategories != null && parentEntity.ChildCategories.Any())
            {
                var children = _context.Category.Include(cat => cat.ChildCategories)
                                                .Include(cat => cat.Products)
                                                .Where(cat => cat.ParentCategoryId == parentEntity.CategoryId);

                foreach (var childCategory in children)
                {
                    RecursiveRemove(childCategory);
                }
            }

            _context.Category.Remove(parentEntity);
        }

        ///// <summary>
        ///// Возвращает список категорий принадлежащих бренду
        ///// </summary>
        ///// <param name="id">Id бренда</param>
        //// GET: api/Categories/ByBrand/5
        //[Route("ByBrand/{id}")]
        //[Authorize]
        //[HttpGet]
        //public ActionResult<IEnumerable<Category>> GetMenusByBrand(int id)
        //{
        //    var categories = _context.Category.Where(p => p.BrandId == id);

        //    if (!categories.Any())
        //    {
        //        return NotFound();
        //    }

        //    var result = categories.ToList();

        //    return result;
        //}

        ///// <summary>
        ///// Изменяет данные категории
        ///// </summary>
        ///// <param name="_categoryData">Новые данные категории</param>
        //// PUT: api/Categories
        //[Authorize]
        //[HttpPut]
        //public IActionResult PutCategories(Category _categoryData)
        //{
        //    if (!IsPutModelValid(_categoryData))
        //    {
        //        return BadRequest();
        //    }

        //    var category = _context.Category.Find(_categoryData.CategoryId);

        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    if (!IsOwner(category))
        //    {
        //        return Forbid();
        //    }

        //    category.CategoryName = _categoryData.CategoryName;
        //    if (!string.IsNullOrEmpty(_categoryData.Image)) category.Image = _categoryData.Image;

        //    _context.SaveChanges();

        //    return Ok();
        //}

        ///// <summary>
        ///// Создает новую категорию в бренде отправителя
        ///// </summary>
        ///// <param name="_category">Данные новой категории</param>
        //// POST: api/Categories
        //[Authorize]
        //[HttpPost]
        //public ActionResult<Category> PostCategory(Category _category)
        //{
        //    if (!IsPostModelValid(_category))
        //    {
        //        return BadRequest();
        //    }

        //    var mySelf = Functions.identityToUser(User.Identity, _context);

        //    var myBrand = _context.Brand.FirstOrDefault(brand => brand.ExecutorId == mySelf.Executor.ExecutorId);

        //    if (myBrand == null)
        //    {
        //        return Forbid();
        //    }

        //    _category.BrandId = myBrand.BrandId;
        //    _category.CreatedDate = DateTime.UtcNow.Date;

        //    //Preventing exploitations
        //    _category.Brand = null;
        //    _category.Products = null;

        //    _context.Category.Add(_category);
        //    _context.SaveChanges();

        //    return Ok();
        //}

        ///// <summary>
        ///// Удаляет выбранную категорию
        ///// </summary>
        ///// <param name="id">Id удаляемой категории</param>
        //// DELETE: api/Categories/5
        //[Route("{id}")]
        //[Authorize]
        //[HttpDelete]
        //public ActionResult<Category> DeleteCategories(int id)
        //{
        //    var category = _context.Category.Find(id);

        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    if (!IsOwner(category))
        //    {
        //        return Forbid();
        //    }

        //    _context.Category.Remove(category);
        //    _context.SaveChanges();

        //    return Ok();
        //}

        //private bool IsOwner(Category _categoryData)
        //{
        //    var mySelf = Functions.identityToUser(User.Identity, _context).Executor;
        //    var brand = _context.Brand.Find(_categoryData.BrandId);

        //    if (brand == null || brand.ExecutorId != mySelf.ExecutorId)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// Валидация получаемых данных
        ///// </summary>
        ///// <returns>Полученные данные являются допустимыми</returns>
        //private bool IsPutModelValid(Category _category)
        //{
        //    try
        //    {
        //        if (_category == null ||
        //            _category.CategoryId <= 0 ||
        //            string.IsNullOrEmpty(_category.CategoryName) ||
        //            IsCategoryNameTaken(_category.CategoryName, _category.BrandId))
        //        {
        //            return false;
        //        }

        //        return true;
        //    }
        //    catch (Exception _ex)
        //    {
        //        _logger.LogWarning($"Ошибка при валидации Category модели PUT метода - {_ex}");
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Валидация получаемых данных
        ///// </summary>
        ///// <returns>Полученные данные являются допустимыми</returns>
        //private bool IsPostModelValid(Category _category)
        //{
        //    try
        //    {
        //        if (_category == null ||
        //            string.IsNullOrEmpty(_category.CategoryName) ||
        //            IsCategoryNameTaken(_category.CategoryName, _category.BrandId))
        //        {
        //            return false;
        //        }

        //        return true;
        //    }
        //    catch (Exception _ex)
        //    {
        //        _logger.LogWarning($"Ошибка при валидации Category модели POST метода - {_ex}");
        //        return false;
        //    }
        //}

        //private bool IsCategoryNameTaken(string _suggestedName, int _brandId)
        //{
        //    var caps = _suggestedName.ToUpper();
        //    if (_context.Category.Any(cat => cat.CategoryName.ToUpper() == caps && cat.BrandId == _brandId))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
