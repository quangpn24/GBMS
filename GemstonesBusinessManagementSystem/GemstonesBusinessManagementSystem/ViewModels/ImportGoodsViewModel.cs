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

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class ImportGoodsViewModel : BaseViewModel
    {
        private long totalPrice = 0;
        public long TotalPrice { get => totalPrice; set { totalPrice = value; OnPropertyChanged(); } }
        private long moneyToPay = 0;
        public long MoneyToPay { get => moneyToPay; set { moneyToPay = value; OnPropertyChanged(); } }

        private ImportGoodsWindow wdImportGoods;

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

        //other
        public ICommand BackCommand { get; set; }
        public ICommand SelectionChangedGoodsTypeCommand { get; set; } // cboGoodsType
        public ICommand OpenImportGoodsWindowCommand { get; set; }
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
            OpenImportGoodsWindowCommand = new RelayCommand<MainWindow>(p => true, p => OpenImportGoodsWindow(p));
        }
        void OpenImportGoodsWindow(MainWindow main)
        {
            ImportGoodsWindow newWindow = new ImportGoodsWindow();
            TotalPrice = 0;
            MoneyToPay = 0;
            main.Hide();
            int idStockReceiptMax = StockReceiptDAL.Instance.GetMaxId();
            if(idStockReceiptMax == -1)
            {
                MessageBox.Show("Lỗi hệ thống!");
                return;
            }    
            newWindow.txbIdReceipt.Text = AddPrefix("PH", idStockReceiptMax + 1);
            newWindow.txbDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
            newWindow.stkImportGoods.Children.Clear();
            this.wdImportGoods = newWindow;
            SetItemSource();
            newWindow.ShowDialog();
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
            receiptTemplate.txbTotal.Text = wdImportGoods.txbMoneyToPay.Text;

            //Load
            for (int i = 0; i < wdImportGoods.stkImportGoods.Children.Count; i++)
            {
                ImportGoodsControl importControl = (ImportGoodsControl)wdImportGoods.stkImportGoods.Children[i];
                StockReceiptInfoControl infoReceiptControl = new StockReceiptInfoControl();
                infoReceiptControl.txbNumericalOder.Text = (i + 1).ToString();
                infoReceiptControl.txbName.Text = importControl.txbName.Text;
                infoReceiptControl.txbGoodsType.Text = importControl.txbGoodsType.Text;
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
            }
            if (k)
            {
                var result = MessageBox.Show("Thanh toán thành công! Bạn có muốn in hóa đơn?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PrintReceipt(wdImportGoods);
                }
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
            //Set item source supplier
            DataTable dataSupllier = SupplierDAL.Instance.GetAll();
            for (int i = 0; i < dataSupllier.Rows.Count; i++)
            {
                Supplier supplier = new Supplier(int.Parse(dataSupllier.Rows[i].ItemArray[0].ToString()),
                    dataSupllier.Rows[i].ItemArray[1].ToString(), dataSupllier.Rows[i].ItemArray[2].ToString(),
                    dataSupllier.Rows[i].ItemArray[3].ToString());
                itemSourceSupplier.Add(supplier);
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
