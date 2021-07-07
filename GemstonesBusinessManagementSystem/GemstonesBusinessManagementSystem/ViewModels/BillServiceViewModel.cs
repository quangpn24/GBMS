using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.Template;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class BillServiceViewModel : BaseViewModel
    {
        private MainWindow main;
        public BillServiceControl checkedItem;
        public ICommand LoadBillServicesCommand { get; set; }
        public ICommand PickBillServiceCommand { get; set; }
        public ICommand ConfirmDeliveriedCommand { get; set; }
        public ICommand PrintBillServiceCommand { get; set; }
        public ICommand DeleteBillServiceCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public BillServiceViewModel()
        {
            LoadBillServicesCommand = new RelayCommand<MainWindow>((p) => true, (p) => LoadBillServices(p));
            PickBillServiceCommand = new RelayCommand<BillServiceControl>((p) => true, (p) => PickBillService(p));
            ConfirmDeliveriedCommand = new RelayCommand<BillServiceTemplateControl>((p) => true, (p) => ConfirmDeliveried(p));
            PrintBillServiceCommand = new RelayCommand<MainWindow>((p) => true, (p) => PrintBillService(p));
            DeleteBillServiceCommand = new RelayCommand<BillServiceControl>((p) => true, (p) => DeleteBillService(p));
            ExportExcelCommand = new RelayCommand<MainWindow>((p) => true, (p) => ExportExcel(p));
        }
        public void LoadBillServices(MainWindow mainWindow)
        {
            this.main = mainWindow;
            if (mainWindow.dpStartDateBS.SelectedDate == null || mainWindow.dpEndDateBS.SelectedDate == null)
            {
                return;
            }
            if (mainWindow.dpStartDateBS.SelectedDate > mainWindow.dpEndDateBS.SelectedDate)
            {
                CustomMessageBox.Show("Vui lòng chọn ngày bắt đầu nhỏ hơn ngày kết thúc!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            main.stkBillService.Children.Clear();
            List<BillService> billServices = BillServiceDAL.Instance.GetByDate((DateTime)mainWindow.dpStartDateBS.SelectedDate, (DateTime)mainWindow.dpEndDateBS.SelectedDate);
            for (int i = 0; i < billServices.Count; i++)
            {
                Customer customer = CustomerDAL.Instance.FindById(billServices[i].IdCustomer.ToString());
                BillServiceControl billServiceControl = new BillServiceControl();
                billServiceControl.txbId.Text = AddPrefix("PD", billServices[i].IdBillService);
                billServiceControl.txbIdCustomer.Text = customer.IdCustomer.ToString();
                billServiceControl.txbNameCustomer.Text = customer.CustomerName;
                billServiceControl.txbTotal.Text = SeparateThousands(billServices[i].Total.ToString());
                billServiceControl.txbRest.Text = SeparateThousands(BillServiceInfoDAL.Instance.CalculateRestMoney(billServices[i].IdBillService.ToString()));
                if (billServices[i].Status == 1)
                {
                    billServiceControl.txbStatus.Text = "Đã giao";
                    billServiceControl.txbStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF01B500");
                }
                else
                {
                    billServiceControl.txbStatus.Text = "Chưa giao";
                }
                if (BillServiceInfoDAL.Instance.IsHaveDeliveried(billServices[i].IdBillService.ToString()))
                {
                    billServiceControl.btnDeleteBillService.Visibility = Visibility.Hidden;
                }
                billServiceControl.IsHitTestVisible = true;
                if (checkedItem != null)
                {
                    if (checkedItem.txbId.Text == billServiceControl.txbId.Text)
                    {
                        billServiceControl.txbId.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
                        billServiceControl.txbNameCustomer.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
                        billServiceControl.txbTotal.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
                        billServiceControl.txbRest.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
                        if (billServiceControl.txbStatus.Text == "Chưa giao")
                        {
                            billServiceControl.txbStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
                        }
                        checkedItem = billServiceControl;
                    }
                }
                mainWindow.stkBillService.Children.Add(billServiceControl);
            }
        }
        public void PickBillService(BillServiceControl billServiceControl)
        {
            BillService billService = BillServiceDAL.Instance.GetBillService(ConvertToIDString(billServiceControl.txbId.Text));
            Customer customer = CustomerDAL.Instance.FindById(billService.IdCustomer.ToString());
            List<BillServiceInfo> billServiceInfos = BillServiceInfoDAL.Instance.GetBillServiceInfos(billService.IdBillService.ToString());
            if (checkedItem != null) // Đưa lại màu xám
            {
                checkedItem.txbId.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbNameCustomer.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbTotal.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbRest.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                if (checkedItem.txbStatus.Text == "Đã giao")
                {
                    checkedItem.txbStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF01B500");
                }
                else
                {
                    checkedItem.txbStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                }

            }
            //Chuyển sang màu đang được chọn
            checkedItem = billServiceControl;

            checkedItem.txbId.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            checkedItem.txbNameCustomer.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            checkedItem.txbTotal.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            checkedItem.txbRest.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            if (checkedItem.txbStatus.Text == "Chưa giao")
            {
                checkedItem.txbStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            }
            //Hiển thị thông tin
            main.txbIdBillServiceBS.Text = billServiceControl.txbId.Text;
            main.txbCreateDateBS.Text = billService.CreatedDate.ToShortDateString();
            main.txbNameCustomerBS.Text = customer.CustomerName;
            main.txbPhoneCustomerBS.Text = customer.PhoneNumber;
            main.txbTotalBS.Text = SeparateThousands(billService.Total.ToString());
            main.txbTotalPaidBS.Text = SeparateThousands(billService.TotalPaidMoney.ToString());
            main.txbRestBS.Text = billServiceControl.txbRest.Text;
            main.stkBillServiceInfo.Children.Clear();
            for (int i = 0; i < billServiceInfos.Count; i++)   //Hiển thị list BillServiceInfo
            {
                Service service = ServiceDAL.Instance.FindById(billServiceInfos[i].IdService.ToString());
                BillServiceTemplateControl billServiceTemplateControl = new BillServiceTemplateControl();
                billServiceTemplateControl.txbNumber.Text = (i + 1).ToString();
                billServiceTemplateControl.txbIdService.Text = billServiceInfos[i].IdService.ToString();
                billServiceTemplateControl.txbName.Text = service.Name;
                billServiceTemplateControl.txbPrice.Text = SeparateThousands(billServiceInfos[i].Price.ToString());
                billServiceTemplateControl.txbCalculateMoney.Text = SeparateThousands((billServiceInfos[i].Price + billServiceInfos[i].Tips).ToString());
                billServiceTemplateControl.txbPaidMoney.Text = SeparateThousands(billServiceInfos[i].PaidMoney.ToString());
                billServiceTemplateControl.txbQuantity.Text = billServiceInfos[i].Quantity.ToString();
                billServiceTemplateControl.txbTotal.Text = SeparateThousands((ConvertToNumber(billServiceTemplateControl.txbCalculateMoney.Text) * billServiceInfos[i].Quantity).ToString());
                if (billServiceInfos[i].Status == 1)  // Đã giao thì bỏ button swap và chuyển màu sang success
                {
                    billServiceTemplateControl.txbDeliveryDate.Text = billServiceInfos[i].DeliveryDate.ToShortDateString();
                    billServiceTemplateControl.btnSwapStatus.Visibility = Visibility.Hidden;
                    billServiceTemplateControl.txbStatus.Text = "Đã giao";
                    billServiceTemplateControl.txbStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF01B500");
                }
                else
                {
                    billServiceTemplateControl.txbRest.Text = SeparateThousands((ConvertToNumber(billServiceTemplateControl.txbTotal.Text) - billServiceInfos[i].PaidMoney).ToString());
                    billServiceTemplateControl.txbStatus.Text = "Chưa giao";
                }
                main.stkBillServiceInfo.Children.Add(billServiceTemplateControl);
            }
            main.btnPrintBS.IsEnabled = true;
        }
        public void ConfirmDeliveried(BillServiceTemplateControl billServiceTemplateControl)
        {
            BillServiceInfo billServiceInfo = BillServiceInfoDAL.Instance.GetBillServiceInfo(ConvertToIDString(checkedItem.txbId.Text), billServiceTemplateControl.txbIdService.Text);
            billServiceInfo.DeliveryDate = DateTime.Now;
            billServiceInfo.Status = 1;
            //billServiceInfo.PaidMoney = (billServiceInfo.Price + billServiceInfo.Tips) * billServiceInfo.Quantity;
            if (BillServiceInfoDAL.Instance.Update(billServiceInfo))
            {
                CustomMessageBox.Show("Cập nhật giao hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                Customer customer = CustomerDAL.Instance.FindById(checkedItem.txbIdCustomer.Text);
                UpdateMembership(customer, ConvertToNumber(billServiceTemplateControl.txbRest.Text));
                checkedItem.btnDeleteBillService.Visibility = Visibility.Hidden;
                checkedItem.txbRest.Text = main.txbRestBS.Text = (double.Parse(main.txbRestBS.Text) - double.Parse(billServiceTemplateControl.txbRest.Text)).ToString();
                billServiceTemplateControl.txbRest.Text = "0";
                billServiceTemplateControl.txbStatus.Text = "Đã giao";
                billServiceTemplateControl.txbDeliveryDate.Text = DateTime.Now.ToShortDateString();
                billServiceTemplateControl.btnSwapStatus.IsHitTestVisible = false;
                billServiceTemplateControl.btnSwapStatus.Visibility = Visibility.Hidden;
                billServiceTemplateControl.txbStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF01B500");
                if (BillServiceInfoDAL.Instance.IsFullDeliveried(ConvertToIDString(checkedItem.txbId.Text)))
                {
                    BillService billService = BillServiceDAL.Instance.GetBillService(ConvertToIDString(checkedItem.txbId.Text));
                    billService.Status = 1;
                    if (BillServiceDAL.Instance.Update(billService))
                    {

                        checkedItem.txbStatus.Text = "Đã giao";
                        checkedItem.txbStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF01B500");
                    }
                }
            }
            else
            {
                CustomMessageBox.Show("Cập nhật thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //billServiceTemplateControl.icStatus.Kind = MaterialDesignThemes.Wpf.PackIconKind.TickCircleOutline;
            //billServiceTemplateControl.btnSwapStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF01B500");
        }
        void UpdateMembership(Customer customer, long paidMoney)
        {
            var totalSpending = customer.TotalSpending + paidMoney;
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
        public void DeleteBillService(BillServiceControl billServiceControl)
        {
            var result = CustomMessageBox.Show("Xác nhận xóa phiếu dịch vụ?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if (BillServiceInfoDAL.Instance.DeleteByIdBillService(ConvertToIDString(billServiceControl.txbId.Text)))
                {
                    if (BillServiceDAL.Instance.Delete(ConvertToIDString(billServiceControl.txbId.Text)))
                    {
                        Customer customer = CustomerDAL.Instance.FindById(billServiceControl.txbIdCustomer.Text);
                        customer.TotalSpending -= ConvertToNumber(billServiceControl.txbTotal.Text);
                        customer.TotalSpending += ConvertToNumber(billServiceControl.txbRest.Text);
                        //CustomerDAL.Instance.AddOrUpdate(customer, true);
                        UpdateMembership(customer, 0);
                        if (billServiceControl == checkedItem)
                        {
                            main.stkBillServiceInfo.Children.Clear();
                            main.txbNameCustomerBS.Text = "";
                            main.txbIdBillServiceBS.Text = "";
                            main.txbCreateDateBS.Text = "";
                            main.txbPhoneCustomerBS.Text = "";
                            main.txbTotalPaidBS.Text = "";
                            main.txbTotalBS.Text = "";
                            main.txbRestBS.Text = "";

                        }
                        main.stkBillService.Children.Remove(billServiceControl);
                        CustomMessageBox.Show("Xóa phiếu dịch vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }

                }
            }
        }

        public void PrintBillService(MainWindow main)
        {
            BillServiceTemplate billServiceTemplate = new BillServiceTemplate();
            billServiceTemplate.txbName.Text = main.txbNameCustomerBS.Text;
            billServiceTemplate.txbAddress.Text = CustomerDAL.Instance.FindByIdBillService(ConvertToIDString(main.txbIdBillServiceBS.Text)).Address;
            billServiceTemplate.txbPhoneNumber.Text = main.txbPhoneCustomerBS.Text;
            billServiceTemplate.txbEmployeeName.Text = CurrentAccount.Name;
            billServiceTemplate.txbId.Text = main.txbIdBillServiceBS.Text;
            billServiceTemplate.txbDate.Text = DateTime.Now.ToShortDateString();
            billServiceTemplate.txbTotal.Text = main.txbTotalBS.Text;
            billServiceTemplate.txbTotalPaid.Text = main.txbTotalPaidBS.Text;
            billServiceTemplate.txbRest.Text = main.txbRestBS.Text;

            List<Parameter> parameters = ParameterDAL.Instance.GetData();
            billServiceTemplate.txbStoreName.Text = parameters[1].Value;
            billServiceTemplate.txbStoreAddress.Text = parameters[2].Value;
            //print 
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() != true) return;
            FixedDocument document = new FixedDocument();
            PageContent temp;
            for (int i = 0; i < main.stkBillServiceInfo.Children.Count; i++) // Duyệt các chi tiết hóa đơn dịch vụ đã chọn
            {
                BillServiceTemplateControl control = main.stkBillServiceInfo.Children[i] as BillServiceTemplateControl;
                BillServiceTemplateControl billServiceTemplateControl = new BillServiceTemplateControl();
                billServiceTemplateControl.txbNumber.Text = (i + 1).ToString();
                billServiceTemplateControl.txbName.Text = control.txbName.Text;
                billServiceTemplateControl.txbPrice.Text = control.txbPrice.Text;
                billServiceTemplateControl.txbCalculateMoney.Text = control.txbCalculateMoney.Text;
                billServiceTemplateControl.txbQuantity.Text = control.txbQuantity.Text;
                billServiceTemplateControl.txbTotal.Text = control.txbTotal.Text;
                billServiceTemplateControl.txbPaidMoney.Text = control.txbPaidMoney.Text;
                billServiceTemplateControl.txbRest.Text = control.txbRest.Text;
                billServiceTemplateControl.txbDeliveryDate.Text = control.txbDeliveryDate.Text;
                billServiceTemplateControl.txbStatus.Text = control.txbStatus.Text;
                billServiceTemplateControl.btnSwapStatus.Visibility = Visibility.Hidden;
                billServiceTemplateControl.grdMain.ColumnDefinitions.RemoveAt(10);
                billServiceTemplateControl.grdMain.Children.Remove(billServiceTemplateControl.grdMain.Children[11]);
                billServiceTemplate.stkBillServiceInfo.Children.Add(billServiceTemplateControl);

                document.DocumentPaginator.PageSize = new Size(billServiceTemplate.grdPrint.ActualWidth, billServiceTemplate.grdPrint.ActualHeight);
                if (billServiceTemplate.stkBillServiceInfo.Children.Count == 10 || i == main.stkBillServiceInfo.Children.Count - 1)
                {
                    billServiceTemplate.grdPrint.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    billServiceTemplate.grdPrint.Arrange(new Rect(0, 0, billServiceTemplate.grdPrint.DesiredSize.Width, billServiceTemplate.grdPrint.DesiredSize.Height));
                    temp = ConvertToPage(billServiceTemplate.grdPrint);
                    document.Pages.Add(temp);
                    billServiceTemplate.stkBillServiceInfo.Children.Clear();
                }
            }
            pd.PrintDocument(document.DocumentPaginator, main.txbIdBillServiceBS.Text);

        }
        public PageContent ConvertToPage(Grid grid)
        {
            FixedPage page = new FixedPage();
            page.Width = grid.ActualWidth;
            page.Height = grid.ActualHeight;
            string gridXaml = XamlWriter.Save(grid);
            gridXaml = gridXaml.Replace("Name=\"ucBillServiceInfo\"", "");
            gridXaml = gridXaml.Replace("Name=\"grdMain\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbIdService\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbNumber\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbName\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbPrice\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbCalculateMoney\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbQuantity\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbTotal\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbPaidMoney\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbDeliveryDate\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbRest\"", "");
            gridXaml = gridXaml.Replace("Name=\"txbStatus\"", "");
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
                    p.Workbook.Properties.Title = "Danh sách phiếu dịch vụ";
                    p.Workbook.Worksheets.Add("sheet");

                    ExcelWorksheet ws = p.Workbook.Worksheets[0];
                    ws.Name = "DSPDV";
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
                    ws.Column(7).Width = 20;
                    ws.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(8).Width = 20;
                    ws.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Column(9).Width = 20;
                    ws.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = { "STT", "Mã phiếu", "Khách hàng", "Người lập", "Ngày lập", "Tổng tiền", "Trả trước", "Còn lại", "Trạng thái" };

                    var countColHeader = arrColumnHeader.Count();

                    // merge các column lại từ column 1 đến số column header
                    // gán giá trị cho cell vừa merge
                    ws.Row(1).Height = 15;
                    ws.Cells[1, 1].Value = "Danh sách phiếu dịch vụ";
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
                    for (int i = 0; i < main.stkBillService.Children.Count; i++)
                    {
                        BillServiceControl control = (BillServiceControl)main.stkBillService.Children[i];
                        ws.Row(rowIndex).Height = 15;
                        colIndex = 1;
                        rowIndex++;
                        string address = "A" + rowIndex + ":I" + rowIndex;
                        ws.Cells[address].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        if (rowIndex % 2 != 0)
                        {
                            ws.Cells[address].Style.Fill.BackgroundColor.SetColor(255, 255, 255, 255);
                        }
                        else
                        {
                            ws.Cells[address].Style.Fill.BackgroundColor.SetColor(255, 229, 241, 255);
                        }
                        BillService billService = BillServiceDAL.Instance.GetBillService(ConvertToIDString(control.txbId.Text));
                        ws.Cells[rowIndex, colIndex++].Value = i + 1;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbId.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbNameCustomer.Text;
                        ws.Cells[rowIndex, colIndex++].Value = EmployeeDAL.Instance.GetByIdAccount(billService.IdAccount.ToString()).Name;
                        ws.Cells[rowIndex, colIndex++].Value = billService.CreatedDate.ToString("dd/MM/yyyy");
                        ws.Cells[rowIndex, colIndex++].Value = control.txbTotal.Text;
                        ws.Cells[rowIndex, colIndex++].Value = SeparateThousands(billService.TotalPaidMoney.ToString());
                        ws.Cells[rowIndex, colIndex++].Value = control.txbRest.Text;
                        ws.Cells[rowIndex, colIndex++].Value = control.txbStatus.Text;
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

