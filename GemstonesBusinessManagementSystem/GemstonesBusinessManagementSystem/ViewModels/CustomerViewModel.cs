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
        private List<CustomerControl> listCustomerControl = new List<CustomerControl>();
        private List<CustomerControl> listSearch = new List<CustomerControl>();
        public bool isEditing = false;
        private PickCustomerWindow pickCustomerWindow;
        private CustomerControl customerControl;

        public ICommand LoadCustomerCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand GoToNextPageCommandCus { get; set; }
        public ICommand GoToPreviousPageCommandCus { get; set; }
        public ICommand FindCustomerCommand { get; set; }
        public ICommand OpenAddCustomerWinDowCommand { get; set; }
        public ICommand SortCustomerCommand { get; set; } 
        public ICommand CountCustomerCommand { get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand PickCustomerCommand { get; set; }

        private MembershipsType filterMembership;
        public MembershipsType FilterMembership { get => filterMembership; set => filterMembership = value; }

        private MembershipsType selectedMembership;
        public MembershipsType SelectedMembership { get => this.selectedMembership; set => this.selectedMembership = value; }

        private ObservableCollection<MembershipsType> itemSourceMembership = new ObservableCollection<MembershipsType>();
        public ObservableCollection<MembershipsType> ItemSourceMembership { get => itemSourceMembership; set => itemSourceMembership = value; }
        private ObservableCollection<MembershipsType> itsAddCustomerMembership = new ObservableCollection<MembershipsType>();
        public ObservableCollection<MembershipsType> ItsAddCustomerMembership { get => itsAddCustomerMembership; set => itsAddCustomerMembership = value; }


        //AddCustomer window
        public ICommand AddCustomerCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        private int currentPage = 0;

        public CustomerViewModel()
        {
            //Grid Customer to mainWindow
            LoadCustomerCommand = new RelayCommand<MainWindow>(p => true, p => { Load(p); });
            GoToNextPageCommandCus = new RelayCommand<MainWindow>(p => true, p => GoToNextPage(p, ++currentPage));
            GoToPreviousPageCommandCus = new RelayCommand<MainWindow>(p => true, p => GoToPreviousPage(p, --currentPage));
            FindCustomerCommand = new RelayCommand<MainWindow>(p => true, p => FindCustomer(p));
            OpenAddCustomerWinDowCommand = new RelayCommand<MainWindow>(p => true, p => OpenAddCustomerWindow(p));
            ExportExcelCommand = new RelayCommand<Window>(p => true, p => ExportExcel());
            SortCustomerCommand = new RelayCommand<MainWindow>(p => true, p => SortCustomer(p));
            FilterCommand = new RelayCommand<MainWindow>(p => true, p => Filter(p));
            //UC customer - addCustomer window
            AddCustomerCommand = new RelayCommand<AddCustomerWindow>(p => true, p => AddCustomer(p));
            ExitCommand = new RelayCommand<AddCustomerWindow>((parameter) => true, (parameter) => parameter.Close());
        }

        /*public void PickCustomer(CustomerControl customerControl)
        {
            pickCustomerWindow.txbId.Text = customerControl.txbId.Text;
            pickCustomerWindow.txbName.Text = customerControl.txbName.Text;
            pickCustomerWindow.txbAddress.Text = customerControl.txbAddress.Text;
            pickCustomerWindow.txbPhoneNumber.Text = customerControl.txbPhoneNumber.Text;
            pickCustomerWindow.txbIdNumber.Text = customerControl.txbIdNumber.Text;
            pickCustomerWindow.txbRank.Text = customerControl.txbRank.Text;
            customerControl.Focus();
        }*/

        public void OpenAddCustomerWindow(MainWindow mainWindow)
        {
            AddCustomerWindow addCustomerWindow = new AddCustomerWindow();
            addCustomerWindow.txtId.Text = AddPrefix("KH", (CustomerDAL.Instance.GetMaxId() + 1));
            addCustomerWindow.ShowDialog();
        }   

        void Load(MainWindow mainWindow)  // load lại label khi Add khách hàng mới
        {
            mainWindow.lbCountCustomer.Content = CustomerDAL.Instance.LoadData().Rows.Count.ToString();
            mainWindow.lbCountAllPrice.Content = CustomerDAL.Instance.CountPrice().ToString();
            SetItemSource(mainWindow);
            LoadCustomerToView(mainWindow, 0);
        }
        void LoadCustomerToView(MainWindow mainWindow, int currentPage)
        {
            this.mainWindow = mainWindow;
            mainWindow.stkCustomer.Children.Clear();
            int start = 0, end = 0;
            this.currentPage = currentPage;
            LoadInfoOfPage(ref start, ref end);
            
            for(int i = start; i < end; i++)
            {
                CustomerControl ucCustomer = new CustomerControl();
                ucCustomer.txbSerial.Text = AddPrefix("KH", customerList[i].IdCustomer);
                ucCustomer.txbName.Text = customerList[i].CustomerName;
                ucCustomer.txbPhone.Text = customerList[i].PhoneNumber.ToString();
                ucCustomer.txbAddress.Text = customerList[i].Address.ToString();
                ucCustomer.txbAllPrice.Text = customerList[i].TotalPrice.ToString();
                //ucCustomer.txbLevelCus.Text = customerList[i].IdMembership == 1 ? "VIP" : "Thân thiết";
                ucCustomer.txbLevelCus.Text = MembershipsTypeDAL.Instance.GetById(customerList[i].IdMembership).Membership;
                mainWindow.stkCustomer.Children.Add(ucCustomer);
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

            mainWindow.txtNumOfCus.Text = String.Format("{0} - {1} of {2} customers", start == end ? 0 : start + 1, end, customerList.Count);
        }
        public void FindCustomer(MainWindow mainWindow)
        {
            mainWindow.cboSelectCustomerIdMembership.SelectedIndex = -1;
            mainWindow.cboSelectCustomerSort.SelectedIndex = -1;
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
            if (string.IsNullOrEmpty(addCustomerWindow.cbMembership.Text))
            {
                MessageBox.Show("Vui lòng chọn loại thành viên!");
                return false;
            }
            if (string.IsNullOrEmpty(addCustomerWindow.txtAddress.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ khách hàng!");
                return false;
            }
            return true;
        }
        void SetItemSource(MainWindow mainWindow)
        {
            itemSourceMembership.Clear();
            itsAddCustomerMembership.Clear();

            MembershipsType membershipAll = new MembershipsType();
            membershipAll.Membership = "Tất cả";
            ItemSourceMembership.Add(membershipAll);
            DataTable dt = MembershipsTypeDAL.Instance.GetActive();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                MembershipsType type = new MembershipsType(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString());
                itemSourceMembership.Add(type);
                itsAddCustomerMembership.Add(type);
            }
        }
        public void AddCustomer(AddCustomerWindow addCustomerWindow) 
        {
            if(CheckData(addCustomerWindow))// kiem tra du lieu dau vao
            {
                if (ConvertToID(addCustomerWindow.txtId.Text) > CustomerDAL.Instance.GetMaxId()) // tao id moi cho khach hang moi
                {
                    if (!CustomerDAL.Instance.IsExisted(addCustomerWindow.txtCMND.Text))   //kiem tra CMND của khách hàng mới
                    {
                        Customer customer = new Customer(ConvertToID(addCustomerWindow.txtId.Text), addCustomerWindow.txtName.Text, (addCustomerWindow.txtPhoneNumber.Text),
                            int.Parse(addCustomerWindow.txtCMND.Text), 0, selectedMembership.IdMembershipsType, addCustomerWindow.txtAddress.Text);

                        if (isEditing)
                        {
                            customer.TotalPrice = int.Parse(customerControl.txbAllPrice.Text);
                        }
                        CustomerDAL.Instance.Add(customer, isEditing);
                        CustomerControl control = new CustomerControl();
                        if (isEditing)
                        {
                            control = customerControl;
                        }
                        control.txbSerial.Text = AddPrefix("KH", customer.IdCustomer);
                        control.txbName.Text = customer.CustomerName;
                        control.txbPhone.Text = customer.PhoneNumber;
                        control.txbAddress.Text = customer.Address.ToString();
                        control.txbAllPrice.Text = customer.TotalPrice.ToString();     //0: thân thiết --- 1:vip
                        control.txbLevelCus.Text = MembershipsTypeDAL.Instance.GetById(customer.IdMembership).Membership;

                        if (!isEditing)
                        {
                            this.mainWindow.stkCustomer.Children.Add(control);
                        }
                        mainWindow.lbCountCustomer.Content = (int.Parse(mainWindow.lbCountCustomer.Content.ToString()) + 1).ToString();
                        mainWindow.lbCountAllPrice.Content = CustomerDAL.Instance.CountPrice().ToString();

                        addCustomerWindow.Close();
                                            
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
                case -1:
                    return;
                case 0:
                    customerList = customerList.OrderByDescending(x => x.TotalPrice).ToList();
                    break;
                case 1:
                    customerList = customerList.OrderBy(x => x.TotalPrice).ToList();
                    break;
            }
            LoadCustomerToView(mainWindow, 0);
        }
        public void Filter(MainWindow mainWindow)
        {
            mainWindow.stkCustomer.Children.Clear();
            if (filterMembership == null)
            {
                return;
            }
            listCustomerControl.Clear();
            if(filterMembership.IdMembershipsType != 0)
            {
                customerList = CustomerDAL.Instance.GetListByIdMembership(filterMembership.IdMembershipsType);
                for (int i = 0; i < listSearch.Count; i++)
                {
                    CustomerControl control = listSearch[i];
                    if (control.txbLevelCus.Text == filterMembership.Membership)
                    {
                        listCustomerControl.Add(control);
                    }
                }
            }
            else //chon tat ca
            {
                customerList = CustomerDAL.Instance.ConvertDBToList();
                for (int i = 0; i < listSearch.Count; i++)
                {
                    listCustomerControl.Add(listSearch[i]);
                }
            }
            this.currentPage = 0;
            LoadCustomerToView(mainWindow, currentPage);
        }
    }
}
