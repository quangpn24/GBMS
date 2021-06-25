using ClosedXML.Excel;
using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
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
            SetItsYearCommand = new RelayCommand<MainWindow>((p) => true, (p) => SetItemSourceYear(p));
            PreviousPageCommand = new RelayCommand<MainWindow>((p) => true, (p) => GoToPreviousPage(p, --currentPage));
            NextPageCommand = new RelayCommand<MainWindow>((p) => true, (p) => GoToNextPage(p, ++currentPage));
        }

        public void ExportExcel()
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
                    int month = int.Parse(selectedMonth.Split(' ')[1]);
                    int year = int.Parse(selectedYear.Split(' ')[1]);
                    // đặt tiêu đề cho file
                    p.Workbook.Properties.Title = string.Format("Danh sách tồn kho tháng {0}/{1}", month, year);
                    p.Workbook.Worksheets.Add("sheet");

                    ExcelWorksheet ws = p.Workbook.Worksheets[0];
                    ws.Name = "DSTK";
                    ws.Cells.Style.Font.Size = 11;
                    ws.Cells.Style.Font.Name = "Calibri";
                    ws.Cells.Style.WrapText = true;
                    ws.Column(1).Width = 10;
                    ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(2).Width = 30;
                    ws.Column(3).Width = 30;
                    ws.Column(4).Width = 20;
                    ws.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(5).Width = 20;
                    ws.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(6).Width = 20;
                    ws.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(7).Width = 20;
                    ws.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(8).Width = 20;
                    ws.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = { "STT", "Tên sản phẩm", "Loại sản phẩm", "Tồn đầu", "Mua vào", "Bán ra", "Tồn cuối", "Đơn vị tính" };

                    var countColHeader = arrColumnHeader.Count();

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge
                    ws.Row(1).Height = 15;
                    ws.Cells[1, 1].Value = string.Format("Danh sách tồn kho tháng {0}/{1}", month, year);
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
                        //set màu thành gray
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
                    currentPage = 0;
                    for (int i = 0; i < goodsList.Count; i++)
                    {
                        StockControl control = (StockControl)mainWindow.stkStock.Children[i % 10];
                        ws.Row(rowIndex).Height = 15;
                        colIndex = 1;
                        rowIndex++;
                        string address = "A" + rowIndex + ":H" + rowIndex;
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
                        ws.Cells[rowIndex, colIndex++].Value = control.txbName.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbType.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbEarlyStock.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbInStock.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbOutStock.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbEndStock.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbUnit.Text;

                        if (i % 10 == 9)
                        {
                            GoToNextPage(mainWindow, currentPage);
                        }
                    }
                    //Lưu file lại
                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }
                MessageBox.Show("Xuất excel thành công!");
            }
            catch
            {
                MessageBox.Show("Có lỗi khi lưu file!");
            }

        }
        public void Search(MainWindow window)
        {
            string nameSearching = mainWindow.txtSearchStock.Text.ToLower();
            goodsList = GoodsDAL.Instance.FindByName(nameSearching);
            Filter(mainWindow);
        }
        void SetItemSourceMonth(MainWindow window)
        {
            itemSourceMonth.Clear();

            if (selectedYear == null)
            {
                return;
            }
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
            selectedYear = window.cboYearStock.SelectedValue.ToString();
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

                control.txbEarlyStock.Text = SeparateThousands(earlyStock.ToString());
                control.txbInStock.Text = SeparateThousands(inStock.ToString());
                control.txbOutStock.Text = SeparateThousands(outStock.ToString());
                control.txbEndStock.Text = SeparateThousands(endStock.ToString());

                stackPanel.Children.Add(control);
            }
        }
        public void LoadStockList(MainWindow window, int curPage)
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
