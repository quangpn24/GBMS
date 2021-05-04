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
    class EmployeeDAL : Connection
    {
        private static EmployeeDAL instance;

        public static EmployeeDAL Instance
        {
            get { if (instance == null) instance = new EmployeeDAL(); return EmployeeDAL.instance; }
            private set { EmployeeDAL.instance = value; }
        }

        private EmployeeDAL()
        {

        }

        public List<Employee> SelectAll()
        {
            conn.Open();

            string queryStr = "select * from Employee where isDeleted = false";
            MySqlCommand cmd = new MySqlCommand(queryStr, conn);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dataReader);

            List<Employee> employees = new List<Employee>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int idAccount = -1;
                if (dt.Rows[i].ItemArray[8].ToString() != "")
                {
                    idAccount = int.Parse(dt.Rows[i].ItemArray[8].ToString());
                }

                Employee employee = new Employee(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(),
                    dt.Rows[i].ItemArray[3].ToString(), dt.Rows[i].ItemArray[4].ToString(),
                    DateTime.Parse(dt.Rows[i].ItemArray[5].ToString()),
                    dt.Rows[i].ItemArray[6].ToString(), DateTime.Parse(dt.Rows[i].ItemArray[7].ToString()),
                    idAccount, Convert.FromBase64String(dt.Rows[i].ItemArray[9].ToString()));
                employees.Add(employee);
            }
            return employees;
        }
    }
}
