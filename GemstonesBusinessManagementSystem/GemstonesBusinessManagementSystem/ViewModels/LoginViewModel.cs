using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text.RegularExpressions;
using GemstonesBusinessManagementSystem.ViewModels;
using GemstonesBusinessManagementSystem.Views;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.DAL;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        public ICommand LogInCommand { get; set; }
        public ICommand OpenSignUpWindowCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        private string password;
        private string username;
        private bool isLogin;

        public string Password { get => password; set => password = value; }
        public string Username { get => username; set => username = value; }
        public bool IsLogin { get => isLogin; set => isLogin = value; }

        public LoginViewModel()
        {
            LogInCommand = new RelayCommand<LoginWindow>(p => true, p => Login(p));
            OpenSignUpWindowCommand = new RelayCommand<Window>((p) => true, (p) => OpenSignUpWindow(p));
            ChangePasswordCommand = new RelayCommand<LoginWindow>((p) => true, (p) => OpenForgotPasswordWindow(p));
        }

        public void OpenForgotPasswordWindow(LoginWindow loginWindow)
        {
            ForgotPasswordWindow forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.txtUsername.Text = null;
            loginWindow.Opacity = 0.5;
            forgotPasswordWindow.ShowDialog();
            loginWindow.Opacity = 1;
            loginWindow.Show();
        }
        public void Login(LoginWindow parameter)
        {
            IsLogin = false;
            if (parameter == null)
            {
                return;
            }
            List<Account> accounts = AccountDAL.Instance.ConvertDBToList();

            if (string.IsNullOrEmpty(parameter.txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.txtUsername.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.txtPassword.Password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                parameter.txtPassword.Focus();
                return;
            }
            string username = parameter.txtUsername.Text;
            string password = MD5Hash(parameter.txtPassword.Password);
            Account acc = new Account();
            foreach (var account in accounts)
            {
                if (account.Username == username && account.Password == password)
                {
                    isLogin = true;
                    acc = new Account(account.IdAccount, account.Username, account.Password, account.Type);
                    break;
                }
            }
            if (isLogin)
            {
                MainWindow main = new MainWindow();
                if (acc.Type != 0)
                {
                    Employee employee = EmployeeDAL.Instance.GetByIdAccount(acc.IdAccount.ToString());
                    CurrentAccount.PositionDetails = PositionDetailDAL.Instance.GetListByPosition(employee.IdPosition);
                    CurrentAccount.IdAccount = acc.IdAccount;
                    CurrentAccount.IdEmployee = employee.IdEmployee;
                    CurrentAccount.Name = employee.Name;
                    CurrentAccount.ImageFile = employee.ImageFile;
                    CurrentAccount.IdPosition = employee.IdPosition;
                    DisplayInfo(main);
                    SetRole(main);
                }
                parameter.Hide();
                main.ShowDialog();
                parameter.Show();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void OpenSignUpWindow(Window parameter)
        {
            SignUpWindow signUp = new SignUpWindow();
            signUp.txtUsername.Text = null;
            parameter.Opacity = 0.5;
            signUp.ShowDialog();
            parameter.Opacity = 1;
            parameter.Show();
        }
        void SetRole(MainWindow window)
        {
            List<PositionDetail> positionDetails = CurrentAccount.PositionDetails;

            window.btnHome.IsEnabled = positionDetails[0].IsPermitted;

            window.btnStore.IsEnabled = positionDetails[1].IsPermitted;
            window.btnService.IsEnabled = positionDetails[2].IsPermitted;
            if (!positionDetails[1].IsPermitted || !positionDetails[2].IsPermitted)
            {
                window.expStore.IsEnabled = false;
                window.expStore.Opacity = 0.5;
            }

            window.btnStock.IsEnabled = positionDetails[3].IsPermitted;
            window.btnImport.IsEnabled = positionDetails[4].IsPermitted;
            if (!positionDetails[3].IsPermitted || !positionDetails[4].IsPermitted)
            {
                window.expWarehouse.IsEnabled = false;
                window.expWarehouse.Opacity = 0.5;
            }

            window.btnSupplier.IsEnabled = positionDetails[5].IsPermitted;
            window.btnCustomer.IsEnabled = positionDetails[6].IsPermitted;
            if (!positionDetails[5].IsPermitted || !positionDetails[6].IsPermitted)
            {
                window.expPartner.IsEnabled = false;
                window.expPartner.Opacity = 0.5;
            }

            window.btnEmployee.IsEnabled = positionDetails[7].IsPermitted;
            window.btnGoods.IsEnabled = positionDetails[8].IsPermitted;
            window.btnServiceM.IsEnabled = positionDetails[9].IsPermitted;
            if (!positionDetails[7].IsPermitted || !positionDetails[8].IsPermitted || !positionDetails[9].IsPermitted)
            {
                window.expManage.IsEnabled = false;
                window.expManage.Opacity = 0.5;
            }

            window.btnBillService.IsEnabled = positionDetails[10].IsPermitted;
            window.btnRevenue.IsEnabled = positionDetails[11].IsPermitted;
            if (!positionDetails[5].IsPermitted || !positionDetails[6].IsPermitted)
            {
                window.expReport.IsEnabled = false;
                window.expReport.Opacity = 0.5;
            }

            window.btnSetting.IsEnabled = positionDetails[12].IsPermitted;
        }
        void DisplayInfo(MainWindow window)
        {
            window.txbUsername.Text = CurrentAccount.Name;

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(CurrentAccount.ImageFile);
            if (imageBrush.ImageSource != null)
            {
                window.imgAccount.Fill = imageBrush;
            }
        }
    }
}
