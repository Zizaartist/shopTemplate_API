using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.StaticValues
{
    public static class Constants
    {
        public static object CONSTRUCTOR_CALLER = null;

        //Статические продукты
        public readonly static int PRODUCT_ID_BOTTLED_WATER = 9;
        public readonly static int PRODUCT_ID_WATER = 10;
        public readonly static int PRODUCT_ID_CONTAINER = 11;
        public readonly static int PRODUCT_ID_DELIVERY = 19;

        public readonly static TimeSpan YAKUTSK_OFFSET = new TimeSpan(9, 0, 0); //GMT +9

        public static void Init() 
        {
            //ClickContext _context = new ClickContext();
            //PRODUCT_ID_BOTTLED_WATER = _context.Products.First(e => e.ProductName == "Бутилированная вода").ProductId;
            //PRODUCT_ID_WATER = _context.Products.First(e => e.ProductName == "Вода").ProductId;
            //PRODUCT_ID_CONTAINER = _context.Products.First(e => e.ProductName == "Тара для воды").ProductId;
        }
    }
}
