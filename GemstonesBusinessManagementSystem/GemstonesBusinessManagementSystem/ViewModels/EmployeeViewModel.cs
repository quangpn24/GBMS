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
using System.Windows.Media.Imaging;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class EmployeeViewModel : BaseViewModel
    {
        //EmployeePositionControl
        public ICommand ViewPositionCommand { get; set; }

        //EmployeePositionWindow
        public ICommand ClearViewCommand { get; set; }
        public ICommand LoadPositionCommand { get; set; }
        public ICommand AddPositionCommand { get; set; }
        public ICommand OpenPositionWindowCommand { get; set; }

        //EmployeeControl
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        //AddEmployeeWindow
        public ICommand SelectImageCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        //MainWindow
        public ICommand PreviousPageCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand OpenAddWindowCommand { get; set; }
        public ICommand LoadEmployeeCommand { get; set; }

        //Mainwindow
        private MainWindow mainWindow;
        public MainWindow MainWindow { get => mainWindow; set => mainWindow = value; }

        private List<Employee> employeeList = EmployeeDAL.Instance.GetList();

        private EmployeeControl employeeControl;

        //PositionWindow
        private EmployeePositionWindow employeePositionWindow;
        public bool isEditingPosition = false;
        private EmployeePositionControl empPosControl;

        private EmployeePosition selectedPosition = new EmployeePosition();
        public EmployeePosition SelectedPosition { get => selectedPosition; set { selectedPosition = value; OnPropertyChanged("SelectedEmployeePosition"); } }

        private ObservableCollection<EmployeePosition> itemSourcePosition = new ObservableCollection<EmployeePosition>();
        public ObservableCollection<EmployeePosition> ItemSourcePosition { get => itemSourcePosition; set { itemSourcePosition = value; OnPropertyChanged(); } }

        public string imageName;
        public bool isEditing = false;

        public EmployeeViewModel()
        {
            //EmployeePositionControl
            ViewPositionCommand = new RelayCommand<EmployeePositionControl>((parameter) => true, (parameter) => ViewPosition(parameter));

            //EmployeePositionWindow
            ClearViewCommand = new RelayCommand<EmployeePositionWindow>((parameter) => true, (parameter) => ClearView(parameter));
            LoadPositionCommand = new RelayCommand<EmployeePositionWindow>((parameter) => true, (parameter) => LoadPosition(parameter));
            AddPositionCommand = new RelayCommand<EmployeePositionWindow>((parameter) => true, (parameter) => AddPosition(parameter));
            OpenPositionWindowCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => OpenEmployeePositionWindow(parameter));

            //EmployeeControl
            EditCommand = new RelayCommand<EmployeeControl>((parameter) => true, (parameter) => OpenEditEmployeeWindow(parameter));
            DeleteCommand = new RelayCommand<EmployeeControl>((parameter) => true, (parameter) => DeleteEmployee(parameter));

            //AddEmployeeWindow
            SelectImageCommand = new RelayCommand<Grid>((parameter) => true, (parameter) => HandleSelectImage(parameter));
            SaveCommand = new RelayCommand<AddEmployeeWindow>((parameter) => true, (parameter) => HandleAddEmployee(parameter));
            ExitCommand = new RelayCommand<AddEmployeeWindow>((parameter) => true, (parameter) => parameter.Close());

            //MainWindow
            PreviousPageCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => Search(parameter));
            NextPageCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => Search(parameter));
            SearchCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => Search(parameter));
            ExportExcelCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => ExportExcel());
            OpenAddWindowCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => OpenAddEmployeeWindow(parameter));
            LoadEmployeeCommand = new RelayCommand<MainWindow>((parameter) => true, (parameter) => { LoadEmployeeList(parameter); SetItemSource(); });
        }

        //EmployeePositionControl
        void ClearView(EmployeePositionWindow window)
        {
            isEditingPosition = false;
            window.txtId.Text = (EmployeePositionDAL.Instance.GetMaxId() + 1).ToString();
            window.txtPosition.Clear();
            window.txtSalaryBase.Clear();
            window.txtStandardWorkDays.Clear();
            window.txtOvertime.Clear();
            window.txtFault.Clear();
        }
        void ViewPosition(EmployeePositionControl control)
        {
            empPosControl = control;
            isEditingPosition = true;
            employeePositionWindow.txbTitle.Text = "Sửa chức vụ";
            employeePositionWindow.txtId.Text = control.txbId.Text;

            employeePositionWindow.txtPosition.Text = control.txbPosition.Text;
            employeePositionWindow.txtPosition.SelectionStart = control.txbPosition.Text.Length;

            employeePositionWindow.txtSalaryBase.Text = control.txbSalaryBase.Text;
            employeePositionWindow.txtSalaryBase.SelectionLength = control.txbSalaryBase.Text.Length;

            employeePositionWindow.txtStandardWorkDays.Text = control.txbWorkdays.Text;
            employeePositionWindow.txtStandardWorkDays.SelectionLength = control.txbWorkdays.Text.Length;
            
            employeePositionWindow.txtOvertime.Text = control.txbShift.Text;
            employeePositionWindow.txtOvertime.SelectionLength = control.txbShift.Text.Length;
            
            employeePositionWindow.txtFault.Text = control.txbFault.Text;
            employeePositionWindow.txtFault.SelectionLength = control.txbFault.Text.Length;
        }

        //EmployeePositionWindow
        void LoadPosition(EmployeePositionWindow window)
        {
            employeePositionWindow = window;

            List<EmployeePosition> positions = EmployeePositionDAL.Instance.GetList();
            window.stkPosition.Children.Clear();

            foreach (var position in positions)
            {
                EmployeePositionControl control = new EmployeePositionControl();
                control.txbId.Text = position.IdEmployeePosition.ToString();
                control.txbPosition.Text = position.Position;
                control.txbSalaryBase.Text = position.SalaryBase.ToString();
                control.txbShift.Text = position.MoneyPerShift.ToString();
                control.txbFault.Text = position.MoneyPerFault.ToString();
                control.txbWorkdays.Text = position.StandardWorkDays.ToString();

                window.stkPosition.Children.Add(control);
            }
        }
        void AddPosition(EmployeePositionWindow window)
        {
            if (string.IsNullOrEmpty(window.txtPosition.Text))
            {
                MessageBox.Show("Vui lòng nhập chức vụ!");
                window.txtPosition.Focus();
                return;
            }
            if (string.IsNullOrEmpty(window.txtSalaryBase.Text))
            {
                MessageBox.Show("Vui lòng nhập lương cơ bản!");
                window.txtSalaryBase.Focus();
                return;
            }
            if (string.IsNullOrEmpty(window.txtStandardWorkDays.Text))
            {
                MessageBox.Show("Vui lòng nhập ngày công chuẩn!");
                window.txtStandardWorkDays.Focus();
                return;
            }
            if (string.IsNullOrEmpty(window.txtOvertime.Text))
            {
                MessageBox.Show("Vui lòng nhập tiền lương tăng ca!");
                window.txtOvertime.Focus();
                return;
            }
            if (string.IsNullOrEmpty(window.txtFault.Text))
            {
                MessageBox.Show("Vui lòng nhập tiền phạt!");
                window.txtFault.Focus();
                return;
            }
            EmployeePosition position = new EmployeePosition(int.Parse(window.txtId.Text), window.txtPosition.Text,
                long.Parse(window.txtSalaryBase.Text), long.Parse(window.txtOvertime.Text),
                long.Parse(window.txtFault.Text), int.Parse(window.txtStandardWorkDays.Text));

            EmployeePositionDAL.Instance.InsertOrUpdate(position, isEditingPosition);
            if (isEditingPosition)
            {
                empPosControl.txbId.Text = window.txtId.Text;
                empPosControl.txbPosition.Text = window.txtPosition.Text;
                empPosControl.txbSalaryBase.Text = window.txtSalaryBase.Text;
                empPosControl.txbWorkdays.Text = window.txtStandardWorkDays.Text;
                empPosControl.txbShift.Text = window.txtOvertime.Text;
                empPosControl.txbFault.Text = window.txtFault.Text;
            }
            else
            {
                EmployeePositionControl control = new EmployeePositionControl();
                control.txbId.Text = position.IdEmployeePosition.ToString();
                control.txbPosition.Text = position.Position;
                control.txbSalaryBase.Text = position.SalaryBase.ToString();
                control.txbShift.Text = position.MoneyPerShift.ToString();
                control.txbFault.Text = position.MoneyPerFault.ToString();
                control.txbWorkdays.Text = position.StandardWorkDays.ToString();

                window.stkPosition.Children.Add(control);
            }
            SetItemSource();
            LoadEmployeeList(mainWindow);
            isEditingPosition = false;
            ClearView(window);
        }
        void OpenEmployeePositionWindow(MainWindow mainWindow)
        {
            EmployeePositionWindow window = new EmployeePositionWindow();
            window.txtId.Text = (EmployeePositionDAL.Instance.GetMaxId() + 1).ToString();
            window.ShowDialog();
        }

        //EmployeeControl
        void OpenEditEmployeeWindow(EmployeeControl control)
        {
            isEditing = true;
            Employee employee = EmployeeDAL.Instance.GetById(control.txbId.Text);
            this.employeeControl = control;

            AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
            if (employee.IdEmployee.ToString() == control.txbId.Text)
            {
                addEmployeeWindow.txtId.Text = employee.IdEmployee.ToString();

                addEmployeeWindow.txtName.Text = employee.Name;
                addEmployeeWindow.txtName.SelectionStart = addEmployeeWindow.txtName.Text.Length;

                addEmployeeWindow.txtPhoneNumber.Text = employee.PhoneNumber;
                addEmployeeWindow.txtPhoneNumber.SelectionStart = addEmployeeWindow.txtPhoneNumber.Text.Length;

                addEmployeeWindow.txtAddress.Text = employee.Address;
                addEmployeeWindow.txtAddress.SelectionStart = addEmployeeWindow.txtAddress.Text.Length;

                addEmployeeWindow.cboPosition.Text = EmployeePositionDAL.Instance.GetById(employee.IdPosition).Position;

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
        void HandleAddEmployee(AddEmployeeWindow window)
        {
            #region
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
            if (window.txtPhoneNumber.Text == "")
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!");
                window.txtPhoneNumber.Focus();
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
                imgByteArr = EmployeeDAL.Instance.GetById(window.txtId.Text).ImageFile;
            }
            imageName = null;
            #endregion
            int idEmployee = int.Parse(window.txtId.Text);
            Employee employee = new Employee(idEmployee, window.txtName.Text, gender,
                window.txtPhoneNumber.Text, window.txtAddress.Text, DateTime.Parse(window.dpBirthDate.Text),
                selectedPosition.IdEmployeePosition, DateTime.Parse(window.dpWorkDate.Text), 1, imgByteArr);

            EmployeeDAL.Instance.InsertOrUpdate(employee, isEditing);

            window.Close();

            //Add usercontrol vào stackpanel
            if (isEditing)
            {
                this.employeeControl.txbId.Text = employee.IdEmployee.ToString();
                this.employeeControl.txbName.Text = employee.Name.ToString();
                this.employeeControl.txbPosition.Text = EmployeePositionDAL.Instance.GetById(employee.IdPosition).Position;
                this.employeeControl.txbPhoneNumber.Text = employee.PhoneNumber.ToString();
                this.employeeControl.txbAddress.Text = employee.Address.ToString();
            }
            else
            {
                EmployeeControl control = new EmployeeControl();
                control.txbId.Text = employee.IdEmployee.ToString();
                control.txbName.Text = employee.Name.ToString();
                control.txbPosition.Text = EmployeePositionDAL.Instance.GetById(employee.IdPosition).Position;
                control.txbPhoneNumber.Text = employee.PhoneNumber.ToString();
                control.txbAddress.Text = employee.Address.ToString();

                employeeList.Add(employee);
                mainWindow.stkEmployeeList.Children.Add(control);
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
        void HandleSelectImage(Grid grid)
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
        void SetItemSource()
        {
            itemSourcePosition.Clear();
            List<EmployeePosition> positions = EmployeePositionDAL.Instance.GetList();
            foreach (var position in positions)
            {
                itemSourcePosition.Add(position);
            }
        }
        void Search(MainWindow mainWindow)
        {
            mainWindow.stkEmployeeList.Children.Clear();
            string nameSearching = mainWindow.txtSearch.Text.ToLower();
            foreach (var employee in employeeList)
            {
                if (employee.Name.ToLower().Contains(nameSearching))
                {
                    EmployeeControl employeeControl = new EmployeeControl();
                    employeeControl.txbId.Text = employee.IdEmployee.ToString();
                    employeeControl.txbName.Text = employee.Name.ToString();
                    employeeControl.txbPosition.Text = EmployeePositionDAL.Instance.GetById(employee.IdPosition).Position;
                    employeeControl.txbPhoneNumber.Text = employee.PhoneNumber.ToString();
                    employeeControl.txbAddress.Text = employee.Address.ToString();

                    mainWindow.stkEmployeeList.Children.Add(employeeControl);
                }
            }
        }
        void ExportExcel()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel |*.xlsx"
            };
            if ((bool)saveFileDialog.ShowDialog())
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(EmployeeDAL.Instance.GetDatatable(), "Danh sách nhân viên");
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                MessageBox.Show("Xuất  danh sách thành công!");
            }
        }
        void OpenAddEmployeeWindow(MainWindow mainWindow)
        {
            isEditing = false;
            AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
            addEmployeeWindow.txtId.Text = (EmployeeDAL.Instance.GetMaxId() + 1).ToString();
            addEmployeeWindow.ShowDialog();
        }
        void LoadEmployeeList(MainWindow main)
        {
            this.mainWindow = main;
            mainWindow.stkEmployeeList.Children.Clear();

            foreach (var employee in employeeList)
            {
                EmployeeControl employeeControl = new EmployeeControl();
                employeeControl.txbId.Text = employee.IdEmployee.ToString();
                employeeControl.txbName.Text = employee.Name.ToString();
                employeeControl.txbPosition.Text = EmployeePositionDAL.Instance.GetById(employee.IdPosition).Position;
                employeeControl.txbPhoneNumber.Text = employee.PhoneNumber.ToString();
                employeeControl.txbAddress.Text = employee.Address.ToString();

                mainWindow.stkEmployeeList.Children.Add(employeeControl);
            }
        }
    }
}
