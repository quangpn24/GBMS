using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Views;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Resources.Template;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Microsoft.Win32;
using ClosedXML.Excel;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class ImportGoodsViewModel : BaseViewModel
    {
        private ReceiptControl checkedItem;
        private long totalPrice = 0;
        public string TotalPrice { get => SeparateThousands(totalPrice.ToString()); set { totalPrice = ConvertToNumber(value); OnPropertyChanged(); } }
        private long moneyToPay = 0;
        public string MoneyToPay { get => SeparateThousands(moneyToPay.ToString()); set { moneyToPay = ConvertToNumber(value); OnPropertyChanged(); } }

        private bool isVND = true;
        private bool isTexboxVND = true;
        private double percentDiscount = 0;
        private long vndDiscount = 0;
        public string VndDiscount { get => SeparateThousands(vndDiscount.ToString()); set { vndDiscount = ConvertToNumber(value); OnPropertyChanged(); } }

        private ImportGoodsWindow wdImportGoods;
        //Main window (manager receipt)
        private List<ReceiptControl> listReceiptControl = new List<ReceiptControl>();// luu tất cả các receipt ban đầu
        private List<ReceiptControl> listReceiptToView = new List<ReceiptControl>();
        private MainWindow mainWindow;
        private int currentPage = 1;

        //cbo goodsType
        private GoodsType selectedGoodsType = new GoodsType();
        public GoodsType SelectedGoodsType { get => selectedGoodsType; set { selectedGoodsType = value; } }
        private ObservableCollection<GoodsType> itemSourceGoodsType = new ObservableCollection<GoodsType>();
        public ObservableCollection<GoodsType> ItemSourceGoodsType { get => itemSourceGoodsType; set { itemSourceGoodsType = value; OnPropertyChanged(); } }

        //cbo supplier
        private Supplier selectedSupplier = new Supplier();
        public Supplier SelectedSupplier { get => selectedSupplier; set { selectedSupplier = value; } }
        private ObservableCollection<Supplier> itemSourceSupplier = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> ItemSourceSupplier { get => itemSourceSupplier; set { itemSourceSupplier = value; OnPropertyChanged(); } }
        private ObservableCollection<Supplier> itemSourceFilter = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> ItemSourceFilter { get => itemSourceFilter; set { itemSourceFilter = value; OnPropertyChanged(); } }

        //import goods control and search goods control
        public ICommand SelectGoodsCommand { get; set; } // search goods control
        public ICommand DeleteCommand { get; set; }
        public ICommand ChangeQuantityCommand { get; set; }

        //search 
        public ICommand SearchCommand { get; set; }
        public ICommand TextChangedSearchCommand { get; set; }
        public ICommand LostFocusSearchBarCommand { get; set; }

        //bill
        public ICommand PayBillCommand { get; set; }
        public ICommand PrintReceiptCommand { get; set; }

        //Grid discount
        public ICommand CustomDiscountCommand { get; set; }
        public ICommand SelectVNDCommand { get; set; }
        public ICommand SelectPercentDiscountCommand { get; set; }
        public ICommand ChangeDiscountCommand { get; set; }
        public ICommand GotFocusDiscountCommand { get; set; }

        // maneger list receipt(in main window)
        public ICommand OpenImportGoodsWindowCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand LoadReceiptCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand SelectReceiptCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand PrintReceiptInfoCommand { get; set; }
        public ICommand SelectFilterCommand { get; set; }
        public ICommand SelectStartDateCommand { get; set; }
        public ICommand SelectEndDateCommand { get; set; }


        //other
        public ICommand BackCommand { get; set; }
        public ICommand SelectionChangedGoodsTypeCommand { get; set; } // cboGoodsType
        public ICommand HiddenGridDiscountCommand { get; set; }
        public ImportGoodsViewModel()
        {
            SelectGoodsCommand = new RelayCommand<SearchGoodsControl>(p => true, p => SelectGoodsResult(p));
            DeleteCommand = new RelayCommand<ImportGoodsControl>(p => true, p => DeleteSelected(p));
            ChangeQuantityCommand = new RelayCommand<ImportGoodsControl>(p => true, p => ChangeQuantity(p));

            PrintReceiptCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => PrintReceipt(p));
            PayBillCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => PayBill(p));
            CustomDiscountCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => VisibleGridDiscount(p));
            SelectPercentDiscountCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => SelectPercent(p));
            SelectVNDCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => SelectVND(p));
            ChangeDiscountCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => ChangeDiscount(p));
            GotFocusDiscountCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => GotFocusDiscount(p));

            TextChangedSearchCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => AutoSuggest(p));
            SearchCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => Search(p));
            LostFocusSearchBarCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => LostFocusSearchBar(p));

            SelectionChangedGoodsTypeCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => SelectGoodsType(p));
            BackCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => p.Close());
            HiddenGridDiscountCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => HiddenGridDiscount(p));


            LoadReceiptCommand = new RelayCommand<MainWindow>(p => true, p => Init(p));
            OpenImportGoodsWindowCommand = new RelayCommand<MainWindow>(p => true, p => OpenImportGoodsWindow(p));
            PreviousPageCommand = new RelayCommand<MainWindow>(p => true, p => GoToPreviousPage(p));
            NextPageCommand = new RelayCommand<MainWindow>(p => true, p => GoToNextPage(p));
            ExportExcelCommand = new RelayCommand<MainWindow>(p => true, p => ExportExcel(p));
            SelectReceiptCommand = new RelayCommand<ReceiptControl>(p => true, p => SelectReceipt(p));
            PrintReceiptInfoCommand = new RelayCommand<MainWindow>(p => true, p => PrintReceiptInfo(p));
            CancelCommand = new RelayCommand<MainWindow>(p => true, p => Cancel(p));
            SelectFilterCommand = new RelayCommand<MainWindow>(p => true, p => HandleFilter(p));
            SelectStartDateCommand = new RelayCommand<MainWindow>(p => true, p => HandleFilter(p));
            SelectEndDateCommand = new RelayCommand<MainWindow>(p => true, p => HandleFilter(p));
        }

        //Grid discount
        void VisibleGridDiscount(ImportGoodsWindow window)
        {
            window.grdDiscount.Visibility = Visibility.Visible;
            window.txtDiscount.Focus();
        }
        void HiddenGridDiscount(ImportGoodsWindow window)
        {
            if (!window.grdDiscount.IsMouseOver)
            {
                window.grdDiscount.Visibility = Visibility.Hidden;
            }
        }
        void SelectVND(ImportGoodsWindow window)
        {
            window.btnVND.Background = (Brush)new BrushConverter().ConvertFrom("#D14040");
            window.btnVND.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#D14040");
            window.btnPercent.Background = (Brush)new BrushConverter().ConvertFrom("#FFBDBDBD");
            window.btnPercent.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFBDBDBD");
            isVND = true;
            window.txtDiscount.Text = VndDiscount.ToString();
        }
        void SelectPercent(ImportGoodsWindow window)
        {
            window.btnVND.Background = (Brush)new BrushConverter().ConvertFrom("#FFBDBDBD");
            window.btnVND.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFBDBDBD");
            window.btnPercent.Background = (Brush)new BrushConverter().ConvertFrom("#d14040");
            window.btnPercent.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#d14040");
            isVND = false;
            window.txtDiscount.Text = percentDiscount.ToString();
        }
        void ChangeDiscount(ImportGoodsWindow wdImportGoods)
        {
            if (string.IsNullOrEmpty(wdImportGoods.txtDiscount.Text))
            {
                wdImportGoods.txtDiscount.Text = "0";
            }
            if (isVND)
            {
                SeparateThousands(wdImportGoods.txtDiscount);
            }
            if (isVND & isTexboxVND)
            {
                vndDiscount = ConvertToNumber(wdImportGoods.txtDiscount.Text);
                if (vndDiscount > ConvertToNumber(wdImportGoods.txbTotalGoodsPrice.Text))
                {
                    wdImportGoods.txtDiscount.Text = wdImportGoods.txbTotalGoodsPrice.Text;
                    vndDiscount = ConvertToNumber(wdImportGoods.txbTotalGoodsPrice.Text);
                }
                percentDiscount = Math.Round(vndDiscount / (double)ConvertToNumber(wdImportGoods.txbTotalGoodsPrice.Text) * 100, 2);
                VndDiscount = vndDiscount.ToString();
            }
            else if (!isVND && !isTexboxVND)
            {
                percentDiscount = double.Parse(wdImportGoods.txtDiscount.Text);
                if (percentDiscount > 100)
                {
                    wdImportGoods.txtDiscount.Text = "100";
                    percentDiscount = 100;
                }
                vndDiscount = long.Parse(Math.Ceiling(percentDiscount * (double)ConvertToNumber(wdImportGoods.txbTotalGoodsPrice.Text) / 100).ToString());
                VndDiscount = vndDiscount.ToString();
            }
            moneyToPay = totalPrice - ConvertToNumber(vndDiscount.ToString());
            MoneyToPay = moneyToPay.ToString();
        }
        void GotFocusDiscount(ImportGoodsWindow window)
        {
            if (isVND)
            {
                isTexboxVND = true;
            }
            else
            {
                isTexboxVND = false;
            }
            window.txtDiscount.SelectionStart = 0;
            window.txtDiscount.SelectionLength = window.txtDiscount.Text.Length;
        }
        //
        void HandleFilter(MainWindow main)
        {
            //Gan lai list ban dau
            listReceiptToView.Clear();
            for (int i = 0; i < listReceiptControl.Count; i++)
            {
                listReceiptToView.Add(listReceiptControl[i]);
            }
            if (main.cboSupplier.SelectedIndex > 0)
            {
                listReceiptToView = listReceiptToView.FindAll(x => x.txbSupplier.Text == selectedSupplier.Name);
                currentPage = 1;
            }
            if (main.dpkStartDate.SelectedDate > main.dpkEndDate.SelectedDate)
            {
                CustomMessageBox.Show("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                main.dpkStartDate.SelectedDate = null;
                main.dpkEndDate.SelectedDate = null;
                return;
            }
            bool cs = main.dpkStartDate.SelectedDate != null; // kiem tra ngay bat dau co null hay khong
            bool ce = main.dpkEndDate.SelectedDate != null; //kiem tra ngay ket thuc co null hay khong
            if (cs && ce)
            {
                DateTime startDate = DateTime.Parse(main.dpkStartDate.Text);
                DateTime endDate = DateTime.Parse(main.dpkEndDate.Text);
                listReceiptToView = listReceiptToView.FindAll(x => DateTime.Parse(x.txbDateReceipt.Text) >= startDate && DateTime.Parse(x.txbDateReceipt.Text) <= endDate);
            }
            LoadReceiptToView(main);
        }
        public void Init(MainWindow main)
        {
            this.mainWindow = main;
            SetItemSource();
            currentPage = 1;
            DataTable dt = StockReceiptDAL.Instance.GetAll();
            listReceiptControl.Clear();
            listReceiptToView.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ReceiptControl control = new ReceiptControl();
                control.txbId.Text = AddPrefix("PN", int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                control.txbImporter.Text = CurrentAccount.Name;
                control.txbDateReceipt.Text = DateTime.Parse(dt.Rows[i].ItemArray[2].ToString()).ToString("dd/MM/yyyy");
                control.txbMoneyToPay.Text = SeparateThousands(dt.Rows[i].ItemArray[3].ToString());
                control.txbSupplier.Text = SupplierDAL.Instance.GetNameById(dt.Rows[i].ItemArray[5].ToString());
                listReceiptControl.Add(control);
                listReceiptToView.Add(control);
            }
            if (listReceiptToView.Count > 0)
            {
                SelectReceipt(listReceiptToView[0]);
            }
            LoadReceiptToView(main);
        }

        void PrintReceiptInfo(MainWindow main)
        {
            StockReceiptTemplate receiptTemplate = new StockReceiptTemplate();
            receiptTemplate.txbIdStockReceipt.Text = main.txbIdReceipt.Text;
            receiptTemplate.txbDate.Text = main.txbDateReceipt.Text;
            receiptTemplate.txbImporter.Text = main.txbImporter.Text;
            receiptTemplate.txbSupplier.Text = main.txbSupplier.Text;
            receiptTemplate.txbTotal.Text = main.txbTotalMoneyGoods.Text;
            receiptTemplate.txbDiscount.Text = main.txbDiscount.Text;
            receiptTemplate.txbMoneyToPay.Text = main.txbMoneyToPayGoods.Text;

            //Load
            for (int i = 0; i < wdImportGoods.stkImportGoods.Children.Count; i++)
            {
                ImportGoodsControl importControl = (ImportGoodsControl)wdImportGoods.stkImportGoods.Children[i];
                StockReceiptInfoControl infoReceiptControl = new StockReceiptInfoControl();
                infoReceiptControl.txbNumericalOder.Text = (i + 1).ToString();
                infoReceiptControl.txbName.Text = importControl.txbName.Text;
                infoReceiptControl.txbUnit.Text = importControl.txbUnit.Text;
                infoReceiptControl.txbQuantity.Text = importControl.nsQuantity.Value.ToString();
                infoReceiptControl.txbImportPrice.Text = importControl.txbImportPrice.Text;
                infoReceiptControl.txbTotalPrice.Text = importControl.txbTotalPrice.Text;
                receiptTemplate.stkStockReceiptInfo.Children.Add(infoReceiptControl);
            }
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    receiptTemplate.btnPrint.Visibility = Visibility.Hidden;
                    printDialog.PrintVisual(receiptTemplate.grdPrint, "Stock receipt");
                }
            }
            finally
            {
                receiptTemplate.btnPrint.Visibility = Visibility.Visible;
            }
        }
        public void Cancel(MainWindow main)
        {
            main.cboSupplier.SelectedIndex = -1;
            main.dpkStartDate.Text = null;
            main.dpkEndDate.Text = null;
        }
        public void SelectReceipt(ReceiptControl control)
        {
            mainWindow.stkReceiptDetail.Children.Clear();
            DataTable dt = StockReceiptInfoDAL.Instance.GetByIdReceipt(ConvertToID(control.txbId.Text));
            long Total = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ReceiptDetailControl receiptDetailControl = new ReceiptDetailControl();
                Goods goods = GoodsDAL.Instance.GetById(dt.Rows[i].ItemArray[1].ToString());
                GoodsType type = GoodsTypeDAL.Instance.GetById(goods.IdGoodsType);
                receiptDetailControl.txbId.Text = AddPrefix("SP", goods.IdGoods);
                receiptDetailControl.txbName.Text = goods.Name;
                receiptDetailControl.txbGoodsType.Text = type.Name;
                receiptDetailControl.txbUnit.Text = type.Unit;
                receiptDetailControl.txbImportPrice.Text = SeparateThousands(goods.ImportPrice.ToString());
                receiptDetailControl.txbQuantity.Text = dt.Rows[i].ItemArray[2].ToString();
                receiptDetailControl.txbTotalPrice.Text = SeparateThousands((goods.ImportPrice * int.Parse(dt.Rows[i].ItemArray[2].ToString())).ToString());
                mainWindow.stkReceiptDetail.Children.Add(receiptDetailControl);
                Total += goods.ImportPrice * int.Parse(dt.Rows[i].ItemArray[2].ToString());
            }
            mainWindow.txbIdReceipt.Text = control.txbId.Text;
            mainWindow.txbDateReceipt.Text = control.txbDateReceipt.Text;
            mainWindow.txbImporter.Text = control.txbImporter.Text;
            mainWindow.txbSupplier.Text = control.txbSupplier.Text;
            mainWindow.txbMoneyToPayGoods.Text = control.txbMoneyToPay.Text;
            mainWindow.txbTotalMoneyGoods.Text = SeparateThousands(Total.ToString());
            mainWindow.txbDiscount.Text = SeparateThousands((Total - ConvertToNumber(control.txbMoneyToPay.Text)).ToString());

            if (checkedItem != null) // dua lai mau xam
            {
                checkedItem.txbId.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbDateReceipt.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbImporter.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbSupplier.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbMoneyToPay.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
            }
            // chuyen sang mau duoc chon
            control.txbId.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            control.txbDateReceipt.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            control.txbImporter.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            control.txbSupplier.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            control.txbMoneyToPay.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            checkedItem = control;
        }
        public void LoadReceiptToView(MainWindow main)
        {
            main.stkReceipt.Children.Clear();
            int start = 0;
            int end = 0;
            LoadInfoOfPage(ref start, ref end);
            for (int i = start; i < end; i++)
            {
                main.stkReceipt.Children.Add(listReceiptToView[i]);
            }
        }

        public void GoToPreviousPage(MainWindow main)
        {
            currentPage--;
            LoadReceiptToView(main);
        }
        public void GoToNextPage(MainWindow main)
        {
            currentPage++;
            LoadReceiptToView(main);
        }
        public void LoadInfoOfPage(ref int start, ref int end)
        {
            mainWindow.btnPrePageReceipt.IsEnabled = (currentPage == 1 ? false : true);
            mainWindow.btnNextPageReceipt.IsEnabled = (currentPage > ((listReceiptToView.Count) / 11) ? false : true);
            start = (currentPage - 1) * 10;
            end = start + 10;
            if (currentPage - 1 == listReceiptToView.Count / 10)
            {
                end = listReceiptToView.Count;
            }
            mainWindow.txtNumOfReceipt.Text = String.Format("Trang {0} trên {1} trang", currentPage, listReceiptToView.Count / 11 + 1);
        }
        public void ExportExcel(MainWindow main)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Mã PN", typeof(string));
            table.Columns.Add("Ngày nhập", typeof(string));
            table.Columns.Add("Người nhập", typeof(string));
            table.Columns.Add("Nhà cung cấp", typeof(string));
            table.Columns.Add("Tổng tiền", typeof(long));
            table.Columns.Add("Giảm giá", typeof(string));
            table.Columns.Add("Thanh toán", typeof(long));


            for (int i = 0; i < listReceiptToView.Count; i++)
            {
                ReceiptControl control = listReceiptToView[i];
                long totalMoneyGoods = StockReceiptInfoDAL.Instance.SumMoneyByIdReceipt(ConvertToIDString(control.txbId.Text));
                string discount = (totalMoneyGoods - long.Parse(control.txbMoneyToPay.Text)).ToString();
                table.Rows.Add(control.txbId.Text, control.txbDateReceipt.Text, control.txbImporter.Text,
                    control.txbSupplier.Text, totalMoneyGoods.ToString(), discount, control.txbMoneyToPay.Text);
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel |*.xlsx"
            };
            if ((bool)saveFileDialog.ShowDialog())
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(table, "Danh sách phiếu nhập hàng");
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                CustomMessageBox.Show("Xuất danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }
        public void OpenImportGoodsWindow(MainWindow main)
        {
            SelectedSupplier = null;
            selectedGoodsType = null;
            ImportGoodsWindow newWindow = new ImportGoodsWindow();
            TotalPrice = "0";
            MoneyToPay = "0";
            //main.Hide();
            int idStockReceiptMax = StockReceiptDAL.Instance.GetMaxId();
            if (idStockReceiptMax == -1)
            {
                CustomMessageBox.Show("Lỗi hệ thống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            newWindow.txbIdReceipt.Text = AddPrefix("PN", idStockReceiptMax + 1);
            newWindow.txbDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            newWindow.stkImportGoods.Children.Clear();
            this.wdImportGoods = newWindow;
            newWindow.ShowDialog();
            HandleFilter(mainWindow);
            HomeViewModel homeVM = (HomeViewModel)main.DataContext;
            homeVM.Uid = "21";
            homeVM.Navigate(main);
            //main.ShowDialog();
        }

        void LostFocusSearchBar(ImportGoodsWindow wdImportGoods)
        {
            if (!wdImportGoods.grdSearchResult.IsMouseOver)
            {
                wdImportGoods.grdSearchResult.Visibility = Visibility.Hidden;
            }
            wdImportGoods.txtSearch.SelectionStart = 0;
            wdImportGoods.txtSearch.SelectionLength = this.wdImportGoods.txtSearch.Text.Length;
        }
        void PrintReceipt(ImportGoodsWindow wdImportGoods)
        {
            StockReceiptTemplate receiptTemplate = new StockReceiptTemplate();
            receiptTemplate.txbIdStockReceipt.Text = wdImportGoods.txbIdReceipt.Text;
            receiptTemplate.txbDate.Text = wdImportGoods.txbDate.Text;
            receiptTemplate.txbSupplier.Text = wdImportGoods.cboSupplier.Text;
            receiptTemplate.txbImporter.Text = CurrentAccount.Name;
            receiptTemplate.txbTotal.Text = wdImportGoods.txbTotalGoodsPrice.Text;
            receiptTemplate.txbDiscount.Text = wdImportGoods.btnDiscount.Content.ToString();
            receiptTemplate.txbMoneyToPay.Text = wdImportGoods.txbMoneyToPay.Text;

            List<Parameter> parameters = ParameterDAL.Instance.GetData();
            receiptTemplate.txbStoreName.Text = parameters[1].Value;
            receiptTemplate.txbStoreAddress.Text = parameters[2].Value;
            receiptTemplate.txbStorePhoneNumber.Text = parameters[3].Value;

            //Load
            for (int i = 0; i < wdImportGoods.stkImportGoods.Children.Count; i++)
            {
                ImportGoodsControl importControl = (ImportGoodsControl)wdImportGoods.stkImportGoods.Children[i];
                StockReceiptInfoControl infoReceiptControl = new StockReceiptInfoControl();
                infoReceiptControl.txbNumericalOder.Text = (i + 1).ToString();
                infoReceiptControl.txbName.Text = importControl.txbName.Text;
                infoReceiptControl.txbUnit.Text = importControl.txbUnit.Text;
                infoReceiptControl.txbQuantity.Text = importControl.nsQuantity.Value.ToString();
                infoReceiptControl.txbImportPrice.Text = importControl.txbImportPrice.Text;
                infoReceiptControl.txbTotalPrice.Text = importControl.txbTotalPrice.Text;
                receiptTemplate.stkStockReceiptInfo.Children.Add(infoReceiptControl);
            }
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    receiptTemplate.btnPrint.Visibility = Visibility.Hidden;
                    printDialog.PrintVisual(receiptTemplate.grdPrint, "Stock receipt");
                }
            }
            finally
            {
                receiptTemplate.btnPrint.Visibility = Visibility.Visible;
            }
        }
        void PayBill(ImportGoodsWindow wdImportGoods)
        {
            bool k;
            if (wdImportGoods.stkImportGoods.Children.Count == 0)
            {
                CustomMessageBox.Show("Hiện tại chưa có sản phẩm nào được nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(wdImportGoods.cboSupplier.Text))
            {
                CustomMessageBox.Show("Vui lòng chọn nhà cung cấp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                wdImportGoods.cboSupplier.Focus();
                return;
            }
            StockReceipt stockReceipt = new StockReceipt(ConvertToID(wdImportGoods.txbIdReceipt.Text),
                CurrentAccount.IdAccount, DateTime.Parse(wdImportGoods.txbDate.Text), ConvertToNumber(wdImportGoods.txbMoneyToPay.Text),
                vndDiscount, selectedSupplier.Id);
            k = StockReceiptDAL.Instance.Insert(stockReceipt);
            for (int i = 0; i < wdImportGoods.stkImportGoods.Children.Count; i++)
            {
                if (!k)
                    break;
                ImportGoodsControl control = (ImportGoodsControl)wdImportGoods.stkImportGoods.Children[i];
                StockReceiptInfo info = new StockReceiptInfo(stockReceipt.Id, ConvertToID(control.txbId.Text),
                    int.Parse(control.nsQuantity.Value.ToString()), ConvertToNumber(control.txbImportPrice.Text));
                k = StockReceiptInfoDAL.Instance.Insert(info);
                k = GoodsDAL.Instance.UpdateQuantity(ConvertToID(control.txbId.Text), info.Quantity);
            }
            if (k)
            {
                var result = CustomMessageBox.Show("Thanh toán thành công! Bạn có muốn in hóa đơn?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PrintReceipt(wdImportGoods);
                }
                //Add vao danh sach phieu nhap
                ReceiptControl receiptControl = new ReceiptControl();
                receiptControl.txbId.Text = wdImportGoods.txbIdReceipt.Text;
                receiptControl.txbDateReceipt.Text = wdImportGoods.txbDate.Text;
                receiptControl.txbImporter.Text = CurrentAccount.Name;
                receiptControl.txbSupplier.Text = SelectedSupplier.Name;
                receiptControl.txbMoneyToPay.Text = wdImportGoods.txbMoneyToPay.Text;
                this.listReceiptToView.Add(receiptControl);
                this.listReceiptControl.Add(receiptControl);
                this.currentPage = 1;
                LoadReceiptToView(mainWindow);

                //Update tab sale
                SaleViewModel saleVM = (SaleViewModel)mainWindow.grdSale.DataContext;
                mainWindow.stkSelectedGoods.Children.Clear();
                saleVM.Search(mainWindow);
                saleVM.LoadDefault(mainWindow);

                //Update tab supplier
                SupplierViewModel supplierVM = (SupplierViewModel)mainWindow.grdSupplier.DataContext;
                supplierVM.Search(mainWindow);
                mainWindow.txbTotalSpentToSupplier.Text = SeparateThousands((ConvertToNumber(mainWindow.txbTotalSpentToSupplier.Text) + ConvertToNumber(wdImportGoods.txbMoneyToPay.Text)).ToString());
                TotalPrice = "0";
                MoneyToPay = "0";

                //Clean
                selectedSupplier = null;
                wdImportGoods.cboSupplier.Text = null;
                wdImportGoods.txtSearch.Text = null;
                TotalPrice = "0";
                MoneyToPay = "0";
                int idStockReceiptMax = StockReceiptDAL.Instance.GetMaxId();
                if (idStockReceiptMax == -1)
                {
                    CustomMessageBox.Show("Lỗi hệ thống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                wdImportGoods.txbIdReceipt.Text = AddPrefix("PN", idStockReceiptMax + 1);
                wdImportGoods.txbDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
                wdImportGoods.stkImportGoods.Children.Clear();
            }
            else
            {
                CustomMessageBox.Show("Nhập kho thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void ChangeQuantity(ImportGoodsControl control)
        {
            totalPrice -= ConvertToNumber(control.txbTotalPrice.Text);
            control.txbTotalPrice.Text = SeparateThousands((ConvertToNumber(control.txbImportPrice.Text) * control.nsQuantity.Value).ToString());
            totalPrice += ConvertToNumber(control.txbTotalPrice.Text);
            moneyToPay = this.totalPrice - vndDiscount;
            if (moneyToPay < 0)
            {
                moneyToPay = 0;
                vndDiscount = totalPrice;
            }
            TotalPrice = totalPrice.ToString();
            MoneyToPay = moneyToPay.ToString();
            VndDiscount = vndDiscount.ToString();
        }
        void DeleteSelected(ImportGoodsControl control)
        {
            this.wdImportGoods.stkImportGoods.Children.Remove(control);
            int tmp = int.Parse(control.txbNumericalOder.Text);
            for (int i = tmp; i <= wdImportGoods.stkImportGoods.Children.Count; i++)
            {
                ((ImportGoodsControl)wdImportGoods.stkImportGoods.Children[i - 1]).txbNumericalOder.Text = i.ToString();
            }
            totalPrice -= ConvertToNumber(control.txbTotalPrice.Text);
            moneyToPay = this.totalPrice - ConvertToNumber(wdImportGoods.btnDiscount.Content.ToString());
            if (moneyToPay < 0)
            {
                moneyToPay = 0;
                vndDiscount = totalPrice;
            }
            TotalPrice = totalPrice.ToString();
            VndDiscount = vndDiscount.ToString();
            MoneyToPay = moneyToPay.ToString();
        }
        void SelectGoodsResult(SearchGoodsControl control)
        {
            this.wdImportGoods.grdSearchResult.Visibility = Visibility.Hidden;
            GoodsType goodsType = GoodsTypeDAL.Instance.GetByIdGoods(ConvertToID(control.txbId.Text));
            ImportGoodsControl importControl = new ImportGoodsControl();
            importControl.txbNumericalOder.Text = (this.wdImportGoods.stkImportGoods.Children.Count + 1).ToString();
            importControl.txbId.Text = control.txbId.Text;
            importControl.txbName.Text = control.txbName.Text;
            importControl.txbUnit.Text = goodsType.Unit;
            importControl.txbGoodsType.Text = goodsType.Name;
            importControl.txbImportPrice.Text = SeparateThousands(control.txbImportPrice.Text);
            importControl.txbTotalPrice.Text = SeparateThousands(control.txbImportPrice.Text);

            for (int i = 0; i < wdImportGoods.stkImportGoods.Children.Count; i++)
            {
                ImportGoodsControl temp = (ImportGoodsControl)wdImportGoods.stkImportGoods.Children[i];
                if (importControl.txbId.Text == temp.txbId.Text)
                {
                    temp.nsQuantity.Value++;
                    return;
                }
            }
            this.wdImportGoods.stkImportGoods.Children.Add(importControl);
            totalPrice += ConvertToNumber(control.txbImportPrice.Text);
            moneyToPay = this.totalPrice - ConvertToNumber(wdImportGoods.btnDiscount.Content.ToString());
            TotalPrice = totalPrice.ToString();
            MoneyToPay = moneyToPay.ToString();
        }
        void SelectGoodsType(ImportGoodsWindow wdImportGoods)
        {
            if (wdImportGoods.cboSelectFast.SelectedIndex == -1)
                return;
            DataTable dt = GoodsDAL.Instance.GetByidGoodsType(selectedGoodsType.IdGoodsType);
            bool isExist;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                isExist = false;
                ImportGoodsControl control = new ImportGoodsControl();
                control.txbNumericalOder.Text = (wdImportGoods.stkImportGoods.Children.Count + 1).ToString();
                control.txbId.Text = AddPrefix("SP", int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                control.txbName.Text = dt.Rows[i].ItemArray[1].ToString();
                control.txbUnit.Text = selectedGoodsType.Unit;
                control.txbGoodsType.Text = selectedGoodsType.Name;
                control.txbImportPrice.Text = SeparateThousands(dt.Rows[i].ItemArray[2].ToString());
                control.txbTotalPrice.Text = control.txbImportPrice.Text;
                for (int j = 0; j < wdImportGoods.stkImportGoods.Children.Count; j++)
                {
                    ImportGoodsControl temp = (ImportGoodsControl)wdImportGoods.stkImportGoods.Children[j];
                    if (control.txbId.Text == temp.txbId.Text)
                    {
                        temp.nsQuantity.Value++;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    wdImportGoods.stkImportGoods.Children.Add(control);
                    totalPrice += ConvertToNumber(control.txbTotalPrice.Text);
                    moneyToPay = this.totalPrice - ConvertToNumber(wdImportGoods.btnDiscount.Content.ToString());
                    TotalPrice = totalPrice.ToString();
                    MoneyToPay = moneyToPay.ToString();
                }
            }
            if (dt.Rows.Count == 0)
            {
                CustomMessageBox.Show("Loại sản phẩm này vẫn chưa có sản phẩm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            wdImportGoods.cboSelectFast.SelectedIndex = -1;
        }
        public void SetItemSource()
        {
            //Set item source goodstype
            itemSourceGoodsType.Clear();
            DataTable dataGoodsType = GoodsTypeDAL.Instance.GetActive();
            for (int i = 0; i < dataGoodsType.Rows.Count; i++)
            {
                GoodsType type = new GoodsType();
                type.IdGoodsType = int.Parse(dataGoodsType.Rows[i].ItemArray[0].ToString());
                type.Name = dataGoodsType.Rows[i].ItemArray[1].ToString();
                type.Unit = dataGoodsType.Rows[i].ItemArray[3].ToString();
                itemSourceGoodsType.Add(type);
            }
            //Set item source supplier // item source filter
            itemSourceSupplier.Clear();
            itemSourceFilter.Clear();
            Supplier temp = new Supplier();
            temp.Name = "Tất cả";
            itemSourceFilter.Add(temp);
            DataTable dataSupllier = SupplierDAL.Instance.GetAll();
            for (int i = 0; i < dataSupllier.Rows.Count; i++)
            {
                Supplier supplier = new Supplier(int.Parse(dataSupllier.Rows[i].ItemArray[0].ToString()),
                    dataSupllier.Rows[i].ItemArray[1].ToString(), dataSupllier.Rows[i].ItemArray[2].ToString(),
                    dataSupllier.Rows[i].ItemArray[3].ToString());
                itemSourceSupplier.Add(supplier);
                itemSourceFilter.Add(supplier);
            }
        }
        void Search(ImportGoodsWindow wdImportGoods)
        {
            if (wdImportGoods.stkSearchResult.Children.Count != 0 && wdImportGoods.txtSearch.IsFocused)
            {
                SearchGoodsControl control = (SearchGoodsControl)wdImportGoods.stkSearchResult.Children[0];
                SelectGoodsResult(control);
                wdImportGoods.txtSearch.Text = control.txbName.Text;
                wdImportGoods.txtSearch.SelectionStart = 0;
                wdImportGoods.txtSearch.SelectionLength = wdImportGoods.txtSearch.Text.Length;
                wdImportGoods.txtSearch.Focus();
                wdImportGoods.grdSearchResult.Visibility = Visibility.Hidden;
            }
        }
        void AutoSuggest(ImportGoodsWindow wdImportGoods)
        {
            wdImportGoods.stkSearchResult.Children.Clear();
            if (string.IsNullOrEmpty(wdImportGoods.txtSearch.Text))
            {
                wdImportGoods.grdSearchResult.Visibility = Visibility.Hidden;
                return;
            }
            wdImportGoods.grdSearchResult.Visibility = Visibility.Visible;
            wdImportGoods.txbNoResult.Visibility = Visibility.Hidden;
            DataTable dt = GoodsDAL.Instance.GetActive();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string goodsName = dt.Rows[i].ItemArray[1].ToString().ToLower();
                if (goodsName.Contains(wdImportGoods.txtSearch.Text.ToLower()))
                {
                    SearchGoodsControl control = new SearchGoodsControl();
                    Byte[] tmp = Convert.FromBase64String(dt.Rows[i].ItemArray[5].ToString());
                    control.imgGoods.Source = Converter.Instance.ConvertByteToBitmapImage(tmp);
                    control.txbName.Text = dt.Rows[i].ItemArray[1].ToString();
                    control.txbId.Text = AddPrefix("SP", int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                    control.txbImportPrice.Text = SeparateThousands(dt.Rows[i].ItemArray[2].ToString());
                    control.txbQuantity.Text = SeparateThousands(dt.Rows[i].ItemArray[3].ToString());
                    wdImportGoods.stkSearchResult.Children.Add(control);
                }
            }
            if (wdImportGoods.stkSearchResult.Children.Count == 0)
            {
                wdImportGoods.txbNoResult.Visibility = Visibility.Visible;
            }
        }
    }
}
