using GemstonesBusinessManagementSystem.Models;
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
    class StockReceiptDAL : Connection
    {
        private static StockReceiptDAL instance;
        public static StockReceiptDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new StockReceiptDAL();
                return instance;
            }
        }
        public bool Insert(StockReceipt stockReceipt)
        {
            try
            {
                OpenConnection();
                string query = "insert into StockReceipt(idStockReceipt, idAccount, receiptDate, total, discount, idSupplier) "
                    + "values(@idStockReceipt,@idAccount, str_to_date(@receiptDate,'%d/%m/%Y'), @total, @discount, @idSupplier)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                string s = stockReceipt.Date.ToString("dd/MM/yyyy");
                cmd.Parameters.AddWithValue("@idStockReceipt", stockReceipt.Id.ToString());
                cmd.Parameters.AddWithValue("@idAccount", stockReceipt.IdAccount.ToString());
                cmd.Parameters.AddWithValue("@receiptDate", s);
                cmd.Parameters.AddWithValue("@total", stockReceipt.TotalMoney.ToString());
                cmd.Parameters.AddWithValue("@discount", stockReceipt.Discount.ToString());
                cmd.Parameters.AddWithValue("@idSupplier", stockReceipt.IdSupplier.ToString());
                return cmd.ExecuteNonQuery() == 1;
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public int NumOfReceiptsBySupplier(string idSupplier)
        {
            try
            {
                OpenConnection();
                string query = "select count(*) from StockReceipt where idSupplier = " + idSupplier;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return int.Parse(reader.GetString(0));
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
        public long SumMoneyBySupplier(string idSupplier)
        {
            try
            {
                OpenConnection();
                string query = "select sum(total) from StockReceipt where idSupplier = " + idSupplier;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return long.Parse(reader.GetString(0));
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
        public DataTable GetAll()
        {
            try
            {
                OpenConnection();
                string query = "Select * from StockReceipt";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
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
        public int GetMaxId()
        {
            try
            {
                OpenConnection();
                string queryString = "select max(idStockReceipt) from StockReceipt";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (!string.IsNullOrEmpty(dt.Rows[0].ItemArray[0].ToString()))
                {
                    return int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
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
        public SortedList<int, int> GetImportDataAgo(string month, string year)
        {
            try
            {
                OpenConnection();

                string queryStr = String.Format("select idGoods, sum(quantity) " +
                    "from stockreceipt join stockreceiptinfo " +
                    "on stockreceipt.idStockReceipt = stockreceiptinfo.idStockReceipt " +
                    "where year(receiptDate) < {0} or " +
                    "(year(receiptDate) = {0} and month(receiptDate) < {1}) " +
                    "and idGoods in (select idGoods from goods) " +
                    "group by idGoods", year, month);

                if (month == "1")
                {
                    queryStr = String.Format("select idGoods, sum(quantity) " +
                        "from stockreceipt join stockreceiptinfo " +
                        "on stockreceipt.idStockReceipt = stockreceiptinfo.idStockReceipt " +
                        "where year(receiptDate) < {0} " +
                        "and idGoods in (select idGoods from goods) " +
                        "group by idGoods", year);
                }
                MySqlCommand cmd = new MySqlCommand(queryStr, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dataReader);
                if (dt.Rows.Count == 0)
                {
                    return new SortedList<int, int>();
                }
                SortedList<int, int> list = new SortedList<int, int>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()));
                }
                return list;
            }
            catch
            {
                return new SortedList<int, int>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public SortedList<int, int> GetImportDataByMonth(string month, string year)
        {
            try
            {
                OpenConnection();

                string queryStr = String.Format("select idGoods, sum(quantity) " +
                    "from stockreceipt join stockreceiptinfo " +
                    "on stockreceipt.idStockReceipt = stockreceiptinfo.idStockReceipt " +
                    "where year(receiptDate) = {0} and month(receiptDate) = {1} " +
                    "and idGoods in (select idGoods from goods) " +
                    "group by idGoods", year, month);
                MySqlCommand cmd = new MySqlCommand(queryStr, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dataReader);
                if (dt.Rows.Count == 0)
                {
                    return new SortedList<int, int>();
                }
                SortedList<int, int> list = new SortedList<int, int>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(int.Parse(dt.Rows[i].ItemArray[0].ToString()), int.Parse(dt.Rows[i].ItemArray[1].ToString()));
                }
                return list;
            }
            catch
            {
                return new SortedList<int, int>();
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
