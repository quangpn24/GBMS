using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.Template;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class SaleViewModel : BaseViewModel
    {
        //Wrap panel
        public ICommand LoadSaleGoodsCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand PickGoodsCommand { get; set; }

        //Payment
        public ICommand CompletePaymentCommand { get; set; }
        public ICommand PrintPaymentCommand { get; set; }

        //Customer
        public ICommand SelectCustomerCommand { get; set; }

        //Selected goods
        public ICommand DeleteCommand { get; set; }
        public ICommand ChangeQuantityCommand { get; set; }

        private MainWindow mainWindow;
        private List<Goods> saleGoodsList = GoodsDAL.Instance.GetList();

        private long subTotal = 0;
        public long SubTotal { get => subTotal; set { subTotal = value; OnPropertyChanged(); } }
        private long total = 0;
        public long Total { get => total; set { total = value; OnPropertyChanged(); } }
        private long discount = 0;
        public long Discount { get => discount; set { discount = value; OnPropertyChanged(); } }

        public SaleViewModel()
        {
            LoadSaleGoodsCommand = new RelayCommand<MainWindow>((p) => true, (p) => { LoadDefault(p); LoadSaleGoods(p); });
            PickGoodsCommand = new RelayCommand<SaleGoodsControl>((p) => true, (p) => PickGoods(p));
            SearchCommand = new RelayCommand<MainWindow>((p) => true, (p) => Search(p));

            CompletePaymentCommand = new RelayCommand<MainWindow>((p) => true, (p) => CompletePayment(p));
            PrintPaymentCommand = new RelayCommand<MainWindow>((p) => true, (p) => PrintPayment(p));

            SelectCustomerCommand = new RelayCommand<MainWindow>((p) => true, (p) => SelectCustomer(p));

            DeleteCommand = new RelayCommand<SelectedGoodsControl>((p) => true, (p) => DeleteSelectedGoods(p));
            ChangeQuantityCommand = new RelayCommand<SelectedGoodsControl>((p) => true, (p) => ChangeQuantity(p));
        }
        
        void ChangeQuantity(SelectedGoodsControl control)
        {
            SubTotal -= long.Parse(control.txbTotalPrice.Text);
            control.txbTotalPrice.Text = (int.Parse(control.txbPrice.Text) * control.nmsQuantity.Value).ToString();
            SubTotal += long.Parse(control.txbTotalPrice.Text);
            Total = SubTotal - Discount;
        }
        void DeleteSelectedGoods(SelectedGoodsControl control)
        {
            SubTotal -= int.Parse(control.txbTotalPrice.Text);
            mainWindow.stkSelectedGoods.Children.Remove(control);
            Total = SubTotal - Discount;
        }
        
        void SelectCustomer(MainWindow window)
        {

        }

        private void CompletePayment(MainWindow window)
        {
            if (string.IsNullOrEmpty(window.txbSaleCustomerName.Text))
            {
                MessageBox.Show("Vui lòng nhập thông tin khách hàng!");
                return;
            }
            if (mainWindow.stkSelectedGoods.Children.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm!");
                return;
            }
            Bill bill = new Bill(ConvertToID(window.txbIdBill.Text), 1, DateTime.Parse(window.txbSaleDate.Text),
                "", Total, ConvertToID(window.txbSaleIdCustomer.Text), window.txbSaleNote.Text);
            bool isSuccess = BillDAL.Instance.Insert(bill);

            int numOfItems = mainWindow.stkSelectedGoods.Children.Count;
            for (int i = 0; i < numOfItems; i++)
            {
                if (!isSuccess)
                {
                    break;
                }
                SelectedGoodsControl control = (SelectedGoodsControl)mainWindow.stkSelectedGoods.Children[i];

                int quantity = int.Parse(control.nmsQuantity.Text.ToString());
                BillInfo billInfo = new BillInfo(bill.IdBill, ConvertToID(control.txbId.Text), quantity);
                BillInfoDAL.Instance.Insert(billInfo);

                isSuccess = GoodsDAL.Instance.UpdateQuantity(ConvertToID(control.txbId.Text), -quantity);
            }

            if (isSuccess)
            {
                var result = MessageBox.Show("Thanh toán thành công! Bạn có muốn in hóa đơn?",
                    "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PrintPayment(mainWindow);
                }
            }
            else
            {
                MessageBox.Show("Thanh toán thất bại!");
            }
        }
        private void PrintPayment(MainWindow window)
        {
            BillTemplate billTemplate = new BillTemplate();

            billTemplate.txbInvoiceDate.Text = window.txbSaleDate.Text;
            billTemplate.txbIdBill.Text = window.txbIdBill.Text;
            billTemplate.txbCustomerName.Text = window.txbSaleCustomerName.Text;
            billTemplate.txbCustomerPhoneNumber.Text = window.txbSaleCustomerPhone.Text;
            billTemplate.txbCustomerAddress.Text = window.txbSaleCustomerAddress.Text;
            billTemplate.txbSubTotal.Text = window.txbSaleSubTotal.Text;
            billTemplate.txbDiscount.Text = window.txbSaleDiscount.Text;
            billTemplate.txbTotal.Text = window.txbSaleTotal.Text;

            int numOfItems = mainWindow.stkSelectedGoods.Children.Count;
            for (int i = 0; i < numOfItems; i++)
            {
                SelectedGoodsControl selectedControl = (SelectedGoodsControl)mainWindow.stkSelectedGoods.Children[i];
                BillInfoControl billInfoControl = new BillInfoControl();
                billInfoControl.txbOrderNum.Text = (i + 1).ToString();
                billInfoControl.txbName.Text = selectedControl.txbName.Text;
                billInfoControl.txbQuantity.Text = selectedControl.nmsQuantity.Text.ToString();
                billInfoControl.txbUnit.Text = selectedControl.txbUnit.Text;
                billInfoControl.txbUnitPrice.Text = selectedControl.txbPrice.Text;
                billInfoControl.txbTotal.Text = selectedControl.txbTotalPrice.Text;

                billTemplate.stkBillInfo.Children.Add(billInfoControl);
            }
            try
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA5);
                
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(billTemplate.grdPrint, window.txbIdBill.Text);
                }
            }
            catch
            {

            }
        }

        void Search(MainWindow window)
        {
            string namesearching = mainWindow.txtSearchSaleGoods.Text.ToLower();
            saleGoodsList = GoodsDAL.Instance.FindByName(namesearching);
            LoadSaleGoods(mainWindow);
        }
        void PickGoods(SaleGoodsControl control)
        {
            SelectedGoodsControl selectedControl = new SelectedGoodsControl();

            selectedControl.txbId.Text = AddPrefix("SP", int.Parse(control.txbId.Text));
            selectedControl.txbName.Text = control.txbName.Text;
            selectedControl.txbType.Text = control.txbType.Text;
            selectedControl.txbUnit.Text = control.txbUnit.Text;
            selectedControl.txbPrice.Text = control.txbPrice.Text;
            selectedControl.txbTotalPrice.Text =
                (int.Parse(control.txbPrice.Text) * selectedControl.nmsQuantity.Value).ToString();
            selectedControl.nmsQuantity.MaxValue = decimal.Parse(control.txbQuantity.Text);

            int numOfItems = mainWindow.stkSelectedGoods.Children.Count;
            for (int i = 0; i < numOfItems; i++)
            {
                SelectedGoodsControl temp = (SelectedGoodsControl)mainWindow.stkSelectedGoods.Children[i];
                if (selectedControl.txbId.Text == temp.txbId.Text)
                {
                    if (int.Parse(control.txbQuantity.Text) == temp.nmsQuantity.Value)
                    {
                        MessageBox.Show("Đã hết hàng!");
                        return;
                    }
                    temp.nmsQuantity.Value++;
                    return;
                }
            }
            SubTotal += int.Parse(selectedControl.txbPrice.Text);
            Total = SubTotal - Discount;
            mainWindow.stkSelectedGoods.Children.Add(selectedControl);
        }
        void LoadDefault(MainWindow window)
        {
            int maxId = BillDAL.Instance.GetMaxId();
            window.txbIdBill.Text = AddPrefix("HD", maxId + 1);
            window.txbSaleDate.Text = DateTime.Now.ToShortDateString();
        }
        void LoadSaleGoods(MainWindow window)
        {
            mainWindow = window;
            mainWindow.wrpSaleGoods.Children.Clear();

            for (int i = 0; i < saleGoodsList.Count; i++)
            {
                if (saleGoodsList[i].Quantity > 0)
                {
                    SaleGoodsControl control = new SaleGoodsControl();

                    control.txbQuantity.Text = saleGoodsList[i].Quantity.ToString();
                    control.txbId.Text = saleGoodsList[i].IdGoods.ToString();
                    control.imgGood.Source = Converter.Instance.ConvertByteToBitmapImage(saleGoodsList[i].ImageFile);
                    control.txbName.Text = saleGoodsList[i].Name;
                    control.txbPrice.Text = saleGoodsList[i].Price.ToString();
                    GoodsType type = GoodsTypeDAL.Instance.GetGoodsTypeWithId(saleGoodsList[i].IdGoodsType);
                    control.txbType.Text = type.Name;
                    control.txbUnit.Text = type.Unit;

                    mainWindow.wrpSaleGoods.Children.Add(control);
                }
            }
        }
    }
}
