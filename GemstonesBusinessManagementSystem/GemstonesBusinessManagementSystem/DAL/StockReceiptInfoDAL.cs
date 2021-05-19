using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GemstonesBusinessManagementSystem.Models;
using MySql.Data.MySqlClient;

namespace GemstonesBusinessManagementSystem.DAL
{
    class StockReceiptInfoDAL : Connection
    {
        private static StockReceiptInfoDAL instance;
        public static StockReceiptInfoDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new StockReceiptInfoDAL();
                return instance;
            }
        }

        public bool Insert(StockReceiptInfo info)
        {
            try
            {
                conn.Open();
                string query = "insert into StockReceiptInfo(idStockReceipt, idGoods, quantity) "
                    + "values(@idStockReceipt,@idGoods, @quantity)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idStockReceipt", info.IdStockReceipt.ToString());
                cmd.Parameters.AddWithValue("@idGoods", info.IdGoods.ToString());
                cmd.Parameters.AddWithValue("@quantity", info.Quantity.ToString());
                return cmd.ExecuteNonQuery() == 1;

            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
