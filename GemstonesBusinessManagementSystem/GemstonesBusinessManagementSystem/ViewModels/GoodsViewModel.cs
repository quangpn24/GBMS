using ClosedXML.Excel;
using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using GemstonesBusinessManagementSystem.Views;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class GoodsViewModel : BaseViewModel
    {
        private GoodsControl goodsControl;
        private bool isUpdate = false;
        private string imageFileName;
        private MainWindow mainWindow;
        private GoodsType selectedGoodsType = new GoodsType();
        public GoodsType SelectedGoodsType { get => selectedGoodsType; set { selectedGoodsType = value; OnPropertyChanged("SelectedGoodsType"); } } 
        private GoodsType selectedGoodsType_Filter = new GoodsType();
        public GoodsType SelectedGoodsType_Filter { get => selectedGoodsType_Filter; set { selectedGoodsType_Filter = value; OnPropertyChanged("SelectedGoodsType"); } }
        private ObservableCollection<GoodsType> itemSourceGoodsType = new ObservableCollection<GoodsType>();
        public ObservableCollection<GoodsType> ItemSourceGoodsType { get => itemSourceGoodsType; set { itemSourceGoodsType = value; OnPropertyChanged(); } }
        public ObservableCollection<GoodsType> ItemSourceGoodsType_Filter { get => itemSourceGoodsType_Filter; set { itemSourceGoodsType_Filter = value; OnPropertyChanged(); } }
        private ObservableCollection<GoodsType> itemSourceGoodsType_Filter = new ObservableCollection<GoodsType>();
        public ICommand ExportExcelCommand { get; set; }
        public ICommand AddGoodsCommand { get; set; }
        public ICommand LoadGoodsCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand EditGoodsCommand { get; set; }
        public ICommand ImportGoodsCommand { get; set; }
        public ICommand DeleteGoodsCommand { get; set; }
        public ICommand LoadGoodsTypeCommand { get; set; }
        public ICommand SelectImageCommand { get; set; }
        public ICommand SelectionChangedGoodsTypeCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public GoodsViewModel()
        {
            ExportExcelCommand = new RelayCommand<MainWindow>(p => true, p => ExportExcel(p));
            AddGoodsCommand = new RelayCommand<MainWindow>(p => true, p => OpenAddGoodsWindow(p));
            LoadGoodsCommand = new RelayCommand<MainWindow>(p => true, p => LoadGoodsToView(p));
            SaveCommand = new RelayCommand<AddGoodsWindow>(p => true, p => AddOrUpdate(p));
            ExitCommand = new RelayCommand<AddGoodsWindow>(p => true, p => p.Close());
            EditGoodsCommand = new RelayCommand<GoodsControl>(p => true, p => OpenAddGoodsWindow_Edit(p));
            ImportGoodsCommand = new RelayCommand<GoodsControl>(p => true, p => ImportGoods(p));
            DeleteGoodsCommand = new RelayCommand<GoodsControl>(p => true, p => DeleteGoods(p));
            LoadGoodsTypeCommand = new RelayCommand<MainWindow>(p => true, p => SetItemSourceGoodsType(p));
            SelectImageCommand = new RelayCommand<Grid>(p => true, p => SelectImage(p));
            SearchCommand = new RelayCommand<MainWindow>(p => true, p => SearchGoods(p));
            SelectionChangedGoodsTypeCommand = new RelayCommand<MainWindow>(p => true, p => SelectionChangedGoodsType(p));
        }
        public void SearchGoods(MainWindow main)
        {
            main.cboFilterType.SelectedIndex = 0;
            main.cboFilter.SelectedIndex = 0;
            LoadGoodsToView(main);
        }
        public void SelectionChangedGoodsType(MainWindow main)
        {
            LoadGoodsToView(main);
            if (selectedGoodsType_Filter.Name == "Tất cả" && selectedGoodsType_Filter.IdGoodsType == 0)// tránh TH người dùng đặt tên loại SP là "Tất cả"
            {
            }
            else
            {
                for (int i = 0; i < main.stkGoods.Children.Count; i++)
                {
                    GoodsControl control = (GoodsControl)main.stkGoods.Children[i];
                    if (control.txbGoodsType.Text != selectedGoodsType_Filter.Name)
                    {
                        main.stkGoods.Children.Remove(control);
                        i--;
                    }
                }
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
        void SetItemSourceGoodsType(MainWindow main)
        {
            itemSourceGoodsType.Clear();
            GoodsType newGoodsType = new GoodsType();
            newGoodsType.Name = "Tất cả";
            itemSourceGoodsType_Filter.Add(newGoodsType);
            DataTable dt = GoodsTypeDAL.Instance.LoadData();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                GoodsType type = new GoodsType(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), double.Parse(dt.Rows[0].ItemArray[2].ToString()),
                    dt.Rows[0].ItemArray[3].ToString());
                itemSourceGoodsType.Add(type);
                itemSourceGoodsType_Filter.Add(type);
            }
        }
        void OpenAddGoodsWindow_Edit(GoodsControl control)
        {
            isUpdate = true;
            goodsControl = control;
            AddGoodsWindow addGoodsWd = new AddGoodsWindow();
            Goods goods = GoodsDAL.Instance.GetGoods(control.txbId.Text.Remove(0, 2));
            addGoodsWd.txtIdGoods.Text = control.txbId.Text;
            addGoodsWd.txtName.Text = control.txbName.Text;
            addGoodsWd.txtName.SelectionStart = addGoodsWd.txtName.Text.Length;
            addGoodsWd.cboGoodsType.Text = control.txbGoodsType.Text;
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = Converter.Instance.ConvertByteToBitmapImage(goods.ImageFile);
            addGoodsWd.grdSelectImg.Background = imageBrush;
            if (addGoodsWd.grdSelectImg.Children.Count > 1)
            {
                addGoodsWd.grdSelectImg.Children.Remove(addGoodsWd.grdSelectImg.Children[0]);
                addGoodsWd.grdSelectImg.Children.Remove(addGoodsWd.grdSelectImg.Children[1]);
            }
            addGoodsWd.ShowDialog();
        }
        void ImportGoods(GoodsControl control)
        {

        }
        void DeleteGoods(GoodsControl control)
        {
            if(GoodsDAL.Instance.Delete(ConvertToID(control.txbId.Text)))
            {
                this.mainWindow.stkGoods.Children.Remove(control);
            }
            else
            {
                MessageBox.Show("Xóa không thành công!!!", "Error");
                return;
            }
        }
        void ExportExcel(MainWindow mainWd)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx"
            };
            if(saveFileDialog.ShowDialog() == true)
            {
                using(XLWorkbook workbook = new XLWorkbook())
                {
                    DataTable dt = GoodsDAL.Instance.LoadData();
                    dt.Columns.Remove("imageFile");
                    workbook.Worksheets.Add(dt, "Goods");
                    workbook.SaveAs(saveFileDialog.FileName);
                }
                MessageBox.Show("Xuất dữ liệu thành công!!!", "Thông báo");
            }
            
        }
        void AddOrUpdate(AddGoodsWindow addGoodsWd)
        {
            if (string.IsNullOrEmpty(addGoodsWd.txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm", "Error");
                addGoodsWd.txtName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(addGoodsWd.cboGoodsType.Text))
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm", "Error");
                addGoodsWd.cboGoodsType.Focus();
                return;
            }
            byte[] imgByteArr;
            try
            {
                imgByteArr = Converter.Instance.ConvertImageToBytes(imageFileName);
            }
            catch
            {
                imgByteArr = GoodsDAL.Instance.GetGoods(addGoodsWd.txtIdGoods.Text).ImageFile;
            }
            Goods newGoods = new Goods(ConvertToID(addGoodsWd.txtIdGoods.Text), addGoodsWd.txtName.Text,
                             0, 0, selectedGoodsType.IdGoodsType, imgByteArr, false);
            if(isUpdate)
            {
                newGoods.Price = long.Parse(goodsControl.txbPrice.Text);
                newGoods.Quantity = int.Parse(goodsControl.txbQuantity.Text);
            }
            GoodsDAL.Instance.InsertOrUpdate(newGoods, isUpdate);
            GoodsControl control = new GoodsControl();
            if(isUpdate)
            {
                control = goodsControl;
            }
           
            control.txbId.Text = AddPrifix("SP", newGoods.IdGoods) + newGoods.IdGoods.ToString();
            control.txbName.Text = newGoods.Name;
            control.txbGoodsType.Text = GoodsTypeDAL.Instance.GetGoodsTypeWithId(newGoods.IdGoodsType).Name;
            control.txbUnit.Text = GoodsTypeDAL.Instance.GetGoodsTypeWithId(newGoods.IdGoodsType).Unit;
            control.txbPrice.Text = newGoods.Price.ToString();
            control.txbQuantity.Text = newGoods.Quantity.ToString();
            if (!isUpdate)
            {
                this.mainWindow.stkGoods.Children.Add(control);
            }
            addGoodsWd.Close();
        }
        void OpenAddGoodsWindow(MainWindow mainWd)
        {
            isUpdate = false;
            AddGoodsWindow addGoodsWd = new AddGoodsWindow();
            int idMax = GoodsDAL.Instance.GetMaxId();
            if(idMax >= 0)
            {
                addGoodsWd.txtIdGoods.Text =AddPrifix("SP", idMax) + (idMax + 1).ToString();
                addGoodsWd.ShowDialog();
            }
        }
        void LoadGoodsToView(MainWindow mainWd)
        {
            this.mainWindow = mainWd;
            mainWd.stkGoods.Children.Clear();
            DataTable dt = GoodsDAL.Instance.LoadData();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string name = dt.Rows[i].ItemArray[1].ToString();
                if (name.ToLower().Contains(mainWd.txtSearchGoods.Text.ToLower()))
                {
                    GoodsControl control = new GoodsControl();
                    control.txbId.Text = AddPrifix("SP",int.Parse(dt.Rows[i].ItemArray[0].ToString())) + dt.Rows[i].ItemArray[0].ToString();
                    control.txbName.Text = dt.Rows[i].ItemArray[1].ToString();
                    control.txbPrice.Text = dt.Rows[i].ItemArray[2].ToString();
                    control.txbQuantity.Text = dt.Rows[i].ItemArray[3].ToString();
                    control.txbGoodsType.Text = GoodsTypeDAL.Instance.GetGoodsTypeWithId(int.Parse(dt.Rows[i].ItemArray[4].ToString())).Name;
                    control.txbUnit.Text = GoodsTypeDAL.Instance.GetGoodsTypeWithId(int.Parse(dt.Rows[i].ItemArray[4].ToString())).Unit;
                    mainWd.stkGoods.Children.Add(control);
                }
            }
        }
    }
}
