using ClosedXML.Excel;
using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class GoodsViewModel : BaseViewModel
    {
        private List<GoodsControl> listControlToView = new List<GoodsControl>(); // Luu nhung control khi dang filter, de hien thi len cac page
        private List<GoodsControl> listSearch = new List<GoodsControl>(); // Luu nhung control sau khi search xong 
        private GoodsControl goodsControl; // Khi edit thi truyen gia tri vao cho window edit
        private bool isUpdate = false;
        private string imageFileName;
        private MainWindow mainWindow;
        private int currentPage; // trang dau tien la trang 1
        private string oldGoods;

        private string importPrice;
        public string ImportPrice { get => importPrice; set => importPrice = value; }

        private GoodsType selectedGoodsType = new GoodsType();
        public GoodsType SelectedGoodsType { get => selectedGoodsType; set { selectedGoodsType = value; OnPropertyChanged("SelectedGoodsType"); } }
        private GoodsType selectedGoodsType_Filter = new GoodsType();
        public GoodsType SelectedGoodsType_Filter { get => selectedGoodsType_Filter; set { selectedGoodsType_Filter = value; OnPropertyChanged("SelectedGoodsType"); } }
        private ObservableCollection<GoodsType> itemSourceGoodsType = new ObservableCollection<GoodsType>();
        public ObservableCollection<GoodsType> ItemSourceGoodsType { get => itemSourceGoodsType; set { itemSourceGoodsType = value; OnPropertyChanged(); } }
        public ObservableCollection<GoodsType> ItemSourceGoodsType_Filter { get => itemSourceGoodsType_Filter; set { itemSourceGoodsType_Filter = value; OnPropertyChanged(); } }
        private ObservableCollection<GoodsType> itemSourceGoodsType_Filter = new ObservableCollection<GoodsType>();

        //AddGoodsWindow
        public ICommand SaveCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }

        //open window
        public ICommand AddGoodsCommand { get; set; }
        public ICommand EditGoodsCommand { get; set; }
        public ICommand OpenImportGoodsCommand { get; set; }
        public ICommand OpenGoodsTypeWindowCommand { get; set; }

        //Search, sort, filter
        public ICommand SelectionChangedGoodsTypeCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand SortListGoodsCommand { get; set; }

        //Pagination
        public ICommand PreviousPageCommand { get; set; }
        public ICommand NextPageCommand { get; set; }
        //Other
        public ICommand ExportExcelCommand { get; set; }
        public ICommand LoadGoodsCommand { get; set; }
        public ICommand DeleteGoodsCommand { get; set; }

        public GoodsViewModel()
        {
            LoadGoodsCommand = new RelayCommand<MainWindow>(p => true, p => Init(p));
            ExportExcelCommand = new RelayCommand<MainWindow>(p => true, p => ExportExcel(p));
            DeleteGoodsCommand = new RelayCommand<GoodsControl>(p => true, p => Delete(p));

            SaveCommand = new RelayCommand<AddGoodsWindow>(p => true, p => AddOrUpdate(p));
            ExitCommand = new RelayCommand<AddGoodsWindow>(p => true, p => p.Close());
            SeparateThousandsCommand = new RelayCommand<TextBox>((parameter) => true, (parameter) => SeparateThousands(parameter));
            SelectImageCommand = new RelayCommand<Grid>(p => true, p => SelectImage(p));
            SeparateThousandsCommand = new RelayCommand<TextBox>(p => true, p => SeparateThousands(p));

            AddGoodsCommand = new RelayCommand<MainWindow>(p => true, p => OpenAddGoodsWindow(p));
            EditGoodsCommand = new RelayCommand<GoodsControl>(p => true, p => OpenEditGoodsWindow(p));
            OpenImportGoodsCommand = new RelayCommand<MainWindow>(p => true, p => OpenImportGoodsWindow(p));
            OpenGoodsTypeWindowCommand = new RelayCommand<MainWindow>(p => true, p => OpenGoodsTypeWindow(p));

            SearchCommand = new RelayCommand<MainWindow>(p => true, p => SearchGoods(p));
            SelectionChangedGoodsTypeCommand = new RelayCommand<MainWindow>(p => true, p => Filter(p));
            SortListGoodsCommand = new RelayCommand<MainWindow>(p => true, p => Sort(p));

            PreviousPageCommand = new RelayCommand<MainWindow>(p => true, p => GoToPreviousPage(p, currentPage));
            NextPageCommand = new RelayCommand<MainWindow>(p => true, p => GoToNextPage(p, currentPage));
        }
        void Init(MainWindow main)
        {
            this.mainWindow = main;
            SetItemSourceGoodsType(main);
            SearchGoods(main);
        }
        void LoadListGoods(MainWindow main, int currentPage)
        {
            main.stkGoods.Children.Clear();
            this.currentPage = currentPage;
            int start = 0;
            int end = 0;
            LoadInfoOfPage(ref start, ref end);
            for (int i = start; i < end; i++)
            {
                main.stkGoods.Children.Add(listControlToView[i]);
            }
        }
        void GoToPreviousPage(MainWindow main, int currentPage)
        {
            LoadListGoods(main, --currentPage);
        }
        void GoToNextPage(MainWindow main, int currentPage)
        {
            LoadListGoods(main, ++currentPage);
        }
        void LoadInfoOfPage(ref int start, ref int end)
        {
            mainWindow.btnPrePageGoods.IsEnabled = (currentPage == 1 ? false : true);
            mainWindow.btnNextPageGoods.IsEnabled = (currentPage > ((listControlToView.Count) / 11) ? false : true);
            start = (currentPage - 1) * 10;
            end = start + 10;
            if (currentPage - 1 == listControlToView.Count / 10)
            {
                end = listControlToView.Count;
            }
            mainWindow.txtNumOfGoods.Text = String.Format("Trang {0} trên {1} trang", currentPage, listControlToView.Count / 11 + 1);
        }
        void Sort(MainWindow main)
        {
            List<GoodsControl> listTemp;
            switch (main.cboSortGoods.SelectedIndex)
            {
                case 0:
                    listTemp = listSearch.OrderBy(x => x.txbName.Text).ToList();
                    break;
                case 1:
                    listTemp = listSearch.OrderByDescending(x => x.txbName.Text).ToList();
                    break;
                case 2:
                    listTemp = listSearch.OrderBy(x => ConvertToNumber(x.txbSalesPrice.Text)).ToList();
                    break;
                case 3:
                    listTemp = listSearch.OrderByDescending(x => ConvertToNumber(x.txbSalesPrice.Text)).ToList();
                    break;
                case 4:
                    listTemp = listSearch.OrderBy(x => ConvertToNumber(x.txbImportPrice.Text)).ToList();
                    break;
                case 5:
                    listTemp = listSearch.OrderByDescending(x => ConvertToNumber(x.txbImportPrice.Text)).ToList();
                    break;
                default:
                    listTemp = listSearch.OrderBy(x => ConvertToID(x.txbId.Text)).ToList();
                    break;
            }
            listSearch = listTemp;
            Filter(main);
        }
        public void SearchGoods(MainWindow main)
        {
            main.cboFilterType.SelectedIndex = 0;
            main.cboSortGoods.SelectedIndex = -1;
            this.currentPage = 1;
            listControlToView.Clear();
            listSearch.Clear();
            DataTable dt = GoodsDAL.Instance.SearchByName(main.txtSearchGoods.Text.ToLower());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GoodsControl control = new GoodsControl();
                GoodsType goodsType = GoodsTypeDAL.Instance.GetById(int.Parse(dt.Rows[i].ItemArray[4].ToString()));

                long importPrice = long.Parse(dt.Rows[i].ItemArray[2].ToString());
                double profitPercentage = goodsType.ProfitPercentage;

                control.txbId.Text = AddPrefix("SP", int.Parse(dt.Rows[i].ItemArray[0].ToString()));
                control.txbName.Text = dt.Rows[i].ItemArray[1].ToString();
                control.txbImportPrice.Text = SeparateThousands(importPrice.ToString());
                control.txbSalesPrice.Text = SeparateThousands(Math.Ceiling(importPrice * (1 + profitPercentage)).ToString());
                control.txbQuantity.Text = dt.Rows[i].ItemArray[3].ToString();
                control.txbGoodsType.Text = goodsType.Name;
                control.txbUnit.Text = goodsType.Unit;
                listControlToView.Add(control);
                listSearch.Add(control);
            }
            LoadListGoods(main, this.currentPage);
        }
        public void Filter(MainWindow main)
        {
            main.stkGoods.Children.Clear();
            if (selectedGoodsType_Filter == null)
            {
                return;
            }
            listControlToView.Clear();
            if (selectedGoodsType_Filter.IdGoodsType != 0)// tránh TH người dùng đặt tên loại SP là "Tất cả"
            {
                for (int i = 0; i < listSearch.Count; i++)
                {
                    GoodsControl control = listSearch[i];
                    if (control.txbGoodsType.Text == selectedGoodsType_Filter.Name)
                    {
                        listControlToView.Add(control);
                    }
                }
            }
            else // chọn tất cả
            {
                for (int i = 0; i < listSearch.Count; i++)
                {
                    listControlToView.Add(listSearch[i]);
                }
            }
            this.currentPage = 1;
            LoadListGoods(main, currentPage);

        }
        void SetItemSourceGoodsType(MainWindow main)
        {
            itemSourceGoodsType_Filter.Clear();
            itemSourceGoodsType.Clear();
            GoodsType newGoodsType = new GoodsType();
            newGoodsType.Name = "Tất cả";
            itemSourceGoodsType_Filter.Add(newGoodsType);
            DataTable dt = GoodsTypeDAL.Instance.GetActive();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GoodsType type = new GoodsType(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), double.Parse(dt.Rows[0].ItemArray[2].ToString()),
                    dt.Rows[0].ItemArray[3].ToString(), true);
                itemSourceGoodsType_Filter.Add(type);
                itemSourceGoodsType.Add(type);
            }
        }
        void OpenGoodsTypeWindow(MainWindow main)
        {
            GoodsTypeWindow newWindow = new GoodsTypeWindow();
            newWindow.ShowDialog();
            SetItemSourceGoodsType(main);
            int indexSort = main.cboSortGoods.SelectedIndex;
            SearchGoods(main);
            main.cboSortGoods.SelectedIndex = indexSort;
            ImportGoodsViewModel importVM = (ImportGoodsViewModel)main.grdImport.DataContext;
            importVM.SetItemSource();

            //Update tab home 
            ReportViewModel reportVM = (ReportViewModel)mainWindow.grdHome.DataContext;
            reportVM.Init(main);
            //update sale
            SaleViewModel saleVM = (SaleViewModel)mainWindow.grdSale.DataContext;
            saleVM.Search(mainWindow);
            saleVM.LoadDefault(mainWindow);
            mainWindow.stkSelectedGoods.Children.Clear();
        }
        void OpenEditGoodsWindow(GoodsControl control)
        {
            isUpdate = true;
            goodsControl = control;
            oldGoods = control.txbName.Text;
            AddGoodsWindow addGoodsWd = new AddGoodsWindow();
            Binding binding = BindingOperations.GetBinding(addGoodsWd.txtName, TextBox.TextProperty);
            binding.ValidationRules.Clear();
            Goods goods = GoodsDAL.Instance.GetById(control.txbId.Text.Remove(0, 2));
            addGoodsWd.txtIdGoods.Text = control.txbId.Text;

            addGoodsWd.txtName.Text = control.txbName.Text;
            addGoodsWd.txtName.SelectionStart = addGoodsWd.txtName.Text.Length;
            addGoodsWd.txtName.SelectionLength = 0;

            addGoodsWd.cboGoodsType.Text = control.txbGoodsType.Text;

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(goods.ImageFile);
            addGoodsWd.grdSelectImg.Background = imageBrush;
            if (addGoodsWd.grdSelectImg.Children.Count > 1)
            {
                addGoodsWd.grdSelectImg.Children.Remove(addGoodsWd.grdSelectImg.Children[0]);
                addGoodsWd.grdSelectImg.Children.Remove(addGoodsWd.grdSelectImg.Children[1]);
            }
            addGoodsWd.txtImportPrice.Text = control.txbImportPrice.Text;
            addGoodsWd.txtImportPrice.SelectionStart = addGoodsWd.txtImportPrice.Text.Length;
            addGoodsWd.txtImportPrice.SelectionLength = 0;
            addGoodsWd.ShowDialog();
        }
        void OpenImportGoodsWindow(MainWindow main)
        {
            ImportGoodsViewModel importVM = (ImportGoodsViewModel)main.grdImport.DataContext;
            importVM.OpenImportGoodsWindow(main);
        }
        void OpenAddGoodsWindow(MainWindow wdMain)
        {
            isUpdate = false;
            AddGoodsWindow addGoodsWd = new AddGoodsWindow();
            addGoodsWd.txtName.Text = null;
            addGoodsWd.txtImportPrice.Text = null;
            int idMax = GoodsDAL.Instance.GetMaxId();
            if (idMax >= 0)
            {
                addGoodsWd.txtIdGoods.Text = AddPrefix("SP", idMax + 1);
                addGoodsWd.cboGoodsType.Text = null;
                addGoodsWd.ShowDialog();
            }
            else
            {
                return;
            }
        }
        void Delete(GoodsControl control)
        {
            var result = CustomMessageBox.Show("Bạn có chắc chắn muốn xóa?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if (GoodsDAL.Instance.Delete(ConvertToID(control.txbId.Text)))
                {
                    listSearch.Remove(control);
                    listControlToView.Remove(control);
                    if (listControlToView.Count <= (this.currentPage - 1) * 10)
                        LoadListGoods(mainWindow, --currentPage);
                    else
                    {
                        LoadListGoods(mainWindow, currentPage);
                    }
                }
                else
                {
                    CustomMessageBox.Show("Xóa không thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        void ExportExcel(MainWindow wdMain)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Mã SP", typeof(string));
            table.Columns.Add("Tên sản phẩm", typeof(string));
            table.Columns.Add("Loại sản phẩm", typeof(string));
            table.Columns.Add("Số lượng", typeof(int));
            table.Columns.Add("Đơn vị tính", typeof(string));
            table.Columns.Add("Giá mua", typeof(string));
            table.Columns.Add("Giá bán", typeof(string));


            for (int i = 0; i < listControlToView.Count; i++)
            {
                GoodsControl control = listControlToView[i];
                table.Rows.Add(control.txbId.Text, control.txbName.Text, control.txbGoodsType.Text,
                    control.txbQuantity.Text, control.txbUnit.Text, control.txbImportPrice.Text,
                    control.txbSalesPrice.Text);
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel |*.xlsx"
            };
            if ((bool)saveFileDialog.ShowDialog())
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(table, "Danh sách hàng hóa");
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                CustomMessageBox.Show("Xuất danh sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }
        public void SelectImage(Grid parameter)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                imageFileName = op.FileName;
                ImageBrush imageBrush = new ImageBrush();
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imageFileName);
                bitmap.EndInit();
                imageBrush.ImageSource = bitmap;
                parameter.Background = imageBrush;
                if (parameter.Children.Count > 1)
                {
                    parameter.Children.Remove(parameter.Children[0]);
                    parameter.Children.Remove(parameter.Children[1]);
                }
            }
        }
        void AddOrUpdate(AddGoodsWindow addGoodsWd)
        {
            if (string.IsNullOrEmpty(addGoodsWd.txtName.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập tên sản phẩm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                addGoodsWd.txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(addGoodsWd.cboGoodsType.Text))
            {
                CustomMessageBox.Show("Vui lòng chọn loại sản phẩm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                addGoodsWd.cboGoodsType.Focus();
                return;
            }
            if (string.IsNullOrEmpty(addGoodsWd.txtImportPrice.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập giá sản phẩm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                addGoodsWd.txtImportPrice.Focus();
                return;
            }
            byte[] imgByteArr;

            ImageBrush imageBrush = (ImageBrush)addGoodsWd.grdSelectImg.Background;
            if (imageBrush == null)
            {
                CustomMessageBox.Show("Vui lòng chọn ảnh!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            imgByteArr = Converter.Instance.ConvertBitmapImageToBytes((BitmapImage)imageBrush.ImageSource);
            if ((!isUpdate || addGoodsWd.txtName.Text != oldGoods) && GoodsDAL.Instance.IsExisted(addGoodsWd.txtName.Text))
            {
                CustomMessageBox.Show("Sản phẩm đã tồn tại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                addGoodsWd.txtName.Focus();
                return;
            }
            Goods newGoods = new Goods(ConvertToID(addGoodsWd.txtIdGoods.Text), addGoodsWd.txtName.Text,
                             ConvertToNumber(addGoodsWd.txtImportPrice.Text), 0, selectedGoodsType.IdGoodsType, imgByteArr, false);
            if (isUpdate)
            {
                newGoods.ImportPrice = ConvertToNumber(addGoodsWd.txtImportPrice.Text);
            }
            if(GoodsDAL.Instance.InsertOrUpdate(newGoods, isUpdate))
            {
                CustomMessageBox.Show("Thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                CustomMessageBox.Show("Thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (isUpdate)
            {
                //Update tab home 
                ReportViewModel reportVM = (ReportViewModel)mainWindow.grdHome.DataContext;
                reportVM.Init(mainWindow);
                //update sale
                SaleViewModel saleVM = (SaleViewModel)mainWindow.grdSale.DataContext;
                saleVM.Search(mainWindow);
                saleVM.LoadDefault(mainWindow);
                mainWindow.stkSelectedGoods.Children.Clear();
            }
            int indexSort = mainWindow.cboSortGoods.SelectedIndex;
            int indexFilter = mainWindow.cboFilterType.SelectedIndex;

            SearchGoods(mainWindow);
            mainWindow.cboSortGoods.SelectedIndex = indexSort;
            mainWindow.cboFilterType.SelectedIndex = indexFilter;
            addGoodsWd.Close();
        }
    }
}
