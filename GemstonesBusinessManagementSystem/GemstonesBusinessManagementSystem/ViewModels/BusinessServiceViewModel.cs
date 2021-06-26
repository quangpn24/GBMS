using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.Template;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
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
    class BusinessServiceViewModel : BaseViewModel
    {
        private MainWindow mainWindow;
        private int totalServices = 0;
        private double totalMoney = 0;
        private double totalPaidMoney = 0;
        private bool isPaidMoney = false;
        private bool isOver = false;
        private long totalSpending = 0;

        public int TotalServices { get => totalServices; set { totalServices = value; OnPropertyChanged(); } }
        public string TotalMoney { get => SeparateThousands(totalMoney.ToString()); set { totalMoney = ConvertToNumber(value); OnPropertyChanged(); } }
        public string TotalPaidMoney { get => SeparateThousands(totalPaidMoney.ToString()); set { totalPaidMoney = ConvertToNumber(value); OnPropertyChanged(); } }

        public ICommand LoadSaleServicesCommand { get; set; }
        public ICommand PickServiceCommand { get; set; }
        public ICommand PickServiceInfoChangedCommand { get; set; }
        public ICommand DeleteBillServiceInfoDetailsCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand PayBillServiceCommand { get; set; }
        public ICommand CheckPaidMoneyCommand { get; set; }
        public ICommand PickCustomerCommand { get; set; }

        public BusinessServiceViewModel()
        {
            LoadSaleServicesCommand = new RelayCommand<MainWindow>((p) => true, (p) => LoadSaleServices(p));
            PickServiceCommand = new RelayCommand<SaleServiceControl>((p) => true, (p) => PickService(p));
            PickServiceInfoChangedCommand = new RelayCommand<SaleServiceDetailsControl>((p) => true, (p) => UpdateBillServiceInfo(p));
            DeleteBillServiceInfoDetailsCommand = new RelayCommand<SaleServiceDetailsControl>((p) => true, (p) => DeteleBillServiceInfo(p));
            SearchCommand = new RelayCommand<MainWindow>((p) => true, (p) => SearchService(p));
            PickCustomerCommand = new RelayCommand<MainWindow>((p) => true, (p) => OpenPickCustomerWd(p));
            PayBillServiceCommand = new RelayCommand<MainWindow>((p) => true, (p) => PayBillService(p));
            //CheckPaidMoneyCommand = new RelayCommand<SaleServiceDetailsControl>((p) => true, (p) => CheckPaidMoney(p));
        }
        void UpdateMembership(int idCustomer, long totalSpending)
        {
            CustomerDAL.Instance.UpdateTotalSpending(idCustomer, totalSpending);
            List<KeyValuePair<long, int>> membershipList = MembershipsTypeDAL.Instance.GetSortedList();
            foreach (var mem in membershipList)
            {
                if (totalSpending >= mem.Key)
                {
                    CustomerDAL.Instance.UpdateMembership(idCustomer, mem.Value);
                    break;
                }
            }
        }
        public PageContent ConvertToPage(Grid grid)
        {
            FixedPage page = new FixedPage();
            page.Width = grid.ActualWidth; ;
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
        public void LoadServicesToView(List<Service> services, MainWindow mainWindow)
        {
            int idBillService = BillServiceDAL.Instance.GetMaxId() + 1;
            mainWindow.txbIdBillService.Text = AddPrefix("PD", idBillService);
            mainWindow.txbDateBillService.Text = DateTime.Today.ToString("dd/MM/yyyy");
            mainWindow.wrpSaleService.Children.Clear();
            foreach (var service in services)
            {
                SaleServiceControl saleService = new SaleServiceControl();
                saleService.txbId.Text = AddPrefix("DV", service.IdService);
                saleService.txbName.Text = service.Name;
                saleService.txbPrice.Text = SeparateThousands(service.Price.ToString());
                mainWindow.wrpSaleService.Children.Add(saleService);
            }
        }
        public void LoadSaleServices(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            List<Service> services = ServiceDAL.Instance.GetActivedServices();
            LoadServicesToView(services, mainWindow);
        }
        public void PickService(SaleServiceControl saleServiceControl)
        {
            saleServiceControl.Focus();
            bool isExist = IsExisted(saleServiceControl.txbId.Text);

            if (!isExist)
            {
                TotalServices++;
                SaleServiceDetailsControl saleServiceDetails = new SaleServiceDetailsControl();
                saleServiceDetails.txbName.Text = saleServiceControl.txbName.Text;
                saleServiceDetails.txbSerial.Text = saleServiceControl.txbId.Text;
                saleServiceDetails.txbPrice.Text = SeparateThousands(saleServiceControl.txbPrice.Text);
                saleServiceDetails.txtTotal.Text = SeparateThousands(saleServiceControl.txbPrice.Text);
                this.mainWindow.stkPickedService.Children.Add(saleServiceDetails);
            }
            else
            {

                for (int i = 0; i < this.mainWindow.stkPickedService.Children.Count; i++)
                {
                    SaleServiceDetailsControl temp = mainWindow.stkPickedService.Children[i] as SaleServiceDetailsControl;
                    if (temp.txbSerial.Text == saleServiceControl.txbId.Text)
                    {
                        temp.nmsQuantity.Value++;
                        break;
                    }
                }
            }
            CalculateMoney();
        }
        public void UpdateBillServiceInfo(SaleServiceDetailsControl saleServiceDetailsControl)
        {
            SeparateThousands(saleServiceDetailsControl.txtTips);
            SeparateThousands(saleServiceDetailsControl.txtPaidMoney);
            int quantity = int.Parse(saleServiceDetailsControl.nmsQuantity.Text.ToString());
            double price = double.Parse(saleServiceDetailsControl.txbPrice.Text);
            double tips = 0;
            if (!String.IsNullOrEmpty(saleServiceDetailsControl.txtTips.Text))
                tips = double.Parse(saleServiceDetailsControl.txtTips.Text);
            saleServiceDetailsControl.txtTotal.Text = string.Format("{0:N0}", quantity * (price + tips));
            CalculateMoney();


        }
        public void DeteleBillServiceInfo(SaleServiceDetailsControl saleServiceDetailsControl)
        {
            TotalServices--;
            mainWindow.stkPickedService.Children.Remove(saleServiceDetailsControl);
            CalculateMoney();
        }
        public void SearchService(MainWindow mainWindow)
        {
            List<Service> services = ServiceDAL.Instance.GetActivedServicesByName(mainWindow.txtSearchSaleService.Text);
            LoadServicesToView(services, mainWindow);
        }
        public void CalculateMoney()
        {
            TotalMoney = TotalPaidMoney = "0";
            for (int i = 0; i < this.mainWindow.stkPickedService.Children.Count; i++)
            {
                SaleServiceDetailsControl temp = mainWindow.stkPickedService.Children[i] as SaleServiceDetailsControl;
                totalMoney += double.Parse(temp.txtTotal.Text);
                if (!String.IsNullOrEmpty(temp.txtPaidMoney.Text))
                {
                    totalPaidMoney += double.Parse(temp.txtPaidMoney.Text);
                }
            }
            TotalMoney = totalMoney.ToString();
            TotalPaidMoney = totalPaidMoney.ToString();
        }
        public bool IsExisted(string idService)
        {
            for (int i = 0; i < this.mainWindow.stkPickedService.Children.Count; i++)
            {
                SaleServiceDetailsControl temp = mainWindow.stkPickedService.Children[i] as SaleServiceDetailsControl;
                if (temp.txbSerial.Text == idService)
                {
                    return true;
                }
            }
            return false;
        }
        public void PayBillService(MainWindow mainWindow)
        {
            if (String.IsNullOrEmpty(mainWindow.txbIdCustomer.Text))
            {
                CustomMessageBox.Show("Vui lòng chọn khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            double prepaymentPercent = double.Parse(ParameterDAL.Instance.GetPrepayment().Value) / 100;
            isPaidMoney = CheckPaidMoney(mainWindow, prepaymentPercent);
            isOver = IsOverMoney(mainWindow);
            if (isOver)
            {
                CustomMessageBox.Show("Vui lòng nhập tiền cọc <= thành tiền trong từng dịch vụ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (isPaidMoney)
            {
                int idBillService = BillServiceDAL.Instance.GetMaxId() + 1;
                BillService billService = new BillService(idBillService, CurrentAccount.IdAccount, DateTime.Now, totalMoney, totalPaidMoney, ConvertToID(mainWindow.txbIdCustomer.Text), 0);
                Customer customer = CustomerDAL.Instance.FindById(billService.IdCustomer.ToString());
                if (totalMoney != 0) // Kiểm tra xem có hàng hóa nào được chọn không
                {
                    if (BillServiceDAL.Instance.Add(billService)) // Tạo hóa đơn mới
                    {
                        for (int i = 0; i < mainWindow.stkPickedService.Children.Count; i++) // Duyệt các chi tiết hóa đơn dịch vụ đã chọn
                        {
                            SaleServiceDetailsControl temp = mainWindow.stkPickedService.Children[i] as SaleServiceDetailsControl;
                            double paidMoney = 0;
                            double tips = 0;
                            if (!String.IsNullOrEmpty(temp.txtPaidMoney.Text))
                                paidMoney = double.Parse(temp.txtPaidMoney.Text);
                            if (!String.IsNullOrEmpty(temp.txtTips.Text))
                                tips = double.Parse(temp.txtTips.Text);
                            BillServiceInfo billServiceInfo = new BillServiceInfo(ConvertToID(mainWindow.txbIdBillService.Text), ConvertToID(temp.txbSerial.Text), double.Parse(temp.txbPrice.Text), tips, Convert.ToInt32(temp.nmsQuantity.Value), paidMoney, 0, DateTime.Now);
                            BillServiceInfoDAL.Instance.Insert(billServiceInfo);
                            customer.TotalPrice += Convert.ToInt64(billService.TotalPaidMoney);
                            //UpdateMembership(customer);
                            //CustomerDAL.Instance.AddOrUpdate(customer, true);
                        }

                        var result = CustomMessageBox.Show("Thanh toán thành công! Bạn có muốn in hóa đơn?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            PrintBill(mainWindow);
                        }
                        UpdateMembership(ConvertToID(mainWindow.txbIdCustomer.Text), totalSpending + Convert.ToInt64(totalPaidMoney));
                        CustomerViewModel customerVM = (CustomerViewModel)mainWindow.grdCustomer.DataContext;
                        customerVM.LoadCustomerToView(mainWindow, 0);
                        
                        mainWindow.stkPickedService.Children.Clear();
                        mainWindow.txbIdBillService.Text = AddPrefix("PD", idBillService + 1);
                        TotalMoney = TotalPaidMoney = "0";
                        TotalServices = 0;
                        mainWindow.txbIdCustomer.Text = "";
                        mainWindow.txbNameCustomer.Text = "";
                        mainWindow.txbAddressCustomer.Text = "";
                        mainWindow.txbPhoneCustomer.Text = "";
                        mainWindow.txbClassCustomer.Text = "";
                    }
                    else
                    {
                        CustomMessageBox.Show("Thanh toán thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    CustomMessageBox.Show("Vui lòng chọn dịch vụ trước khi thanh toán!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                CustomMessageBox.Show(string.Format("Vui lòng nhập tiền đặt cọc >= {0}% tiền thanh toán trong từng dịch vụ!", prepaymentPercent * 100), "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public bool CheckPaidMoney(MainWindow mainWindow, double prepaymentPercent)
        {
            for (int i = 0; i < mainWindow.stkPickedService.Children.Count; i++)
            {
                SaleServiceDetailsControl temp = mainWindow.stkPickedService.Children[i] as SaleServiceDetailsControl;
                if (String.IsNullOrEmpty(temp.txtPaidMoney.Text) && prepaymentPercent == 0)
                    return true;
                if (!String.IsNullOrEmpty(temp.txtPaidMoney.Text))
                {
                    if (double.Parse(temp.txtPaidMoney.Text) / double.Parse(temp.txtTotal.Text) < prepaymentPercent)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsOverMoney(MainWindow mainWindow)
        {
            for (int i = 0; i < mainWindow.stkPickedService.Children.Count; i++)
            {
                SaleServiceDetailsControl temp = mainWindow.stkPickedService.Children[i] as SaleServiceDetailsControl;
                if (!String.IsNullOrEmpty(temp.txtPaidMoney.Text))
                {
                    if (double.Parse(temp.txtPaidMoney.Text) / double.Parse(temp.txtTotal.Text) > 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void PrintBill(MainWindow mainWindow)
        {
            BillServiceTemplate billServiceTemplate = new BillServiceTemplate();
            billServiceTemplate.txbName.Text = mainWindow.txbNameCustomer.Text;
            billServiceTemplate.txbAddress.Text = mainWindow.txbAddressCustomer.Text;
            billServiceTemplate.txbPhoneNumber.Text = mainWindow.txbPhoneCustomer.Text;
            billServiceTemplate.txbEmployeeName.Text = CurrentAccount.Name;
            billServiceTemplate.txbId.Text = mainWindow.txbIdBillService.Text;
            billServiceTemplate.txbDate.Text = mainWindow.txbDateBillService.Text;
            billServiceTemplate.txbTotal.Text = mainWindow.txbTotalBillService.Text;
            billServiceTemplate.txbTotalPaid.Text = mainWindow.txbTotalPaidBillService.Text;
            billServiceTemplate.txbRest.Text = SeparateThousands((double.Parse(mainWindow.txbTotalBillService.Text) - double.Parse(mainWindow.txbTotalPaidBillService.Text)).ToString());
            List<Parameter> parameters = ParameterDAL.Instance.GetData();
            billServiceTemplate.txbStoreName.Text = parameters[1].Value;
            //print 
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() != true) return;
            FixedDocument document = new FixedDocument();
            PageContent temp;
            for (int i = 0; i < mainWindow.stkPickedService.Children.Count; i++) // Duyệt các chi tiết hóa đơn dịch vụ đã chọn
            {
                SaleServiceDetailsControl control = mainWindow.stkPickedService.Children[i] as SaleServiceDetailsControl;
                BillServiceTemplateControl billServiceTemplateControl = new BillServiceTemplateControl();
                billServiceTemplateControl.txbNumber.Text = (i + 1).ToString();
                billServiceTemplateControl.txbName.Text = control.txbName.Text;
                billServiceTemplateControl.txbPrice.Text = control.txbPrice.Text;
                double tips = 0;
                if (!String.IsNullOrEmpty(control.txtTips.Text))
                {
                    tips = double.Parse(control.txtTips.Text);
                }
                billServiceTemplateControl.txbCalculateMoney.Text = SeparateThousands((double.Parse(control.txbPrice.Text) + tips).ToString());
                billServiceTemplateControl.txbQuantity.Text = control.nmsQuantity.Text.ToString();
                billServiceTemplateControl.txbTotal.Text = control.txtTotal.Text;
                billServiceTemplateControl.txbPaidMoney.Text = control.txtPaidMoney.Text;
                billServiceTemplateControl.txbRest.Text = SeparateThousands((double.Parse(control.txtTotal.Text) - double.Parse(control.txtPaidMoney.Text)).ToString());
                billServiceTemplateControl.txbDeliveryDate.Text = "";
                billServiceTemplateControl.txbStatus.Text = "Chưa giao";
                billServiceTemplateControl.btnSwapStatus.Visibility = Visibility.Hidden;
                billServiceTemplateControl.grdMain.ColumnDefinitions.RemoveAt(10);
                billServiceTemplateControl.grdMain.Children.Remove(billServiceTemplateControl.grdMain.Children[11]);
                billServiceTemplate.stkBillServiceInfo.Children.Add(billServiceTemplateControl);

                document.DocumentPaginator.PageSize = new Size(billServiceTemplate.grdPrint.ActualWidth, billServiceTemplate.grdPrint.ActualHeight);
                if (billServiceTemplate.stkBillServiceInfo.Children.Count == 10 || i == mainWindow.stkPickedService.Children.Count - 1)
                {
                    billServiceTemplate.grdPrint.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    billServiceTemplate.grdPrint.Arrange(new Rect(0, 0, billServiceTemplate.grdPrint.DesiredSize.Width, billServiceTemplate.grdPrint.DesiredSize.Height));
                    temp = ConvertToPage(billServiceTemplate.grdPrint);
                    document.Pages.Add(temp);
                    billServiceTemplate.stkBillServiceInfo.Children.Clear();
                }
            }
            pd.PrintDocument(document.DocumentPaginator, mainWindow.txbIdBillService.Text);
            CustomMessageBox.Show("In hóa đơn thành công", "Thông tin", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        public void OpenPickCustomerWd(MainWindow mainWindow)
        {
            PickCustomerWindow pickCustomerWindow = new PickCustomerWindow();
            pickCustomerWindow.ShowDialog();
            mainWindow.txbIdCustomer.Text = pickCustomerWindow.txbId.Text;
            mainWindow.txbNameCustomer.Text = pickCustomerWindow.txbName.Text;
            mainWindow.txbAddressCustomer.Text = pickCustomerWindow.txbAddress.Text;
            mainWindow.txbPhoneCustomer.Text = pickCustomerWindow.txbPhoneNumber.Text;
            mainWindow.txbClassCustomer.Text = pickCustomerWindow.txbRank.Text;
            if (!String.IsNullOrEmpty(pickCustomerWindow.txbSpending.Text))
            {
                totalSpending = ConvertToNumber(pickCustomerWindow.txbSpending.Text);
            }
        }
    }
}
