using ApiClick.StaticValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    public class ErrorReport
    {
        //Not nullable
        [Key]
        public int ErrorReportId { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_MAX)]
        public string Text { get; set; }

        //Nullable
        public int? UserId { get; set; }

        //Nav. properties
        public virtual User User { get; set; }

        /// <summary>
        /// Проверяет валидность модели, полученной от клиента
        /// </summary>
        public static bool ModelIsValid(ErrorReport _errorReport)
        {
            try
            {
                if (_errorReport == null ||
                    //Required
                    string.IsNullOrEmpty(_errorReport.Text))
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex)
            {
                Debug.WriteLine($"Ошибка при проверке данных отчета об ошибке lmao - {_ex}");
                return false;
            }
        }
    }
}
