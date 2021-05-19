using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.DAL
{
    class SupplierDAL : Connection
    {
        private static SupplierDAL instance;

        public static SupplierDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new SupplierDAL();
                return instance;
            }
        }
        public DataTable GetAll()
        {
            try
            {
                conn.Open();
                string query = "select * from Supplier";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                cmd.ExecuteNonQuery();
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
                conn.Close();
            }
        }
    }
}
