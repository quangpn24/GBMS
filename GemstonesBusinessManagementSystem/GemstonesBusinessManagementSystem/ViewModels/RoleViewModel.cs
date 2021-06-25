using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
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
using System.Windows.Media;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class RoleViewModel : BaseViewModel
    {
        public ICommand LoadCommand { get; set; }
        public ICommand ViewPermissionCommand { get; set; }
        public ICommand UpdatePermissionCommand { get; set; }
        public ICommand CheckAllCommand { get; set; }

        private List<EmployeePosition> positionList = EmployeePositionDAL.Instance.GetList();
        private MainWindow mainWindow;
        private int selectedPosition = -1;

        public RoleViewModel()
        {
            LoadCommand = new RelayCommand<MainWindow>((p) => true, (p) => LoadEmployeePosition(p));
            ViewPermissionCommand = new RelayCommand<RoleControl>((p) => true, (p) => InitPermission(p));
            UpdatePermissionCommand = new RelayCommand<Button>((p) => true, (p) => { UpdatePermission(p); SetRole(); });
            CheckAllCommand = new RelayCommand<MainWindow>((p) => true, (p) => CheckAll(p));
        }
        void CheckAll(MainWindow window)
        {
            int n = mainWindow.stkPermission.Children.Count;
            if (n == 0)
            {
                return;
            }
            bool isChecked = (bool)window.cbCheckAll.IsChecked;
            for (int i = 0; i < n; i++)
            {
                PermissionControl control = (PermissionControl)(mainWindow.stkPermission.Children[i]);
                control.cbIsPermitted.IsChecked = isChecked;
            }
        }
        void SetRole()
        {
            if (CurrentAccount.Type == 0)
            {
                return;
            }
            List<PositionDetail> positionDetails =
                PositionDetailDAL.Instance.GetListByPosition(CurrentAccount.IdPosition);

            mainWindow.btnHome.IsEnabled = positionDetails[0].IsPermitted;

            mainWindow.btnStore.IsEnabled = positionDetails[1].IsPermitted;
            mainWindow.btnService.IsEnabled = positionDetails[2].IsPermitted;
            if (positionDetails[1].IsPermitted || positionDetails[2].IsPermitted)
            {
                mainWindow.expStore.IsEnabled = true;
                mainWindow.expStore.Opacity = 1;
            }
            else
            {
                mainWindow.expStore.IsEnabled = false;
                mainWindow.expStore.Opacity = 0.5;
            }

            mainWindow.btnStock.IsEnabled = positionDetails[3].IsPermitted;
            mainWindow.btnImport.IsEnabled = positionDetails[4].IsPermitted;
            if (positionDetails[3].IsPermitted || positionDetails[4].IsPermitted)
            {
                mainWindow.expWarehouse.IsEnabled = true;
                mainWindow.expWarehouse.Opacity = 1;
            }
            else
            {
                mainWindow.expWarehouse.IsEnabled = false;
                mainWindow.expWarehouse.Opacity = 0.5;
            }

            mainWindow.btnSupplier.IsEnabled = positionDetails[5].IsPermitted;
            mainWindow.btnCustomer.IsEnabled = positionDetails[6].IsPermitted;
            if (positionDetails[5].IsPermitted || positionDetails[6].IsPermitted)
            {
                mainWindow.expPartner.IsEnabled = true;
                mainWindow.expPartner.Opacity = 1;
            }
            else
            {
                mainWindow.expPartner.IsEnabled = false;
                mainWindow.expPartner.Opacity = 0.5;
            }

            mainWindow.btnEmployee.IsEnabled = positionDetails[7].IsPermitted;
            mainWindow.btnGoods.IsEnabled = positionDetails[8].IsPermitted;
            mainWindow.btnServiceM.IsEnabled = positionDetails[9].IsPermitted;
            if (positionDetails[7].IsPermitted || positionDetails[8].IsPermitted || positionDetails[9].IsPermitted)
            {
                mainWindow.expManage.IsEnabled = true;
                mainWindow.expManage.Opacity = 1;
            }
            else
            {
                mainWindow.expManage.IsEnabled = false;
                mainWindow.expManage.Opacity = 0.5;
            }

            mainWindow.btnBillService.IsEnabled = positionDetails[10].IsPermitted;
            mainWindow.btnRevenue.IsEnabled = positionDetails[11].IsPermitted;
            if (positionDetails[10].IsPermitted || positionDetails[11].IsPermitted)
            {
                mainWindow.expReport.IsEnabled = true;
                mainWindow.expReport.Opacity = 1;
            }
            else
            {
                mainWindow.expReport.IsEnabled = false;
                mainWindow.expReport.Opacity = 0.5;
            }

            mainWindow.btnSetting.IsEnabled = positionDetails[12].IsPermitted;
            if (!positionDetails[12].IsPermitted)
            {
                mainWindow.grdSetting.Visibility = Visibility.Collapsed;
                string fore = "#666666";
                string back = "#FFFFFF";
                mainWindow.btnSetting.Foreground = (Brush)new BrushConverter().ConvertFrom(fore);
                mainWindow.btnSetting.Background = (Brush)new BrushConverter().ConvertFrom(back);
            }
        }
        void UpdatePermission(Button btn)
        {
            int success = 0;
            int n = mainWindow.stkPermission.Children.Count;
            for (int i = 0; i < n; i++)
            {
                PermissionControl control = (PermissionControl)(mainWindow.stkPermission.Children[i]);
                PositionDetail positionDetail = new PositionDetail(selectedPosition,
                    int.Parse(control.txbId.Text), (bool)control.cbIsPermitted.IsChecked);
                bool tmp = PositionDetailDAL.Instance.InsertOrUpdate(positionDetail, true);
                success += tmp ? 1 : 0;
            }
            mainWindow.stkPermission.Children.Clear();
            btn.IsEnabled = false;
            mainWindow.cbCheckAll.IsChecked = false;
            mainWindow.cbCheckAll.IsEnabled = false;
            if (success == n)
            {
                CustomMessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                CustomMessageBox.Show("Cập nhật không thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        void InitPermission(RoleControl control)
        {
            mainWindow.cbCheckAll.IsEnabled = true;
            mainWindow.btnUpdatePermission.IsEnabled = true;
            mainWindow.stkPermission.Children.Clear();
            selectedPosition = ConvertToID(control.txbId.Text);
            List<Permission> permissionList = PermissionDAL.Instance.GetList();
            List<PositionDetail> positionDetailList = PositionDetailDAL.Instance.GetListByPosition(selectedPosition);
            int n = permissionList.Count;
            for (int i = 0; i < n; i++)
            {
                PermissionControl permissionControl = new PermissionControl();
                permissionControl.txbId.Text = permissionList[i].IdPermission.ToString();
                permissionControl.txbRoleName.Text = permissionList[i].PermissionName;
                permissionControl.cbIsPermitted.IsChecked = positionDetailList[i].IsPermitted;
                mainWindow.stkPermission.Children.Add(permissionControl);
            }
        }
        void LoadEmployeePosition(MainWindow window)
        {
            this.mainWindow = window;
            mainWindow.stkPosition.Children.Clear();
            int n = positionList.Count;
            for (int i = 0; i < n; i++)
            {
                RoleControl roleControl = new RoleControl();
                roleControl.txbId.Text = AddPrefix("CV", positionList[i].IdEmployeePosition);
                roleControl.txbRoleName.Text = positionList[i].Position;
                mainWindow.stkPosition.Children.Add(roleControl);
            }
        }
    }
}
