using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GemstonesBusinessManagementSystem.Views;
using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class GoodsTypeViewModel : BaseViewModel
    {
        private bool isUpdate = false;
        private bool isActiveTab = true;
        private string oldType;
        private string profitPercentage;
        private GoodsTypeWindow wdGoodsType;
        private GoodsTypeControl goodsTypeControl;
        Binding newBinding;
        public ICommand LoadCommand { get; set; }
        public ICommand SelectionChangedTabItemCommand { get; set; }
        public ICommand SelectedGoodsTypeCommand { get; set; }
        public ICommand InactivateCommand { get; set; }
        public ICommand ActivateCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public string ProfitPercentage { get => profitPercentage; set => profitPercentage = value; }

        public GoodsTypeViewModel()
        {
            LoadCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => Init(p));
            SelectionChangedTabItemCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => SelectedTabItem(p));
            SelectedGoodsTypeCommand = new RelayCommand<GoodsTypeControl>(p => true, p => SelectGoodsType(p));
            InactivateCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => Inactivate(p));
            ActivateCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => Activate(p));
            CancelCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => Cancel(p));
            SaveCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => AddOrUpdate(p));
        }
        void AddOrUpdate(GoodsTypeWindow wdGoodsType)
        {
            if (string.IsNullOrEmpty(wdGoodsType.txtName.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập tên loại sản phẩm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                wdGoodsType.txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(wdGoodsType.txtUnit.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập đơn vị tính!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                wdGoodsType.txtUnit.Focus();
                return;
            }
            if (string.IsNullOrEmpty(wdGoodsType.txtProfitPercentage.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập phần trăm lợi nhuận!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                wdGoodsType.txtProfitPercentage.Focus();
                return;
            }

            if ((!isUpdate || wdGoodsType.txtName.Text != oldType) && GoodsTypeDAL.Instance.IsExisted(wdGoodsType.txtName.Text))
            {
                CustomMessageBox.Show("Loại sản phẩm đã tồn tại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                wdGoodsType.txtName.Focus();
                return;
            }
            GoodsType type = new GoodsType(ConvertToID(wdGoodsType.txtId.Text), wdGoodsType.txtName.Text,
                int.Parse(wdGoodsType.txtProfitPercentage.Text) / 100.0, wdGoodsType.txtUnit.Text, true);
            GoodsTypeDAL.Instance.InsertOrUpdate(type, isUpdate);
            GoodsTypeControl control = new GoodsTypeControl();
            if (isUpdate)
            {
                control = goodsTypeControl;
            }
            control.txbId.Text = wdGoodsType.txtId.Text;
            control.txbName.Text = wdGoodsType.txtName.Text;
            control.txbProfitPercentage.Text = wdGoodsType.txtProfitPercentage.Text + "%";
            control.txbUnit.Text = wdGoodsType.txtUnit.Text;
            if (!isUpdate)
            {
                wdGoodsType.stkActive_ActiveTab.Children.Add(control);
                Cancel(wdGoodsType);
            }
        }
        void Cancel(GoodsTypeWindow wdGoodsType)
        {
            this.wdGoodsType.txbTitle.Text = "Thêm loại sản phẩm";
            this.wdGoodsType.btnSave.Content = "Thêm";
            int idMax = GoodsTypeDAL.Instance.GetMaxId();
            if (idMax == -1)
            {
                CustomMessageBox.Show("Lỗi hệ thống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (newBinding != null)
            {
                wdGoodsType.txtName.SetBinding(TextBox.TextProperty, newBinding);
            }
            this.wdGoodsType.txtId.Text = AddPrefix("LS", idMax + 1);
            this.wdGoodsType.txtName.Text = null;
            this.wdGoodsType.txtProfitPercentage.Text = null;
            this.wdGoodsType.txtUnit.Text = null;
            isUpdate = false;
        }
        void Inactivate(GoodsTypeWindow wdGoodsType)
        {
            if (!GoodsTypeDAL.Instance.IsActive(ConvertToID(goodsTypeControl.txbId.Text)))
            {
                CustomMessageBox.Show("Loại sản phẩm này đã ngừng hoạt động!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var result = CustomMessageBox.Show("Tất cả các sản phẩm có loại này sẽ ngừng hoạt động! Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if (GoodsTypeDAL.Instance.InactivateOrReactivate(ConvertToID(goodsTypeControl.txbId.Text), false))
                {
                    GoodsDAL.Instance.InactivateOrReActivate(ConvertToID(goodsTypeControl.txbId.Text), false);
                    this.wdGoodsType.stkActive_InactiveTab.Children.Remove(goodsTypeControl);
                    this.wdGoodsType.stkInactive_InactiveTab.Children.Add(goodsTypeControl);
                }
            }
        }
        void Activate(GoodsTypeWindow wdGoodsType)
        {
            if (GoodsTypeDAL.Instance.IsActive(ConvertToID(goodsTypeControl.txbId.Text)))
            {
                CustomMessageBox.Show("Loại sản phẩm này đang hoạt động!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (GoodsTypeDAL.Instance.InactivateOrReactivate(ConvertToID(goodsTypeControl.txbId.Text), true))
            {
                GoodsDAL.Instance.InactivateOrReActivate(ConvertToID(goodsTypeControl.txbId.Text), true);
                this.wdGoodsType.stkInactive_InactiveTab.Children.Remove(goodsTypeControl);
                this.wdGoodsType.stkActive_InactiveTab.Children.Add(goodsTypeControl);
            }
        }
        void Init(GoodsTypeWindow wdGoodsType)
        {
            this.wdGoodsType = wdGoodsType;
            int idMax = GoodsTypeDAL.Instance.GetMaxId();
            if (idMax == -1)
            {
                CustomMessageBox.Show("Lỗi hệ thống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            this.wdGoodsType.txtId.Text = AddPrefix("LS", idMax + 1);
            LoadActiveTab(wdGoodsType);
        }
        void SelectGoodsType(GoodsTypeControl control)
        {
            if (goodsTypeControl != null) // dua lai mau xam
            {
                goodsTypeControl.txbId.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                goodsTypeControl.txbName.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                goodsTypeControl.txbProfitPercentage.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
                goodsTypeControl.txbUnit.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF4F4F4F");
            }
            // chuyen sang mau duoc chon
            control.txbId.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF00329E");   
            control.txbName.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF00329E");
            control.txbProfitPercentage.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF00329E");
            control.txbUnit.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF00329E");
            if (isActiveTab) //  to edit
            {
                Binding binding = BindingOperations.GetBinding(this.wdGoodsType.txtName, TextBox.TextProperty);
                if (binding != null)
                {
                    newBinding = CloneBinding(binding as BindingBase, binding.Source) as Binding;
                }

                BindingOperations.ClearBinding(this.wdGoodsType.txtName, TextBox.TextProperty);
                isUpdate = true;
                this.goodsTypeControl = control;
                oldType = goodsTypeControl.txbName.Text;
                this.wdGoodsType.txbTitle.Text = "Sửa thông tin";
                this.wdGoodsType.btnSave.Content = "Cập nhật";
                this.wdGoodsType.txtId.Text = control.txbId.Text;
                this.wdGoodsType.txtName.Text = control.txbName.Text;
                this.wdGoodsType.txtProfitPercentage.Text = control.txbProfitPercentage.Text.Remove(control.txbProfitPercentage.Text.Length - 1, 1);
                this.wdGoodsType.txtUnit.Text = control.txbUnit.Text;

            }
            else // to inactivate or activate
            {
                isUpdate = false;
                this.goodsTypeControl = control;
            }

          
        }
        void SelectedTabItem(GoodsTypeWindow wdGoodsType)
        {
            if (wdGoodsType.tabControl.SelectedIndex == 0)
            {
                isActiveTab = true;
                LoadActiveTab(wdGoodsType);
            }
            else
            {
                isActiveTab = false;
                LoadInactiveTab(wdGoodsType);
            }
        }
        void LoadActiveTab(GoodsTypeWindow wdGoodsType)
        {
            wdGoodsType.stkActive_ActiveTab.Children.Clear();
            DataTable dt = GoodsTypeDAL.Instance.GetActive();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GoodsTypeControl control = new GoodsTypeControl();
                control.txbId.Text = AddPrefix("LS", int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                control.txbName.Text = dt.Rows[i].ItemArray[1].ToString();
                control.txbProfitPercentage.Text = (double.Parse(dt.Rows[i].ItemArray[2].ToString()) * 100).ToString() + "%";
                control.txbUnit.Text = dt.Rows[i].ItemArray[3].ToString();
                wdGoodsType.stkActive_ActiveTab.Children.Add(control);
            }
        }
        void LoadInactiveTab(GoodsTypeWindow wdGoodsType)
        {
            wdGoodsType.stkActive_InactiveTab.Children.Clear();
            wdGoodsType.stkInactive_InactiveTab.Children.Clear();
            DataTable dt = GoodsTypeDAL.Instance.GetAll();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GoodsTypeControl control = new GoodsTypeControl();
                control.txbId.Text = AddPrefix("LS", int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                control.txbName.Text = dt.Rows[i].ItemArray[1].ToString();
                control.txbProfitPercentage.Text = (double.Parse(dt.Rows[i].ItemArray[2].ToString()) * 100).ToString() + " % ";
                control.txbUnit.Text = dt.Rows[i].ItemArray[3].ToString();
                if (bool.Parse(dt.Rows[i].ItemArray[4].ToString()))
                {
                    wdGoodsType.stkActive_InactiveTab.Children.Add(control);
                }
                else
                {
                    wdGoodsType.stkInactive_InactiveTab.Children.Add(control);
                }
            }
        }
    }
}
