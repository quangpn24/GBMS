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
        private CustomerControl selectedUCCustomer;

        public ICommand LoadCustomerCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand GoToNextPageCommandCus { get; set; }
        public ICommand GoToPreviousPageCommandCus { get; set; }
        public ICommand FindCustomerCommand { get; set; }
        public ICommand OpenAddCustomerWinDowCommand { get; set; }
        public ICommand SortCustomerCommand { get; set; } 
        public ICommand CountCustomerCommand { get; set; }
        //AddCustomer window
        public ICommand AddCustomerCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        private int currentPage = 0;

        public CustomerViewModel()
        {
            //Grid Customer to mainWindow
            LoadCustomerCommand = new RelayCommand<MainWindow>(p => true, p => LoadCustomerToView(p, 0));
            GoToNextPageCommandCus = new RelayCommand<MainWindow>(p => true, p => GoToNextPage(p, ++currentPage));
            GoToPreviousPageCommandCus = new RelayCommand<MainWindow>(p => true, p => GoToPreviousPage(p, --currentPage));
            FindCustomerCommand = new RelayCommand<MainWindow>(p => true, p => FindCustomer(p));
            OpenAddCustomerWinDowCommand = new RelayCommand<MainWindow>(p => true, p => OpenAddCustomerWindow(p));
            ExportExcelCommand = new RelayCommand<Window>(p => true, p => ExportExcel());
            SortCustomerCommand = new RelayCommand<MainWindow>(p => true, p => SortCustomer(p));
            CountCustomerCommand = new RelayCommand<MainWindow>(p => true, p => CountCustomer(p));
            //UC customer - addCustomer window
            AddCustomerCommand = new RelayCommand<AddCustomerWindow>(p => true, p => AddCustomer(p));
            ExitCommand = new RelayCommand<AddCustomerWindow>((parameter) => true, (parameter) => parameter.Close());
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
                ucCustomer.txbAllPrice.Text = customerList[i].TotalPrice.ToString();
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
        public bool CheckData(AddCustomerWindow addCustomerWindow)
        {
            if (string.IsNullOrEmpty(addCustomerWindow.txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!");
                return false;
            }
            if (string.IsNullOrEmpty(addCustomerWindow.txtPhoneNumber.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại khách hàng!");
                return false;
            }
            if (string.IsNullOrEmpty(addCustomerWindow.txtCMND.Text))
            {
                MessageBox.Show("Vui lòng nhập số CMND khách hàng!");
                return false;
            }
            return true;
        }
        public void AddCustomer(AddCustomerWindow addCustomerWindow) 
        {
            if(CheckData(addCustomerWindow))// kiem tra du lieu dau vao
            {
                if (ConvertToID(addCustomerWindow.txtId.Text) > CustomerDAL.Instance.GetMaxId()) // tao id moi cho khach hang moi
                {
                    if (!CustomerDAL.Instance.IsExisted(addCustomerWindow.txtCMND.Text))   //kiem tra CMND của khách hàng mới
                    {
                        Customer customer = new Customer(ConvertToID(addCustomerWindow.txtId.Text), addCustomerWindow.txtName.Text,
                            (addCustomerWindow.txtPhoneNumber.Text), int.Parse(addCustomerWindow.txtCMND.Text), long.Parse(addCustomerWindow.txbTotalPrice.Text));
                        if (CustomerDAL.Instance.Add(customer))
                        {
                            MessageBox.Show("Thành công!");
                            addCustomerWindow.Close();

                            CustomerControl ucCustomer = new CustomerControl();
                            ucCustomer.txbSerial.Text = AddPrefix("KH", customer.IdCustomer);
                            ucCustomer.txbName.Text = customer.CustomerName;
                            ucCustomer.txbPhone.Text = customer.PhoneNumber;
                            ucCustomer.txbCMND.Text = customer.IdNumber.ToString();
                            ucCustomer.txbAllPrice.Text = customer.TotalPrice.ToString();
                            customerList.Add(customer);
                            if (currentPage == (customerList.Count - 1) / 10)
                            {
                                mainWindow.stkCustomer.Children.Add(ucCustomer);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Thất bại!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Khách hàng này đã tồn tại!");
                    }
                }
                else //id đã xuất hiện thì cập nhật
                {
                    if (!CustomerDAL.Instance.IsExisted(addCustomerWindow.txtCMND.Text))
                    {
                        Customer customer = new Customer(ConvertToID(addCustomerWindow.txtId.Text), addCustomerWindow.txtName.Text,
                            (addCustomerWindow.txtPhoneNumber.Text), int.Parse(addCustomerWindow.txtCMND.Text), long.Parse(addCustomerWindow.txbTotalPrice.Text));
                        if (CustomerDAL.Instance.Update(customer))
                        {
                            MessageBox.Show("Cập nhật thành công!");
                            addCustomerWindow.Close();

                            selectedUCCustomer.txbName.Text = customer.CustomerName;
                            selectedUCCustomer.txbPhone.Text = customer.PhoneNumber;
                            selectedUCCustomer.txbCMND.Text = customer.IdNumber.ToString();
                            selectedUCCustomer.txbAllPrice.Text = customer.TotalPrice.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Thất bại!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Khách hàng này đã tồn tại!");
                    }
                }    
            }
            int start = 0, end = 0;
            LoadInfoOfPage(ref start, ref end);
        }
        public void ExportExcel()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(CustomerDAL.Instance.LoadData(), "Customers");
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                MessageBox.Show("Xuất dữ liệu thành công!");
            }
        }
        public void SortCustomer(MainWindow mainWindow)
        {
            switch (mainWindow.cboSelectCustomerSort.SelectedIndex)
            {
                case 0:
                    customerList = customerList.OrderByDescending(x => x.TotalPrice).ToList();
                    break;
                case 1:
                    customerList = customerList.OrderBy(x => x.TotalPrice).ToList();
                    break;
            }
            LoadCustomerToView(mainWindow, 0);
        }
        public void CountCustomer(MainWindow mainWindow)
        {
            /*DataTable dt = new DataTable();
            mainWindow.lbCountCustomer.Content = dt.Rows.Count.ToString();*/
        }
    }
}
