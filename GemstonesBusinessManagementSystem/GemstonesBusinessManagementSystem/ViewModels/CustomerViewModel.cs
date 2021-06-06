using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class CustomerViewModel : BaseViewModel
    {
        public ICommand LoadCustomerCommand { get; set; }
        public ICommand ClosingWdCommand { get; set; }
        public ICommand PickCustomerCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand FindCustomerCommand { get; set; }
        public ICommand GoToNextPageCommandCus { get; set; }
        public ICommand GoToPreviousPageCommandCus { get; set; }

        private PickCustomerWindow pickCustomerWindow;
        private List<Customer> customerList = CustomerDAL.Instance.ConvertDBToList();
        private int currentPage = 0;
        public CustomerViewModel()
        {
            LoadCustomerCommand = new RelayCommand<Window>(p => true, p => LoadCustomerToView(p, 0));
            ClosingWdCommand = new RelayCommand<PickCustomerWindow>((p) => true, (p) => CloseWindow(p));
            PickCustomerCommand = new RelayCommand<CustomerControl>(p => true, p => PickCustomer(p));
            ConfirmCommand = new RelayCommand<PickCustomerWindow>(p => true, p => ConfirmCustomer(p));
            FindCustomerCommand = new RelayCommand<PickCustomerWindow>(p => true, p => FindCustomer(p));
            GoToNextPageCommandCus = new RelayCommand<PickCustomerWindow>(p => true, p => GoToNextPage(p, ++currentPage));
            GoToPreviousPageCommandCus = new RelayCommand<PickCustomerWindow>(p => true, p => GoToPreviousPage(p, --currentPage));
        }
        public void GoToNextPage(PickCustomerWindow pickCustomerWindow, int currentPage)
        {
            LoadCustomerToView(pickCustomerWindow, currentPage);
        }
        public void GoToPreviousPage(PickCustomerWindow pickCustomerWindow, int currentPage)
        {
            LoadCustomerToView(pickCustomerWindow, currentPage);
        }
        public void FindCustomer(PickCustomerWindow pickCustomerWindow)
        {
            customerList = CustomerDAL.Instance.FindByName(pickCustomerWindow.txtSearchCustomer.Text);
            currentPage = 0;
            LoadCustomerToView(pickCustomerWindow, currentPage);
        }
        public void ConfirmCustomer(PickCustomerWindow pickCustomerWindow)
        {
            if (String.IsNullOrEmpty(pickCustomerWindow.txbId.Text))
            {
                MessageBox.Show("Vui lòng chọn khách hàng!");
            }
            else
            {
                pickCustomerWindow.Close();
            }
        }
        public void PickCustomer(CustomerControl customerControl)
        {
            pickCustomerWindow.txbId.Text = customerControl.txbId.Text;
            pickCustomerWindow.txbName.Text = customerControl.txbName.Text;
            pickCustomerWindow.txbAddress.Text = customerControl.txbAddress.Text;
            pickCustomerWindow.txbPhoneNumber.Text = customerControl.txbPhoneNumber.Text;
            pickCustomerWindow.txbIdNumber.Text = customerControl.txbIdNumber.Text;
            pickCustomerWindow.txbRank.Text = customerControl.txbRank.Text;
            customerControl.Focus();
        }
        public void CloseWindow(PickCustomerWindow pickCustomerWindow)
        {
            if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
            {
                pickCustomerWindow.txbId.Text = pickCustomerWindow.txbName.Text
                    = pickCustomerWindow.txbRank.Text = pickCustomerWindow.txbIdNumber.Text
                    = pickCustomerWindow.txbAddress.Text = pickCustomerWindow.txbPhoneNumber.Text = "";
            }
        }
        public void LoadCustomerToView(Window window, int currentPage)
        {
            this.pickCustomerWindow = window as PickCustomerWindow;
            this.pickCustomerWindow.stkCustomer.Children.Clear();
            int start = 0, end = 0;
            this.currentPage = currentPage;
            LoadInfoOfPage(ref start, ref end);

            for (int i = start; i < end; i++)
            {
                CustomerControl ucCustomer = new CustomerControl();
                ucCustomer.txbId.Text = AddPrefix("KH", customerList[i].IdCustomer);
                ucCustomer.txbName.Text = customerList[i].CustomerName;
                ucCustomer.txbPhoneNumber.Text = customerList[i].PhoneNumber.ToString();
                ucCustomer.txbIdNumber.Text = customerList[i].IdNumber.ToString();
                ucCustomer.txbAddress.Text = customerList[i].Address.ToString();
                pickCustomerWindow.stkCustomer.Children.Add(ucCustomer);
            }

        }
        public void LoadInfoOfPage(ref int start, ref int end)
        {
            pickCustomerWindow.btnPrePageCus.IsEnabled = currentPage == 0 ? false : true;
            pickCustomerWindow.btnNextPageCus.IsEnabled = currentPage == (customerList.Count - 1) / 10 ? false : true;

            start = currentPage * 10;
            end = (currentPage + 1) * 10;
            if (currentPage == customerList.Count / 10)
                end = customerList.Count;

            pickCustomerWindow.txtNumOfCus.Text = String.Format("Trang {0} trên {1} trang", currentPage+1, (customerList.Count-1)/10+1);
        }
    }
}
