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
    class BillInfoDAL : Connection
    {
        private static BillInfoDAL instance;

        public static BillInfoDAL Instance
        {
            get { if (instance == null) instance = new BillInfoDAL(); return BillInfoDAL.instance; }
            private set { BillInfoDAL.instance = value; }
        }
        private BillInfoDAL()
        {

        }
        public bool Insert(BillInfo billInfo)
        {
            try
            {
                OpenConnection();
                string query = "insert into BillInfo (idBill,idGoods,quantity,price) " +
                    "values(@idBill,@idGoods,@quantity,@price)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idBill", billInfo.IdBill);
                cmd.Parameters.AddWithValue("@idGoods", billInfo.IdGoods);
                cmd.Parameters.AddWithValue("@quantity", billInfo.Quantity);
                cmd.Parameters.AddWithValue("@price", billInfo.Price);

                return cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception e)
            {
                CustomMessageBox.Show(e.Message.ToString(), "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        public List<BillInfo> GetBillInfos(string idBill)
        {
            List<BillInfo> billInfos = new List<BillInfo>();
            try
            {
                OpenConnection();
                string queryString = "SELECT * FROM BillInfo WHERE idBill=" + idBill;
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    BillInfo billInfo = new BillInfo(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()),
                        int.Parse(dataTable.Rows[i].ItemArray[1].ToString()), int.Parse(dataTable.Rows[i].ItemArray[2].ToString()),
                        long.Parse(dataTable.Rows[i].ItemArray[3].ToString()));
                    billInfos.Add(billInfo);
                }
                return billInfos;
            }
            catch
            {
                return billInfos;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool Delete(string idBill)
        {
            try
            {
                OpenConnection();
                string queryString = "DELETE FROM BillInfo WHERE idBill = " + idBill;
                MySqlCommand command = new MySqlCommand(queryString, conn);
                return command.ExecuteNonQuery() > 0;
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
    }
}
