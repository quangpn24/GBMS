using ClosedXML.Excel;
using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    public class ServiceViewModel : BaseViewModel
    {
        private int currentPage = 0;
        private string oldName;
        private MainWindow mainWindow;
        private ServiceControl selectedUCService;
        List<Service> services = ServiceDAL.Instance.ConvertDBToList();

        //UC Service
        public ICommand OpenUpdateWindowCommand { get; set; }
        public ICommand DeleteServiceCommand { get; set; }

        //Grid Service in mainWindow
        public ICommand LoadServiceCommand { get; set; }
        public ICommand OpenAddServiceWinDowCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand GoToNextPageCommand { get; set; }
        public ICommand GoToPreviousPageCommand { get; set; }
        public ICommand FindServiceCommand { get; set; }
        public ICommand RestoreServiceCommand { get; set; }
        public ICommand FilterServiceCommand { get; set; }
        public ICommand SortServiceCommand { get; set; }
        //AddService Window
        public ICommand AddServiceCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ServiceViewModel()
        {
            //UC Service  - AddService Window
            OpenUpdateWindowCommand = new RelayCommand<ServiceControl>((parameter) => true, (parameter) => OpenUpdateWindow(parameter));
            DeleteServiceCommand = new RelayCommand<ServiceControl>((parameter) => true, (parameter) => DeleteService(parameter));          
            AddServiceCommand = new RelayCommand<AddServiceWindow>((parameter) => true, (parameter) => AddService(parameter));
            ExitCommand = new RelayCommand<AddServiceWindow>((parameter) => true, (parameter) => parameter.Close());
            //Grid Service in MainWindow
            LoadServiceCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => LoadServices(parameter, 0));
            OpenAddServiceWinDowCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => OpenAddServiceWinDow(parameter));
            ExportExcelCommand = new RelayCommand<Window>((parameter) => true, (parameter) => ExportExcel());
            GoToNextPageCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => GoToNextPage(parameter, ++currentPage));
            GoToPreviousPageCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => LoadServices(parameter, --currentPage));
            FindServiceCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => FindService(parameter));
            RestoreServiceCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => RestoreService(parameter));
            FilterServiceCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => FilterService(parameter));
            SortServiceCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => SortService(parameter));         
        }
        //UC Service  - AddService Window
        public void OpenUpdateWindow(ServiceControl uCService)
        {
            
            selectedUCService = uCService;
            AddServiceWindow addService = new AddServiceWindow();
            addService.txtIdService.Text = uCService.txbSerial.Text;
            oldName=addService.txtNameOfService.Text = uCService.txbName.Text;
            addService.txtPriceOfService.Text = uCService.txbPrice.Text;
            addService.cboStatus.SelectedIndex = uCService.txbStatus.Text == "Đang hoạt động" ? 0 : 1;
            addService.ShowDialog();
        }
        public void AddService(AddServiceWindow addServiceWindow)
        {
            
            if (CheckData(addServiceWindow))
            {
                
                if (int.Parse(addServiceWindow.txtIdService.Text) > ServiceDAL.Instance.FindMaxId())
                {
                    if (!ServiceDAL.Instance.isExist(addServiceWindow.txtNameOfService.Text))
                    {
                        Service service = new Service(int.Parse(addServiceWindow.txtIdService.Text), addServiceWindow.txtNameOfService.Text, long.Parse(addServiceWindow.txtPriceOfService.Text), 0, addServiceWindow.cboStatus.SelectedIndex, 0);
                        if (ServiceDAL.Instance.AddService(service))
                        {
                            MessageBox.Show("Thành công!");
                            addServiceWindow.Close();
                            services.Add(service);
                            ServiceControl uCService = new ServiceControl();
                            uCService.txbSerial.Text = service.IdService.ToString();
                            uCService.txbName.Text = service.Name;
                            uCService.txbPrice.Text = service.Price.ToString();
                            uCService.txbStatus.Text = service.IsActived == 0 ? "Đang hoạt động" : "Dừng hoạt động";
                            uCService.txbHiredNumber.Text = service.NumberOfHired.ToString();
                            if (currentPage == (services.Count - 1) / 10)
                                mainWindow.stkService.Children.Add(uCService);
                        }
                        else
                        {
                            MessageBox.Show("Thất bại!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tên dịch vụ đã tồn tại!");
                    }    
                }
                else
                {
                    if(!(ServiceDAL.Instance.isExist(addServiceWindow.txtNameOfService.Text)&&oldName!= addServiceWindow.txtNameOfService.Text))
                    {
                        Service service = new Service(int.Parse(addServiceWindow.txtIdService.Text), addServiceWindow.txtNameOfService.Text, long.Parse(addServiceWindow.txtPriceOfService.Text), 0, addServiceWindow.cboStatus.SelectedIndex, 0);
                        if (ServiceDAL.Instance.UpdateService(service))
                        {
                            //MessageBox.Show("Thành công!");
                            addServiceWindow.Close();
                            selectedUCService.txbName.Text = service.Name;
                            selectedUCService.txbPrice.Text = service.Price.ToString();
                            selectedUCService.txbStatus.Text = service.IsActived == 0 ? "Đang hoạt động" : "Dừng hoạt động";
                            selectedUCService.txbHiredNumber.Text = service.NumberOfHired.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Thất bại!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tên dịch vụ đã tồn tại!");
                    }
                }
            }
            int start = 0, end = 0;
            LoadInfoOfPage(ref start, ref end);
        }
        public void DeleteService(ServiceControl uCService)
        {
            if (MessageBox.Show("Xác nhận xóa dịch vụ?", "Thông báo?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (ServiceDAL.Instance.DeleteService(uCService.txbSerial.Text))
                {
                    //MessageBox.Show("Xóa thành công!");
                    mainWindow.stkService.Children.Remove(uCService);
                    services.RemoveAll(x => x.IdService == int.Parse(uCService.txbSerial.Text));
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!");
                }
            }
            int start = 0, end = 0;
            LoadInfoOfPage(ref start, ref end);
        }
        public bool CheckData(AddServiceWindow addServiceWindow)
        {
            if (string.IsNullOrEmpty(addServiceWindow.txtNameOfService.Text))
            {
                MessageBox.Show("Vui lòng nhập tên dịch vụ!");
                return false;
            }
            if (string.IsNullOrEmpty(addServiceWindow.txtPriceOfService.Text))
            {
                MessageBox.Show("Vui lòng nhập giá dịch vụ!");
                return false;
            }
            return true;
        }
        //Grid Service in MainWindow
        public void LoadServices(MainWindow mainWindow, int currentPage)
        {
            this.mainWindow = mainWindow;
            mainWindow.stkService.Children.Clear();
            int start = 0, end = 0;
            LoadInfoOfPage(ref start, ref end);
            for (int i = start; i < end; i++)
            {
                ServiceControl uCService = new ServiceControl();
                uCService.txbSerial.Text = services[i].IdService.ToString();
                uCService.txbName.Text = services[i].Name;
                uCService.txbPrice.Text = services[i].Price.ToString();
                uCService.txbStatus.Text = services[i].IsActived == 0 ? "Đang hoạt động" : "Dừng hoạt động";
                uCService.txbHiredNumber.Text = services[i].NumberOfHired.ToString();
                mainWindow.stkService.Children.Add(uCService);
            }

        }
        public void GoToNextPage(MainWindow mainWindow, int currentPage)
        {
            LoadServices(mainWindow, currentPage);
        }
        public void GoToPreviousPage(MainWindow mainWindow, int currentPage)
        {
            LoadServices(mainWindow, currentPage);
        }
        public void FindService(MainWindow mainWindow)
        {
            services = ServiceDAL.Instance.FindServiceByName(mainWindow.txtSearchService.Text);
            currentPage = 0;
            LoadServices(mainWindow, currentPage);
            mainWindow.cboSelectFilter.SelectedIndex = -1;
            mainWindow.cboSelectSort.SelectedIndex = -1;
        }
        public void OpenAddServiceWinDow(MainWindow mainWindow)
        {
            AddServiceWindow addServiceWindow = new AddServiceWindow();
            addServiceWindow.txtIdService.Text = (ServiceDAL.Instance.FindMaxId() + 1).ToString();
            addServiceWindow.ShowDialog();
        }
        public void ExportExcel()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(ServiceDAL.Instance.GetServices(), "Services");
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                MessageBox.Show("Xuất dữ liệu thành công!");
            }
        }
        public void RestoreService(MainWindow mainWindow)
        {
            if (ServiceDAL.Instance.RestoreData())
            {
                MessageBox.Show("Khôi phục dữ liệu thành công!");
                services = ServiceDAL.Instance.ConvertDBToList();
                LoadServices(mainWindow, 0);
            }
            else
            {
                MessageBox.Show("Khôi phục dữ liệu thất bại!");
            }
        }
        public void LoadInfoOfPage(ref int start, ref int end)
        {
            mainWindow.btnPreviousPage.IsEnabled = currentPage == 0 ? false : true;
            mainWindow.btnNextPage.IsEnabled = currentPage == (services.Count - 1) / 10 ? false : true;

            start = currentPage * 10;
            end = (currentPage + 1) * 10;
            if (currentPage == services.Count / 10)
                end = services.Count;

            mainWindow.txtNumberOfItems.Text = String.Format("{0} - {1} of {2} items", start == end ? 0 : start + 1, end, services.Count);
        }
        public void FilterService(MainWindow mainWindow)
        {
            List<Service> temp = new List<Service>(services);
            switch (mainWindow.cboSelectFilter.SelectedIndex)
            {
                case 0:
                    services.RemoveAll(x => x.IsActived == 1);
                    break;
                case 1:
                    services.RemoveAll(x => x.IsActived == 0);
                    break;
            }

            LoadServices(mainWindow, 0);
            services = temp;
        }
        public void SortService(MainWindow mainWindow)
        {
            switch (mainWindow.cboSelectSort.SelectedIndex)
            {
                case 0:
                    services = services.OrderBy(x => x.Name).ToList();
                    break;
                case 1:
                    services = services.OrderBy(x => x.Price).ToList();
                    break;
                case 2:
                    services = services.OrderBy(x => x.NumberOfHired).ToList();
                    break;
            }

            LoadServices(mainWindow, 0);
        }
    }
}
