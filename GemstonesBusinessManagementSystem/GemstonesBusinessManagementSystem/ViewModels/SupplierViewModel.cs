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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Windows.Controls;

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
            OpenAddSupplierWindowCommand = new RelayCommand<Button>(p => true, p => OpenAddSupplierWindow(p));
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
                        ListSupplierToView = ListSupplierToView.OrderBy(x => ConvertToNumber(x.txbTotal.Text)).ToList();
                        break;
                    case 1:
                        ListSupplierToView = ListSupplierToView.OrderByDescending(x => ConvertToNumber(x.txbTotal.Text)).ToList();
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
            newWindow.Title = "Sửa thông tin nhà cung cấp";
            newWindow.btnSave.Content = "Cập nhật";
            newWindow.btnSave.ToolTip = "Cập nhật nhà cung cấp";
            newWindow.ShowDialog();
        }
        void AddOrUpdate(AddSupplierWindow wdAddSupplier)
        {
            if (string.IsNullOrEmpty(wdAddSupplier.txtName.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập tên nhà cung cấp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                wdAddSupplier.txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(wdAddSupplier.txtAddress.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập địa chỉ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                wdAddSupplier.txtAddress.Focus();
                return;
            }
            if (string.IsNullOrEmpty(wdAddSupplier.txtPhoneNumber.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập số điện thoại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                wdAddSupplier.txtPhoneNumber.Focus();
                return;
            }
            try
            {
                long temp = long.Parse(wdAddSupplier.txtPhoneNumber.Text);
            }
            catch
            {
                CustomMessageBox.Show("Số điện thoại không bao gồm chữ cái!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                wdAddSupplier.txtPhoneNumber.Focus();
                return;
            }
            Supplier newSupplier = new Supplier(ConvertToID(wdAddSupplier.txtId.Text), wdAddSupplier.txtName.Text, wdAddSupplier.txtAddress.Text, wdAddSupplier.txtPhoneNumber.Text);
            if ((!isUpdate || wdAddSupplier.txtName.Text != oldSupplier) && SupplierDAL.Instance.IsExisted(wdAddSupplier.txtName.Text))
            {
                CustomMessageBox.Show("Tên nhà cung cấp đã tồn tại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    CustomMessageBox.Show("Cập nhật nhà cung cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    CustomMessageBox.Show("Thêm nhà cung cấp thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
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
            else
            {
                CustomMessageBox.Show("Thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void OpenAddSupplierWindow(Button btn)
        {
            isUpdate = false;
            AddSupplierWindow newWindow = new AddSupplierWindow();
            int idMax = SupplierDAL.Instance.GetMaxId();
            if (idMax == -1)
            {
                return;
            }
            newWindow.txtId.Text = AddPrefix("NC", idMax + 1);
            newWindow.txtAddress.Text = null;
            newWindow.txtName.Text = null;
            newWindow.txtPhoneNumber.Text = null;
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
                control.txbNumOfReceipts.Text = SeparateThousands(StockReceiptDAL.Instance.NumOfReceiptsBySupplier(dt.Rows[i].ItemArray[0].ToString()).ToString());
                control.txbTotal.Text = SeparateThousands(StockReceiptDAL.Instance.SumMoneyBySupplier(dt.Rows[i].ItemArray[0].ToString()).ToString());
                total += ConvertToNumber(control.txbTotal.Text);
                ListSupplierToView.Add(control);
            }
            main.txbSupplierQuantity.Text = SeparateThousands(ListSupplierToView.Count.ToString());
            main.txbTotalSpentToSupplier.Text = SeparateThousands(total.ToString());
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
                    p.Workbook.Properties.Title = "Danh sách nhà cung cấp";
                    p.Workbook.Worksheets.Add("sheet");

                    ExcelWorksheet ws = p.Workbook.Worksheets[0];
                    ws.Name = "DSNCC";
                    ws.Cells.Style.Font.Size = 11;
                    ws.Cells.Style.Font.Name = "Calibri";
                    ws.Cells.Style.WrapText = true;
                    ws.Column(1).Width = 10;
                    ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(2).Width = 30;
                    ws.Column(3).Width = 30;
                    ws.Column(4).Width = 30;
                    ws.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(5).Width = 20;
                    ws.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(6).Width = 30;
                    ws.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = { "STT", "Tên NCC", "Địa chỉ", "Số điện thoại", "Số đơn hàng", "Tổng tiền" };

                    var countColHeader = arrColumnHeader.Count();

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge
                    ws.Row(1).Height = 15;
                    ws.Cells[1, 1].Value = "Danh sách thông tin nhà cung cấp";
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
                    for (int i = 0; i < ListSupplierToView.Count; i++)
                    {
                        ws.Row(rowIndex).Height = 15;
                        SupplierControl control = ListSupplierToView[i];
                        colIndex = 1;
                        rowIndex++;
                        string address = "A" + rowIndex + ":F" + rowIndex;
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
                        ws.Cells[rowIndex, colIndex++].Value = control.txbAddress.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbPhoneNumber.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbNumOfReceipts.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbTotal.Text;
                    }
                    //Lưu file lại
                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }
                CustomMessageBox.Show("Xuất danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch
            {
            }

        }
    }
}
