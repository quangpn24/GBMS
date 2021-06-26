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
using System.Windows.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class CustomerViewModel : BaseViewModel
    {
        private MainWindow mainWindow;
        public List<Customer> customerList = CustomerDAL.Instance.ConvertDBToList();
        private List<CustomerControl> listCustomerControl = new List<CustomerControl>();
        private List<CustomerControl> listSearch = new List<CustomerControl>();
        public bool isEditing = false;
        public bool isEditingMembership = false;
        private MembershipControl membershipControl;
        private string oldMembership;
        private PickCustomerWindow pickCustomerWindow;
        private CustomerControl customerControl;
        private PickCustomerControl pickedItem;
        Binding newBinding;
        private string name;
        private string phoneNumber;
        private string idNumber;
        private string address;
        private string target;
        public string Name { get => name; set { name = value; OnPropertyChanged(); } }
        public string PhoneNumber { get => phoneNumber; set { phoneNumber = value; OnPropertyChanged(); } }
        public string IdNumber { get => idNumber; set { idNumber = value; OnPropertyChanged(); } }
        public string Address { get => address; set { address = value; OnPropertyChanged(); } }
        public string Target { get => target; set { target = value; OnPropertyChanged("Target"); } }

        //CustomerWindow
        public ICommand LoadCustomerCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand GoToNextPageCommandCus { get; set; }
        public ICommand GoToPreviousPageCommandCus { get; set; }
        public ICommand FindCustomerCommand { get; set; }
        public ICommand OpenAddCustomerWindowCommand { get; set; }
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
        public ICommand SeparateThousandsCommand { get; set; }
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
            LoadPickCustomerCommand = new RelayCommand<PickCustomerWindow>(p => true, p => LoadPickCustomerToView(p, 0));
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
            OpenAddCustomerWindowCommand = new RelayCommand<Button>(p => true, p => OpenAddCustomerWindow(p));
            ExportExcelCommand = new RelayCommand<MainWindow>(p => true, p => ExportExcel(p));
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
            SeparateThousandsCommand = new RelayCommand<TextBox>((parameter) => true, (parameter) => SeparateThousands(parameter));
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
                CustomMessageBox.Show("Vui lòng chọn khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                pickCustomerWindow.Close();
            }
        }
        public void PickCustomer(PickCustomerControl pickCustomerControl)
        {
            if(pickedItem!=null)
            {
                pickedItem.txbId.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                pickedItem.txbName.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                pickedItem.txbPhoneNumber.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                pickedItem.txbIdNumber.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                pickedItem.txbAddress.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                pickedItem.txbRank.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
            }    
            pickCustomerControl.txbId.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            pickCustomerControl.txbName.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            pickCustomerControl.txbPhoneNumber.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            pickCustomerControl.txbIdNumber.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            pickCustomerControl.txbAddress.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            pickCustomerControl.txbRank.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            pickedItem = pickCustomerControl; 
            pickCustomerWindow.txbId.Text = pickCustomerControl.txbId.Text;
            pickCustomerWindow.txbName.Text = pickCustomerControl.txbName.Text;
            pickCustomerWindow.txbAddress.Text = pickCustomerControl.txbAddress.Text;
            pickCustomerWindow.txbPhoneNumber.Text = pickCustomerControl.txbPhoneNumber.Text;
            pickCustomerWindow.txbIdNumber.Text = pickCustomerControl.txbIdNumber.Text;
            pickCustomerWindow.txbRank.Text = pickCustomerControl.txbRank.Text;
            pickCustomerWindow.txbSpending.Text = pickCustomerControl.txbSpending.Text;
            pickCustomerControl.Focus();
        }
        public void LoadPickCustomerToView(PickCustomerWindow window, int currentPage)
        {
            customerList = CustomerDAL.Instance.ConvertDBToList();
            this.pickCustomerWindow = window;
            this.pickCustomerWindow.stkCustomer.Children.Clear();
            int start = 0, end = 0;
            this.currentPage = currentPage;
            LoadInfoOfPagePickCustomer(ref start, ref end);

            for (int i = start; i < end; i++)
            {
                PickCustomerControl ucCustomer = new PickCustomerControl();
                MembershipsType membershipsType = MembershipsTypeDAL.Instance.GetById(customerList[i].IdMembership);
                ucCustomer.txbId.Text = AddPrefix("KH", customerList[i].IdCustomer);
                ucCustomer.txbName.Text = customerList[i].CustomerName;
                ucCustomer.txbPhoneNumber.Text = customerList[i].PhoneNumber.ToString();
                ucCustomer.txbIdNumber.Text = customerList[i].IdNumber.ToString();
                ucCustomer.txbAddress.Text = customerList[i].Address.ToString();
                ucCustomer.txbSpending.Text = SeparateThousands(customerList[i].TotalPrice.ToString());
                ucCustomer.txbRank.Text = membershipsType.Membership;
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
                CustomMessageBox.Show("Không thể xóa vì tồn tại khách hàng có hạng thành viên này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBoxResult result = CustomMessageBox.Show("Xác nhận xóa hạng thành viên?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool isSuccess = MembershipsTypeDAL.Instance.Delete(idMembership);
                    if (isSuccess)
                    {
                        addMembershipWindow.stkMembership.Children.Remove(control);
                    }
                    else
                    {
                        CustomMessageBox.Show("Xoá thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        void ViewMembership(MembershipControl control)
        {
            Binding binding = BindingOperations.GetBinding(addMembershipWindow.txtMembership, TextBox.TextProperty);
            if (binding != null)
            {
                newBinding = CloneBinding(binding as BindingBase, binding.Source) as Binding;

            }
            BindingOperations.ClearBinding(addMembershipWindow.txtMembership, TextBox.TextProperty);

            oldMembership = control.txbMembership.Text;
            membershipControl = control;
            isEditingMembership = true;
            addMembershipWindow.txbTitle.Text = "Sửa hạng thành viên";
            addMembershipWindow.txtId.Text = control.txbId.Text;
            addMembershipWindow.btnSave.Content = "Cập nhật";

            addMembershipWindow.txtMembership.Text = control.txbMembership.Text;
            addMembershipWindow.txtMembership.SelectionStart = control.txbMembership.Text.Length;

            addMembershipWindow.txtTarget.Text = control.txbTarget.Text;
            addMembershipWindow.txtTarget.SelectionLength = control.txbTarget.Text.Length;
        }
        void ClearView(AddMembershipWindow window)
        {
            if (newBinding != null)
            {
                window.txtMembership.SetBinding(TextBox.TextProperty, newBinding);
            }
            isEditingMembership = false;
            window.txtId.Text = AddPrefix("TV", (MembershipsTypeDAL.Instance.GetMaxId() + 1));
            window.txtTarget.Text = null;
            window.txtMembership.Text = null;
        }
        void OpenMembershipWindow(MainWindow mainWindow)
        {
            AddMembershipWindow window = new AddMembershipWindow();
            window.txtId.Text = AddPrefix("TV", (MembershipsTypeDAL.Instance.GetMaxId() + 1));
            window.txtMembership.Text = null;
            window.txtTarget.Text = null;
            window.ShowDialog();
            for (int i = 0; i < customerList.Count; i++)
            {
                UpdateMembership(customerList[i], 0);
            }
            LoadCustomerToView(mainWindow, currentPage);
        }
        void UpdateMembership(Customer customer, long paidMoney)
        {
            var totalSpending = customer.TotalPrice + paidMoney;
            CustomerDAL.Instance.UpdateTotalSpending(customer.IdCustomer, totalSpending);
            List<KeyValuePair<long, int>> membershipList = MembershipsTypeDAL.Instance.GetSortedList();
            foreach (var mem in membershipList)
            {
                if (totalSpending >= mem.Key)
                {
                    customer.IdMembership = mem.Value;
                    CustomerDAL.Instance.UpdateMembership(customer.IdCustomer, mem.Value);
                    break;
                }
            }
        }

        void LoadMembership(AddMembershipWindow window)
        {
            addMembershipWindow = window;
            List<MembershipsType> membershipsTypes = MembershipsTypeDAL.Instance.GetList();
            window.stkMembership.Children.Clear();
            foreach (var membershipType in membershipsTypes)
            {
                if (membershipType.IdMembershipsType == 0)
                {
                    continue;
                }
                MembershipControl control = new MembershipControl();
                control.txbId.Text = AddPrefix("TV", membershipType.IdMembershipsType);
                control.txbMembership.Text = membershipType.Membership;
                control.txbTarget.Text = SeparateThousands(membershipType.Target.ToString());

                window.stkMembership.Children.Add(control);
            }
        }
        public void OpenAddCustomerWindow(Button btn)
        {
            AddCustomerWindow addCustomerWindow = new AddCustomerWindow();
            addCustomerWindow.txtId.Text = AddPrefix("KH", (CustomerDAL.Instance.GetMaxId() + 1));
            addCustomerWindow.txtAddress.Text = null;
            addCustomerWindow.txtCMND.Text = null;
            addCustomerWindow.txtName.Text = null;
            addCustomerWindow.txtPhoneNumber.Text = null;
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
            addCustomerWindow.btnSave.Content = "Cập nhật";
            addCustomerWindow.ShowDialog();
        }

        void AddOrUpdateMembership(AddMembershipWindow window)
        {
            if (string.IsNullOrEmpty(window.txtMembership.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập hạng thành viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                window.txtMembership.Focus();
                return;
            }
            if (string.IsNullOrEmpty(window.txtTarget.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập chỉ tiêu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                window.txtTarget.Focus();
                return;
            }
            if (window.txtMembership.Text != oldMembership && MembershipsTypeDAL.Instance.IsExisted(window.txtMembership.Text))
            {
                CustomMessageBox.Show("Hạng thành viên đã tồn tại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                window.txtMembership.Focus();
                return;
            }

            MembershipsType membership = new MembershipsType(ConvertToID(window.txtId.Text), window.txtMembership.Text,
                ConvertToNumber(window.txtTarget.Text));

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
                control.txbTarget.Text = SeparateThousands(membership.Target.ToString());

                window.stkMembership.Children.Add(control);
            }
            SetItemSource(mainWindow);
            LoadCustomerToView(mainWindow, 0);
            int start = 0, end = 0;
            LoadInfoOfPage(ref start, ref end);
            isEditingMembership = false;
            ClearView(window);
        }

        public void Load(MainWindow mainWindow)  // load lại label khi Add khách hàng mới
        {
            mainWindow.txbCountCustomer.Text = SeparateThousands(CustomerDAL.Instance.LoadData().Rows.Count.ToString());
            mainWindow.txbCountAllPrice.Text = SeparateThousands(CustomerDAL.Instance.CountPrice().ToString());
            SetItemSource(mainWindow);
            LoadCustomerToView(mainWindow, 0);
        }
        public void LoadCustomerToView(MainWindow mainWindow, int currentPage)
        {
            this.mainWindow = mainWindow;
            mainWindow.stkCustomer.Children.Clear();
            int start = 0, end = 0;
            this.currentPage = currentPage;
            LoadInfoOfPage(ref start, ref end);

            for (int i = start; i < end; i++)
            {
                CustomerControl ucCustomer = new CustomerControl();
                ucCustomer.txbSerial.Text = AddPrefix("KH", customerList[i].IdCustomer);
                ucCustomer.txbName.Text = customerList[i].CustomerName;
                ucCustomer.txbPhone.Text = customerList[i].PhoneNumber.ToString();
                ucCustomer.txbAddress.Text = customerList[i].Address.ToString();
                ucCustomer.txbAllPrice.Text = SeparateThousands(customerList[i].TotalPrice.ToString());
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
                CustomMessageBox.Show("Vui lòng nhập tên khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(addCustomerWindow.txtPhoneNumber.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập số điện thoại khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(addCustomerWindow.txtCMND.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập số CMND khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(addCustomerWindow.txtAddress.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập địa chỉ khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MembershipsType type = new MembershipsType(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(), 1);
                itemSourceMembership.Add(type);
                itsAddCustomerMembership.Add(type);
            }
        }
        public void AddOrUpdateCustomer(AddCustomerWindow addCustomerWindow)
        {
            if (CheckData(addCustomerWindow))// kiem tra du lieu dau vao
            {
                Customer customer = new Customer(ConvertToID(addCustomerWindow.txtId.Text), addCustomerWindow.txtName.Text, (addCustomerWindow.txtPhoneNumber.Text), addCustomerWindow.txtAddress.Text,
                    (addCustomerWindow.txtCMND.Text), 0, 0);

                if (isEditing)
                {
                    customer.TotalPrice = ConvertToNumber(customerControl.txbAllPrice.Text);
                }
                else
                {
                    CustomerControl control = new CustomerControl();
                    control.txbSerial.Text = addCustomerWindow.txtId.Text;
                    control.txbName.Text = customer.CustomerName.ToString();
                    control.txbPhone.Text = customer.PhoneNumber.ToString();
                    control.txbAddress.Text = customer.Address.ToString();
                    control.txbAllPrice.Text = SeparateThousands(customer.TotalPrice.ToString());
                    control.txbLevelCus.Text = MembershipsTypeDAL.Instance.GetById(customer.IdMembership).Membership;
                    mainWindow.txbCountCustomer.Text = SeparateThousands((int.Parse(mainWindow.txbCountCustomer.Text.ToString()) + 1).ToString());
                }
                if (CustomerDAL.Instance.AddOrUpdate(customer, isEditing))
                {
                    if (isEditing)
                    {
                        CustomMessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                    else
                    {
                        LoadPickCustomerToView(pickCustomerWindow, 0);
                        CustomMessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
                else
                {
                    CustomMessageBox.Show("Thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                int indexSort = mainWindow.cboSelectCustomerSort.SelectedIndex;
                int indexFilter = mainWindow.cboSelectCustomerIdMembership.SelectedIndex;

                FindCustomer(mainWindow);
                mainWindow.cboSelectCustomerSort.SelectedIndex = indexSort;
                mainWindow.cboSelectCustomerIdMembership.SelectedIndex = indexFilter;

                mainWindow.txbCountAllPrice.Text = CustomerDAL.Instance.CountPrice().ToString();

                addCustomerWindow.Close();
            }
            int start = 0, end = 0;
            LoadInfoOfPage(ref start, ref end);
        }
        public void ExportExcel(MainWindow main)
        {
            string filePath = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel |*.xlsx"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                filePath = saveFileDialog.FileName;
            }
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage p = new ExcelPackage())
                {
                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = "Danh sách khách hàng";
                    p.Workbook.Worksheets.Add("sheet");

                    ExcelWorksheet ws = p.Workbook.Worksheets[0];
                    ws.Name = "DSKH";
                    ws.Cells.Style.Font.Size = 11;
                    ws.Cells.Style.Font.Name = "Calibri";
                    ws.Cells.Style.WrapText = true;
                    ws.Column(1).Width = 10;
                    ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(2).Width = 30;
                    ws.Column(3).Width = 30;
                    ws.Column(4).Width = 30;
                    ws.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(5).Width = 30;
                    ws.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(6).Width = 20;
                    ws.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(7).Width = 30;
                    ws.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = { "STT", "Tên khách hàng", "Địa chỉ", "Số điện thoại", "Số CMND", "Hạng thành viên", "Tổng tiền" };

                    var countColHeader = arrColumnHeader.Count();

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge
                    ws.Row(1).Height = 15;
                    ws.Cells[1, 1].Value = "Danh sách khách hàng";
                    ws.Cells[1, 1, 1, countColHeader].Merge = true;

                    ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;
                    ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int colIndex = 1;
                    int rowIndex = 2;
                    //tạo các header từ column header đã tạo từ bên trên
                    foreach (var item in arrColumnHeader)
                    {
                        ws.Row(rowIndex).Height = 15;
                        var cell = ws.Cells[rowIndex, colIndex];
                        //set màu 
                        var fill = cell.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(255, 29, 161, 242);
                        cell.Style.Font.Bold = true;
                        //căn chỉnh các border
                        var border = cell.Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        cell.Value = item;
                        colIndex++;
                    }

                    // lấy ra danh sách nhà cung cấp
                    for (int i = 0; i < customerList.Count; i++)
                    {
                        ws.Row(rowIndex).Height = 15;
                        Customer customer = customerList[i];
                        colIndex = 1;
                        rowIndex++;
                        string address = "A" + rowIndex + ":G" + rowIndex;
                        ws.Cells[address].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        if (rowIndex % 2 != 0)
                        {
                            ws.Cells[address].Style.Fill.BackgroundColor.SetColor(255, 255, 255, 255);
                        }
                        else
                        {
                            ws.Cells[address].Style.Fill.BackgroundColor.SetColor(255, 229, 241, 255);
                        }

                        ws.Cells[rowIndex, colIndex++].Value = i + 1;
                        ws.Cells[rowIndex, colIndex++].Value = customer.CustomerName;
                        ws.Cells[rowIndex, colIndex++].Value = customer.Address;
                        ws.Cells[rowIndex, colIndex++].Value = customer.PhoneNumber;
                        ws.Cells[rowIndex, colIndex++].Value = customer.IdCustomer;
                        ws.Cells[rowIndex, colIndex++].Value = MembershipsTypeDAL.Instance.GetById(customer.IdMembership).Membership;
                        ws.Cells[rowIndex, colIndex++].Value = customer.TotalPrice;
                    }
                    //Lưu file lại
                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }
                CustomMessageBox.Show("Xuất dữ liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch
            {
                CustomMessageBox.Show("Có lỗi khi lưu file!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
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
            LoadCustomerToView(mainWindow, currentPage);
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
            if (filterMembership.IdMembershipsType != 0)
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
