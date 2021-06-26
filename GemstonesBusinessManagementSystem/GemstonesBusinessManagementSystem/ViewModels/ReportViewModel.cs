using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GemstonesBusinessManagementSystem.DAL;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.Views;
using GemstonesBusinessManagementSystem.Resources.UserControls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Data;
using System.Windows.Threading;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class ReportViewModel : BaseViewModel
    {

        private ObservableCollection<string> itemSourceTime = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceTime { get => itemSourceTime; set { itemSourceTime = value; OnPropertyChanged(); } }

        private ObservableCollection<string> itemSourceYear = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceYear { get => itemSourceYear; set { itemSourceYear = value; OnPropertyChanged(); } }

        //dashboard
        private string todayRevenue;
        public string TodayRevenue { get => todayRevenue; set { todayRevenue = value; OnPropertyChanged(); } }
        private string todaySpend;
        public string TodaySpend { get => todaySpend; set { todaySpend = value; OnPropertyChanged(); } }
        private string todayBillQuantity;
        public string TodayBillQuantity { get => todayBillQuantity; set { todayBillQuantity = value; OnPropertyChanged(); } }
        private string increasingPercent;
        public string IncreasingPercent { get => increasingPercent; set { increasingPercent = value; OnPropertyChanged(); } }

        //Column chart
        private Func<double, string> formatter;
        public Func<double, string> Formatter { get => formatter; set { formatter = value; OnPropertyChanged(); } }
        private string axisXTitle;
        public string AxisXTitle { get => axisXTitle; set { axisXTitle = value; OnPropertyChanged(); } }
        private string nameChart;
        public string NameChart { get => nameChart; set { nameChart = value; OnPropertyChanged(); } }

        private List<string> labels = new List<string>();
        public List<string> Labels { get => labels; set { labels = value; OnPropertyChanged(); } }
        private SeriesCollection seriesCollection;
        public SeriesCollection SeriesCollection { get => seriesCollection; set { seriesCollection = value; OnPropertyChanged(); } }

        //pie chart
        private Func<ChartPoint, string> labelPoint;
        public Func<ChartPoint, string> LabelPoint { get => labelPoint; set { labelPoint = value; OnPropertyChanged(); } }
        private string namePieChart;
        public string NamePieChart { get => namePieChart; set { namePieChart = value; OnPropertyChanged(); } }
        private SeriesCollection pieSeriesCollection;
        public SeriesCollection PieSeriesCollection { get => pieSeriesCollection; set { pieSeriesCollection = value; OnPropertyChanged(); } }

        public ICommand LoadReportCommand { get; set; }
        public ICommand SelectTimePieChartCommand { get; set; }
        public ICommand SelectPeriodCommand { get; set; }
        public ICommand SelectMonthCommand { get; set; }
        public ICommand SelectYearCommand { get; set; }

        public ReportViewModel()
        {
            LoadReportCommand = new RelayCommand<MainWindow>(p => true, p => Init(p));
            SelectTimePieChartCommand = new RelayCommand<MainWindow>(p => true, p => SelectPeriodPieChart(p));
            SelectPeriodCommand = new RelayCommand<MainWindow>(p => true, p => SelectPeriod(p));
            SelectMonthCommand = new RelayCommand<MainWindow>(p => true, p => SelectMonth(p));
            SelectYearCommand = new RelayCommand<MainWindow>(p => true, p => SelectYear(p));
        }
        public void Init(MainWindow main)
        {
            LoadDashBoard(main);
            SetItemSourceYear();
            LoadBestSeller(main);
            LoadDefaultChart(main);
        }
        public void LoadDefaultChart(MainWindow main)
        {
            string currentDay = DateTime.Now.Day.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            string lastMonth = (int.Parse(currentMonth) - 1).ToString();
            string currentYear = DateTime.Now.Year.ToString();

            main.cboSelectTimePie.SelectedIndex = -1;
            main.cboSelectPeriod.SelectedIndex = -1;
            main.cboSelectYear.SelectedIndex = -1;
            main.cboSelectTime.SelectedIndex = -1;
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            timer.Tick += (s, e) =>
            {
                main.cboSelectTimePie.SelectedIndex = 0;
                main.cboSelectPeriod.SelectedIndex = 0;
                main.cboSelectYear.SelectedIndex = 0;
                main.cboSelectTime.SelectedIndex = DateTime.Now.Month - 1;

                timer.Stop();
            };
            timer.Start();
        }
        void LoadDashBoard(MainWindow main)
        {

            TodayRevenue = Converter.Instance.Nice(ReportDAL.Instance.GetTodayRevenue(), 1);
            TodaySpend = Converter.Instance.Nice(ReportDAL.Instance.GetTodaySpend(), 1);
            TodayBillQuantity = string.Format("{0:N0}", ReportDAL.Instance.GetTodayBillQuantity());
            long today_Revenue = ReportDAL.Instance.GetTodayRevenue();
            long yesterday_Revenue = ReportDAL.Instance.GetYesterdayRenvenue();
            double percent = 0;
            bool flag = true;
            if (yesterday_Revenue > 0)
            {
                percent = Math.Round(((double)today_Revenue - yesterday_Revenue) / yesterday_Revenue * 100, 2);
            }
            else if (yesterday_Revenue == 0)
            {
                flag = false;
            }
            main.icoIncreasing.Visibility = System.Windows.Visibility.Hidden;
            main.icoDecreasing.Visibility = System.Windows.Visibility.Hidden;
            if (!flag)
            {
                main.txbIncreasingPercent.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                if (percent >= 0)
                {
                    main.icoIncreasing.Visibility = System.Windows.Visibility.Visible;
                    main.txbIncreasingPercent.Foreground = (Brush)new BrushConverter().ConvertFrom("#1BA345");
                }
                else
                {
                    main.icoDecreasing.Visibility = System.Windows.Visibility.Visible;
                    main.txbIncreasingPercent.Foreground = (Brush)new BrushConverter().ConvertFrom("#DE3E44");
                    percent = Math.Abs(percent);
                }
            }
            IncreasingPercent = percent.ToString() + "%";
        }
        void SelectYear(MainWindow main)
        {
            SelectPeriod(main);
        }
        void SelectPeriod(MainWindow main)
        {
            if (main.cboSelectYear.SelectedIndex == -1)
                return;
            itemSourceTime.Clear();
            main.cboSelectTime.Visibility = System.Windows.Visibility.Hidden;
            switch (main.cboSelectPeriod.SelectedIndex)
            {
                case 0:
                    main.cboSelectTime.Visibility = System.Windows.Visibility.Visible;
                    int year = int.Parse(main.cboSelectYear.SelectedItem.ToString().Split(' ')[1]);
                    if (year < DateTime.Now.Year)
                    {
                        for (int i = 1; i <= 12; i++)
                        {
                            itemSourceTime.Add("Tháng " + i.ToString());
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= DateTime.Now.Month; i++)
                        {
                            itemSourceTime.Add("Tháng " + i.ToString());
                        }
                    }
                    break;
                case 1:
                    ShowReportByMonth(main);
                    break;
                case 2:
                    ShowReportByQuarter(main);
                    break;
                default:
                    break;
            }
        }
        void SelectMonth(MainWindow main)
        {
            AxisXTitle = "Ngày";
            labelPoint = chartPoint => Converter.Instance.Nice(chartPoint.Y, 1);
            if (main.cboSelectYear.SelectedIndex == -1)
                return;
            if (main.cboSelectTime.SelectedIndex == -1)
                return;
            string year = main.cboSelectYear.SelectedItem.ToString().Split(' ')[1];
            string month = main.cboSelectTime.SelectedItem.ToString().Split(' ')[1];
            NameChart = string.Format("Doanh thu và chi tiêu theo ngày trong tháng {0}/{1}", month, year);
            Labels.Clear();
            Labels = ReportDAL.Instance.GetDateInMonth(month, year);
            SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Chi tiêu",
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FFF44336"),
                        Values = ReportDAL.Instance.GetSependByDay(month, year, Labels),
                        DataLabels = true,
                        LabelPoint = labelPoint,
                    },
                    new ColumnSeries
                    {
                        Title = "Doanh thu",
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                        Values = ReportDAL.Instance.GetRevenueByDay(month, year, Labels),
                        DataLabels = true,
                        LabelPoint = labelPoint,
                    }
                };
            Formatter = value => Converter.Instance.Nice(value, 1);
        }
        void ShowReportByMonth(MainWindow main)
        {
            AxisXTitle = "Tháng";
            labelPoint = chartPoint => Converter.Instance.Nice(chartPoint.Y, 1);
            if (main.cboSelectYear.SelectedIndex == -1)
                return;
            string year = main.cboSelectYear.SelectedItem.ToString().Split(' ')[1];
            NameChart = "Doanh thu và chi tiêu theo tháng trong năm " + year;
            Labels.Clear();
            Labels = ReportDAL.Instance.GetMonthInYear(year);
            SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Chi tiêu",
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FFF44336"),
                        Values = ReportDAL.Instance.GetSependByMonth(year, Labels),
                        DataLabels = true,
                        LabelPoint = labelPoint,
                    },
                    new ColumnSeries
                    {
                        Title = "Doanh thu",
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                        Values = ReportDAL.Instance.GetRevenueByMonth(year, Labels),
                        DataLabels = true,
                        LabelPoint = labelPoint,
                    }
                };
            Formatter = value => Converter.Instance.Nice(value, 1);
        }
        void ShowReportByQuarter(MainWindow main)
        {
            AxisXTitle = "Quý";
            labelPoint = chartPoint => Converter.Instance.Nice(chartPoint.Y, 1);
            if (main.cboSelectYear.SelectedIndex == -1)
                return;
            string year = main.cboSelectYear.SelectedItem.ToString().Split(' ')[1];
            NameChart = "Doanh thu và chi tiêu theo quý trong năm " + year;
            Labels.Clear();
            Labels = ReportDAL.Instance.GetQuarterInYear(year);
            ; SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Chi tiêu",
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FFF44336"),
                        Values = ReportDAL.Instance.GetSependByQuarter(year, Labels),
                        DataLabels = true,
                        LabelPoint = labelPoint,
                    },
                    new ColumnSeries
                    {
                        Title = "Doanh thu",
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF1976D2"),
                        Values = ReportDAL.Instance.GetRevenueByQuarter(year, Labels),
                        DataLabels = true,
                        LabelPoint = labelPoint,
                    }
                };
            Formatter = value => Converter.Instance.Nice(value, 1);
        }
        void SetItemSourceYear()
        {
            itemSourceYear.Clear();
            for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 5; i--)
            {
                itemSourceYear.Add("Năm " + i.ToString());
            }
        }
        void LoadBestSeller(MainWindow main)
        {
            main.stkBestSeller.Children.Clear();
            DataTable dt = ReportDAL.Instance.GetBestSeller();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BestSellerControl control = new BestSellerControl();
                Goods goods = GoodsDAL.Instance.GetById(dt.Rows[i].ItemArray[0].ToString());
                long importPrice = goods.ImportPrice;
                double profitPercentage = GoodsTypeDAL.Instance.GetById(goods.IdGoodsType).ProfitPercentage;
                long salePrice = (long)Math.Ceiling(importPrice * (1 + profitPercentage));

                control.txbNumeralOrder.Text = (i + 1).ToString();
                control.txbName.Text = goods.Name;
                control.txbQuantity.Text = dt.Rows[i].ItemArray[1].ToString();
                control.txbPrice.Text = string.Format("{0:N0}", salePrice);

                main.stkBestSeller.Children.Add(control);
            }
        }

        void SelectPeriodPieChart(MainWindow main)
        {
            labelPoint = chartPoint => Converter.Instance.Nice(chartPoint.Y, 1);
            if (main.cboSelectTimePie.SelectedIndex == 0)
            {
                NamePieChart = String.Format("Thống kê doanh thu hôm nay({0})", DateTime.Today.Date.ToString("dd/MM/yyyy"));
                PieSeriesCollection = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Bán hàng",
                        Values = ReportDAL.Instance.GetSalesRevenueToday(),
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF00329E"),
                        DataLabels = true,
                        FontSize = 14,
                        LabelPoint = labelPoint,
                    },
                    new PieSeries
                    {
                        Title="Dịch vụ",
                        Values = ReportDAL.Instance.GetServiceRevenueToday(),
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF01B500"),
                        DataLabels = true,
                        FontSize = 14,
                       LabelPoint = labelPoint,
                    },
                };
            }
            else if (main.cboSelectTimePie.SelectedIndex == 1)
            {
                var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
                var diff = DateTime.Now.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
                if (diff < 0)
                    diff += 7;
                string starOftWeek = DateTime.Now.AddDays(-diff).Day.ToString();

                NamePieChart = String.Format("Thống kê doanh thu tuần nay({0}-{1})", starOftWeek, DateTime.Now.Date.ToString("dd/MM/yyyy"));
                PieSeriesCollection = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Bán hàng",
                        Values = ReportDAL.Instance.GetSalesRevenueThisWeek(),
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF00329E"),
                        DataLabels = true,
                        FontSize = 16,
                        LabelPoint = labelPoint,
                    },
                    new PieSeries
                    {
                        Title="Dịch vụ",
                        Values = ReportDAL.Instance.GetServiceRevenueThisWeek(),
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF01B500"),
                        DataLabels = true,
                        FontSize = 16,
                        LabelPoint = labelPoint,
                    },
                };
            }
            else
            {
                NamePieChart = String.Format("Thống kê doanh thu tháng nay({0})", DateTime.Today.Date.ToString("MM/yyyy"));
                PieSeriesCollection = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Bán hàng",
                        Values = ReportDAL.Instance.GetSalesRevenueThisMonth(),
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF00329E"),
                        DataLabels = true,
                        FontSize = 16,
                        LabelPoint = labelPoint,
                    },
                    new PieSeries
                    {
                        Title="Dịch vụ",
                        Values = ReportDAL.Instance.GetServiceRevenueThisMonth(),
                        Fill = (Brush)new BrushConverter().ConvertFrom("#FF01B500"),
                        DataLabels = true,
                        FontSize = 16,
                        LabelPoint = labelPoint,
                    },
                };
            }
        }
    }
}
