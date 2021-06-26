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
using System.Data;
using GemstonesBusinessManagementSystem.Resources.Template;
using System.Printing;
using System.Windows.Documents;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class BillViewModel : BaseViewModel
    {
        public ICommand LoadBillCommand { get; set; }
        public ICommand PickBillCommand { get; set; }
        public ICommand PrintBillCommand { get; set; }
        public ICommand DeleteBillCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }

        private InvoiceControl checkedItem;
        private MainWindow mainWindow;

        private string customerName;
        private string customerPhoneNumber;
        private string customerAddress;
        private string idBill;
        private string employeeName;
        private string invoiceDate;
        private long quantity = 0;
        private string total = null;

        public string CustomerName { get => customerName; set { customerName = value; OnPropertyChanged(); } }
        public string CustomerPhoneNumber { get => customerPhoneNumber; set { customerPhoneNumber = value; OnPropertyChanged(); } }
        public string CustomerAddress { get => customerAddress; set { customerAddress = value; OnPropertyChanged(); } }
        public string IdBill { get => idBill; set { idBill = value; OnPropertyChanged(); } }
        public string EmployeeName { get => employeeName; set { employeeName = value; OnPropertyChanged(); } }
        public string InvoiceDate { get => invoiceDate; set { invoiceDate = value; OnPropertyChanged(); } }
        public long Quantity { get => quantity; set { quantity = value; OnPropertyChanged(); } }
        public string Total { get => SeparateThousands(total); set { total = value; OnPropertyChanged(); } }

        public BillViewModel()
        {
            LoadBillCommand = new RelayCommand<MainWindow>(p => true, p => LoadBill(p));
            PickBillCommand = new RelayCommand<InvoiceControl>(p => true, p => PickBill(p));
            PrintBillCommand = new RelayCommand<MainWindow>(p => true, p => Print(p));
            DeleteBillCommand = new RelayCommand<InvoiceControl>(p => true, p => DeleteBill(p));
            ExportExcelCommand = new RelayCommand<MainWindow>(p => true, p => ExportExcel(p));
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
                    CustomerDAL.Instance.UpdateMembership(customer.IdCustomer, mem.Value);
                    break;
                }
            }
        }
        void DeleteBill(InvoiceControl control)
        {
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu dịch vụ?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            string idBill = ConvertToIDString(control.txbId.Text);
            if (confirm == MessageBoxResult.Yes)
            {
                BillInfoDAL.Instance.Delete(idBill);
                BillDAL.Instance.Delete(idBill);
                Customer customer = CustomerDAL.Instance.FindById(control.txbIdCustomer.Text);
                customer.TotalPrice -= ConvertToNumber(control.txbPrice.Text);
                UpdateMembership(customer, 0);
                if (control == checkedItem)
                {
                    mainWindow.stkBillInfo.Children.Clear();
                    IdBill = "";
                    CustomerName = "";
                    CustomerPhoneNumber = "";
                    InvoiceDate = "";
                    EmployeeName = "";
                    Total = "";
                }
                mainWindow.stkBill.Children.Remove(control);
                MessageBox.Show("Xóa hoá đơn thành công");
            }
        }
        public void LoadBill(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            if (mainWindow.dpStartDateBill.SelectedDate == null || mainWindow.dpEndDateBill.SelectedDate == null)
            {
                return;
            }
            if (mainWindow.dpStartDateBill.SelectedDate > mainWindow.dpEndDateBill.SelectedDate)
            {
                CustomMessageBox.Show("Vui lòng chọn ngày bắt đầu nhỏ hơn ngày kết thúc!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            mainWindow.stkBill.Children.Clear();
            List<Bill> billList = BillDAL.Instance.GetByDate((DateTime)mainWindow.dpStartDateBill.SelectedDate,
                (DateTime)mainWindow.dpEndDateBill.SelectedDate);
            int numOfBill = billList.Count;
            for (int i = 0; i < numOfBill; i++)
            {
                Customer customer = CustomerDAL.Instance.FindById(billList[i].IdCustomer.ToString());
                Employee employee = EmployeeDAL.Instance.GetByIdAccount(billList[i].IdAccount.ToString());
                InvoiceControl invoiceControl = new InvoiceControl();
                invoiceControl.txbId.Text = AddPrefix("HD", billList[i].IdBill);
                invoiceControl.txbIdCustomer.Text = customer.IdCustomer.ToString();
                invoiceControl.txbCustomerName.Text = customer.CustomerName;
                invoiceControl.txbEmployeeName.Text = employee.Name;
                invoiceControl.txbPrice.Text = SeparateThousands(billList[i].TotalMoney.ToString());

                mainWindow.stkBill.Children.Add(invoiceControl);
            }
        }
        void PickBill(InvoiceControl invoiceControl)
        {
            mainWindow.btnPrintBill.IsEnabled = true;
            Bill bill = BillDAL.Instance.GetBill(ConvertToIDString(invoiceControl.txbId.Text));
            Customer customer = CustomerDAL.Instance.FindById(bill.IdCustomer.ToString());
            List<BillInfo> billInfos = BillInfoDAL.Instance.GetBillInfos(bill.IdBill.ToString());
            if (checkedItem != null) // dua lai mau xam
            {
                checkedItem.txbId.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbCustomerName.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbEmployeeName.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbPrice.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
            }
            // chuyen sang mau duoc chon
            invoiceControl.txbId.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF00329E");
            invoiceControl.txbCustomerName.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF00329E");
            invoiceControl.txbEmployeeName.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF00329E");
            invoiceControl.txbPrice.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF00329E");
            checkedItem = invoiceControl;
            // hien thi thong tin
            IdBill = invoiceControl.txbId.Text;
            InvoiceDate = bill.InvoiceDate.ToShortDateString();
            EmployeeName = EmployeeDAL.Instance.GetByIdAccount(bill.IdAccount.ToString()).Name;
            CustomerName = customer.CustomerName;
            CustomerPhoneNumber = customer.PhoneNumber;
            CustomerAddress = customer.Address;
            Total = bill.TotalMoney.ToString();
            mainWindow.stkBillInfo.Children.Clear();
            for (int i = 0; i < billInfos.Count; i++)
            {
                Goods goods = GoodsDAL.Instance.GetById(billInfos[i].IdGoods.ToString());
                GoodsType type = GoodsTypeDAL.Instance.GetById(goods.IdGoodsType);

                InvoiceInfoControl control = new InvoiceInfoControl();
                control.txbNumber.Text = (i + 1).ToString();
                control.txbName.Text = goods.Name;
                control.txbUnit.Text = type.Unit;
                control.txbUnitPrice.Text = SeparateThousands(billInfos[i].Price.ToString());
                control.txbQuantity.Text = billInfos[i].Quantity.ToString();
                control.txbTotal.Text = SeparateThousands((ConvertToNumber(control.txbUnitPrice.Text) * billInfos[i].Quantity).ToString());

                mainWindow.stkBillInfo.Children.Add(control);
            }
        }
        void Print(MainWindow mainWindow)
        {
            BillTemplate billTemplate = new BillTemplate();

            billTemplate.txbIdBill.Text = IdBill;
            billTemplate.txbInvoiceDate.Text = InvoiceDate;
            billTemplate.txbCustomerName.Text = CustomerName;
            billTemplate.txbCustomerPhoneNumber.Text = CustomerPhoneNumber;
            billTemplate.txbCustomerAddress.Text = CustomerAddress;
            billTemplate.txbTotal.Text = Total.ToString();
            billTemplate.txbEmployeeName.Text = EmployeeName;

            List<Parameter> parameters = ParameterDAL.Instance.GetData();
            billTemplate.txbStoreName.Text = parameters[1].Value;
            billTemplate.txbStoreAddress.Text = parameters[2].Value;
            int numOfItems = mainWindow.stkBillInfo.Children.Count;

            //print
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() != true) return;
            FixedDocument document = new FixedDocument();
            PageContent temp;
            for (int i = 0; i < numOfItems; i++)
            {
                InvoiceInfoControl control = (InvoiceInfoControl)mainWindow.stkBillInfo.Children[i];
                BillInfoControl billInfoControl = new BillInfoControl();
                billInfoControl.txbOrderNum.Text = control.txbNumber.Text;
                billInfoControl.txbName.Text = control.txbName.Text;
                billInfoControl.txbQuantity.Text = control.txbQuantity.Text;
                billInfoControl.txbUnit.Text = control.txbUnit.Text;
                billInfoControl.txbUnitPrice.Text = control.txbUnitPrice.Text;
                billInfoControl.txbTotal.Text = control.txbTotal.Text;

                billTemplate.stkBillInfo.Children.Add(billInfoControl);

                document.DocumentPaginator.PageSize = new Size(billTemplate.grdPrint.ActualWidth, billTemplate.grdPrint.ActualHeight);
                if (billTemplate.stkBillInfo.Children.Count == 10 || i == numOfItems - 1)
                {
                    billTemplate.grdPrint.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    billTemplate.grdPrint.Arrange(new Rect(0, 0, billTemplate.grdPrint.DesiredSize.Width, billTemplate.grdPrint.DesiredSize.Height));
                    temp = ConvertToPage(billTemplate.grdPrint);
                    document.Pages.Add(temp);
                    billTemplate.stkBillInfo.Children.Clear();
                }
            }

            pd.PrintDocument(document.DocumentPaginator, IdBill);
        }
        public PageContent ConvertToPage(Grid grid)
        {
            FixedPage page = new FixedPage();
            page.Width = grid.ActualWidth;
            page.Height = grid.ActualHeight;
            string gridXaml = XamlWriter.Save(grid);
            gridXaml = gridXaml.Replace("Name=\"txbOrderNum\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbUnitPrice\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbName\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbQuantity\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbUnit\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbTotal\"", "");
            StringReader stringReader = new StringReader(gridXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Grid newGrid = (Grid)XamlReader.Load(xmlReader);

            page.Children.Add(newGrid);
            PageContent pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(page);
            return pageContent;
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
                    p.Workbook.Properties.Title = "Danh sách phiếu mua hàng";
                    p.Workbook.Worksheets.Add("sheet");

                    ExcelWorksheet ws = p.Workbook.Worksheets[0];
                    ws.Name = "DSPMH";
                    ws.Cells.Style.Font.Size = 11;
                    ws.Cells.Style.Font.Name = "Calibri";
                    ws.Cells.Style.WrapText = true;
                    ws.Column(1).Width = 10;
                    ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(2).Width = 20;
                    ws.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(3).Width = 30;
                    ws.Column(4).Width = 30;
                    ws.Column(5).Width = 20;
                    ws.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(6).Width = 30;
                    ws.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = { "STT", "Mã hóa đơn", "Khách hàng", "Người lập", "Ngày lập", "Tổng tiền" };

                    var countColHeader = arrColumnHeader.Count();

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge
                    ws.Row(1).Height = 15;
                    ws.Cells[1, 1].Value = "Danh sách phiếu mua hàng";
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
                    for (int i = 0; i < mainWindow.stkBill.Children.Count; i++)
                    {
                        InvoiceControl control = (InvoiceControl)mainWindow.stkBill.Children[i];
                        ws.Row(rowIndex).Height = 15;
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
                        ws.Cells[rowIndex, colIndex++].Value = control.txbId.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbCustomerName.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbEmployeeName.Text;
                        ws.Cells[rowIndex, colIndex++].Value = BillDAL.Instance.GetBill(ConvertToIDString(control.txbId.Text)).InvoiceDate.ToString("dd/MM/yyyy");
                        ws.Cells[rowIndex, colIndex++].Value = control.txbPrice.Text;
                    }
                    //Lưu file lại
                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }
                CustomMessageBox.Show("Xuất danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch
            {
                CustomMessageBox.Show("Có lỗi khi lưu file!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}