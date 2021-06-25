using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class BillServiceViewModel : BaseViewModel
    {
        private MainWindow main;
        private BillServiceControl checkedItem;
        public ICommand LoadBillServicesCommand { get; set; }
        public ICommand PickBillServiceCommand { get; set; }
        public ICommand ConfirmDeliveriedCommand { get; set; }
        public ICommand PrintBillServiceCommand { get; set; }
        public ICommand DeleteBillServiceCommand { get; set; }
        public BillServiceViewModel()
        {
            LoadBillServicesCommand = new RelayCommand<MainWindow>((p) => true, (p) => LoadBillServices(p));
            PickBillServiceCommand = new RelayCommand<BillServiceControl>((p) => true, (p) => PickBillService(p));
            ConfirmDeliveriedCommand = new RelayCommand<BillServiceTemplateControl>((p) => true, (p) => ConfirmDeliveried(p));
            PrintBillServiceCommand = new RelayCommand<MainWindow>((p) => true, (p) => PrintBillService(p));
            DeleteBillServiceCommand = new RelayCommand<BillServiceControl>((p) => true, (p) => DeleteBillService(p));
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
                MessageBox.Show("Vui lòng chọn ngày bắt đầu nhỏ hơn ngày kết thúc!");
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
            billServiceControl.txbId.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            billServiceControl.txbNameCustomer.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            billServiceControl.txbTotal.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            billServiceControl.txbRest.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            if (billServiceControl.txbStatus.Text == "Chưa giao")
            {
                billServiceControl.txbStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            }
            checkedItem = billServiceControl;
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
                MessageBox.Show("Cập nhật thất bại!");
            }
            //billServiceTemplateControl.icStatus.Kind = MaterialDesignThemes.Wpf.PackIconKind.TickCircleOutline;
            //billServiceTemplateControl.btnSwapStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF01B500");
        }
        public void PrintBillService(MainWindow mainWindow)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(mainWindow.grdPrintBS, "Bill Service");
                }
            }
            finally
            {
                main.btnPrintBS.IsEnabled = true;
            }
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
        public void DeleteBillService(BillServiceControl billServiceControl)
        {
            var result = MessageBox.Show("Xác nhận xóa phiếu dịch vụ?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (BillServiceInfoDAL.Instance.DeleteByIdBillService(ConvertToIDString(billServiceControl.txbId.Text)))
                {
                    if (BillServiceDAL.Instance.Delete(ConvertToIDString(billServiceControl.txbId.Text)))
                    {
                        Customer customer = CustomerDAL.Instance.FindById(billServiceControl.txbIdCustomer.Text);
                        customer.TotalPrice -= ConvertToNumber(billServiceControl.txbTotal.Text);
                        customer.TotalPrice += ConvertToNumber(billServiceControl.txbRest.Text);
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
                        MessageBox.Show("Xóa phiếu dịch vụ thành công");
                    }

                }
            }
        }

    }
}

