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
            get { if (instance == null) instance = new StockReceiptDAL(); return StockReceiptDAL.instance; }
            private set { StockReceiptDAL.instance = value; }
        }

        private StockReceiptDAL()
        {

        }
        public SortedList<int, int> GetImportData(string month, string year)
        {
            try
            {
                OpenConnection();

                string queryStr = String.Format("select idGoods, sum(quantity) " +
                    "from stockreceipt join stockreceiptinfo " +
                    "on stockreceipt.idStockReceipt = stockreceiptinfo.idStockReceipt " +
                    "where year(dateTimeStockReceipt) = {0} and month(dateTimeStockReceipt) <= {1} " +
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
