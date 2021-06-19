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

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class BillViewModel : BaseViewModel
    {
        public ICommand LoadBillCommand { get; set; }
        public ICommand PickBillCommand { get; set; }
        public ICommand PrintBillCommand { get; set; }

        private InvoiceControl checkedItem;
        private MainWindow mainWindow;

        private string customerName;
        private string customerPhoneNumber;
        private string customerAddress;
        private string idBill;
        private string employeeName;
        private string invoiceDate;
        private long quantity = 0;
        private long total = 0;

        public string CustomerName { get => customerName; set { customerName = value; OnPropertyChanged(); } }
        public string CustomerPhoneNumber { get => customerPhoneNumber; set { customerPhoneNumber = value; OnPropertyChanged(); } }
        public string CustomerAddress { get => customerAddress; set { customerAddress = value; OnPropertyChanged(); } }
        public string IdBill { get => idBill; set { idBill = value; OnPropertyChanged(); } }
        public string EmployeeName { get => employeeName; set { employeeName = value; OnPropertyChanged(); } }
        public string InvoiceDate { get => invoiceDate; set { invoiceDate = value; OnPropertyChanged(); } }
        public long Quantity { get => quantity; set { quantity = value; OnPropertyChanged(); } }
        public long Total { get => total; set { total = value; OnPropertyChanged(); } }

        public BillViewModel()
        {
            LoadBillCommand = new RelayCommand<MainWindow>(p => true, p => LoadBill(p));
            PickBillCommand = new RelayCommand<InvoiceControl>(p => true, p => PickBill(p));
            PrintBillCommand = new RelayCommand<MainWindow>(p => true, p => Print(p));
        }

        void LoadBill(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            if (mainWindow.dpStartDateBill.SelectedDate == null || mainWindow.dpEndDateBill.SelectedDate == null)
            {
                return;
            }
            if (mainWindow.dpStartDateBill.SelectedDate > mainWindow.dpEndDateBill.SelectedDate)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu nhỏ hơn ngày kết thúc!");
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
                invoiceControl.txbCustomerName.Text = customer.CustomerName;
                invoiceControl.txbEmployeeName.Text = employee.Name;
                invoiceControl.txbPrice.Text = billList[i].TotalMoney.ToString();

                mainWindow.stkBill.Children.Add(invoiceControl);
            }
        }
        void PickBill(InvoiceControl invoiceControl)
        {
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
            CustomerName = customer.CustomerName;
            CustomerPhoneNumber = customer.PhoneNumber;
            CustomerAddress = customer.Address;
            Total = bill.TotalMoney;

            mainWindow.stkBillInfo.Children.Clear();
            for (int i = 0; i < billInfos.Count; i++)
            {
                Goods goods = GoodsDAL.Instance.GetById(billInfos[i].IdGoods.ToString());
                GoodsType type = GoodsTypeDAL.Instance.GetById(goods.IdGoodsType);
                double profitPercentage = type.ProfitPercentage;

                InvoiceInfoControl control = new InvoiceInfoControl();
                control.txbNumber.Text = (i + 1).ToString();
                control.txbName.Text = goods.Name;
                control.txbUnitPrice.Text = Math.Round(goods.ImportPrice * (1 + profitPercentage)).ToString();
                control.txbUnit.Text = type.Unit;
                control.txbPrice.Text = billInfos[i].Price.ToString();
                control.txbQuantity.Text = billInfos[i].Quantity.ToString();
                control.txbTotal.Text = (float.Parse(control.txbPrice.Text) * billInfos[i].Quantity).ToString();

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

            int numOfItems = mainWindow.stkBillInfo.Children.Count;
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
            }
            try
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA5);

                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(billTemplate.grdPrint, IdBill);
                }
            }
            catch
            {

            }
        }
    }
}