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
                MessageBox.Show(e.Message.ToString());
                return false;
            }
        }
    }
}
