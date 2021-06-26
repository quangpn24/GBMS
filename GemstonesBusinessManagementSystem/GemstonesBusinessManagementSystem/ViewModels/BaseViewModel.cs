using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string MD5Hash(string str)
        {
            StringBuilder hash = new StringBuilder();
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(new UTF8Encoding().GetBytes(str));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("X2"));
            }
            return hash.ToString();
        }
        //chuyển từ format seperate thousands sang kiểu số nguyên
        public long ConvertToNumber(string str)
        {
            string[] s = str.Split(',');
            string tmp = "";
            foreach (string a in s)
            {
                tmp += a;
            }

            return long.Parse(tmp);
        }
        //Chuyển sang dạng 0,000,000
        public void SeparateThousands(TextBox txt)
        {
            if (!string.IsNullOrEmpty(txt.Text))
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                ulong valueBefore = ulong.Parse(ConvertToNumber(txt.Text).ToString(), System.Globalization.NumberStyles.AllowThousands);
                txt.Text = String.Format(culture, "{0:N0}", valueBefore);
                txt.Select(txt.Text.Length, 0);
            }
        }
        public string SeparateThousands(String text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                ulong valueBefore = ulong.Parse(ConvertToNumber(text).ToString(), System.Globalization.NumberStyles.AllowThousands);
                string res = String.Format(culture, "{0:N0}", valueBefore);
                return res;
            }
            return "";
        }
        //Chỉ cho nhập số
        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        public void NumberValidationTextBox(object sender, KeyEventArgs e)
        {

            e.Handled = e.Key == Key.Space;
        }
        //Thêm tiền tố cho mã
        public string AddPrefix(string namePrefix, int id)
        {
            int temp = id;
            string prefix = namePrefix;
            while (temp < 1000)
            {
                temp *= 10;
                prefix += "0";
            }
            return prefix + id.ToString();
        }
        public string ConvertToIDString(string input)
        {
            return input.Remove(0, 2);
        }
        public int ConvertToID(string input)
        {
            return int.Parse(input.Remove(0, 2));
        }
        public BindingBase CloneBinding(BindingBase bindingBase, object source)
        {
            var binding = bindingBase as Binding;
            if (binding != null)
            {
                var result = new Binding
                {
                    Source = source,
                    AsyncState = binding.AsyncState,
                    BindingGroupName = binding.BindingGroupName,
                    BindsDirectlyToSource = binding.BindsDirectlyToSource,
                    Converter = binding.Converter,
                    ConverterCulture = binding.ConverterCulture,
                    ConverterParameter = binding.ConverterParameter,
                    //ElementName = binding.ElementName,
                    FallbackValue = binding.FallbackValue,
                    IsAsync = binding.IsAsync,
                    Mode = binding.Mode,
                    NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
                    NotifyOnValidationError = binding.NotifyOnValidationError,
                    Path = binding.Path,
                    //RelativeSource = binding.RelativeSource,
                    StringFormat = binding.StringFormat,
                    TargetNullValue = binding.TargetNullValue,
                    UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
                    UpdateSourceTrigger = binding.UpdateSourceTrigger,
                    ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
                    ValidatesOnExceptions = binding.ValidatesOnExceptions,
                    XPath = binding.XPath,
                };

                foreach (var validationRule in binding.ValidationRules)
                {
                    result.ValidationRules.Add(validationRule);
                }

                return result;
            }

            var multiBinding = bindingBase as MultiBinding;
            if (multiBinding != null)
            {
                var result = new MultiBinding
                {
                    BindingGroupName = multiBinding.BindingGroupName,
                    Converter = multiBinding.Converter,
                    ConverterCulture = multiBinding.ConverterCulture,
                    ConverterParameter = multiBinding.ConverterParameter,
                    FallbackValue = multiBinding.FallbackValue,
                    Mode = multiBinding.Mode,
                    NotifyOnSourceUpdated = multiBinding.NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = multiBinding.NotifyOnTargetUpdated,
                    NotifyOnValidationError = multiBinding.NotifyOnValidationError,
                    StringFormat = multiBinding.StringFormat,
                    TargetNullValue = multiBinding.TargetNullValue,
                    UpdateSourceExceptionFilter = multiBinding.UpdateSourceExceptionFilter,
                    UpdateSourceTrigger = multiBinding.UpdateSourceTrigger,
                    ValidatesOnDataErrors = multiBinding.ValidatesOnDataErrors,
                    ValidatesOnExceptions = multiBinding.ValidatesOnDataErrors,
                };

                foreach (var validationRule in multiBinding.ValidationRules)
                {
                    result.ValidationRules.Add(validationRule);
                }

                foreach (var childBinding in multiBinding.Bindings)
                {
                    result.Bindings.Add(CloneBinding(childBinding, source));
                }

                return result;
            }

            var priorityBinding = bindingBase as PriorityBinding;
            if (priorityBinding != null)
            {
                var result = new PriorityBinding
                {
                    BindingGroupName = priorityBinding.BindingGroupName,
                    FallbackValue = priorityBinding.FallbackValue,
                    StringFormat = priorityBinding.StringFormat,
                    TargetNullValue = priorityBinding.TargetNullValue,
                };

                foreach (var childBinding in priorityBinding.Bindings)
                {
                    result.Bindings.Add(CloneBinding(childBinding, source));
                }

                return result;
            }

            throw new NotSupportedException("Failed to clone binding");
        }

    }
}
