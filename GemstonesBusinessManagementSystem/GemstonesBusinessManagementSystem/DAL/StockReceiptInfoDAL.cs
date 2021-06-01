using System;
using System.Collections.Generic;
using System.Data;
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
                OpenConnection();
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
                CloseConnection();
            }
        }

        public DataTable GetByIdReceipt(int idReceipt)
        {
            try
            {
                OpenConnection();
                string query = "select * from StockReceiptInfo where idStockReceipt = " + idReceipt.ToString();
                MySqlCommand cmd = new MySqlCommand(query, conn);
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
    }
}
