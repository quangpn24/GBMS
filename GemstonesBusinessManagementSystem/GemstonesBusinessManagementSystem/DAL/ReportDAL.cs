using GemstonesBusinessManagementSystem.Models;
using LiveCharts;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GemstonesBusinessManagementSystem.DAL
{
    class ReportDAL : Connection
    {
        private static ReportDAL instance;

        public static ReportDAL Instance
        {
            get { if (instance == null) instance = new ReportDAL(); return instance; }
            private set { ReportDAL.instance = value; }
        }

        //Column Chart
        public ChartValues<long> GetRevenueByQuarter(string year, List<string> quarterInYear)
        {
            try
            {
                OpenConnection();
                long[] revenues = new long[quarterInYear.Count];
                for (int i = 0; i < quarterInYear.Count; i++)
                {
                    revenues[i] = 0;
                }
                string query = string.Format("Select quarter, sum(total) from " +
                    "(select quarter(invoiceDate) as quarter, sum(totalMoney) as total from Bill where year(invoiceDate) = {0} "
                    + " group by quarter " +
                    "union all " +
                    "select quarter(createdDate) as quarter, sum(total) as total from BillService where year(createdDate) = {0} " +
                    "group by quarter) " +
                    "Expediture group by quarter order by quarter", year);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int index = quarterInYear.FindIndex(x => x == reader.GetString(0));
                    revenues[index] = long.Parse(reader.GetString(1));
                }
                return new ChartValues<long>(revenues);
            }
            catch
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public ChartValues<long> GetSependByQuarter(string year, List<string> quarterInYear)
        {
            try
            {
                OpenConnection();
                long[] revenues = new long[quarterInYear.Count];
                for (int i = 0; i < quarterInYear.Count; i++)
                {
                    revenues[i] = 0;
                }
                string query = string.Format("select quarter(receiptDate) as quarter,  sum(total) from StockReceipt where year(receiptDate) = {0} "
                     + " group by quarter order by quarter ", year);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int index = quarterInYear.FindIndex(x => x == reader.GetString(0));
                    revenues[index] = long.Parse(reader.GetString(1));
                }
                return new ChartValues<long>(revenues);
            }
            catch
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public ChartValues<long> GetRevenueByMonth(string year, List<string> monthInYear)
        {
            try
            {
                OpenConnection();
                long[] revenues = new long[monthInYear.Count];
                for (int i = 0; i < monthInYear.Count; i++)
                {
                    revenues[i] = 0;
                }
                string query = string.Format("Select month, sum(total) from " +
                    "(select month(invoiceDate) as month, sum(totalMoney) as total from Bill where year(invoiceDate) = {0} "
                    + " group by month " +
                    "union all " +
                    "select month(createdDate) as month, sum(total) as total from BillService where year(createdDate) = {0} " +
                    "group by month) " +
                    "Expediture group by month order by month", year);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int index = monthInYear.FindIndex(x => x == reader.GetString(0));
                    revenues[index] = long.Parse(reader.GetString(1));
                }
                return new ChartValues<long>(revenues);
            }
            catch
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public ChartValues<long> GetSependByMonth(string year, List<string> monthInYear)
        {
            try
            {
                long[] revenues = new long[monthInYear.Count];
                for (int i = 0; i < monthInYear.Count; i++)
                {
                    revenues[i] = 0;
                }
                OpenConnection();
                string query = string.Format("select month(receiptDate) as month,  sum(total) from StockReceipt where year(receiptDate) = {0} "
                    + " group by month order by month ", year);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int index = monthInYear.FindIndex(x => x == reader.GetString(0));
                    revenues[index] = long.Parse(reader.GetString(1));
                }
                return new ChartValues<long>(revenues);
            }
            catch
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public ChartValues<long> GetRevenueByDay(string month, string year, List<string> dateInMonth)
        {
            try
            {
                OpenConnection();
                long[] revenues = new long[dateInMonth.Count];
                for (int i = 0; i < dateInMonth.Count; i++)
                {
                    revenues[i] = 0;
                }
                string query = string.Format("Select date, sum(total) from " +
                    "(select day(invoiceDate) as date, sum(totalMoney) as total from Bill where year(invoiceDate) = {0} and month(invoiceDate) = {1} "
                    + "group by date " +
                    "union all " +
                    "select day(createdDate) as date, sum(total) as total from BillService where year(createdDate) = {0} and month(createdDate) = {1} " +
                    "group by date) " +
                    "Expediture group by date order by date", year, month);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int index = dateInMonth.FindIndex(x => x == reader.GetString(0));
                    revenues[index] = long.Parse(reader.GetString(1));
                }
                return new ChartValues<long>(revenues);
            }
            catch
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public ChartValues<long> GetSependByDay(string month, string year, List<string> dateInMonth)
        {
            try
            {
                long[] revenues = new long[dateInMonth.Count];
                for (int i = 0; i < dateInMonth.Count; i++)
                {
                    revenues[i] = 0;
                }
                OpenConnection();
                string query = string.Format("select day(receiptDate) as date, sum(total) from StockReceipt where month(receiptDate) = {0} "
                    + " and year(receiptDate) = {1} group by date order by date", month, year);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int index = dateInMonth.FindIndex(x => x == reader.GetString(0));
                    revenues[index] = long.Parse(reader.GetString(1));
                }
                return new ChartValues<long>(revenues);
            }
            catch
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }

        //Get labels
        public List<string> GetDateInMonth(string month, string year)
        {
            try
            {
                OpenConnection();
                List<string> dates = new List<string>();
                string query = String.Format("select day(invoiceDate) as date from Bill where month(invoiceDate) = {0} and year(invoiceDate) = {1} " +
                    "group by date " +
                    "union " +
                    "select day(createdDate) as date from BillService where month(createdDate) = {0} and year(createdDate) = {1} " +
                    "group by date " +
                    "union " +
                    "select day(receiptDate) as date from StockReceipt where month(receiptDate) = {0} and year(receiptDate) = {1} " +
                    "group by date order by date", month, year);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dates.Add(reader.GetString(0));
                }
                return dates;
            }
            catch
            {
                return new List<string>();
            }
            finally
            {
                CloseConnection();
            }
        }

        public List<string> GetMonthInYear(string year)
        {
            try
            {
                OpenConnection();
                List<string> dates = new List<string>();
                string query = String.Format("select month(invoiceDate) as month from Bill where year(invoiceDate) = {0} " +
                    "group by month " +
                    "union " +
                    "select month(createdDate) as month from BillService where year(createdDate) = {0} " +
                    "group by month " +
                    "union " +
                    "select month(receiptDate) as month from StockReceipt where year(receiptDate) = {0} " +
                    "group by month order by month", year);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dates.Add(reader.GetString(0));
                }
                return dates;
            }
            catch
            {
                return new List<string>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<string> GetQuarterInYear(string year)
        {
            try
            {
                OpenConnection();
                List<string> dates = new List<string>();
                string query = String.Format("select quarter(invoiceDate) as quarter from Bill where year(invoiceDate) = {0} " +
                    "group by quarter " +
                    "union " +
                    "select quarter(createdDate) as quarter from BillService where year(createdDate) = {0} " +
                    "group by quarter " +
                    "union " +
                    "select quarter(receiptDate) as quarter from StockReceipt where year(receiptDate) = {0} " +
                    "group by quarter order by quarter", year);
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dates.Add(reader.GetString(0));
                }
                return dates;
            }
            catch
            {
                return new List<string>();
            }
            finally
            {
                CloseConnection();
            }
        }
        //dashboard
        public long GetTodayRevenue()
        {
            try
            {
                OpenConnection();
                string query = string.Format("select sum(total) as total, date from " +
                    "(select sum(totalMoney) as total,  date(invoiceDate) as date from Bill where date(invoiceDate) = '{0}' group by date " +
                    "union all " +
                    "select sum(total) as total, date(createdDate) as date from BillService where date(createdDate) = '{0}' group by date) " +
                    " Expediture group by date ", DateTime.Today.Date.ToString("yyyy-MM-dd"));
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                long todayRevenue = 0;
                if (reader.HasRows && !reader.IsDBNull(0))
                    todayRevenue = long.Parse(reader.GetString(0));
                return todayRevenue;
            }
            catch
            {
                return -1;
            }
            finally
            {
                CloseConnection();
            }
        }
        public long GetTodaySpend()
        {
            try
            {
                OpenConnection();
                long todaySpend = 0;
                string query = string.Format("Select sum(total) from StockReceipt where date(receiptDate) = '{0}' ", DateTime.Now.Date.ToString("yyyyy-MM-dd"));
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (!reader.IsDBNull(0))
                    todaySpend += long.Parse(reader.GetString(0));
                return todaySpend;
            }
            catch
            {
                return 0;
            }
            finally
            {
                CloseConnection();
            }
        }
        public int GetTodayBillQuantity()
        {
            try
            {
                OpenConnection();
                string query = string.Format("select sum(quantity) as sum1, date from " +
                    "(select count(*) as quantity ,  date(invoiceDate) as date from Bill where date(invoiceDate) = '{0}' " +
                    "group by date " +
                    "union all " +
                    "select count(*) as quantity, date(createdDate) as date from BillService where date(createdDate) = '{0}' " +
                    "group by date)" +
                    " Expediture group by date ", DateTime.Today.Date.ToString("yyyy-MM-dd"));
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int quantity = 0;
                reader.Read();
                if (reader.HasRows && !reader.IsDBNull(0))
                    quantity = int.Parse(reader.GetString(0));
                return quantity;
            }
            catch
            {
                return -1;
            }
            finally
            {
                CloseConnection();
            }
        }
        public long GetYesterdayRenvenue()
        {
            try
            {
                OpenConnection();
                string query = string.Format("select sum(total) as total, date from " +
                    "(select sum(totalMoney) as total,  date(invoiceDate) as date from Bill where date(invoiceDate) = subdate('{0}', 1) group by date " +
                    "union all " +
                    "select sum(total) as total, date(createdDate) as date from BillService where date(createdDate) = subdate('{0}', 1) group by date) " +
                    " Expediture group by date ", DateTime.Today.Date.ToString("yyyy-MM-dd"));
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int quantity = 0;
                reader.Read();
                if (!reader.IsDBNull(0))
                    quantity = int.Parse(reader.GetString(0));
                return quantity;
            }
            catch
            {
                return -1;
            }
            finally
            {
                CloseConnection();
            }
        }

        //Pie chart
        public ChartValues<long> GetSalesRevenueToday()
        {
            try
            {
                List<long> salesRevenueToday = new List<long>();
                OpenConnection();
                string query = string.Format("select date(invoiceDate) as date, sum(totalMoney) from Bill where date(invoiceDate) = '{0}'"
                   + " group by date", DateTime.Today.Date.ToString("yyyy-MM-dd"));
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (!reader.IsDBNull(1))
                    salesRevenueToday.Add(long.Parse(reader.GetString(1)));
                return new ChartValues<long>(salesRevenueToday);
            }
            catch
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public ChartValues<long> GetServiceRevenueToday()
        {
            try
            {
                List<long> serviceRevenueToday = new List<long>();
                OpenConnection();
                string query = string.Format("select date(createdDate) as date, sum(total) from BillService where date(createdDate) = '{0}'"
                   + " group by date", DateTime.Today.Date.ToString("yyyy-MM-dd"));
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (!reader.IsDBNull(1))
                    serviceRevenueToday.Add(long.Parse(reader.GetString(1)));
                return new ChartValues<long>(serviceRevenueToday);
            }
            catch
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public ChartValues<long> GetSalesRevenueThisWeek()
        {
            try
            {
                List<long> salesRevenueThisWeek = new List<long>();
                OpenConnection();
                string query = "select week(invoiceDate, 1) as week, sum(totalMoney) from Bill where week(invoiceDate, 1) = week(now(), 1) "
                   + " group by week";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (!reader.IsDBNull(1))
                    salesRevenueThisWeek.Add(long.Parse(reader.GetString(1)));
                return new ChartValues<long>(salesRevenueThisWeek);
            }
            catch
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public ChartValues<long> GetServiceRevenueThisWeek()
        {
            try
            {
                List<long> serviceRevenueToday = new List<long>();
                OpenConnection();
                string query = "select week(createdDate,1) as week, sum(total) from BillService where week(createdDate, 1) = week(now(), 1) "
                   + " group by week";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (!reader.IsDBNull(1))
                    serviceRevenueToday.Add(long.Parse(reader.GetString(1)));
                return new ChartValues<long>(serviceRevenueToday);
            }
            catch (Exception)
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public ChartValues<long> GetSalesRevenueThisMonth()
        {
            try
            {
                List<long> salesRevenueToday = new List<long>();
                OpenConnection();
                string query = "select month(invoiceDate) as month, sum(totalMoney) from Bill where month(invoiceDate) = month(now()) "
                  + " group by month";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (!reader.IsDBNull(1))
                    salesRevenueToday.Add(long.Parse(reader.GetString(1)));
                return new ChartValues<long>(salesRevenueToday);
            }
            catch
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public ChartValues<long> GetServiceRevenueThisMonth()
        {
            try
            {
                List<long> serviceRevenueToday = new List<long>();
                OpenConnection();
                string query = "select month(createdDate) as month, sum(total) from BillService where month(createdDate) = month(now()) "
                   + " group by month";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                if (!reader.IsDBNull(1))
                    serviceRevenueToday.Add(long.Parse(reader.GetString(1)));
                return new ChartValues<long>(serviceRevenueToday);
            }
            catch (Exception)
            {
                return new ChartValues<long>();
            }
            finally
            {
                CloseConnection();
            }
        }
        //Best seller
        public DataTable GetBestSeller()
        {
            try
            {
                DataTable dt = new DataTable();
                OpenConnection();
                string query = "Select BillInfo.idGoods, sum(BillInfo.quantity) as quantity from BillInfo Join Goods " +
                    "on Goods.idGoods=BillInfo.idGoods where Goods.isDeleted=0 " +
                    "group by idGoods " +
                    "order by quantity DESC, price DESC " +
                    "limit 10";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                return dt;
            }
            catch
            {
                return new DataTable();
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
