using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class SettingViewModel : BaseViewModel
    {
        // id 1: Prepayment PerCent
        //     2: Store Name
        //     3: Store Address
        //     4: Store Phone Number
        //     5: Store Email
        private List<Parameter> parameters = ParameterDAL.Instance.GetData();

        private string prepaymentPercent;
        private string storeName;
        private string storeAddress;
        private string phoneNumber;
        private string email;
        private string password;
        private string newPassword;
        private string confirmNewPassword;
        public string PrepaymentPercent { get => prepaymentPercent; set { prepaymentPercent = value; OnPropertyChanged(); } }
        public string StoreName { get => storeName; set { storeName = value; OnPropertyChanged(); } }
        public string StoreAddress { get => storeAddress; set { storeAddress = value; OnPropertyChanged(); } }
        public string PhoneNumber { get => phoneNumber; set { phoneNumber = value; OnPropertyChanged(); } }
        public string Email { get => email; set { email = value; OnPropertyChanged(); } }

        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        public string NewPassword { get => newPassword; set { newPassword = value; OnPropertyChanged(); } }
        public string ConfirmNewPassword { get => confirmNewPassword; set { confirmNewPassword = value; OnPropertyChanged(); } }

        //Personal info window
        private string name;
        private string userAddress;
        private string birthDate;
        private string userPhoneNumber;
        public string Name { get => name; set { name = value; OnPropertyChanged(); } }
        public string UserAddress { get => userAddress; set { userAddress = value; OnPropertyChanged(); } }
        public string UserPhoneNumber { get => userPhoneNumber; set { userPhoneNumber = value; OnPropertyChanged(); } }
        public string BirthDate { get => birthDate; set { birthDate = value; OnPropertyChanged(); } }

        string imageFileName;
        //Command
        public ICommand LoadDefaultCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand Update_PrepaymentCommand { get; set; }
        public ICommand Undo_PrepaymentCommand { get; set; }
        public ICommand Update_StoreInfoCommand { get; set; }
        public ICommand Undo_StoreInfoCommand { get; set; }
        public ICommand ClickAvatarCommand { get; set; }
        // Change user info & change password
        public ICommand SelectImageCommand { get; set; }
        public ICommand UpdateUserInfoCommand { get; set; }
        public ICommand ExitCommand{ get; set; }
        public ICommand OpenPersonalInfoWindowCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }
        public ICommand OpenChangePasswordWindowCommand { get; set; }


        public SettingViewModel()
        {
            LoadDefaultCommand = new RelayCommand<MainWindow>(p => true, p => LoadDefault(p));
            Update_PrepaymentCommand = new RelayCommand<MainWindow>(p => true, p => UpdatePrepayment(p));
            Undo_PrepaymentCommand = new RelayCommand<MainWindow>(p => true, p => UndoPrepayment(p));
            Update_StoreInfoCommand = new RelayCommand<MainWindow>(p => true, p => UpdateStoreInfo(p));
            Undo_StoreInfoCommand = new RelayCommand<MainWindow>(p => true, p => UndoStoreInfo(p));

            ClickAvatarCommand = new RelayCommand<MainWindow>(p => true, p => {
                p.grdChangeInfo.Visibility = Visibility.Visible;
                p.btnPersonalInfo.Focus();
                });
            OpenPersonalInfoWindowCommand = new RelayCommand<MainWindow>(p => true, p => OpenPersonalInfoWindow(p));
            OpenChangePasswordWindowCommand = new RelayCommand<MainWindow>(p => true, p => OpenChangePasswordWindow(p));

            SelectImageCommand = new RelayCommand<Grid>(p => true, p => SelectImage(p));
            UpdateUserInfoCommand = new RelayCommand<PersonalInfoWindow>(p => true, p => UpdateUserInfo(p));

            ExitCommand = new RelayCommand<Window>(p => true, p => p.Close());

            ChangePasswordCommand = new RelayCommand<ChangePasswordWindow>(p => true, p => ChangePassword(p));
        }
        void LoadDefault(MainWindow main)
        {
            //Thông tin cửa hàng
            for (int i = 0; i < parameters.Count; i++)
            {
                switch (parameters[i].IdParameter)
                {
                    case 1:
                        PrepaymentPercent = parameters[i].Value;
                        main.txtPrepayment.SelectionStart = 0;
                        main.txtPrepayment.SelectionLength = PrepaymentPercent.Length;
                        break;
                    case 2:
                        StoreName = parameters[i].Value;
                        main.txtStoreName.SelectionStart = 0;
                        main.txtStoreName.SelectionLength = StoreName.Length;
                        break;
                    case 3:
                        StoreAddress = parameters[i].Value;
                        main.txtStoreAddress.SelectionStart = 0;
                        main.txtStoreAddress.SelectionLength = storeAddress.Length;
                        break;
                    case 4:
                        PhoneNumber = parameters[i].Value;
                        main.txtStorePhoneNumber.SelectionStart = 0;
                        main.txtStorePhoneNumber.SelectionLength = PhoneNumber.Length;
                        break;
                    case 5:
                        Email = parameters[i].Value;
                        main.txtEmail.SelectionStart = 0;
                        main.txtEmail.SelectionLength = Email.Length;
                        break;
                    default:
                        break;
                }
            }
        }

        void UpdatePrepayment(MainWindow main)
        {
            if (ParameterDAL.Instance.UpdatePrepayment(PrepaymentPercent))
            {
                parameters[0].Value = PrepaymentPercent;
                MessageBox.Show("Thành công!");
            }
            else
            {
                MessageBox.Show("Thất bại!");
            }
        }
        void UpdateStoreInfo(MainWindow main)
        {
            bool k1 = ParameterDAL.Instance.UpdateStoreInfo(2, StoreName);
            bool k2 = ParameterDAL.Instance.UpdateStoreInfo(3, StoreAddress);
            bool k3 = ParameterDAL.Instance.UpdateStoreInfo(4, PhoneNumber);
            bool k4 = ParameterDAL.Instance.UpdateStoreInfo(5, Email);
            if (k1 && k2 && k3 && k4)
            {
                parameters[1].Value = StoreName;
                parameters[2].Value = StoreAddress;
                parameters[3].Value = PhoneNumber;
                parameters[4].Value = Email;
                MessageBox.Show("Thành công!");
            }
            else
            {
                MessageBox.Show("Thất bại!");
            }
        }

        void UndoPrepayment(MainWindow main)
        {
            PrepaymentPercent = parameters[0].Value;
        }
        void UndoStoreInfo(MainWindow main)
        {
            StoreName = parameters[1].Value;
            StoreAddress = parameters[2].Value;
            PhoneNumber = parameters[3].Value;
            Email = parameters[4].Value;
        }

        //window user info
        public void SelectImage(Grid parameter)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imageFileName = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imageFileName);
                bitmap.EndInit();
                imageBrush.ImageSource = bitmap;
                parameter.Background = imageBrush;
                if (parameter.Children.Count > 1)
                {
                    parameter.Children.Remove(parameter.Children[0]);
                    parameter.Children.Remove(parameter.Children[1]);
                }
            }
        }

        public void UpdateUserInfo(PersonalInfoWindow window)
        {
            if (string.IsNullOrEmpty(Name))
            {
                MessageBox.Show("Nhập tên");
                return;
            }
            if (string.IsNullOrEmpty(BirthDate))
            {
                MessageBox.Show("Nhập ngày sinh");
                return;
            }
            if (string.IsNullOrEmpty(UserAddress))
            {
                MessageBox.Show("Nhập địa chỉ");
                return;
            }
            if (string.IsNullOrEmpty(UserPhoneNumber))
            {
                MessageBox.Show("Nhập số điện thoại");
                return;
            }
            byte[] imgByteArr;

            ImageBrush imageBrush = (ImageBrush)window.grdSelectImage.Background;
            if (imageBrush == null)
            {
                MessageBox.Show("Vui lòng chọn ảnh!");
                return;
            }
            imgByteArr = Converter.Instance.ConvertBitmapImageToBytes((BitmapImage)imageBrush.ImageSource);
            string gender;
            if (window.rdoMale.IsChecked.Value == true)
                gender = "Nam";
            else
                gender = "Nữ";
            Employee employee = EmployeeDAL.Instance.GetById("1");
            employee.Name = Name;
            employee.DateOfBirth = DateTime.Parse(BirthDate);
            employee.Address = UserAddress;
            employee.PhoneNumber = UserPhoneNumber;
            employee.Gender = gender;
            employee.ImageFile = imgByteArr;
            EmployeeDAL.Instance.InsertOrUpdate(employee, true);
            MessageBox.Show("Thành công");
            window.Close();
        }

        void OpenPersonalInfoWindow(MainWindow main)
        {
            PersonalInfoWindow window = new PersonalInfoWindow();
            //current account
            Employee employee = EmployeeDAL.Instance.GetById("1");

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(employee.ImageFile);
            window.grdSelectImage.Background = imageBrush;
            if (window.grdSelectImage.Children.Count > 1)
            {
                window.grdSelectImage.Children.Remove(window.grdSelectImage.Children[0]);
                window.grdSelectImage.Children.Remove(window.grdSelectImage.Children[1]);
            }
            Name = employee.Name;
            window.txtName.SelectionStart = 0;
            window.txtName.SelectionLength = Name.Length;
            UserAddress = employee.Address;
            window.txtAddress.SelectionStart = 0;
            window.txtAddress.SelectionLength = UserAddress.Length;
            UserPhoneNumber = employee.PhoneNumber;
            window.txtPhoneNumber.SelectionStart = 0;
            window.txtPhoneNumber.SelectionLength = UserPhoneNumber.Length;
            BirthDate = employee.DateOfBirth.ToString();
            if (employee.Gender == "Nam")
            {
                window.rdoMale.IsChecked = true;
            }
            else
            {
                window.rdoFemale.IsChecked = true;
            }
            window.ShowDialog();
        }

        //window change password
        void ChangePassword(ChangePasswordWindow window)
        {
            if (string.IsNullOrEmpty(window.pwbPassword.Password))
            {
                MessageBox.Show("Nhập mật khẩu");
                window.pwbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(window.pwbNewPassword.Password))
            {
                MessageBox.Show("Nhập mật khẩu");
                window.pwbNewPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(window.pwbConfirmPassword.Password))
            {
                MessageBox.Show("Nhập mật khẩu");
                window.pwbConfirmPassword.Focus();
                return;
            }
            //current account
            if (MD5Hash(window.pwbPassword.Password) != "current account")
            {
                MessageBox.Show("Mật khẩu không đúng");
                window.pwbPassword.Focus();
                return;
            }
            if (window.pwbConfirmPassword.Password != window.pwbNewPassword.Password)
            {
                MessageBox.Show("Nhập lại mật khẩu không trùng khớp");
                window.pwbNewPassword.Password = null;
                window.pwbConfirmPassword.Password = null;
                window.pwbNewPassword.Focus();
                return;
            }
            AccountDAL.Instance.UpdatePasswordByUsername("admin", MD5Hash(window.pwbNewPassword.Password));
            MessageBox.Show("Thành công");
            window.Close();
        }
        void OpenChangePasswordWindow(MainWindow main)
        {
            ChangePasswordWindow window = new ChangePasswordWindow();
            window.ShowDialog();
        }
    }
}