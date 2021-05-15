using ClosedXML.Excel;
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
    class CustomerViewModel : BaseViewModel
    {
        private MainWindow mainWindow;
        private List<Customer> customerList = CustomerDAL.Instance.ConvertDBToList(); 
        private bool isUpdate = false;
        public ICommand ExportExcelCommand { get; set; }
        public ICommand AddCustomerCommand { get; set; }
        public ICommand LoadCustomerCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        //public ICommand ExportExcelCommand { get; set; }
        public ICommand GoToNextPageCommandCus { get; set; }
        public ICommand GoToPreviousPageCommandCus { get; set; }
        public ICommand FindCustomerCommand { get; set; }
        public ICommand OpenAddCustomerWinDowCommand { get; set; }

        private int currentPage = 0;

        public CustomerViewModel()
        {
            //Grid Customer to mainWindow
            LoadCustomerCommand = new RelayCommand<MainWindow>(p => true, p => LoadCustomerToView(p, 0));
            GoToNextPageCommandCus = new RelayCommand<MainWindow>(p => true, p => GoToNextPage(p, ++currentPage));
            GoToPreviousPageCommandCus = new RelayCommand<MainWindow>(p => true, p => GoToPreviousPage(p, --currentPage));
            FindCustomerCommand = new RelayCommand<MainWindow>(p => true, p => FindCustomer(p));
            OpenAddCustomerWinDowCommand = new RelayCommand<MainWindow>(p => true, p => OpenAddCustomerWindow(p));
        }

        public void OpenAddCustomerWindow(MainWindow mainWindow)
        {
            AddCustomerWindow addCustomerWindow = new AddCustomerWindow();
            addCustomerWindow.txtId.Text = AddPrefix("KH", (CustomerDAL.Instance.GetMaxId() + 1));
            addCustomerWindow.ShowDialog();
        }

        void LoadCustomerToView(MainWindow mainWd, int currentPage)
        {
            this.mainWindow = mainWd;
            mainWd.stkCustomer.Children.Clear();
            int start = 0, end = 0;
            this.currentPage = currentPage;
            LoadInfoOfPage(ref start, ref end);
            
            for(int i = start; i < end; i++)
            {
                CustomerControl ucCustomer = new CustomerControl();
                ucCustomer.txbSerial.Text = AddPrefix("KH", customerList[i].IdCustomer);
                ucCustomer.txbName.Text = customerList[i].CustomerName;
                ucCustomer.txbPhone.Text = customerList[i].PhoneNumber.ToString();
                ucCustomer.txbCMND.Text = customerList[i].IdNumber.ToString();
                mainWd.stkCustomer.Children.Add(ucCustomer);
            }
        }
        public void GoToNextPage(MainWindow mainWindow, int currentPage)
        {
            LoadCustomerToView(mainWindow, currentPage);
        }
        public void GoToPreviousPage(MainWindow mainWindow, int currentPage)
        {
            LoadCustomerToView(mainWindow, currentPage);
        }
        public void LoadInfoOfPage(ref int start, ref int end)
        {
            mainWindow.btnPrePageCus.IsEnabled = currentPage == 0 ? false : true;
            mainWindow.btnNextPageCus.IsEnabled = currentPage == (customerList.Count - 1) / 10 ? false : true;

            start = currentPage * 10;
            end = (currentPage + 1) * 10;
            if (currentPage == customerList.Count / 10)
                end = customerList.Count;

            mainWindow.txtNumOfCus.Text = String.Format("{0} - {1} of {2} items", start == end ? 0 : start + 1, end, customerList.Count);
        }
        public void FindCustomer(MainWindow mainWindow)
        {
            customerList = CustomerDAL.Instance.FindByName(mainWindow.txtSearchCustomer.Text);
            currentPage = 0;
            LoadCustomerToView(mainWindow, currentPage);
        }
    }
}
