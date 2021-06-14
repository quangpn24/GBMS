using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GemstonesBusinessManagementSystem.DAL
{
    public class Connection
    {
        private string strCon;
        public MySqlConnection conn;
        public Connection()
        {
            try
            {
                strCon = ConfigurationManager.ConnectionStrings["DBMS"].ConnectionString;
            }
            catch
            {
                return;
            }
            conn = new MySqlConnection(strCon);
        }
        public void OpenConnection()
        {
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["DBMS"].ConnectionString;
                    conn.Open();
                }
            }
            catch
            {
                conn.Close();
                MessageBox.Show("Mất kết nối đến cơ sở dữ liệu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void CloseConnection()
        {
            try
            {
                conn.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
