using ClosedXML.Excel;
using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class StockViewModel : BaseViewModel
    {
        public ICommand LoadStockCommand { get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand SetItsMonthCommand { get; set; }
        public ICommand SetItsYearCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public ICommand NextPageCommand { get; set; }

        private MainWindow mainWindow;
        private List<Goods> goodsList = GoodsDAL.Instance.GetList();
        private int currentPage = 0;

        //Combobox
        private ObservableCollection<string> itemSourceMonth = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceMonth { get => itemSourceMonth; set { itemSourceMonth = value; OnPropertyChanged(); } }
        private ObservableCollection<string> itemSourceYear = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceYear { get => itemSourceYear; set { itemSourceYear = value; OnPropertyChanged(); } }

        private string selectedMonth;
        public string SelectedMonth { get => selectedMonth; set { selectedMonth = value; OnPropertyChanged(); } }
        private string selectedYear;
        public string SelectedYear { get => selectedYear; set { selectedYear = value; OnPropertyChanged(); } }

        public StockViewModel()
        {
            ExportExcelCommand = new RelayCommand<MainWindow>((p) => true, (p) => ExportExcel());
            SearchCommand = new RelayCommand<MainWindow>((p) => true, (p) => Search(p));
            LoadStockCommand = new RelayCommand<MainWindow>((p) => true, (p) => LoadStockList(p, 0));
            FilterCommand = new RelayCommand<MainWindow>((p) => true, (p) => Filter(p));
            SetItsMonthCommand = new RelayCommand<MainWindow>((p) => true, (p) => SetItemSourceMonth(p));
            SetItsYearCommand = new RelayCommand<MainWindow>((p) => true, (p) => { SetItemSourceYear(p); SetDefaultComboBox(p); });
            PreviousPageCommand = new RelayCommand<MainWindow>((p) => true, (p) => GoToPreviousPage(p, --currentPage));
            NextPageCommand = new RelayCommand<MainWindow>((p) => true, (p) => GoToNextPage(p, ++currentPage));
        }

        void ExportExcel()
        {
            if (selectedYear == null || selectedMonth == null)
            {
                MessageBox.Show("Vui lòng chọn thời gian!");
                return;
            }
            DataTable table = new DataTable();
            table.Columns.Add("Mã SP", typeof(string));
            table.Columns.Add("Tên sản phẩm", typeof(string));
            table.Columns.Add("Loại sản phẩm", typeof(string));
            table.Columns.Add("Tồn đầu", typeof(int));
            table.Columns.Add("Mua vào", typeof(int));
            table.Columns.Add("Bán ra", typeof(int));
            table.Columns.Add("Tồn cuối", typeof(int));
            table.Columns.Add("Đơn vị tính", typeof(string));

            List<Goods> listGoods = GoodsDAL.Instance.GetList();
            int numOfItems = listGoods.Count;
            StackPanel stackPanel = new StackPanel();
            LoadStackPanel(0, numOfItems, listGoods, ref stackPanel);

            for (int i = 0; i < numOfItems; i++)
            {
                StockControl control = (StockControl)stackPanel.Children[i];
                table.Rows.Add(control.txbId.Text, control.txbName.Text, control.txbType.Text,
                    control.txbEarlyStock.Text, control.txbInStock.Text, control.txbOutStock.Text,
                    control.txbEndStock.Text, control.txbUnit.Text);
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel |*.xlsx"
            };
            if ((bool)saveFileDialog.ShowDialog())
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(table, "Báo cáo tồn kho");
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                MessageBox.Show("Xuất danh sách thành công!");
            }
        }
        void Search(MainWindow window)
        {
            string nameSearching = mainWindow.txtSearchStock.Text.ToLower();
            goodsList = GoodsDAL.Instance.FindByName(nameSearching);
            Filter(mainWindow);
        }
        void SetItemSourceMonth(MainWindow window)
        {
            itemSourceMonth.Clear();

            int currentYear = DateTime.Now.Year;
            int lastMonth = DateTime.Now.Month;
            if (int.Parse(selectedYear.Split(' ')[1]) != currentYear)
            {
                lastMonth = 12;
            }
            for (int i = 1; i <= lastMonth; i++)
            {
                itemSourceMonth.Add(String.Format("Tháng {0}", i));
            }

            window.cboMonthStock.SelectedIndex = DateTime.Now.Month - 1;
        }
        void SetItemSourceYear(MainWindow window)
        {
            itemSourceYear.Clear();

            int currentYear = DateTime.Now.Year;
            itemSourceYear.Add(String.Format("Năm {0}", currentYear - 2));
            itemSourceYear.Add(String.Format("Năm {0}", currentYear - 1));
            itemSourceYear.Add(String.Format("Năm {0}", currentYear));

            window.cboYearStock.SelectedIndex = 2;
        }
        void SetDefaultComboBox(MainWindow window)
        {

        }
        void LoadStackPanel(int start, int end, List<Goods> list, ref StackPanel stackPanel)
        {
            string month = selectedMonth.Split(' ')[1];
            string year = selectedYear.Split(' ')[1];
            SortedList<int, int> pastImportList = StockReceiptDAL.Instance.GetImportDataAgo(month, year);
            SortedList<int, int> importList = StockReceiptDAL.Instance.GetImportDataByMonth(month, year);
            SortedList<int, int> pastSoldList = BillDAL.Instance.GetSoldDataAgo(month, year);
            SortedList<int, int> soldList = BillDAL.Instance.GetSoldDataByMonth(month, year);

            for (int i = start; i < end; i++)
            {
                StockControl control = new StockControl();
                GoodsType type = GoodsTypeDAL.Instance.GetById(list[i].IdGoodsType);
                control.txbId.Text = AddPrefix("SP", list[i].IdGoods);
                control.txbName.Text = list[i].Name;
                control.txbType.Text = type.Name;
                control.txbUnit.Text = type.Unit;

                int earlyImportIndex = pastImportList.IndexOfKey(list[i].IdGoods);
                int importIndex = importList.IndexOfKey(list[i].IdGoods);
                int earlySellIndex = pastSoldList.IndexOfKey(list[i].IdGoods);
                int sellIndex = soldList.IndexOfKey(list[i].IdGoods);

                int earlyStock = 0, inStock = 0, outStock = 0;
                if (earlyImportIndex != -1)
                {
                    earlyStock = pastImportList.Values[earlyImportIndex];
                }
                if (earlySellIndex != -1)
                {
                    earlyStock = pastSoldList.Values[earlySellIndex];
                }
                if (earlyImportIndex != -1 && earlySellIndex != -1)
                {
                    earlyStock = pastImportList.Values[earlyImportIndex] - pastSoldList.Values[earlySellIndex];
                }
                if (importIndex != -1)
                {
                    inStock = importList.Values[importIndex];
                }
                if (sellIndex != -1)
                {
                    outStock = soldList.Values[sellIndex];
                }
                int endStock = earlyStock + inStock - outStock;

                control.txbEarlyStock.Text = earlyStock.ToString();
                control.txbInStock.Text = inStock.ToString();
                control.txbOutStock.Text = outStock.ToString();
                control.txbEndStock.Text = endStock.ToString();

                stackPanel.Children.Add(control);
            }
        }
        void LoadStockList(MainWindow window, int curPage)
        {
            mainWindow = window;
            mainWindow.stkStock.Children.Clear();

            if (selectedYear == null || selectedMonth == null)
                return;

            int start = 0, end = 0;
            this.currentPage = curPage;
            LoadInfoOfPage(ref start, ref end);

            LoadStackPanel(start, end, goodsList, ref mainWindow.stkStock);

            mainWindow.txtNumOfStock.Text = string.Format("{0} trong {1} mặt hàng", mainWindow.stkStock.Children.Count, goodsList.Count);
        }
        void LoadInfoOfPage(ref int start, ref int end)
        {
            mainWindow.btnPrePageStock.IsEnabled = currentPage != 0;
            mainWindow.btnNextPageStock.IsEnabled = currentPage != (goodsList.Count - 1) / 10;

            start = currentPage * 10;
            end = (currentPage + 1) * 10;
            if (currentPage == goodsList.Count / 10)
                end = goodsList.Count;
        }
        void Filter(MainWindow mainWindow)
        {
            if (mainWindow.cboMonthStock.SelectedIndex == -1 || mainWindow.cboYearStock.SelectedIndex == -1)
                return;
            LoadStockList(mainWindow, 0);
        }
        void GoToNextPage(MainWindow mainWindow, int currentPage)
        {
            LoadStockList(mainWindow, currentPage);
        }
        void GoToPreviousPage(MainWindow mainWindow, int currentPage)
        {
            LoadStockList(mainWindow, currentPage);
        }
    }
}
