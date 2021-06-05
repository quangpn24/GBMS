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

        public BillViewModel()
        {
            LoadBillCommand = new RelayCommand<MainWindow>(p => true, p => LoadBill(p));
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
                InvoiceControl invoiceControl = new InvoiceControl();
                invoiceControl.txbSerial.Text = AddPrefix("HD", bills[i].IdBill);
                invoiceControl.txbNameCustomer.Text = customer.CustomerName;
                invoiceControl.txbNameEmployee.Text = customer.CustomerName;
                invoiceControl.txbPrice.Text = bills[i].TotalPrice.ToString();
                if(bills[i].Status1 == 1)
                {
                    invoiceControl.txbStatus.Text = "Đã thanh toán";
                    invoiceControl.txbStatus.Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FF01B500");
                }
                else
                {
                    invoiceControl.txbStatus.Text = "Chưa thanh toán";
                }
                invoiceControl.IsHitTestVisible = true;
                mainWindow.stkBill.Children.Add(invoiceControl);
            }
        }
        public void PickBill(InvoiceControl invoiceControl)
        {
            Bill bill = BillDAL.Instance.GetBill(ConvertToIDString(invoiceControl.txbSerial.Text));
            Customer customer = CustomerDAL.Instance.FindById(bill.IdCustomer.ToString());
            //List<Bill> bills = 
        }
    }
}