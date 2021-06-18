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
    class PermissionDAL : Connection
    {
        private static PermissionDAL instance;

        public static PermissionDAL Instance
        {
            get { if (instance == null) instance = new PermissionDAL(); return PermissionDAL.instance; }
            private set { PermissionDAL.instance = value; }
        }
        public List<Permission> GetList()
        {
            try
            {
                OpenConnection();

                string queryStr = "select * from Permission";
                MySqlCommand cmd = new MySqlCommand(queryStr, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dataReader);

                List<Permission> permissionList = new List<Permission>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Permission permission = new Permission(int.Parse(dt.Rows[i].ItemArray[0].ToString()), 
                        dt.Rows[i].ItemArray[1].ToString());
                    permissionList.Add(permission);
                }
                return permissionList;
            }
            catch
            {
                return new List<Permission>();
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
