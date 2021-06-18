using GemstonesBusinessManagementSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<BillInfo> GetBillInfos(string idBill)
        {
            List<BillInfo> billInfos = new List<BillInfo>();
            try
            {
                OpenConnection();
                string queryString = "SELECT * FROM billinfo WHERE idBill=" + idBill;
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    BillInfo billInfo = new BillInfo(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()),
                        int.Parse(dataTable.Rows[i].ItemArray[1].ToString()), double.Parse(dataTable.Rows[i].ItemArray[2].ToString()),
                        int.Parse(dataTable.Rows[i].ItemArray[3].ToString()), 0);
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
    }
}
