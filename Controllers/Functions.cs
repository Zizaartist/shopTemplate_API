using ApiClick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiClick.Controllers
{
    public class Functions
    {
        private List<Type> allowedTypes = new List<Type>() { typeof(int?), 
                                                            typeof(string), 
                                                            typeof(decimal) };

        /// <summary>
        /// Создает новый экземпляр модели и заполняет его только примитивными типами, без навигационных свойств
        /// </summary>
        public T getCleanModel<T>(T input) 
        {
            T obj = (T)Activator.CreateInstance(typeof(T));
            //Получаем список свойств примитивного типа
            var listOfProperties = obj.GetType().GetProperties().Where(e => e.PropertyType.IsPrimitive || allowedTypes.Contains(e.PropertyType)).ToList();
            //Заполняем значениями входного параметра пустой объект 
            listOfProperties.ForEach(e => e.SetValue(obj, e.GetValue(input)));

            return obj;
        }

        /// <summary>
        /// Возвращает список объектов без навигационных свойств
        /// </summary>
        public List<T> getCleanListOfModels<T>(List<T> input) 
        {
            List<T> result = new List<T>();
            input.ForEach(e => result.Add(getCleanModel(e)));
            return result;
        }

        /// <summary>
        /// Возвращает чистую модель пользователя без личных данных
        /// </summary>
        public UserCl getCleanUser(UserCl input) 
        {
            var cleanObj = new UserCl()
            {
                Name = input.Name,
                Phone = input.Phone
            };
            return cleanObj;
        }

        /// <summary>
        /// Возвращает владельца личности токена
        /// </summary>
        /// <param name="identity">Данные личности, взятые из токена</param>
        /// <param name="_context">Контекст, в котором производится поиск</param>
        /// <returns>Пользователь, найденный в контексте</returns>
        public UserCl identityToUser(IIdentity identity, ClickContext _context)
        {
            return _context.UserCl.FirstOrDefault(u => u.Phone == identity.Name);
        }

        public bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^((8|\+7|7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$").Success;
        }

        /// <summary>
        /// Конвертирует телефон в единый формат
        /// </summary>
        public string convertNormalPhoneNumber(string originalNumber) 
        {
            //Базировал на https://bit.ly/3lEsT2R
            string processedNumber = originalNumber;
            //Сперва удаляем лишние символы
            List<string> junkSymbols = new List<string>() 
            {
                "(", ")", "+", "-"
            };
            junkSymbols.ForEach(e => processedNumber = processedNumber.Replace(e, ""));
            //Если в начале нет 7 или 8 - вставить код самому. Пока плевать на интернационализацию
            return "7" + ((processedNumber.StartsWith("7") || processedNumber.StartsWith("8")) ? 
                                        processedNumber.Substring(1) : processedNumber);
        }
    }
}
