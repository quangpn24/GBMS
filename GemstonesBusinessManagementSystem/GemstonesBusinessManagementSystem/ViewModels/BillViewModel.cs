﻿using ClosedXML.Excel;
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
using System.Windows.Media.Imaging;
using System.Data;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class BillViewModel : BaseViewModel
    {
        private MainWindow main;
        private InvoiceControl checkedItem;
        public ICommand LoadBillCommand { get; set; }
        public ICommand PickBillCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public BillViewModel()
        {
            LoadBillCommand = new RelayCommand<MainWindow>(p => true, p => LoadBill(p));
            PickBillCommand = new RelayCommand<InvoiceControl>(p => true, p => PickBill(p));
            DeleteCommand = new RelayCommand<InvoiceControl>(p => true, p => Delete(p));
        }

        public void LoadBill(MainWindow mainWindow)
        {
            this.main = mainWindow;
            if (mainWindow.dpStartDateBill.SelectedDate == null || mainWindow.dpStartDateBill.SelectedDate == null)
            {
                return;
            }
            if (mainWindow.dpStartDateBill.SelectedDate > mainWindow.dpStartDateBill.SelectedDate)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu nhỏ hơn ngày kết thúc!");
                return;
            }
            main.stkBill.Children.Clear();
            List<Bill> bills = BillDAL.Instance.GetByDate((DateTime)mainWindow.dpStartDateBill.SelectedDate, (DateTime)mainWindow.dpEndDateBill.SelectedDate);
            for(int i = 0; i < bills.Count; i++)
            {
                Customer customer = CustomerDAL.Instance.FindById(bills[i].IdCustomer.ToString());
                Employee employee = EmployeeDAL.Instance.GetById(bills[i].IdAccount.ToString());
                InvoiceControl invoiceControl = new InvoiceControl();
                invoiceControl.txbSerial.Text = AddPrefix("HD", bills[i].IdBill);
                invoiceControl.txbNameCustomer.Text = customer.CustomerName;
                invoiceControl.txbNameEmployee.Text = employee.Name;
                invoiceControl.txbPrice.Text = bills[i].TotalPrice.ToString();
                invoiceControl.IsHitTestVisible = true;
                mainWindow.stkBill.Children.Add(invoiceControl);
            }
        }
        public void PickBill(InvoiceControl invoiceControl)
        {
            Bill bill = BillDAL.Instance.GetBill(ConvertToIDString(invoiceControl.txbSerial.Text));
            Customer customer = CustomerDAL.Instance.FindById(bill.IdCustomer.ToString());
            List<BillInfo> billInfos = BillInfoDAL.Instance.GetBillInfos(bill.IdBill.ToString());
            if (checkedItem != null) // dua lai mau xam
            {
                checkedItem.txbSerial.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbNameCustomer.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbNameEmployee.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                checkedItem.txbPrice.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
            }
            // chuyen sang mau duoc chon
            invoiceControl.txbSerial.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            invoiceControl.txbNameCustomer.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            invoiceControl.txbNameEmployee.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            invoiceControl.txbPrice.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF00329E");
            checkedItem = invoiceControl;
            // hien thi thong tin
            main.txbIdBill.Text = invoiceControl.txbSerial.Text;
            main.txbCreateDateBill.Text = bill.BillDate.ToShortDateString();
            main.txbNameCustomerBill.Text = customer.CustomerName;
            main.txbPhoneCustomerBill.Text = customer.PhoneNumber;
            main.txbTotalBill.Text = bill.TotalPrice.ToString();
            main.stkBillInfo.Children.Clear();
            //hien thi list billinfo
            for (int i = 0; i < billInfos.Count; i++)
            {
                Goods goods = GoodsDAL.Instance.FindById(billInfos[i].IdGoods.ToString());
                InfoInvoiceControl infoInvoiceControl = new InfoInvoiceControl();
                infoInvoiceControl.txbNumber.Text = (i + 1).ToString();
                infoInvoiceControl.txbNameGoods.Text = goods.Name;
                infoInvoiceControl.txbPrice.Text = billInfos[i].Price.ToString();
                infoInvoiceControl.txbQuantity.Text = billInfos[i].Quantity.ToString();
                infoInvoiceControl.txbTotal.Text = (float.Parse(infoInvoiceControl.txbPrice.Text) * billInfos[i].Quantity).ToString();

                main.stkBillInfo.Children.Add(infoInvoiceControl);
            }
        }
        void Delete (InvoiceControl invoiceControl)
        {
            string idBill = ConvertToIDString(invoiceControl.txbSerial.Text);
            MessageBoxResult result = MessageBox.Show("Xác nhận xóa hóa đơn?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                bool isSuccess = BillDAL.Instance.Delete(idBill);
                if (isSuccess)
                {
                    main.stkBill.Children.Remove(invoiceControl);
                    LoadBill(main);
                }
                else
                {
                    MessageBox.Show("Xoá thất bại");
                }
            }
        }
    }
}