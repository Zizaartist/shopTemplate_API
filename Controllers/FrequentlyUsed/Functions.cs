using ApiClick.Models;
using ApiClick.Models.EnumModels;
using ApiClick.StaticValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiClick.Controllers.FrequentlyUsed
{
    public class Functions
    {
        private List<Type> allowedTypes = new List<Type>() { typeof(int?), 
                                                            typeof(string), 
                                                            typeof(decimal),
                                                            typeof(DateTime),
                                                            typeof(Banknote),
                                                            typeof(Category),
                                                            typeof(OrderStatus),
                                                            typeof(PaymentMethod),
                                                            typeof(UserRole)};

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
        public User getCleanUser(User input) 
        {
            var cleanObj = new User()
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
        public User identityToUser(IIdentity identity, ClickContext _context)
        {
            return _context.Users.FirstOrDefault(u => u.Phone == identity.Name);
        }

        /// <summary>
        /// Возвращает _count элементов начиная со страницы _startingPage
        /// </summary>
        /// <param name="_initialQuery">Изначальный набор</param>
        /// <param name="_startingPage">Начальный индекс выборки</param>
        /// <param name="_pageSize">Количество элементов на странице</param>
        public IQueryable<T> GetPageRange<T>(IQueryable<T> _initialQuery, int _startingPage, int _pageSize) 
        {
            //страница 0, 20 элементов = 0-19 элементы
            //страница 5, 10 элементов = 50-59 элементы
            return _initialQuery.Skip(_startingPage * _pageSize).Take(_pageSize);
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
            if (originalNumber == null) 
            {
                return null;
            }
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

        public bool phoneIsRegistered(string correctPhone, ClickContext _context)
        {
            var user = _context.Users.FirstOrDefault(u => u.Phone == correctPhone);
            return user != null;
        }
    }
}
