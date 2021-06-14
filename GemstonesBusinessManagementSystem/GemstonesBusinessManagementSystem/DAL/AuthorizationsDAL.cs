using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using GemstonesBusinessManagementSystem.Models;
using MySql.Data.MySqlClient;

namespace GemstonesBusinessManagementSystem.DAL
{
    class AuthorizationsDAL : Connection
    {
        private static AuthorizationsDAL instance;

        public static AuthorizationsDAL Instance
        {
            get { if (instance == null) instance = new AuthorizationsDAL(); return AuthorizationsDAL.instance; }
            private set { AuthorizationsDAL.instance = value; }
        }
        public bool CheckData(string authKey)
        {
            DataTable dt = new DataTable();
            //List<Authorizations> authorizations = new List<Authorizations>();
            try
            {
                OpenConnection();
                string query = "select * from authorizations where authkey = '" + authKey + "'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    return true;
                }
                else 
                {
                    return false;
                }
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
