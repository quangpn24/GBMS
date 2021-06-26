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
    class ExistValidation : ValidationRule
    {
        public string ErrorMessage { get; set; }
        public string ErrorMessageNull { get; set; }
        public string ElementName { get; set; }


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
            switch (ElementName)
            {
                case "UserName":
                    if (AccountDAL.Instance.IsExistUserName(value.ToString()))
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                    break;
                case "GoodsName":
                    if (GoodsDAL.Instance.IsExistGoodsName(value.ToString()))
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                    break;
                case "PositionEmployee":
                    if (EmployeePositionDAL.Instance.IsExistPosition(value.ToString()))
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                    break;
                case "Membership":
                    if (MembershipsTypeDAL.Instance.IsExistMembership(value.ToString()))
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                    break;
                case "ServiceName":
                    if (ServiceDAL.Instance.IsExistServiceName(value.ToString()))
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                    break;
                case "GoodsTypeName":
                    if (GoodsTypeDAL.Instance.IsExistGoodsTypeName(value.ToString()))
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                    break;
                case "StandardDays":
                    if (string.Compare(value.ToString(), "30") > 0 || string.Compare(value.ToString(), "1") < 0)
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                    break;
                default:
                    break;
            }
            return result;
        }
    }

}
