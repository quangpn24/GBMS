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
using System.Diagnostics;
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
        public bool isEditingMembership = false;
        private MembershipControl membershipControl;
        private string oldMembership;
        private PickCustomerWindow pickCustomerWindow;
        private CustomerControl customerControl;

        //CustomerWindow
        public ICommand LoadCustomerCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand GoToNextPageCommandCus { get; set; }
        public ICommand GoToPreviousPageCommandCus { get; set; }
        public ICommand FindCustomerCommand { get; set; }
        public ICommand OpenAddCustomerWinDowCommand { get; set; }
        public ICommand SortCustomerCommand { get; set; } 
        public ICommand CountCustomerCommand { get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand EditCommand { get; set; }

        //PickCustomer
        public ICommand LoadPickCustomerCommand { get; set; }
        public ICommand FindPickCustomerCommand { get; set; }
        public ICommand GoToNextPageCommandPickCus { get; set; }
        public ICommand GoToPreviousPageCommandPickCus { get; set; }
        public ICommand PickCustomerCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand ClosingWdCommand { get; set; }

        //MembershipWindow
        public ICommand OpenMembershipWindowCommand { get; set; }
        public ICommand LoadMembershipCommand { get; set; }
        public ICommand ClearViewCommand { get; set; }
        public ICommand AddMembershipCommand { get; set; }
        private AddMembershipWindow addMembershipWindow;

        //MembershipControl
        public ICommand DeleteMembershipCommand { get; set; }
        public ICommand ViewMembershipCommand { get; set; }

        private MembershipsType filterMembership;
        public MembershipsType FilterMembership { get => filterMembership; set => filterMembership = value; }

        private MembershipsType selectedMembership;
        public MembershipsType SelectedMembership { get => this.selectedMembership; set => this.selectedMembership = value; }

        private ObservableCollection<MembershipsType> itemSourceMembership = new ObservableCollection<MembershipsType>();
        public ObservableCollection<MembershipsType> ItemSourceMembership { get => itemSourceMembership; set => itemSourceMembership = value; }
        private ObservableCollection<MembershipsType> itsAddCustomerMembership = new ObservableCollection<MembershipsType>();
        public ObservableCollection<MembershipsType> ItsAddCustomerMembership { get => itsAddCustomerMembership; set => itsAddCustomerMembership = value; }


        //AddCustomer window
        public ICommand SaveCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        private int currentPage = 0;

        public CustomerViewModel()
        {
            //pickCustomer
            LoadPickCustomerCommand = new RelayCommand<Window>(p => true, p => LoadPickCustomerToView(p, 0));
            ConfirmCommand = new RelayCommand<PickCustomerWindow>(p => true, p => ConfirmCustomer(p));
            PickCustomerCommand = new RelayCommand<PickCustomerControl>(p => true, p => PickCustomer(p));
            ClosingWdCommand = new RelayCommand<PickCustomerWindow>((p) => true, (p) => CloseWindow(p));
            FindPickCustomerCommand = new RelayCommand<PickCustomerWindow>(p => true, p => FindPickCustomer(p));
            GoToNextPageCommandCus = new RelayCommand<PickCustomerWindow>(p => true, p => GoToNextPagePickCustomer(p, ++currentPage));
            GoToPreviousPageCommandCus = new RelayCommand<PickCustomerWindow>(p => true, p => GoToPreviousPagePickCustomer(p, --currentPage));
            //Grid Customer to mainWindow
            LoadCustomerCommand = new RelayCommand<MainWindow>(p => true, p => { Load(p); });
            GoToNextPageCommandCus = new RelayCommand<MainWindow>(p => true, p => GoToNextPage(p, ++currentPage));
            GoToPreviousPageCommandCus = new RelayCommand<MainWindow>(p => true, p => GoToPreviousPage(p, --currentPage));
            FindCustomerCommand = new RelayCommand<MainWindow>(p => true, p => FindCustomer(p));
            OpenAddCustomerWinDowCommand = new RelayCommand<MainWindow>(p => true, p => OpenAddCustomerWindow(p));
            ExportExcelCommand = new RelayCommand<Window>(p => true, p => ExportExcel());
            SortCustomerCommand = new RelayCommand<MainWindow>(p => true, p => SortCustomer(p));
            FilterCommand = new RelayCommand<MainWindow>(p => true, p => Filter(p));
            EditCommand = new RelayCommand<CustomerControl>((p) => true, (p) => OpenEditWindow(p));
            //UC customer - addCustomer window
            SaveCommand = new RelayCommand<AddCustomerWindow>(p => true, p => AddOrUpdateCustomer(p));
            ExitCommand = new RelayCommand<AddCustomerWindow>((parameter) => true, (parameter) => parameter.Close());
            //MembershipWindow
            OpenMembershipWindowCommand = new RelayCommand<MainWindow>(p => true, p => OpenMembershipWindow(p));
            LoadMembershipCommand = new RelayCommand<AddMembershipWindow>(p => true, p => LoadMembership(p));
            ClearViewCommand = new RelayCommand<AddMembershipWindow>((p) => true, (p) => ClearView(p));
            AddMembershipCommand = new RelayCommand<AddMembershipWindow>(p => true, p => AddOrUpdateMembership(p));
            //MembershipControl
            ViewMembershipCommand = new RelayCommand<MembershipControl>(p => true, p => ViewMembership(p));
            DeleteMembershipCommand = new RelayCommand<MembershipControl>(p => true, p => DeleteMembership(p));
        }

        //PickCustomer
        public void CloseWindow(PickCustomerWindow pickCustomerWindow)
        {
            if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
            {
                pickCustomerWindow.txbId.Text = pickCustomerWindow.txbName.Text
                    = pickCustomerWindow.txbRank.Text = pickCustomerWindow.txbIdNumber.Text
                    = pickCustomerWindow.txbAddress.Text = pickCustomerWindow.txbPhoneNumber.Text = "";
            }
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
        public void PickCustomer(PickCustomerControl pickCustomerControl)
        {
            pickCustomerWindow.txbId.Text = pickCustomerControl.txbId.Text;
            pickCustomerWindow.txbName.Text = pickCustomerControl.txbName.Text;
            pickCustomerWindow.txbAddress.Text = pickCustomerControl.txbAddress.Text;
            pickCustomerWindow.txbPhoneNumber.Text = pickCustomerControl.txbPhoneNumber.Text;
            pickCustomerWindow.txbIdNumber.Text = pickCustomerControl.txbIdNumber.Text;
            pickCustomerWindow.txbRank.Text = pickCustomerControl.txbRank.Text;
            pickCustomerControl.Focus();
        }
        public void LoadPickCustomerToView(Window window, int currentPage)
        {
            this.pickCustomerWindow = window as PickCustomerWindow;
            this.pickCustomerWindow.stkCustomer.Children.Clear();
            int start = 0, end = 0;
            this.currentPage = currentPage;
            LoadInfoOfPagePickCustomer(ref start, ref end);

            for (int i = start; i < end; i++)
            {
                PickCustomerControl ucCustomer = new PickCustomerControl();
                ucCustomer.txbId.Text = AddPrefix("KH", customerList[i].IdCustomer);
                ucCustomer.txbName.Text = customerList[i].CustomerName;
                ucCustomer.txbPhoneNumber.Text = customerList[i].PhoneNumber.ToString();
                ucCustomer.txbIdNumber.Text = customerList[i].IdNumber.ToString();
                ucCustomer.txbAddress.Text = customerList[i].Address.ToString();
                pickCustomerWindow.stkCustomer.Children.Add(ucCustomer);
            }

        }
        public void LoadInfoOfPagePickCustomer(ref int start, ref int end)
        {
            pickCustomerWindow.btnPrePageCus.IsEnabled = currentPage == 0 ? false : true;
            pickCustomerWindow.btnNextPageCus.IsEnabled = currentPage == (customerList.Count - 1) / 10 ? false : true;

            start = currentPage * 10;
            end = (currentPage + 1) * 10;
            if (currentPage == customerList.Count / 10)
                end = customerList.Count;

            pickCustomerWindow.txtNumOfCus.Text = String.Format("Trang {0} trên {1} trang", currentPage + 1, (customerList.Count - 1) / 10 + 1);
        }
        public void GoToNextPagePickCustomer(PickCustomerWindow pickCustomerWindow, int currentPage)
        {
            LoadPickCustomerToView(pickCustomerWindow, currentPage);
        }
        public void GoToPreviousPagePickCustomer(PickCustomerWindow pickCustomerWindow, int currentPage)
        {
            LoadPickCustomerToView(pickCustomerWindow, currentPage);
        }
        public void FindPickCustomer(PickCustomerWindow pickCustomerWindow)
        {
            customerList = CustomerDAL.Instance.FindByName(pickCustomerWindow.txtSearchCustomer.Text);
            currentPage = 0;
            LoadPickCustomerToView(pickCustomerWindow, currentPage);
        }


        //Customer
        void DeleteMembership(MembershipControl control)
        {
            string idMembership = ConvertToIDString(control.txbId.Text);
            if (CustomerDAL.Instance.IsMembership(idMembership))
            {
                MessageBox.Show("Không thể xóa vì tồn tại khách hàng có hạng thành viên này");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Xác nhận xóa hạng thành viên?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool isSuccess = MembershipsTypeDAL.Instance.Delete(idMembership);
                    if (isSuccess)
                    {
                        addMembershipWindow.stkMembership.Children.Remove(control);
                    }
                    else
                    {
                        MessageBox.Show("Xoá thất bại");
                    }
                }
            }
        }
        void ViewMembership(MembershipControl control)
        {
            oldMembership = control.txbMembership.Text;
            membershipControl = control;
            isEditingMembership = true;
            addMembershipWindow.txbTitle.Text = "Sửa hạng thành viên";
            addMembershipWindow.txtId.Text = control.txbId.Text;

            addMembershipWindow.txtMembership.Text = control.txbMembership.Text;
            addMembershipWindow.txtMembership.SelectionStart = control.txbMembership.Text.Length;

            addMembershipWindow.txtTarget.Text = control.txbTarget.Text;
            addMembershipWindow.txtTarget.SelectionLength = control.txbTarget.Text.Length;
        }
        void ClearView(AddMembershipWindow window)
        {
            isEditingMembership = false;
            window.txtId.Text = AddPrefix("TV", (MembershipsTypeDAL.Instance.GetMaxId() + 1));
            window.txtTarget.Clear();
            window.txtMembership.Clear();
        }
        void OpenMembershipWindow(MainWindow mainWindow)
        {
            AddMembershipWindow window = new AddMembershipWindow();
            window.txtId.Text = AddPrefix("TV", (MembershipsTypeDAL.Instance.GetMaxId() + 1));
            window.ShowDialog();
        }
        void LoadMembership(AddMembershipWindow window)
        {
            addMembershipWindow = window;
            List<MembershipsType> membershipsTypes = MembershipsTypeDAL.Instance.GetList();
            window.stkMembership.Children.Clear();
            foreach(var membershipType in membershipsTypes)
            {
                if(membershipType.IdMembershipsType == 0)
                {
                    continue;
                }
                MembershipControl control = new MembershipControl();
                control.txbId.Text = AddPrefix("TV", membershipType.IdMembershipsType);
                control.txbMembership.Text = membershipType.Membership;
                control.txbTarget.Text = membershipType.Target.ToString();

                window.stkMembership.Children.Add(control);
            }
        }
        public void OpenAddCustomerWindow(MainWindow mainWindow)
        {
            AddCustomerWindow addCustomerWindow = new AddCustomerWindow();
            addCustomerWindow.txtId.Text = AddPrefix("KH", (CustomerDAL.Instance.GetMaxId() + 1));
            addCustomerWindow.ShowDialog();
        }   
        void OpenEditWindow(CustomerControl control)
        {
            isEditing = true;
            this.customerControl = control;
            AddCustomerWindow addCustomerWindow = new AddCustomerWindow();
            Customer customer = CustomerDAL.Instance.FindById(ConvertToIDString(control.txbSerial.Text));

            addCustomerWindow.txtId.Text = control.txbSerial.Text;

            addCustomerWindow.txtName.Text = customer.CustomerName;
            addCustomerWindow.txtName.SelectionStart = addCustomerWindow.txtName.Text.Length;

            addCustomerWindow.txtPhoneNumber.Text = customer.PhoneNumber;
            addCustomerWindow.txtPhoneNumber.SelectionStart = addCustomerWindow.txtPhoneNumber.Text.Length;

            addCustomerWindow.txtCMND.Text = customer.IdNumber.ToString();
            addCustomerWindow.txtCMND.SelectionStart = addCustomerWindow.txtCMND.Text.Length;


            addCustomerWindow.txtAddress.Text = customer.Address;
            addCustomerWindow.txtAddress.SelectionStart = addCustomerWindow.txtAddress.Text.Length;

            addCustomerWindow.btnSave.ToolTip = "Cập nhật thông tin khách hàng";
            addCustomerWindow.Title = "Cập nhật thông tin khách hàng";
            addCustomerWindow.ShowDialog();
        }

        void AddOrUpdateMembership(AddMembershipWindow window)
        {
            if (string.IsNullOrEmpty(window.txtMembership.Text))
            {
                MessageBox.Show("Vui lòng nhập hạng thành viên!");
                window.txtMembership.Focus();
                return;
            }
            if (string.IsNullOrEmpty(window.txtTarget.Text))
            {
                MessageBox.Show("Vui lòng nhập chỉ tiêu!");
                window.txtTarget.Focus();
                return;
            }
            if (window.txtMembership.Text != oldMembership && MembershipsTypeDAL.Instance.IsExisted(window.txtMembership.Text))
            {
                MessageBox.Show("Hạng thành viên đã tồn tại!");
                window.txtMembership.Focus();
                return;
            }

            MembershipsType membership = new MembershipsType(ConvertToID(window.txtId.Text), window.txtMembership.Text,
                double.Parse(window.txtTarget.Text));

            MembershipsTypeDAL.Instance.InsertOrUpdate(membership, isEditingMembership);
            if (isEditingMembership)
            {
                membershipControl.txbId.Text = window.txtId.Text;
                membershipControl.txbMembership.Text = window.txtMembership.Text;
                membershipControl.txbTarget.Text = window.txtTarget.Text;

                window.txbTitle.Text = "Thêm hạng thành viên";
            }
            else
            {
                MembershipControl control = new MembershipControl();
                control.txbId.Text = window.txtId.Text;
                control.txbMembership.Text = membership.Membership;
                control.txbTarget.Text = membership.Target.ToString();

                window.stkMembership.Children.Add(control);
            }
            SetItemSource(mainWindow);
            LoadCustomerToView(mainWindow, 0);
            int start = 0, end = 0;
            LoadInfoOfPage(ref start, ref end);
            isEditingMembership = false;
            ClearView(window);
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

            mainWindow.txtNumOfCus.Text = String.Format("Trang {0} trên {1} trang", currentPage + 1, (customerList.Count - 1) / 10 + 1);
        }
        public void FindCustomer(MainWindow mainWindow)
        {
            mainWindow.cboSelectCustomerIdMembership.SelectedIndex = -1;
            mainWindow.cboSelectCustomerSort.SelectedIndex = -1;
            string nameSearching = mainWindow.txtSearchCustomer.Text.ToLower();
            customerList = CustomerDAL.Instance.FindByName(nameSearching);
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
                MembershipsType type = new MembershipsType(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(),1);
                itemSourceMembership.Add(type);
                itsAddCustomerMembership.Add(type);
            }
        }
        public void AddOrUpdateCustomer(AddCustomerWindow addCustomerWindow) 
        {
            if(CheckData(addCustomerWindow))// kiem tra du lieu dau vao
            {
                Customer customer = new Customer(ConvertToID(addCustomerWindow.txtId.Text), addCustomerWindow.txtName.Text, (addCustomerWindow.txtPhoneNumber.Text), addCustomerWindow.txtAddress.Text,
                    (addCustomerWindow.txtCMND.Text), 0, 0);

                if (isEditing)
                {
                    customer.TotalPrice = int.Parse(customerControl.txbAllPrice.Text);                   
                }
                else
                {
                    CustomerControl control = new CustomerControl();
                    control.txbSerial.Text = addCustomerWindow.txtId.Text;
                    control.txbName.Text = customer.CustomerName.ToString();
                    control.txbPhone.Text = customer.PhoneNumber.ToString();
                    control.txbAddress.Text = customer.Address.ToString();
                    control.txbAllPrice.Text = customer.TotalPrice.ToString();
                    control.txbLevelCus.Text = MembershipsTypeDAL.Instance.GetById(customer.IdMembership).Membership;
                    mainWindow.lbCountCustomer.Content = (int.Parse(mainWindow.lbCountCustomer.Content.ToString()) + 1).ToString();
                }
                CustomerDAL.Instance.AddOrUpdate(customer, isEditing);

                int indexSort = mainWindow.cboSelectCustomerSort.SelectedIndex;
                int indexFilter = mainWindow.cboSelectCustomerIdMembership.SelectedIndex;

                FindCustomer(mainWindow);
                mainWindow.cboSelectCustomerSort.SelectedIndex = indexSort;
                mainWindow.cboSelectCustomerIdMembership.SelectedIndex = indexFilter;

                mainWindow.lbCountAllPrice.Content = CustomerDAL.Instance.CountPrice().ToString();

                addCustomerWindow.Close();
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
            if (mainWindow.cboSelectCustomerIdMembership.SelectedIndex == -1)
            {
                return;
            }
            string nameSearching = mainWindow.txtSearchCustomer.Text.ToLower();
            customerList = CustomerDAL.Instance.FindByName(nameSearching);
            if (mainWindow.cboSelectCustomerIdMembership.SelectedIndex == 0)
            {
                LoadCustomerToView(mainWindow, 0);
                SortCustomer(mainWindow);
                return;
            }
            customerList.RemoveAll(x => x.IdMembership != filterMembership.IdMembershipsType);
            if (mainWindow.cboSelectCustomerIdMembership.SelectedIndex != -1)
            {
                SortCustomer(mainWindow);
            }
            else
            {
                LoadCustomerToView(mainWindow, 0);
            }
            if (filterMembership == null)
            {
                return;
            }
            listCustomerControl.Clear();
            if(filterMembership.IdMembershipsType != 0)
            {
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
