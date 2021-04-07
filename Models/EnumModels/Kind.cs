using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    /// <summary>
    /// Вид товара
    /// </summary>
    public enum Kind
    {
        food,
        bottledWater,
        water,
        flowers
    }

    public class KindDictionaries
    {
        public static Dictionary<string, Kind> GetKindFromString = new Dictionary<string, Kind>()
        {
            { "Еда", Kind.food },
            { "Бутилированная вода", Kind.bottledWater },
            { "Вода", Kind.water },
            { "Цветы", Kind.flowers }
        };

        public static Dictionary<Kind, string> GetStringFromKind = new Dictionary<Kind, string>
        {
            { Kind.food, "Еда" },
            { Kind.bottledWater, "Бутилированная вода" },
            { Kind.water, "Вода" },
            { Kind.flowers, "Цветы" }
        };
    }
}
