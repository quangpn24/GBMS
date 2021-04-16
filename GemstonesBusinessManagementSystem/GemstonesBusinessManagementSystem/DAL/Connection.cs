using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.DAL
{
    public class Connection
    {
        private string strCon;
        public MySqlConnection conn;

        public Connection()
        {
            strCon = "server=localhost;user id=root;persistsecurityinfo=False;database=gemstonesbusinessmanagementsystem";
            conn = new MySqlConnection(strCon);
        }
    }
}
