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
    class BillDAL : Connection
    {
        private static BillDAL instance;

        public static BillDAL Instance
        {
            get { if (instance == null) instance = new BillDAL(); return BillDAL.instance; }
            private set { BillDAL.instance = value; }
        }

        private BillDAL()
        {

        }
        public SortedList<int, int> GetSoldDataAgo(string month, string year)
        {
            try
            {
                OpenConnection();

                string queryStr = String.Format("select idGoods, sum(quantity) " +
                    "from bill join billinfo " +
                    "on bill.idBill = billinfo.idBill " +
                    "where year(invoiceDate) = {0} and month(invoiceDate) < {1} " +
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
        public SortedList<int, int> GetSoldDataByMonth(string month, string year)
        {
            try
            {
                OpenConnection();

                string queryStr = String.Format("select idGoods, sum(quantity) " +
                    "from bill join billinfo " +
                    "on bill.idBill = billinfo.idBill " +
                    "where year(invoiceDate) = {0} and month(invoiceDate) = {1} " +
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
