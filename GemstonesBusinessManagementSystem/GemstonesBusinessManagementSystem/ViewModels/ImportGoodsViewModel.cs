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
        private long totalPrice = 0;
        public long TotalPrice { get => totalPrice; set { totalPrice = value; OnPropertyChanged(); } }
        private long moneyToPay = 0;
        public long MoneyToPay { get => moneyToPay; set { moneyToPay = value; OnPropertyChanged(); } }

        private ImportGoodsWindow wdImportGoods;
        //Main window (manager receipt)
        private List<ReceiptControl> listReceiptControl = new List<ReceiptControl>();// luu tất cả các receipt ban đầu
        private List<ReceiptControl> listReceiptToView = new List<ReceiptControl>();
        private MainWindow mainWindow;
        private int currentPage = 1;

        //cbo goodsType
        private GoodsType selectedGoodsType = new GoodsType();
        public GoodsType SelectedGoodsType { get => selectedGoodsType; set { selectedGoodsType = value; OnPropertyChanged("SelectedGoodsType"); } }
        private ObservableCollection<GoodsType> itemSourceGoodsType = new ObservableCollection<GoodsType>();
        public ObservableCollection<GoodsType> ItemSourceGoodsType { get => itemSourceGoodsType; set { itemSourceGoodsType = value; OnPropertyChanged(); } }

        //cbo supplier
        private Supplier selectedSupplier = new Supplier();
        public Supplier SelectedSupplier { get => selectedSupplier; set { selectedSupplier = value; OnPropertyChanged("SelectedSupplier"); } }
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
        public ICommand LostFocusDiscountCommand { get; set; }
        public ICommand ChangeDiscountCommand { get; set; }

        // maneger list receipt(in main window)
        public ICommand OpenImportGoodsWindowCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand LoadReceiptCommand { get; set; }
        public ICommand ApproveCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand SelectReceiptCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand PrintReceiptInfoCommand { get; set; }


        //other
        public ICommand BackCommand { get; set; }
        public ICommand SelectionChangedGoodsTypeCommand { get; set; } // cboGoodsType
        public ImportGoodsViewModel()
        {
            SelectGoodsCommand = new RelayCommand<SearchGoodsControl>(p => true, p => SelectGoodsResult(p));
            DeleteCommand = new RelayCommand<ImportGoodsControl>(p => true, p => DeleteSelected(p));
            ChangeQuantityCommand = new RelayCommand<ImportGoodsControl>(p => true, p => ChangeQuantity(p));

            PrintReceiptCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => PrintReceipt(p));
            PayBillCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => PayBill(p));
            LostFocusDiscountCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => LostFocusDiscount(p));
            ChangeDiscountCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => ChangeDiscount(p));

            TextChangedSearchCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => AutoSuggest(p));
            SearchCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => Search(p));
            LostFocusSearchBarCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => LostFocusSearchBar(p));

            SelectionChangedGoodsTypeCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => SelectGoodsType(p));
            BackCommand = new RelayCommand<ImportGoodsWindow>(p => true, p => p.Close());


            LoadReceiptCommand = new RelayCommand<MainWindow>(p => true, p => Init(p));
            OpenImportGoodsWindowCommand = new RelayCommand<MainWindow>(p => true, p => OpenImportGoodsWindow(p));
            PreviousPageCommand = new RelayCommand<MainWindow>(p => true, p => GoToPreviousPage(p));
            NextPageCommand = new RelayCommand<MainWindow>(p => true, p => GoToNextPage(p));
            ApproveCommand = new RelayCommand<MainWindow>(p => true, p => ApproveRequest(p));
            ExportExcelCommand = new RelayCommand<MainWindow>(p => true, p => ExportExcel(p));
            SelectReceiptCommand = new RelayCommand<ReceiptControl>(p => true, p => SelectReceipt(p));
            PrintReceiptInfoCommand = new RelayCommand<MainWindow>(p => true, p => PrintReceiptInfo(p));
            CancelCommand = new RelayCommand<MainWindow>(p => true, p => Cancel(p));
        }
        public void Init(MainWindow main)
        {
            this.mainWindow = main;
            SetItemSource();
            currentPage = 1;
            DataTable dt = StockReceiptDAL.Instance.GetAll();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ReceiptControl control = new ReceiptControl();
                control.txbId.Text = AddPrefix("PN", int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                //control.txbImporter.Text = EmployeeDAL.Instance.GetNameByIdAccount(dt.Rows[i].ItemArray[1].ToString());
                //Chua lam current account nen tam de id nha
                control.txbImporter.Text = dt.Rows[i].ItemArray[1].ToString();
                control.txbDateReceipt.Text = DateTime.Parse(dt.Rows[i].ItemArray[2].ToString()).ToString("dd/MM/yyyy");
                control.txbMoneyToPay.Text = dt.Rows[i].ItemArray[3].ToString();
                control.txbSupplier.Text = SupplierDAL.Instance.GetNameById(dt.Rows[i].ItemArray[4].ToString());
                listReceiptControl.Add(control);
                listReceiptToView.Add(control);
            }
            if(listReceiptToView.Count > 0)
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
                receiptDetailControl.txbImportPrice.Text = goods.ImportPrice.ToString();
                receiptDetailControl.txbQuantity.Text = dt.Rows[i].ItemArray[2].ToString();
                receiptDetailControl.txbTotalPrice.Text = (goods.ImportPrice * int.Parse(dt.Rows[i].ItemArray[2].ToString())).ToString();
                mainWindow.stkReceiptDetail.Children.Add(receiptDetailControl);
                Total += goods.ImportPrice * int.Parse(dt.Rows[i].ItemArray[2].ToString());
            }
            mainWindow.txbIdReceipt.Text = control.txbId.Text;
            mainWindow.txbDateReceipt.Text = control.txbDateReceipt.Text;
            mainWindow.txbImporter.Text = control.txbImporter.Text;
            mainWindow.txbSupplier.Text = control.txbSupplier.Text;
            mainWindow.txbMoneyToPayGoods.Text = control.txbMoneyToPay.Text;
            mainWindow.txbDiscount.Text = (((Total - long.Parse(control.txbMoneyToPay.Text)) * 100) / Total).ToString() + "%";
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
        public void ApproveRequest(MainWindow main)
        {
            //Gan lai list ban dau
            listReceiptToView.Clear();
            for (int i = 0; i < listReceiptControl.Count; i++)
            {
                listReceiptToView.Add(listReceiptControl[i]);
            }

            if (!string.IsNullOrEmpty(main.cboSupplier.Text) && main.cboSupplier.SelectedIndex != 0)
            {
                listReceiptToView = listReceiptToView.FindAll(x => x.txbSupplier.Text == main.cboSupplier.Text);
                currentPage = 1;
            }
            bool cs = main.dpkStartDate.SelectedDate != null; // kiem tra ngay bat dau co null hay khong
            bool ce = main.dpkEndDate.SelectedDate != null; //kiem tra ngay ket thuc co null hay khong
            if (cs && ce)
            {
                DateTime startDate = DateTime.Parse(main.dpkStartDate.Text);
                DateTime endDate = DateTime.Parse(main.dpkEndDate.Text);
                listReceiptToView = listReceiptToView.FindAll(x => DateTime.Parse(x.txbDateReceipt.Text) >= startDate && DateTime.Parse(x.txbDateReceipt.Text) <= endDate);
            }
            else if ((cs && !ce) || (!cs && ce))
            {
                MessageBox.Show("Để trống cả hai hoặc nhập đầy đủ khoảng thời gian");
                return;
            }
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
            mainWindow.txtNumOfReceipt.Text = String.Format("{0} trong {1} mặt hàng", end - start, listReceiptToView.Count);
        }
        public void ExportExcel(MainWindow main) // cần custom lại
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    DataTable dt = StockReceiptDAL.Instance.GetAll();
                    dt.Columns.Remove("imageFile");
                    workbook.Worksheets.Add(dt, "Goods");
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                MessageBox.Show("Xuất dữ liệu thành công!!!", "Thông báo");
            }

        }
        public void OpenImportGoodsWindow(MainWindow main)
        {
            SelectedSupplier = null;
            ImportGoodsWindow newWindow = new ImportGoodsWindow();
            TotalPrice = 0;
            MoneyToPay = 0;
            main.Hide();
            int idStockReceiptMax = StockReceiptDAL.Instance.GetMaxId();
            if (idStockReceiptMax == -1)
            {
                MessageBox.Show("Lỗi hệ thống!");
                return;
            }
            newWindow.txbIdReceipt.Text = AddPrefix("PN", idStockReceiptMax + 1);
            newWindow.txbDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            newWindow.stkImportGoods.Children.Clear();
            this.wdImportGoods = newWindow;
            newWindow.ShowDialog();
            SelectedSupplier = null;
            main.Show();
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
        void LostFocusDiscount(ImportGoodsWindow wdImportGoods)
        {
            if (string.IsNullOrEmpty(wdImportGoods.txtDiscount.Text))
            {
                wdImportGoods.txtDiscount.Text = "0";
            }
            wdImportGoods.txtDiscount.SelectionStart = 0;
            wdImportGoods.txtDiscount.SelectionLength = wdImportGoods.txtDiscount.Text.Length;
        }
        void ChangeDiscount(ImportGoodsWindow wdImportGoods)
        {
            if (!string.IsNullOrEmpty(wdImportGoods.txtDiscount.Text))
            {
                MoneyToPay = TotalPrice - TotalPrice * int.Parse(wdImportGoods.txtDiscount.Text) / 100;
            }
        }
        void PrintReceipt(ImportGoodsWindow wdImportGoods)
        {
            StockReceiptTemplate receiptTemplate = new StockReceiptTemplate();
            receiptTemplate.txbIdStockReceipt.Text = wdImportGoods.txbIdReceipt.Text;
            receiptTemplate.txbDate.Text = wdImportGoods.txbDate.Text;
            receiptTemplate.txbSupplier.Text = wdImportGoods.cboSupplier.Text;
            //receiptTemplate.txbImporter.Text = 1: current account
            receiptTemplate.txbTotal.Text = wdImportGoods.txbTotalGoodsPrice.Text;
            receiptTemplate.txbDiscount.Text = wdImportGoods.txtDiscount.Text + "%";
            receiptTemplate.txbMoneyToPay.Text = wdImportGoods.txbMoneyToPay.Text;

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
            if(wdImportGoods.stkImportGoods.Children.Count == 0)
            {
                MessageBox.Show("Hiện tại chưa có sản phẩm nào được nhập!");
                return;
            }
            if (string.IsNullOrEmpty(wdImportGoods.cboSupplier.Text))
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp!");
                wdImportGoods.cboSupplier.Focus();
                return;
            }
            StockReceipt stockReceipt = new StockReceipt(ConvertToID(wdImportGoods.txbIdReceipt.Text),
                1, DateTime.Parse(wdImportGoods.txbDate.Text), long.Parse(wdImportGoods.txbMoneyToPay.Text),
                selectedSupplier.Id);
            k = StockReceiptDAL.Instance.Insert(stockReceipt);
            for (int i = 0; i < wdImportGoods.stkImportGoods.Children.Count; i++)
            {
                if (!k)
                    break;
                ImportGoodsControl control = (ImportGoodsControl)wdImportGoods.stkImportGoods.Children[i];
                StockReceiptInfo info = new StockReceiptInfo(stockReceipt.Id, ConvertToID(control.txbId.Text),
                    int.Parse(control.nsQuantity.Value.ToString()));
                k = StockReceiptInfoDAL.Instance.Insert(info);
                int quantity = GoodsDAL.Instance.GetQuantityById(ConvertToID(control.txbId.Text));
                if (quantity != -1)
                {
                    k = GoodsDAL.Instance.UpdateQuantity(ConvertToID(control.txbId.Text), quantity + info.Quantity);
                }
            }
            if (k)
            {
                var result = MessageBox.Show("Thanh toán thành công! Bạn có muốn in hóa đơn?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PrintReceipt(wdImportGoods);
                }

                //Add vao danh sach phieu nhap
                ReceiptControl receiptControl = new ReceiptControl();
                receiptControl.txbId.Text = wdImportGoods.txbIdReceipt.Text;
                receiptControl.txbDateReceipt.Text = wdImportGoods.txbDate.Text;
                //receiptControl.txbImporter.Text = cureent account
                receiptControl.txbSupplier.Text = SelectedSupplier.Name;
                receiptControl.txbMoneyToPay.Text = wdImportGoods.txbMoneyToPay.Text;
                this.listReceiptToView.Add(receiptControl);
                this.listReceiptControl.Add(receiptControl);
                this.currentPage = 1;
                LoadReceiptToView(mainWindow);

                wdImportGoods.Close();
            }
            else
            {
                MessageBox.Show("Nhập kho thất bại!");
            }
        }
        void ChangeQuantity(ImportGoodsControl control)
        {
            TotalPrice -= long.Parse(control.txbTotalPrice.Text);
            control.txbTotalPrice.Text = (int.Parse(control.txbImportPrice.Text) * control.nsQuantity.Value).ToString();
            TotalPrice += long.Parse(control.txbTotalPrice.Text);
            MoneyToPay = this.totalPrice - this.totalPrice * int.Parse(this.wdImportGoods.txtDiscount.Text) / 100;
        }
        void DeleteSelected(ImportGoodsControl control)
        {
            this.wdImportGoods.stkImportGoods.Children.Remove(control);
            int tmp = int.Parse(control.txbNumericalOder.Text);
            for (int i = tmp; i <= wdImportGoods.stkImportGoods.Children.Count; i++)
            {
                ((ImportGoodsControl)wdImportGoods.stkImportGoods.Children[i - 1]).txbNumericalOder.Text = i.ToString();
            }
            TotalPrice -= long.Parse(control.txbTotalPrice.Text);
            MoneyToPay = this.totalPrice - this.totalPrice * int.Parse(this.wdImportGoods.txtDiscount.Text) / 100;
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
            importControl.txbImportPrice.Text = control.txbImportPrice.Text;
            importControl.txbTotalPrice.Text = control.txbImportPrice.Text;

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
            TotalPrice += long.Parse(control.txbImportPrice.Text);
            MoneyToPay = this.totalPrice - this.totalPrice * int.Parse(this.wdImportGoods.txtDiscount.Text) / 100;
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
                control.txbNumericalOder.Text = (this.wdImportGoods.stkImportGoods.Children.Count + 1).ToString();
                control.txbId.Text = AddPrefix("SP", int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                control.txbName.Text = dt.Rows[i].ItemArray[1].ToString();
                control.txbUnit.Text = selectedGoodsType.Unit;
                control.txbGoodsType.Text = selectedGoodsType.Name;
                control.txbImportPrice.Text = dt.Rows[i].ItemArray[2].ToString();
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
                    TotalPrice += long.Parse(control.txbTotalPrice.Text);
                    MoneyToPay = this.totalPrice - this.totalPrice * int.Parse(this.wdImportGoods.txtDiscount.Text) / 100;
                }
            }
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Loại sản phẩm này vẫn chưa có sản phẩm!");
            }
            wdImportGoods.cboSelectFast.SelectedIndex = -1;
        }
        void SetItemSource()
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
                    control.txbImportPrice.Text = dt.Rows[i].ItemArray[2].ToString();
                    control.txbQuantity.Text = dt.Rows[i].ItemArray[3].ToString();
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
