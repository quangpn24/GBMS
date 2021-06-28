using GemstonesBusinessManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GemstonesBusinessManagementSystem.Validations
{
    class NotExistValidation : ValidationRule
    {
        public string ErrorMessage { get; set; }
        public string ErrorMessageNull { get; set; }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            if (value == null)
            {
                return new ValidationResult(true, null);
            }
            if (value.ToString() == "")
            {
                return new ValidationResult(false, this.ErrorMessageNull);
            }
            if (!AccountDAL.Instance.IsExistUsername(value.ToString()))
            {
                result = new ValidationResult(false, this.ErrorMessage);
            }
            return result;
        }
    }

}
