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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class EmployeeViewModel : BaseViewModel
    {
        //EmployeeControl
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        //AddEmployeeWindow
        public ICommand SelectImageCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        //MainWindow
        public ICommand SearchCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand OpenAddEmployeeWindowCommand { get; set; }
        public ICommand LoadEmployeeCommand { get; set; }

        private MainWindow mainWindow;
        public MainWindow MainWindow { get => mainWindow; set => mainWindow = value; }

        public string imageName;
        public bool isEditing = false;

        public EmployeeViewModel()
        {
            //EmployeeControl
            EditCommand = new RelayCommand<EmployeeControl>((parameter) => true, (parameter) => OpenEditEmployeeWindow(parameter));
            DeleteCommand = new RelayCommand<EmployeeControl>((parameter) => true, (parameter) => DeleteEmployee(parameter));

            //AddEmployeeWindow
            SelectImageCommand = new RelayCommand<Grid>((parameter) => true, (parameter) => HandleSelectImage(parameter));
            SaveCommand = new RelayCommand<AddEmployeeWindow>((parameter) => true, (parameter) => HandleAddEmployee(parameter));
            ExitCommand = new RelayCommand<AddEmployeeWindow>((parameter) => true, (parameter) => parameter.Close());

            //MainWindow
            SearchCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => Search(parameter));
            ExportExcelCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => ExportExcel());
            OpenAddEmployeeWindowCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => OpenAddEmployeeWindow(parameter));
            LoadEmployeeCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => LoadEmployeeList(parameter));
        }

        //EmployeeControl
        public void OpenEditEmployeeWindow(EmployeeControl employeeControl)
        {
            isEditing = true;
            Employee employee = EmployeeDAL.Instance.GetByIdEmployee(employeeControl.txbId.Text);

            AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
            if (employee.IdEmployee.ToString() == employeeControl.txbId.Text)
            {
                addEmployeeWindow.txtIdEmployee.Text = employee.IdEmployee.ToString();

                addEmployeeWindow.txtName.Text = employee.Name;
                addEmployeeWindow.txtName.SelectionStart = addEmployeeWindow.txtName.Text.Length;
                addEmployeeWindow.txtName.SelectionLength = 0;

                addEmployeeWindow.txtTelephoneNumber.Text = employee.PhoneNumber;
                addEmployeeWindow.txtTelephoneNumber.SelectionStart = addEmployeeWindow.txtTelephoneNumber.Text.Length;
                addEmployeeWindow.txtTelephoneNumber.SelectionLength = 0;

                addEmployeeWindow.txtAddress.Text = employee.Address;
                addEmployeeWindow.txtAddress.SelectionStart = addEmployeeWindow.txtAddress.Text.Length;
                addEmployeeWindow.txtAddress.SelectionLength = 0;

                addEmployeeWindow.cboPosition.Text = employee.Position;

                if (employee.Gender == "Nam")
                    addEmployeeWindow.rdoMale.IsChecked = true;
                else
                    addEmployeeWindow.rdoFemale.IsChecked = true;
                addEmployeeWindow.dpBirthDate.SelectedDate = DateTime.Parse(employee.DateOfBirth.ToString());
                addEmployeeWindow.dpWorkDate.SelectedDate = DateTime.Parse(employee.StartingDate.ToString());
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(employee.ImageFile);
                addEmployeeWindow.grdSelectImage.Background = imageBrush;
                if (addEmployeeWindow.grdSelectImage.Children.Count > 1)
                {
                    addEmployeeWindow.grdSelectImage.Children.Remove(addEmployeeWindow.grdSelectImage.Children[0]);
                    addEmployeeWindow.grdSelectImage.Children.Remove(addEmployeeWindow.grdSelectImage.Children[1]);
                }
            }
            addEmployeeWindow.btnSave.ToolTip = "Cập nhật thông tin nhân viên";
            //if (CurrentAccount.Type == 1)
            //{
            //    addEmployeeWindow.cboPositionManage.IsEnabled = false;
            //}
            addEmployeeWindow.Title = "Cập nhật thông tin nhân viên";
            addEmployeeWindow.ShowDialog();
            //Employee fixedEmployee = EmployeeDAL.Instance.GetEmployeeByIdEmployee(employeeControl.txbId.Text);
            //employeeControl.txbName.Text = fixedEmployee.Name;
            //employeeControl.txbPosition.Text = fixedEmployee.Position;
        }
        void DeleteEmployee(EmployeeControl employeeControl)
        {
            MessageBoxResult result = MessageBox.Show("Xác nhận xóa nhân viên?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool isSuccess = EmployeeDAL.Instance.Delete(employeeControl.txbId.Text);
                if (isSuccess)
                {
                    MessageBox.Show("Đã xóa thành công!");
                }
                else
                {
                    MessageBox.Show("Xoá thất bại");
                }
                mainWindow.stkEmployeeList.Children.Remove(employeeControl);
            }
        }

        //AddEmployeeWindow
        public void HandleAddEmployee(AddEmployeeWindow window)
        {
            string gender = "";
            if (string.IsNullOrEmpty(window.txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!");
                window.txtName.Focus();
                return;
            }
            if (window.cboPosition.Text == "")
            {
                MessageBox.Show("Vui lòng nhập chức vụ!");
                window.cboPosition.Focus();
                return;
            }
            else
            {
                if (window.cboPosition.Text != "Bảo vệ" && window.cboPosition.Text != "Nhân viên quản lý" && window.cboPosition.Text != "Nhân viên thu ngân")
                {
                    MessageBox.Show("Vui lòng nhập đúng chức vụ!");
                    window.cboPosition.Focus();
                    return;
                }
            }
            if (window.dpBirthDate.Text == "")
            {
                MessageBox.Show("Vui lòng nhập ngày sinh!");
                window.dpBirthDate.Focus();
                return;
            }
            else
            {
                DateTime dateTime = new DateTime();
                if (!DateTime.TryParse(window.dpBirthDate.Text, out dateTime))
                {
                    MessageBox.Show("Vui lòng nhập lại ngày sinh!");
                    window.dpBirthDate.Focus();
                    return;
                }
            }
            if (window.txtAddress.Text == "")
            {
                MessageBox.Show("Vui lòng nhập địa chỉ!");
                window.txtAddress.Focus();
                return;
            }
            if (window.txtTelephoneNumber.Text == "")
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!");
                window.txtTelephoneNumber.Focus();
                return;
            }
            if (window.dpWorkDate.Text == "")
            {
                MessageBox.Show("Vui lòng nhập ngày vào làm!");
                window.dpWorkDate.Focus();
                return;
            }
            else
            {
                DateTime dateTime = new DateTime();
                if (!DateTime.TryParse(window.dpWorkDate.Text, out dateTime))
                {
                    MessageBox.Show("Vui lòng nhập lại ngày vào làm!");
                    window.dpWorkDate.Focus();
                    return;
                }
                if (dateTime < DateTime.Parse(window.dpBirthDate.Text))
                {
                    MessageBox.Show("Vui lòng nhập lại ngày vào làm lớn hơn ngày sinh!");
                    window.dpWorkDate.Focus();
                    return;
                }
            }
            if (window.rdoMale.IsChecked.Value == true)
                gender = "Nam";
            else
                gender = "Nữ";
            if (window.grdSelectImage.Background == null)
            {
                MessageBox.Show("Vui lòng thêm hình ảnh!");
                return;
            }

            byte[] imgByteArr;
            try
            {
                imgByteArr = Converter.Instance.ConvertImageToBytes(imageName);
            }
            catch
            {
                imgByteArr = EmployeeDAL.Instance.GetByIdEmployee(window.txtIdEmployee.Text).ImageFile;
            }
            imageName = null;
            int idEmployee = int.Parse(window.txtIdEmployee.Text);
            Employee employee = new Employee(idEmployee, window.txtName.Text, gender,
                window.txtTelephoneNumber.Text, window.txtAddress.Text, DateTime.Parse(window.dpBirthDate.Text),
                window.cboPosition.Text, DateTime.Parse(window.dpWorkDate.Text), -1, imgByteArr);

            //Employee current = EmployeeDAL.Instance.GetEmployeeByIdEmployee(window.txtIDEmployee.Text);
            //if (current != null && current.IdAccount != -1)
            //{
            //    if (employee.Position == "Nhân viên thu ngân")
            //    {
            //        AccountDAL.Instance.UpdateType(new Account(current.IdAccount, "", "", 2));
            //    }
            //    if (employee.Position == "Nhân viên quản lý")
            //    {
            //        AccountDAL.Instance.UpdateType(new Account(current.IdAccount, "", "", 1));
            //    }
            //    if (employee.Position == "Bảo vệ")
            //    {
            //        int temp = current.IdAccount;
            //        current.IdAccount = -1;
            //        EmployeeDAL.Instance.UpdateIdAccount(current);
            //        AccountDAL.Instance.DeleteAccount(temp.ToString());
            //    }
            //}
            EmployeeDAL.Instance.InsertOrUpdate(employee, isEditing);
            //SetBaseSalary(parameter);
            window.Close();

            //Add usercontrol vào stackpanel
            if (isEditing)
            {
                LoadEmployeeList(mainWindow);
            }
            else
            {
                EmployeeControl employeeControl = new EmployeeControl();
                employeeControl.txbId.Text = employee.IdEmployee.ToString();
                employeeControl.txbName.Text = employee.Name.ToString();
                employeeControl.txbPosition.Text = employee.Position.ToString();
                employeeControl.txbPhoneNumber.Text = employee.PhoneNumber.ToString();
                employeeControl.txbAddress.Text = employee.Address.ToString();

                mainWindow.stkEmployeeList.Children.Add(employeeControl);
            }
        }
        public void SetPickedDay(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            try
            {
                datePicker.Text = ((DateTime)datePicker.SelectedDate).ToString();
            }
            catch
            {

            }
        }
        public void HandleSelectImage(Grid grid)
        {
            OpenFileDialog op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png"
            };
            if (op.ShowDialog() == true)
            {
                imageName = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imageName);
                bitmap.EndInit();
                imageBrush.ImageSource = bitmap;
                grid.Background = imageBrush;
                if (grid.Children.Count > 1)
                {
                    grid.Children.Remove(grid.Children[0]);
                    grid.Children.Remove(grid.Children[1]);
                }
            }
        }

        //MainWindow
        public void Search(MainWindow mainWindow)
        {
            mainWindow.stkEmployeeList.Children.Clear();
            string nameSearching = mainWindow.txtSearch.Text.ToLower();
            List<Employee> employeeList = EmployeeDAL.Instance.SelectAll();
            foreach (var employee in employeeList)
            {
                if (employee.Name.ToLower().Contains(nameSearching))
                {
                    EmployeeControl employeeControl = new EmployeeControl();
                    employeeControl.txbId.Text = employee.IdEmployee.ToString();
                    employeeControl.txbName.Text = employee.Name.ToString();
                    employeeControl.txbPosition.Text = employee.Position.ToString();
                    employeeControl.txbPhoneNumber.Text = employee.PhoneNumber.ToString();
                    employeeControl.txbAddress.Text = employee.Address.ToString();

                    mainWindow.stkEmployeeList.Children.Add(employeeControl);
                }
            }
        }
        public void ExportExcel()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel |*.xlsx"
            };
            if ((bool)saveFileDialog.ShowDialog())
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(EmployeeDAL.Instance.LoadEmployeeDatatable(), "Danh sách nhân viên");
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                MessageBox.Show("Xuất  danh sách thành công!");
            }
        }
        public void OpenAddEmployeeWindow(MainWindow mainWindow)
        {
            //mainWindow.btnAddEmployee. = false;
            AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
            addEmployeeWindow.txtIdEmployee.Text = (EmployeeDAL.Instance.GetMaxIdEmployee() + 1).ToString();
            addEmployeeWindow.ShowDialog();
        }
        public void LoadEmployeeList(MainWindow main)
        {
            this.mainWindow = main;
            List<Employee> employeeList = EmployeeDAL.Instance.SelectAll();
            mainWindow.stkEmployeeList.Children.Clear();

            foreach (var employee in employeeList)
            {
                EmployeeControl employeeControl = new EmployeeControl();
                employeeControl.txbId.Text = employee.IdEmployee.ToString();
                employeeControl.txbName.Text = employee.Name.ToString();
                employeeControl.txbPosition.Text = employee.Position.ToString();
                employeeControl.txbPhoneNumber.Text = employee.PhoneNumber.ToString();
                employeeControl.txbAddress.Text = employee.Address.ToString();

                mainWindow.stkEmployeeList.Children.Add(employeeControl);
            }
        }
    }
}
