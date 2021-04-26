using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
            //QuangPn
            //strCon = "server=localhost;user id=root;password=pnq0326089954;persistsecurityinfo=False;database=gemstonesbusinessmanagementsystem";
            //Trung Huỳnh
            //strCon = "server=localhost;user id=root;password=trunghuynh;persistsecurityinfo=False;database=gemstonesbusinessmanagementsystem";
            conn = new MySqlConnection(strCon);
        }
        public bool OpenConnection()
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                return true;
            }
            try
            {
                conn.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
        public bool CloseConnection()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
