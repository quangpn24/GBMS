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
    class LoginViewModel :BaseViewModel
    {
        public ICommand LogInCommand { get; set; }
        public ICommand OpenSignUpWindowCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        private string password;
        public string Password { get => password; set => password = value; }
        public string Username { get => username; set => username = value; }
        public bool IsLogin { get => isLogin; set => isLogin = value; }

        private string username;
        private bool isLogin;
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
            loginWindow.WindowStyle = WindowStyle.None;
            forgotPasswordWindow.ShowDialog();
            loginWindow.WindowStyle = WindowStyle.SingleBorderWindow;
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
            foreach (var account in accounts)
            {
                if (account.Username == username && account.Password == password)
                {
                    isLogin = true;
                }
            }
            if(isLogin)
            {
                MainWindow main = new MainWindow();
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
            parameter.WindowStyle = WindowStyle.None;
            signUp.ShowDialog();
            parameter.WindowStyle = WindowStyle.SingleBorderWindow;
            parameter.Opacity = 1;
            parameter.Show();
        }
    }
}
