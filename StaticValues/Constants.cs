using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.StaticValues
{
    public static class Constants
    {
        public static object CONSTRUCTOR_CALLER = null;

        //Product ID
        public readonly static int PRODUCT_ID_BOTTLED_WATER = 9;
        public readonly static int PRODUCT_ID_WATER = 10;
        public readonly static int PRODUCT_ID_CONTAINER = 11;

        public static void Init() 
        {
            //ClickContext _context = new ClickContext();
            //PRODUCT_ID_BOTTLED_WATER = _context.ProductCl.First(e => e.ProductName == "Бутилированная вода").ProductId;
            //PRODUCT_ID_WATER = _context.ProductCl.First(e => e.ProductName == "Вода").ProductId;
            //PRODUCT_ID_CONTAINER = _context.ProductCl.First(e => e.ProductName == "Тара для воды").ProductId;
        }
    }
}
