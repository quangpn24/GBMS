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

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class GoodsTypeViewModel : BaseViewModel
    {
        private bool isUpdate = false;
        private bool isActiveTab = true;
        private string oldType;
        private GoodsTypeWindow wdGoodsType;
        private GoodsTypeControl goodsTypeControl;
        public ICommand LoadCommand { get; set; }
        public ICommand SelectionChangedTabItemCommand { get; set; }
        public ICommand SelectedGoodsTypeCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand InactivateCommand { get; set; }
        public ICommand ActivateCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public GoodsTypeViewModel()
        {
            LoadCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => Init(p));
            SelectionChangedTabItemCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => SelectedTabItem(p));
            SelectedGoodsTypeCommand = new RelayCommand<GoodsTypeControl>(p => true, p => SelectGoodsType(p));
            DeleteCommand = new RelayCommand<GoodsTypeControl>(p => true, p => Delete(p));
            InactivateCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => Inactivate(p));
            ActivateCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => Activate(p));
            CancelCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => Cancel(p));
            SaveCommand = new RelayCommand<GoodsTypeWindow>(p => true, p => AddOrUpdate(p));
        }
        void AddOrUpdate(GoodsTypeWindow wdGoodsType)
        {
            if (string.IsNullOrEmpty(wdGoodsType.txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại sản phẩm");
                wdGoodsType.txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(wdGoodsType.txtUnit.Text))
            {
                MessageBox.Show("Vui lòng nhập đơn vị tính");
                wdGoodsType.txtUnit.Focus();
                return;
            }
            if (string.IsNullOrEmpty(wdGoodsType.txtProfitPercentage.Text))
            {
                MessageBox.Show("Vui lòng nhập phần trăm lợi nhuận");
                wdGoodsType.txtProfitPercentage.Focus();
                return;
            }
            if(GoodsTypeDAL.Instance.IsExisted(wdGoodsType.txtName.Text) && wdGoodsType.txtName.Text != oldType)
            {
                MessageBox.Show("Loại sản phẩm đã tồn tại!");
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
            }
        }
        void Cancel(GoodsTypeWindow wdGoodsType)
        {
            this.wdGoodsType.txbTitle.Text = "Thêm loại sản phẩm";
            this.wdGoodsType.btnSave.Content = "Thêm";
            this.wdGoodsType.txtId.Text = AddPrefix("LS", GoodsTypeDAL.Instance.GetMaxId() + 1);
            this.wdGoodsType.txtName.Text = "";
            this.wdGoodsType.txtProfitPercentage.Text = "";
            this.wdGoodsType.txtUnit.Text = "";
            isUpdate = false;
        }
        void Inactivate(GoodsTypeWindow wdGoodsType)
        {
            if(!GoodsTypeDAL.Instance.IsActive(ConvertToID(goodsTypeControl.txbId.Text)))
            {
                MessageBox.Show("Loại sản phẩm này đã ngừng hoạt động!", "Thông báo",MessageBoxButton.OK ,MessageBoxImage.Error);
                return;
            }
            var result = MessageBox.Show("Tất cả các sản phẩm có loại này sẽ ngừng hoạt động! Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
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
                MessageBox.Show("Loại sản phẩm này đang hoạt động!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (GoodsTypeDAL.Instance.InactivateOrReactivate(ConvertToID(goodsTypeControl.txbId.Text), true))
            {
                GoodsDAL.Instance.InactivateOrReActivate(ConvertToID(goodsTypeControl.txbId.Text), true);
                this.wdGoodsType.stkInactive_InactiveTab.Children.Remove(goodsTypeControl);
                this.wdGoodsType.stkActive_InactiveTab.Children.Add(goodsTypeControl);
            }
        }
        void Delete(GoodsTypeControl control)
        {
            var result = MessageBox.Show("Bạn có chắc muốn xóa!", "Thông báo", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                if (GoodsDAL.Instance.IsExistGoodsType(ConvertToID(control.txbId.Text)))
                {
                    MessageBox.Show("Không thể xóa vì vẫn còn sản phẩm có loại này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (GoodsTypeDAL.Instance.Delete(ConvertToID(control.txbId.Text)))
                    {
                        MessageBox.Show("Thành công!!!");
                        this.wdGoodsType.stkActive_ActiveTab.Children.Remove(control);
                    }
                    else
                    {
                        MessageBox.Show("Thành công!!!");
                    }
                }
            }
        }
        void Init(GoodsTypeWindow wdGoodsType)
        {
            this.wdGoodsType = wdGoodsType;
            wdGoodsType.txtId.Text = AddPrefix("LS", GoodsTypeDAL.Instance.GetMaxId() + 1);
            LoadActiveTab(wdGoodsType);
        }
        void SelectGoodsType(GoodsTypeControl contorl)
        {
            if (isActiveTab) //  to edit
            {
                isUpdate = true;
                this.goodsTypeControl = contorl;
                oldType = goodsTypeControl.txbName.Text;
                this.wdGoodsType.txbTitle.Text = "Sửa thông tin";
                this.wdGoodsType.btnSave.Content = "Sửa";
                this.wdGoodsType.txtId.Text = contorl.txbId.Text;
                this.wdGoodsType.txtName.Text = contorl.txbName.Text;
                this.wdGoodsType.txtProfitPercentage.Text = contorl.txbProfitPercentage.Text.Remove(2, 1);
                this.wdGoodsType.txtUnit.Text = contorl.txbUnit.Text;
            }
            else // to inactivate or activate
            {
                isUpdate = false;
                this.goodsTypeControl = contorl;
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
