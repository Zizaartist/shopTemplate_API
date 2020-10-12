using ApiClick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
