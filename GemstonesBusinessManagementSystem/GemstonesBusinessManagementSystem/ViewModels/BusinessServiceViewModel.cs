using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.Template;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GemstonesBusinessManagementSystem.ViewModels
{


    class BusinessServiceViewModel : BaseViewModel
    {
        private MainWindow mainWindow;
        private int totalServices = 0;
        private float totalMoney = 0;
        private float totalPaidMoney = 0;
        private bool isPaidMoney = false;

        public int TotalServices { get => totalServices; set { totalServices = value; OnPropertyChanged(); } }
        public float TotalMoney { get => totalMoney; set { totalMoney = value; OnPropertyChanged(); } }
        public float TotalPaidMoney { get => totalPaidMoney; set { totalPaidMoney = value; OnPropertyChanged(); } }

        public ICommand LoadSaleServicesCommand { get; set; }
        public ICommand PickServiceCommand { get; set; }
        public ICommand PickServiceInfoChangedCommand { get; set; }
        public ICommand DeleteBillServiceInfoDetailsCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand PayBillServiceCommand { get; set; }
        public ICommand CheckPaidMoneyCommand { get; set; }
        public ICommand PickCustomerCommand { get; set; }
        //billservicetemplate
        public ICommand PrintBillServiceCommand { get; set; }

        public BusinessServiceViewModel()
        {
            LoadSaleServicesCommand = new RelayCommand<MainWindow>((p) => true, (p) => LoadSaleServices(p));
            PickServiceCommand = new RelayCommand<SaleServiceControl>((p) => true, (p) => PickService(p));
            PickServiceInfoChangedCommand = new RelayCommand<SaleServiceDetailsControl>((p) => true, (p) => UpdateBillServiceInfo(p));
            DeleteBillServiceInfoDetailsCommand = new RelayCommand<SaleServiceDetailsControl>((p) => true, (p) => DeteleBillServiceInfo(p));
            SearchCommand = new RelayCommand<MainWindow>((p) => true, (p) => SearchService(p));
            PickCustomerCommand = new RelayCommand<MainWindow>((p) => true, (p) => OpenPickCustomerWd(p));
            PayBillServiceCommand = new RelayCommand<MainWindow>((p) => true, (p) => PayBillService(p));
            CheckPaidMoneyCommand = new RelayCommand<SaleServiceDetailsControl>((p) => true, (p) => CheckPaidMoney(p));
            PrintBillServiceCommand = new RelayCommand<BillServiceTemplate>((p) => true, (p) => PrintBillService(p));
        }
        public void PrintBillService(BillServiceTemplate billServiceTemplate)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    billServiceTemplate.btnPrint.Visibility = Visibility.Hidden;
                    printDialog.PrintVisual(billServiceTemplate.grdPrint, "Bill Service");
                }
            }
            finally
            {
                billServiceTemplate.btnPrint.Visibility = Visibility.Visible;
            }
        }
        public void LoadServicesToView(List<Service> services, MainWindow mainWindow)
        {
            mainWindow.wrpSaleService.Children.Clear();
            foreach (var service in services)
            {
                SaleServiceControl saleService = new SaleServiceControl();
                saleService.txbId.Text = AddPrefix("DV", service.IdService);
                saleService.txbName.Text = service.Name;
                saleService.txbPrice.Text = service.Price.ToString();
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
                saleServiceDetails.txbPrice.Text = saleServiceControl.txbPrice.Text;
                saleServiceDetails.txtTotal.Text = saleServiceControl.txbPrice.Text;
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
            float price = float.Parse(saleServiceDetailsControl.txbPrice.Text);
            float tips = 0;
            if (!String.IsNullOrEmpty(saleServiceDetailsControl.txtTips.Text))
                tips = float.Parse(saleServiceDetailsControl.txtTips.Text);
            saleServiceDetailsControl.txtTotal.Text = string.Format("{0:N0}", quantity * (price + tips));
            CalculateMoney();


        }
        public void CheckPaidMoney(SaleServiceDetailsControl saleServiceDetailsControl)
        {
            if (!String.IsNullOrEmpty(saleServiceDetailsControl.txtPaidMoney.Text))
            {
                float paidMoney = float.Parse(saleServiceDetailsControl.txtPaidMoney.Text);
                float total = float.Parse(saleServiceDetailsControl.txtTotal.Text);
                if (paidMoney / total < 0.5)
                {
                    isPaidMoney = false;
                    MessageBox.Show("Số tiền đặt cọc >= 50%");
                }
                else
                {
                    isPaidMoney = true;
                }
            }
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
            TotalMoney = TotalPaidMoney = 0;
            for (int i = 0; i < this.mainWindow.stkPickedService.Children.Count; i++)
            {
                SaleServiceDetailsControl temp = mainWindow.stkPickedService.Children[i] as SaleServiceDetailsControl;
                TotalMoney += float.Parse(temp.txtTotal.Text);
                if (!String.IsNullOrEmpty(temp.txtPaidMoney.Text))
                {
                    TotalPaidMoney += float.Parse(temp.txtPaidMoney.Text);
                }
            }
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
                MessageBox.Show("Vui lòng chọn khách hàng!");
                return;
            }
            if (isPaidMoney)
            {
                int idBillService = BillServiceDAL.Instance.GetMaxId() + 1;
                BillService billService = new BillService(idBillService, 1, DateTime.Now, totalMoney, totalPaidMoney, 1, 0);
                if (TotalMoney != 0) // Kiểm tra xem có hàng hóa nào được chọn không
                {
                    if (BillServiceDAL.Instance.Add(billService)) // Tạo hóa đơn mới
                    {
                        for (int i = 0; i < mainWindow.stkPickedService.Children.Count; i++) // Duyệt các chi tiết hóa đơn dịch vụ đã chọn
                        {
                            SaleServiceDetailsControl temp = mainWindow.stkPickedService.Children[i] as SaleServiceDetailsControl;
                            float paidMoney = 0;
                            float tips = 0;
                            if (!String.IsNullOrEmpty(temp.txtPaidMoney.Text))
                                paidMoney = float.Parse(temp.txtPaidMoney.Text);
                            if (!String.IsNullOrEmpty(temp.txtTips.Text))
                                tips = float.Parse(temp.txtTips.Text);
                            BillServiceInfo billServiceInfo = new BillServiceInfo(idBillService, ConvertToID(temp.txbSerial.Text), float.Parse(temp.txbPrice.Text), tips, Convert.ToInt32(temp.nmsQuantity.Value), paidMoney, 0, DateTime.Now);
                            BillServiceInfoDAL.Instance.Insert(billServiceInfo);
                        }
                        mainWindow.txbIdBillService.Text = AddPrefix("PD", idBillService);
                        var result = MessageBox.Show("Thanh toán thành công! Bạn có muốn in hóa đơn?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            PrintBill(mainWindow);
                        }
                        mainWindow.stkPickedService.Children.Clear();
                        TotalMoney = TotalPaidMoney = TotalServices = 0;
                    }
                    else
                    {
                        MessageBox.Show("Thanh toán thất bại!");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng mua hàng trước khi thanh toán!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tiền đặt cọc >= 50% tiền thanh toán!");
            }
        }
        public void PrintBill(MainWindow mainWindow)
        {
            BillServiceTemplate billServiceTemplate = new BillServiceTemplate();
            billServiceTemplate.txbName.Text = mainWindow.txbNameCustomer.Text;
            billServiceTemplate.txbAddress.Text = mainWindow.txbAddressCustomer.Text;
            billServiceTemplate.txbPhoneNumber.Text = mainWindow.txbPhoneCustomer.Text;
            billServiceTemplate.txbId.Text = mainWindow.txbIdBillService.Text;
            billServiceTemplate.txbDate.Text = DateTime.Now.ToShortDateString();
            billServiceTemplate.txbTotal.Text = mainWindow.txbTotalBillService.Text;
            billServiceTemplate.txbTotalPaid.Text = mainWindow.txbTotalPaidBillService.Text;
            billServiceTemplate.txbRest.Text = (float.Parse(mainWindow.txbTotalBillService.Text) - float.Parse(mainWindow.txbTotalPaidBillService.Text)).ToString();
            for (int i = 0; i < mainWindow.stkPickedService.Children.Count; i++) // Duyệt các chi tiết hóa đơn dịch vụ đã chọn
            {
                SaleServiceDetailsControl temp = mainWindow.stkPickedService.Children[i] as SaleServiceDetailsControl;
                BillServiceTemplateControl billServiceTemplateControl = new BillServiceTemplateControl();
                billServiceTemplateControl.txbNumber.Text = (i + 1).ToString();
                billServiceTemplateControl.txbName.Text = temp.txbName.Text;
                billServiceTemplateControl.txbPrice.Text = temp.txbPrice.Text;
                float tips = 0;
                if (!String.IsNullOrEmpty(temp.txtTips.Text))
                {
                    tips = float.Parse(temp.txtTips.Text);
                }
                billServiceTemplateControl.txbCalculateMoney.Text = (float.Parse(temp.txbPrice.Text) + tips).ToString();
                billServiceTemplateControl.txbQuantity.Text = temp.nmsQuantity.Text.ToString();
                billServiceTemplateControl.txbTotal.Text = temp.txtTotal.Text;
                billServiceTemplateControl.txbPaidMoney.Text = temp.txtPaidMoney.Text;
                billServiceTemplateControl.txbRest.Text = (float.Parse(temp.txtTotal.Text) - float.Parse(temp.txtPaidMoney.Text)).ToString();
                billServiceTemplateControl.txbDeliveryDate.Text = "";
                billServiceTemplateControl.txbStatus.Text = "Chưa giao";
                billServiceTemplateControl.btnSwapStatus.Visibility = Visibility.Hidden;
                billServiceTemplate.stkBillServiceInfo.Children.Add(billServiceTemplateControl);
            }
            billServiceTemplate.ShowDialog();

        }
        public void OpenPickCustomerWd(MainWindow mainWindow)
        {
            PickCustomerWindow pickCustomerWindow = new PickCustomerWindow();
            pickCustomerWindow.ShowDialog();
            mainWindow.txbIdCustomer.Text = pickCustomerWindow.txbId.Text;
            mainWindow.txbNameCustomer.Text = pickCustomerWindow.txbName.Text;
            mainWindow.txbAddressCustomer.Text = pickCustomerWindow.txbAddress.Text;
            mainWindow.txbPhoneCustomer.Text = pickCustomerWindow.txbPhoneNumber.Text;
        }
    }
}
