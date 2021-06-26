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

        private string idCustomer;
        private string customerName;
        private string customerPhoneNumber;
        private string customerClass;
        private string customerAddress;
        private long customerSpending;
        private string idBill;
        private string invoiceDate;
        private long quantity = 0;
        private long total = 0;

        public string IdCustomer { get => idCustomer; set { idCustomer = value; OnPropertyChanged(); } }
        public string CustomerName { get => customerName; set { customerName = value; OnPropertyChanged(); } }
        public string CustomerPhoneNumber { get => customerPhoneNumber; set { customerPhoneNumber = value; OnPropertyChanged(); } }
        public string CustomerClass { get => customerClass; set { customerClass = value; OnPropertyChanged(); } }
        public string CustomerAddress { get => customerAddress; set { customerAddress = value; OnPropertyChanged(); } }
        public long CustomerSpending { get => customerSpending; set { customerSpending = value; OnPropertyChanged(); } }
        public string IdBill { get => idBill; set { idBill = value; OnPropertyChanged(); } }
        public string InvoiceDate { get => invoiceDate; set { invoiceDate = value; OnPropertyChanged(); } }
        public long Quantity { get => quantity; set { quantity = value; OnPropertyChanged(); } }
        public string Total { get => SeparateThousands(total.ToString()); set { total = ConvertToNumber(value); OnPropertyChanged(); } }

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
        void OnChangeQuantity()
        {
            Quantity = 0;
            int n = mainWindow.stkSelectedGoods.Children.Count;
            for (int i = 0; i < n; i++)
            {
                SelectedGoodsControl control = (SelectedGoodsControl)mainWindow.stkSelectedGoods.Children[i];
                int tmp = int.Parse(control.nmsQuantity.Text.ToString());
                Quantity += tmp;
            }
        }
        void ChangeQuantity(SelectedGoodsControl control)
        {
            OnChangeQuantity();
            total -= ConvertToNumber(control.txbTotalPrice.Text);
            control.txbTotalPrice.Text = SeparateThousands((ConvertToNumber(control.txbPrice.Text) * control.nmsQuantity.Value).ToString());
            total += ConvertToNumber(control.txbTotalPrice.Text);
            Total = total.ToString();
        }
        void DeleteSelectedGoods(SelectedGoodsControl control)
        {
            total -= ConvertToNumber(control.txbTotalPrice.Text);
            Total = total.ToString();
            mainWindow.stkSelectedGoods.Children.Remove(control);
            OnChangeQuantity();
        }

        void SelectCustomer(MainWindow window)
        {
            PickCustomerWindow pickCustomerWindow = new PickCustomerWindow();
            pickCustomerWindow.ShowDialog();
            IdCustomer = pickCustomerWindow.txbId.Text;
            CustomerName = pickCustomerWindow.txbName.Text;
            CustomerAddress = pickCustomerWindow.txbAddress.Text;
            CustomerPhoneNumber = pickCustomerWindow.txbPhoneNumber.Text;
            CustomerClass = pickCustomerWindow.txbRank.Text;
            if (!string.IsNullOrEmpty(pickCustomerWindow.txbSpending.Text))
            {
                CustomerSpending = ConvertToNumber(pickCustomerWindow.txbSpending.Text);
            }
        }

        private void CompletePayment(MainWindow window)
        {
            if (string.IsNullOrEmpty(IdCustomer))
            {
                CustomMessageBox.Show("Vui lòng nhập thông tin khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (mainWindow.stkSelectedGoods.Children.Count == 0)
            {
                CustomMessageBox.Show("Vui lòng chọn sản phẩm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Bill bill = new Bill(ConvertToID(window.txbIdBillSale.Text), CurrentAccount.IdAccount,
                DateTime.Parse(InvoiceDate), total, ConvertToID(IdCustomer),
                window.txbSaleNote.Text);
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
                BillInfo billInfo = new BillInfo(bill.IdBill, ConvertToID(control.txbId.Text), quantity, ConvertToNumber(control.txbPrice.Text));
                BillInfoDAL.Instance.Insert(billInfo);
                GoodsDAL.Instance.UpdateQuantity(ConvertToID(control.txbId.Text), -quantity);
            }

            if (isSuccess)
            {
                var result = CustomMessageBox.Show("Thanh toán thành công! Bạn có muốn in hóa đơn?",
                    "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    PrintPayment(mainWindow);
                }
            }
            else
            {
                CustomMessageBox.Show("Thanh toán thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UpdateMembership(ConvertToID(IdCustomer), CustomerSpending + total);
            //CustomerDAL.Instance.UpdateTotalSpending(ConvertToID(IdCustomer), total);
            CustomerViewModel customerVM = (CustomerViewModel)mainWindow.grdCustomer.DataContext;
            customerVM.LoadCustomerToView(mainWindow, 0);

            BillViewModel billVM = (BillViewModel)mainWindow.grdBill.DataContext;
            billVM.LoadBill(mainWindow);

            mainWindow.stkSelectedGoods.Children.Clear();
            LoadDefault(mainWindow);
            Search(mainWindow);
        }
        private void PrintPayment(MainWindow window)
        {
            BillTemplate billTemplate = new BillTemplate();

            billTemplate.txbInvoiceDate.Text = InvoiceDate;
            billTemplate.txbIdBill.Text = IdBill;
            billTemplate.txbCustomerName.Text = CustomerName;
            billTemplate.txbCustomerPhoneNumber.Text = CustomerPhoneNumber;
            billTemplate.txbCustomerAddress.Text = CustomerAddress;
            billTemplate.txbTotal.Text = Total.ToString();

            List<Parameter> parameters = ParameterDAL.Instance.GetData();
            billTemplate.txbStoreName.Text = parameters[1].Value;
            billTemplate.txbStoreAddress.Text = parameters[2].Value;
            billTemplate.txbStorePhoneNumber.Text = parameters[3].Value;

            if (string.IsNullOrEmpty(window.txbSaleNote.Text))
            {
                billTemplate.stkNote.Visibility = Visibility.Hidden;
            }
            else
            {
                billTemplate.txbNote.Text = window.txbSaleNote.Text;
            }

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
                    printDialog.PrintVisual(billTemplate.grdPrint, window.txbIdBillSale.Text);
                }
            }
            catch
            {

            }
        }

        public void Search(MainWindow window)
        {
            string namesearching = mainWindow.txtSearchSaleGoods.Text.ToLower();
            saleGoodsList = GoodsDAL.Instance.FindByName(namesearching);
            LoadSaleGoods(mainWindow);
        }
        void PickGoods(SaleGoodsControl control)
        {
            if (int.Parse(control.txbQuantity.Text) == 0)
            {
                CustomMessageBox.Show("Đã hết hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            SelectedGoodsControl selectedControl = new SelectedGoodsControl();

            selectedControl.txbId.Text = AddPrefix("SP", int.Parse(control.txbId.Text));
            selectedControl.txbName.Text = control.txbName.Text;
            selectedControl.txbType.Text = control.txbType.Text;
            selectedControl.txbUnit.Text = control.txbUnit.Text;
            selectedControl.txbPrice.Text = control.txbPrice.Text;
            selectedControl.txbTotalPrice.Text =
                SeparateThousands((decimal.Parse(control.txbPrice.Text) * selectedControl.nmsQuantity.Value).ToString());
            selectedControl.nmsQuantity.MaxValue = decimal.Parse(control.txbQuantity.Text);

            int numOfItems = mainWindow.stkSelectedGoods.Children.Count;
            for (int i = 0; i < numOfItems; i++)
            {
                SelectedGoodsControl temp = (SelectedGoodsControl)mainWindow.stkSelectedGoods.Children[i];
                if (selectedControl.txbId.Text == temp.txbId.Text)
                {
                    if (int.Parse(control.txbQuantity.Text) == temp.nmsQuantity.Value)
                    {
                        CustomMessageBox.Show("Đã hết hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    temp.nmsQuantity.Value++;
                    return;
                }
            }
            total += ConvertToNumber(selectedControl.txbPrice.Text);
            Total = total.ToString();
            mainWindow.stkSelectedGoods.Children.Add(selectedControl);
            OnChangeQuantity();
        }
        public void LoadDefault(MainWindow window)
        {
            int maxId = BillDAL.Instance.GetMaxId();
            IdBill = AddPrefix("HD", maxId + 1);
            InvoiceDate = DateTime.Now.ToShortDateString();
            Quantity = 0;
            Total = "0";
            IdCustomer = "";
            CustomerName = "";
            CustomerPhoneNumber = "";
            CustomerClass = "";
            CustomerAddress = "";
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
                    GoodsType type = GoodsTypeDAL.Instance.GetById(saleGoodsList[i].IdGoodsType);
                    double profitPercentage = type.ProfitPercentage;
                    control.txbPrice.Text = SeparateThousands(Math.Ceiling(saleGoodsList[i].ImportPrice * (1 + profitPercentage)).ToString());
                    control.txbType.Text = type.Name;
                    control.txbUnit.Text = type.Unit;

                    mainWindow.wrpSaleGoods.Children.Add(control);
                }
            }
        }
    }
}
