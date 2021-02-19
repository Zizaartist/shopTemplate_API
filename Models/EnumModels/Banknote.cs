using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models.EnumModels
{
    /// <summary>
    /// Модель, хранящая номиналы банкнот, сдачу для которых должен иметь при себе курьер
    /// </summary>
    public enum Banknote
    {
        fiveHundred,
        thousand,
        twoThousand,
        fiveThousand
    }

    public class BanknoteDictionaries 
    {
        public static Dictionary<string, Banknote> GetBanknoteFromString = new Dictionary<string, Banknote>()
        {
            { "500", Banknote.fiveHundred },
            { "1000", Banknote.thousand },
            { "2000", Banknote.twoThousand },
            { "5000", Banknote.fiveThousand }
        };

        public static Dictionary<Banknote, string> GetStringFromBanknote = new Dictionary<Banknote, string>
        {
            { Banknote.fiveHundred, "500" },
            { Banknote.thousand, "1000" },
            { Banknote.twoThousand, "2000" },
            { Banknote.fiveThousand, "5000" }
        };
    }
    
}
