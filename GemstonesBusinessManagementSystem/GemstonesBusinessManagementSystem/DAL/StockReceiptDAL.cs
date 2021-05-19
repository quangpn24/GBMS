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
                conn.Open();
                string query = "insert into StockReceipt(idStockReceipt, idAccount, dateTimeStockReceipt, total, idSupplier) "
                    + "values(@idStockReceipt,@idAccount, str_to_date(@dateTimeStockReceipt,'%d/%m/%Y'), @total, @idSupplier)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                string s = stockReceipt.Date.ToString("dd/MM/yyyy");
                cmd.Parameters.AddWithValue("@idStockReceipt", stockReceipt.Id.ToString());
                cmd.Parameters.AddWithValue("@idAccount", stockReceipt.IdAccount.ToString());
                cmd.Parameters.AddWithValue("@dateTimeStockReceipt", s);
                cmd.Parameters.AddWithValue("@total", stockReceipt.TotalMoney.ToString());
                cmd.Parameters.AddWithValue("@idSupplier", stockReceipt.IdSupplier.ToString());
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


        public int GetMaxId()
        {
            try
            {
                conn.Open();
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
                conn.Close();
            }
        }
    }
}
