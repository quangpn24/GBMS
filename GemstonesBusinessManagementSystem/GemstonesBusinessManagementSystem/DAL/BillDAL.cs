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
        public bool Insert(Bill bill)
        {
            try
            {
                OpenConnection();
                string query = "insert into Bill (idBill,idAccount,invoiceDate,totalMoney,idCustomer,note) " +
                    "values(@idBill,@idAccount,@invoiceDate,@totalMoney,@idCustomer,@note)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idBill", bill.IdBill);
                cmd.Parameters.AddWithValue("@idAccount", bill.IdAccount);
                cmd.Parameters.AddWithValue("@invoiceDate", bill.InvoiceDate);
                cmd.Parameters.AddWithValue("@totalMoney", bill.TotalMoney);
                cmd.Parameters.AddWithValue("@idCustomer", bill.IdCustomer);
                cmd.Parameters.AddWithValue("@note", bill.Note);

                return cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
                return false;
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
                string queryString = "select max(idBill) from Bill";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataReader rdr = command.ExecuteReader();
                rdr.Read();
                int maxId = int.Parse(rdr.GetString(0));
                return maxId;
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
        public Bill GetBill(string idBill)
        {
            try
            {
                OpenConnection();
                string query = "select * from Bill where idBill = " + idBill;

                MySqlCommand command = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                Bill res;
                res = new Bill(int.Parse(idBill), int.Parse(dataTable.Rows[0].ItemArray[1].ToString()), DateTime.Parse(dataTable.Rows[0].ItemArray[2].ToString()),
                    long.Parse(dataTable.Rows[0].ItemArray[3].ToString()), int.Parse(dataTable.Rows[0].ItemArray[4].ToString()), dataTable.Rows[0].ItemArray[5].ToString());

                return res;
            }
            catch
            {
                return new Bill();
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<Bill> GetByDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                OpenConnection();
                string start = startDate.ToString("yyyy-MM-dd");
                string end = endDate.ToString("yyyy-MM-dd");
                string query = string.Format("select * from Bill where invoiceDate >= '{0}' and invoiceDate <= '{1}';", start, end);

                MySqlCommand command = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                List<Bill> res = new List<Bill>();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Bill temp;

                    temp = new Bill(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), int.Parse(dataTable.Rows[i].ItemArray[1].ToString()), DateTime.Parse(dataTable.Rows[i].ItemArray[2].ToString()),
                    long.Parse(dataTable.Rows[i].ItemArray[3].ToString()), int.Parse(dataTable.Rows[i].ItemArray[4].ToString()), dataTable.Rows[i].ItemArray[5].ToString());

                    res.Add(temp);
                }
                return res;
            }
            catch
            {
                return new List<Bill>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public SortedList<int, int> GetSoldDataAgo(string month, string year)
        {
            try
            {
                OpenConnection();

                string queryStr = String.Format("select idGoods, sum(quantity) " +
                    "from bill join billinfo " +
                    "on bill.idBill = billinfo.idBill " +
                    "where year(invoiceDate) < {0} or (year(invoiceDate) < {0} and month(invoiceDate) < {1}) " +
                    "and idGoods in (select idGoods from goods) " +
                    "group by idGoods", year, month);
                if (month == "1")
                {
                    queryStr = String.Format("select idGoods, sum(quantity) " +
                        "from bill join billinfo " +
                        "on bill.idBill = billinfo.idBill " +
                        "where year(invoiceDate) < {0} " +
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
