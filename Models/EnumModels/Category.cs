using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    /// <summary>
    /// Enum модель, отвечающая за хранение категорий
    /// </summary>
    public enum Category
    {
        food,
        bottledWater,
        water,
        flowers
    }

    public class CategoryDictionaries
    {
        public static Dictionary<string, Category> GetCategoryFromString = new Dictionary<string, Category>()
        {
            { "Еда", Category.food },
            { "Бутилированная вода", Category.bottledWater },
            { "Вода", Category.water },
            { "Цветы", Category.flowers }
        };

        public static Dictionary<Category, string> GetStringFromCategory = new Dictionary<Category, string>
        {
            { Category.food, "Еда" },
            { Category.bottledWater, "Бутилированная вода" },
            { Category.water, "Вода" },
            { Category.flowers, "Цветы" }
        };
    }
}
