using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Views;
using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using System.Data;
using ClosedXML.Excel;
using Microsoft.Win32;
using System.Windows;
using System.Collections.ObjectModel;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class SupplierViewModel : BaseViewModel
    {
        private List<SupplierControl> listSupplierToView = new List<SupplierControl>();
        public List<SupplierControl> ListSupplierToView { get => listSupplierToView; set => listSupplierToView = value; }
        private MainWindow mainWindow;
        private int currentPage;
        private bool isUpdate = false;
        private string oldSupplier;
        private SupplierControl supplierControl;
        private ObservableCollection<string> itemSource = new ObservableCollection<string>();
        public ObservableCollection<string> IteamSource { get => itemSource; set { itemSource = value; OnPropertyChanged(); } }

        public ICommand LoadSupplierCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand OpenAddSupplierWindowCommand { get; set; }
        public ICommand SaveCommnad { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand EditSupplierCommand { get; set; }
        public ICommand SelectSortTypeCommand { get; set; }
        public ICommand SortCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }

        public SupplierViewModel()
        {
            LoadSupplierCommand = new RelayCommand<MainWindow>(p => true, p => Init(p));
            ExportExcelCommand = new RelayCommand<MainWindow>(p => true, p => ExportExcel(p));
            SearchCommand = new RelayCommand<MainWindow>(p => true, p => Search(p));
            OpenAddSupplierWindowCommand = new RelayCommand<MainWindow>(p => true, p => OpenAddSupplierWindow(p));
            SaveCommnad = new RelayCommand<AddSupplierWindow>(p => true, p => AddOrUpdate(p));
            ExitCommand = new RelayCommand<AddSupplierWindow>(p => true, p => p.Close());
            EditSupplierCommand = new RelayCommand<SupplierControl>(p => true, p => OpenEditSupplierWindow(p));
            SelectSortTypeCommand = new RelayCommand<MainWindow>(p => true, p => SelectSortType(p));
            SortCommand = new RelayCommand<MainWindow>(p => true, p => Sort(p));
            PreviousPageCommand = new RelayCommand<MainWindow>(p => true, p => GoToPreviousPage(p));
            NextPageCommand = new RelayCommand<MainWindow>(p => true, p => GoToNextPage(p));
        }
        void Sort(MainWindow main)
        {

            if (main.cboSortType.SelectedIndex == 0)
            {
                switch (main.cboSortSupplier.SelectedIndex)
                {
                    case 0:
                        ListSupplierToView = ListSupplierToView.OrderBy(x => x.txbName.Text).ToList();
                        break;
                    case 1:
                        ListSupplierToView = ListSupplierToView.OrderByDescending(x => x.txbName.Text).ToList();
                        break;
                    default:
                        break;
                }
            }
            else if (main.cboSortType.SelectedIndex == 1)
            {
                switch (main.cboSortSupplier.SelectedIndex)
                {
                    case 0:
                        ListSupplierToView = ListSupplierToView.OrderBy(x => int.Parse(x.txbNumOfReceipts.Text)).ToList();
                        break;
                    case 1:
                        ListSupplierToView = ListSupplierToView.OrderByDescending(x => int.Parse(x.txbNumOfReceipts.Text)).ToList();
                        break;
                    default:
                        break;
                }
            }
            else if (main.cboSortType.SelectedIndex == 2)
            {
                switch (main.cboSortSupplier.SelectedIndex)
                {
                    case 0:
                        ListSupplierToView = ListSupplierToView.OrderBy(x => long.Parse(x.txbTotal.Text)).ToList();
                        break;
                    case 1:
                        ListSupplierToView = ListSupplierToView.OrderByDescending(x => long.Parse(x.txbTotal.Text)).ToList();
                        break;
                    default:
                        break;
                }
            }

            LoadSupplierToView(main);
        }
        void SelectSortType(MainWindow main)
        {
            itemSource.Clear();
            switch (main.cboSortType.SelectedIndex)
            {
                case 0:
                    main.cboSortSupplier.SelectedIndex = -1;
                    itemSource.Add("Từ A -> Z");
                    itemSource.Add("Từ Z -> A");
                    break;
                case 1:
                case 2:
                    main.cboSortSupplier.SelectedIndex = -1;
                    itemSource.Add("Tăng dần");
                    itemSource.Add("Giảm dần");
                    break;
                default:
                    break;
            }
        }
        void OpenEditSupplierWindow(SupplierControl control)
        {
            isUpdate = true;
            AddSupplierWindow newWindow = new AddSupplierWindow();
            newWindow.txtId.Text = control.txbId.Text;
            newWindow.txtName.Text = control.txbName.Text;
            newWindow.txtAddress.Text = control.txbAddress.Text;
            newWindow.txtPhoneNumber.Text = control.txbPhoneNumber.Text;
            oldSupplier = control.txbName.Text;
            supplierControl = control;
            newWindow.ShowDialog();
        }
        void AddOrUpdate(AddSupplierWindow wdAddSupplier)
        {
            if (string.IsNullOrEmpty(wdAddSupplier.txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp!");
                wdAddSupplier.txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(wdAddSupplier.txtAddress.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ!");
                wdAddSupplier.txtAddress.Focus();
                return;
            }
            if (string.IsNullOrEmpty(wdAddSupplier.txtPhoneNumber.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!");
                wdAddSupplier.txtPhoneNumber.Focus();
                return;
            }
            try
            {
                long temp = long.Parse(wdAddSupplier.txtPhoneNumber.Text);
            }
            catch 
            {
                MessageBox.Show("Số điện thoại không bao gồm chữ cái!");
                wdAddSupplier.txtPhoneNumber.Focus();
                return;
            }
            Supplier newSupplier = new Supplier(ConvertToID(wdAddSupplier.txtId.Text), wdAddSupplier.txtName.Text, wdAddSupplier.txtAddress.Text, wdAddSupplier.txtPhoneNumber.Text);
            if ((!isUpdate || wdAddSupplier.txtName.Text != oldSupplier) && SupplierDAL.Instance.IsExisted(wdAddSupplier.txtName.Text))
            {
                MessageBox.Show("Tên nhà cung cấp đã tồn tại!");
                wdAddSupplier.txtName.Focus();
                return;
            }

            if (SupplierDAL.Instance.InsertOrUpdate(newSupplier, isUpdate))
            {
                if (isUpdate)
                {
                    supplierControl.txbName.Text = newSupplier.Name;
                    supplierControl.txbAddress.Text = newSupplier.Address;
                    supplierControl.txbPhoneNumber.Text = newSupplier.PhoneNumber;
                }
                else
                {
                    SupplierControl control = new SupplierControl();
                    control.txbId.Text = AddPrefix("NC", newSupplier.Id);
                    control.txbName.Text = newSupplier.Name;
                    control.txbAddress.Text = newSupplier.Address;
                    control.txbPhoneNumber.Text = newSupplier.PhoneNumber;
                    control.txbNumOfReceipts.Text = "0";
                    control.txbTotal.Text = "0";
                    Search(mainWindow);
                    mainWindow.txbSupplierQuantity.Text = (int.Parse(mainWindow.txbSupplierQuantity.Text) + 1).ToString();
                }
                var importVM = mainWindow.grdImport.DataContext as ImportGoodsViewModel;
                importVM.Init(mainWindow);
                wdAddSupplier.Close();
            }
        }
        void OpenAddSupplierWindow(MainWindow main)
        {
            isUpdate = false;
            AddSupplierWindow newWindow = new AddSupplierWindow();
            int idMax = SupplierDAL.Instance.GetMaxId();
            if (idMax == -1)
            {
                return;
            }
            newWindow.txtId.Text = AddPrefix("NC", idMax + 1);
            newWindow.ShowDialog();
        }
        public void Search(MainWindow main)
        {
            DataTable dt = SupplierDAL.Instance.SearchByName(main.txtSearchSupplier.Text);
            ListSupplierToView.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SupplierControl control = new SupplierControl();
                control.txbId.Text = AddPrefix("NC", int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                control.txbName.Text = dt.Rows[i].ItemArray[1].ToString();
                control.txbAddress.Text = dt.Rows[i].ItemArray[2].ToString().ToString();
                control.txbPhoneNumber.Text = dt.Rows[i].ItemArray[3].ToString();
                control.txbNumOfReceipts.Text = StockReceiptDAL.Instance.NumOfReceiptsBySupplier(dt.Rows[i].ItemArray[0].ToString()).ToString();
                control.txbTotal.Text = StockReceiptDAL.Instance.SumMoneyBySupplier(dt.Rows[i].ItemArray[0].ToString()).ToString();
                ListSupplierToView.Add(control);
            }
            Sort(main);
        }
        public void Init(MainWindow main)
        {
            this.mainWindow = main;
            main.stkSupplier.Children.Clear(); 
            ListSupplierToView.Clear();
            currentPage = 1;
            DataTable dt = SupplierDAL.Instance.GetAll();
            long total = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SupplierControl control = new SupplierControl();
                control.txbId.Text = AddPrefix("NC", int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                control.txbName.Text = dt.Rows[i].ItemArray[1].ToString();
                control.txbAddress.Text = dt.Rows[i].ItemArray[2].ToString().ToString();
                control.txbPhoneNumber.Text = dt.Rows[i].ItemArray[3].ToString();
                control.txbNumOfReceipts.Text = StockReceiptDAL.Instance.NumOfReceiptsBySupplier(dt.Rows[i].ItemArray[0].ToString()).ToString();
                control.txbTotal.Text = StockReceiptDAL.Instance.SumMoneyBySupplier(dt.Rows[i].ItemArray[0].ToString()).ToString();
                total += long.Parse(control.txbTotal.Text);
                ListSupplierToView.Add(control);
            }
            main.txbSupplierQuantity.Text = ListSupplierToView.Count.ToString();
            main.txbTotalSpentToSupplier.Text = total.ToString();
            LoadSupplierToView(main);
        }
        public void LoadSupplierToView(MainWindow main)
        {
            main.stkSupplier.Children.Clear();

            int start = 0;
            int end = 0;
            LoadInfoOfPage(ref start, ref end);
            for (int i = start; i < end; i++)
            {
                main.stkSupplier.Children.Add(ListSupplierToView[i]);
            }
        }

        public void GoToPreviousPage(MainWindow main)
        {
            currentPage--;
            LoadSupplierToView(main);
        }
        public void GoToNextPage(MainWindow main)
        {
            currentPage++;
            LoadSupplierToView(main);
        }
        public void LoadInfoOfPage(ref int start, ref int end)
        {
            mainWindow.btnPrePageSupplier.IsEnabled = (currentPage == 1 ? false : true);
            mainWindow.btnNextPageSupplier.IsEnabled = (currentPage > ((ListSupplierToView.Count) / 11) ? false : true);
            start = (currentPage - 1) * 10;
            end = start + 10;
            if (currentPage - 1 == ListSupplierToView.Count / 10)
            {
                end = ListSupplierToView.Count;
            }
            mainWindow.txtNumOfSupplier.Text = String.Format("Trang {0} trên {1} trang", currentPage, ListSupplierToView.Count / 11 + 1);
        }
        public void ExportExcel(MainWindow main)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Mã NCC", typeof(string));
            table.Columns.Add("Tên nhà cung cấp", typeof(string));
            table.Columns.Add("Địa chỉ", typeof(string));
            table.Columns.Add("Số điện thoại", typeof(string));
            table.Columns.Add("Số đơn hàng", typeof(int));
            table.Columns.Add("Tổng tiền", typeof(long));

            for (int i = 0; i < ListSupplierToView.Count; i++)
            {
                SupplierControl control = ListSupplierToView[i];
                table.Rows.Add(control.txbId.Text, control.txbName.Text, control.txbAddress.Text,
                    control.txbPhoneNumber.Text, control.txbNumOfReceipts.Text, control.txbTotal.Text);
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel |*.xlsx"
            };
            if ((bool)saveFileDialog.ShowDialog())
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(table, "Danh sách nhà cung cấp");
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                MessageBox.Show("Xuất danh sách thành công!");
            }
        }
    }
}
