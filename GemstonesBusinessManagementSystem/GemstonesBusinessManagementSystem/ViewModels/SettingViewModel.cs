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
        // id 1: PrepaymentPerCent
        //     2: StoreName
        //     3: StoreAddress
        //     4: StorePhone Number
        //     5: StoreEmail
        //     6: StoreAvatar
        private List<Parameter> parameters = ParameterDAL.Instance.GetData();
        private MainWindow main;

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
        public ICommand Update_PrepaymentCommand { get; set; }
        public ICommand Undo_PrepaymentCommand { get; set; }
        public ICommand Update_StoreInfoCommand { get; set; }
        public ICommand Undo_StoreInfoCommand { get; set; }
        public ICommand ClickAvatarCommand { get; set; }
        public ICommand CheckRangeCommand { get; set; }
        // Change user info & change password
        public ICommand SelectImageCommand { get; set; }
        public ICommand UpdateUserInfoCommand { get; set; }
        public ICommand ExitCommand { get; set; }
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
            CheckRangeCommand = new RelayCommand<TextBox>(p => true, p => CheckRange(p));

            ClickAvatarCommand = new RelayCommand<MainWindow>(p => true, p =>
            {
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

        void CheckRange(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            if (int.Parse(textBox.Text) > 100)
            {
                textBox.Text = "100";
            }
        }
        void LoadDefault(MainWindow main)
        {
            this.main = main;
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
                        main.txtStoreName.Text = StoreName;
                        main.txbStoreName.Text = StoreName;
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
                    case 6:
                        if (!string.IsNullOrEmpty(parameters[i].Value))
                        {
                            ImageBrush imageBrush = new ImageBrush();
                            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(Convert.FromBase64String(parameters[i].Value));
                            main.grdSelectImage.Background = imageBrush;
                            if (main.grdSelectImage.Children.Count > 1)
                            {
                                main.grdSelectImage.Children.Remove(main.grdSelectImage.Children[0]);
                                main.grdSelectImage.Children.Remove(main.grdSelectImage.Children[1]);
                            }
                            //avatar
                            main.imgStore.Source = imageBrush.ImageSource;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        void UpdatePrepayment(MainWindow main)
        {
            if (string.IsNullOrEmpty(PrepaymentPercent))
            {
                main.txtPrepayment.Focus();
                return;
            }
            if (ParameterDAL.Instance.UpdatePrepayment(PrepaymentPercent))
            {
                parameters[0].Value = PrepaymentPercent;
                CustomMessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                CustomMessageBox.Show("Cập nhật thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void UpdateStoreInfo(MainWindow main)
        {
            if (string.IsNullOrEmpty(StoreName))
            {
                main.txtStoreName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(StoreAddress))
            {
                main.txtStoreAddress.Focus();
                return;
            }
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                main.txtStorePhoneNumber.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Email))
            {
                main.txtEmail.Focus();
                return;
            }
            byte[] imgByteArr;
            ImageBrush imageBrush = (ImageBrush)main.grdSelectImage.Background;
            if (imageBrush == null)
            {
                CustomMessageBox.Show("Vui lòng chọn ảnh!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            imgByteArr = Converter.Instance.ConvertBitmapImageToBytes((BitmapImage)imageBrush.ImageSource);

            try
            {
                long temp = long.Parse(PhoneNumber);
            }
            catch
            {
                CustomMessageBox.Show("SĐT không bao gồm chữ cái!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                main.txtStorePhoneNumber.Focus();
                PhoneNumber = null;
                return;
            }
            bool k1 = ParameterDAL.Instance.UpdateStoreInfo(2, StoreName);
            bool k2 = ParameterDAL.Instance.UpdateStoreInfo(3, StoreAddress);
            bool k3 = ParameterDAL.Instance.UpdateStoreInfo(4, PhoneNumber);
            bool k4 = ParameterDAL.Instance.UpdateStoreInfo(5, Email);
            bool k5 = ParameterDAL.Instance.UpdateStoreInfo(6, Convert.ToBase64String(imgByteArr));
            if (k1 && k2 && k3 && k4 && k5)
            {
                parameters[1].Value = StoreName;
                parameters[2].Value = StoreAddress;
                parameters[3].Value = PhoneNumber;
                parameters[4].Value = Email;
                parameters[5].Value = Convert.ToBase64String(imgByteArr);
                CustomMessageBox.Show("Cập nhật thông tin cửa hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                main.imgStore.Source = imageBrush.ImageSource;
                main.txbStoreName.Text = parameters[1].Value;
            }
            else
            {
                CustomMessageBox.Show("Cập nhật thông tin cửa hàng thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void UndoPrepayment(MainWindow main)
        {
            PrepaymentPercent = parameters[0].Value;
        }
        void UndoStoreInfo(MainWindow main)
        {
            main.txtStoreName.Text = parameters[1].Value;
            main.txtStoreAddress.Text = parameters[2].Value;
            main.txtStorePhoneNumber.Text = parameters[3].Value;
            main.txtEmail.Text = parameters[4].Value;
                ImageBrush imageBrush = new ImageBrush();
            if (!string.IsNullOrEmpty(parameters[5].Value))
            {
                imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(Convert.FromBase64String(parameters[5].Value));
                main.grdSelectImage.Background = imageBrush;
                if (main.grdSelectImage.Children.Count > 1)
                {
                    main.grdSelectImage.Children.Remove(main.grdSelectImage.Children[0]);
                    main.grdSelectImage.Children.Remove(main.grdSelectImage.Children[1]);
                }
            }
            else
            {
                imageBrush.ImageSource = new BitmapImage(new Uri("/Resources/Images/avatar.jpg", UriKind.Relative));
                main.grdSelectImage.Background = imageBrush;
                if (main.grdSelectImage.Children.Count > 1)
                {
                    main.grdSelectImage.Children.Remove(main.grdSelectImage.Children[0]);
                    main.grdSelectImage.Children.Remove(main.grdSelectImage.Children[1]);
                }
            }
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
                CustomMessageBox.Show("Vui lòng nhập tên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(BirthDate))
            {
                CustomMessageBox.Show("Vui lòng nhập ngày sinh!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(UserAddress))
            {
                CustomMessageBox.Show("Vui lòng nhập địa chỉ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(UserPhoneNumber))
            {
                CustomMessageBox.Show("Vui lòng nhập số điện thoại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            byte[] imgByteArr;

            ImageBrush imageBrush = (ImageBrush)window.grdSelectImage.Background;
            if (imageBrush == null)
            {
                CustomMessageBox.Show("Vui lòng chọn ảnh!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            imgByteArr = Converter.Instance.ConvertBitmapImageToBytes((BitmapImage)imageBrush.ImageSource);
            try
            {
                long temp = long.Parse(PhoneNumber);
            }
            catch
            {
                CustomMessageBox.Show("SĐT không bao gồm chữ cái!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                main.txtStorePhoneNumber.Focus();
                PhoneNumber = null;
                return;
            }
            string gender;
            if (window.rdoMale.IsChecked.Value == true)
                gender = "Nam";
            else
                gender = "Nữ";
            Employee employee = EmployeeDAL.Instance.GetById(CurrentAccount.IdEmployee.ToString());
            employee.Name = Name;
            employee.DateOfBirth = DateTime.Parse(BirthDate);
            employee.Address = UserAddress;
            employee.PhoneNumber = UserPhoneNumber;
            employee.Gender = gender;
            employee.ImageFile = imgByteArr;
            if (EmployeeDAL.Instance.UpdateUserInfo(employee))
            {
                CustomMessageBox.Show("Cập nhật thông tin cá nhân thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                CustomMessageBox.Show("Cập nhật thông tin cá nhân thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            CurrentAccount.Name = Name;
            CurrentAccount.ImageFile = imgByteArr;
            main.txbUsername.Text = CurrentAccount.Name;
            //update display
            if (imageBrush.ImageSource != null)
            {
                main.imgAccount.Fill = imageBrush;
            }
            EmployeeViewModel employeeVM = (EmployeeViewModel)main.grdEmployee.DataContext;
            employeeVM.Search(main);
            ImportGoodsViewModel importVM = (ImportGoodsViewModel)main.grdImport.DataContext;
            importVM.Init(main);

            window.Close();
        }

        void OpenPersonalInfoWindow(MainWindow main)
        {
            PersonalInfoWindow window = new PersonalInfoWindow();
            Employee employee = EmployeeDAL.Instance.GetById(CurrentAccount.IdEmployee.ToString());

            ImageBrush imageBrush = new ImageBrush();
            if (employee.ImageFile != null)
            {
                imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(employee.ImageFile);
                window.grdSelectImage.Background = imageBrush;            }
            else
            {
                imageBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/GemstonesBusinessManagementSystem;component/Resources/Images/avatar.jpg"));
                window.grdSelectImage.Background = imageBrush;
            }
            if (window.grdSelectImage.Children.Count > 1)
            {
                window.grdSelectImage.Children.Remove(window.grdSelectImage.Children[0]);
                window.grdSelectImage.Children.Remove(window.grdSelectImage.Children[1]);
            }
            window.txtName.Text = employee.Name;
            window.txtName.SelectionStart = 0;
            window.txtName.SelectionLength = Name.Length;
            window.txtAddress.Text = employee.Address;
            window.txtAddress.SelectionStart = 0;
            window.txtAddress.SelectionLength = UserAddress.Length;
            window.txtPhoneNumber.Text = employee.PhoneNumber;
            window.txtPhoneNumber.SelectionStart = 0;
            window.txtPhoneNumber.SelectionLength = UserPhoneNumber.Length;
            window.dpBirthDate.SelectedDate = employee.DateOfBirth;
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
                CustomMessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                window.pwbPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(window.pwbNewPassword.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mật khẩu mới!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                window.pwbNewPassword.Focus();
                return;
            }
            if (string.IsNullOrEmpty(window.pwbConfirmPassword.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mật khẩu xác nhận!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                window.pwbConfirmPassword.Focus();
                return;
            }
            if (MD5Hash(window.pwbPassword.Password) != AccountDAL.Instance.GetPasswordById(CurrentAccount.IdAccount.ToString()))
            {
                CustomMessageBox.Show("Mật khẩu không đúng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                window.pwbPassword.Focus();
                return;
            }
            if (window.pwbConfirmPassword.Password != window.pwbNewPassword.Password)
            {
                CustomMessageBox.Show("Mật khẩu xác nhận không trùng khớp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                window.pwbNewPassword.Password = null;
                window.pwbConfirmPassword.Password = null;
                window.pwbNewPassword.Focus();
                return;
            }
            if(AccountDAL.Instance.UpdatePasswordByUsername(CurrentAccount.IdAccount.ToString(), MD5Hash(window.pwbNewPassword.Password)))
            {
                CustomMessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                CustomMessageBox.Show("Đổi mật khẩu thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            window.Close();
        }
        void OpenChangePasswordWindow(MainWindow main)
        {
            ChangePasswordWindow window = new ChangePasswordWindow();
            window.ShowDialog();
        }
    }
}