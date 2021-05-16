using GemstonesBusinessManagementSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public DataTable GetDatatable()
        {
            try
            {
                OpenConnection();

                string queryStr = "Select * from Employee";
                MySqlCommand cmd = new MySqlCommand(queryStr, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);
                return dataTable;
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
        public List<Employee> GetList()
        {
            try
            {
                OpenConnection();

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
                        int.Parse(dt.Rows[i].ItemArray[6].ToString()), DateTime.Parse(dt.Rows[i].ItemArray[7].ToString()),
                        idAccount, Convert.FromBase64String(dt.Rows[i].ItemArray[9].ToString()));
                    employees.Add(employee);
                }
                return employees;
            }
            catch
            {
                return new List<Employee>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public void InsertOrUpdate(Employee employee, bool isUpdating = false)
        {
            try
            {
                OpenConnection();
                string query = "";
                if (isUpdating)
                {
                    query = "update Employee set name=@name,gender=@gender,phonenumber=@phonenumber,address=@address," +
                        "dateofBirth=@dateofBirth,idPosition=@idPosition,startingdate=@startingdate,imageFile=@imageFile," +
                        "isDeleted=@isDeleted where idEmployee=" + employee.IdEmployee;

                }
                else
                {
                    query = "insert into Employee " +
                        "(idEmployee,name,gender,phonenumber,address,dateofBirth,idPosition,startingdate,idAccount,imageFile,isDeleted) " +
                        "values(@idEmployee,@name,@gender,@phoneNumber,@address,@dateofBirth,@idPosition,@startingdate,@idAccount,@imageFile,@isDeleted)";
                }
                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@idEmployee", employee.IdEmployee);
                cmd.Parameters.AddWithValue("@name", employee.Name);
                cmd.Parameters.AddWithValue("@gender", employee.Gender);
                cmd.Parameters.AddWithValue("@phoneNumber", employee.PhoneNumber);
                cmd.Parameters.AddWithValue("@address", employee.Address);
                cmd.Parameters.AddWithValue("@dateofBirth", employee.DateOfBirth);
                cmd.Parameters.AddWithValue("@idPosition", employee.IdPosition.ToString());
                cmd.Parameters.AddWithValue("@startingdate", employee.StartingDate);
                cmd.Parameters.AddWithValue("@idAccount", employee.IdAccount);
                cmd.Parameters.AddWithValue("@imageFile", Convert.ToBase64String(employee.ImageFile));
                cmd.Parameters.AddWithValue("@isDeleted", employee.IsDeleted);

                int row = cmd.ExecuteNonQuery();
                if (row != 1)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
                return;
            }
        }
        public bool Delete(string idEmployee)
        {
            try
            {
                OpenConnection();
                string query = "update Employee set isDeleted = true where idEmployee = " + idEmployee;
                MySqlCommand command = new MySqlCommand(query, conn);

                return (command.ExecuteNonQuery() > 0);
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
        public Employee GetById(string idEmployee)
        {
            try
            {
                OpenConnection();
                string queryString = "select * from Employee where idEmployee = " + idEmployee;

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                int idAccount = -1;
                if (dt.Rows[0].ItemArray[8].ToString() != "")
                {
                    idAccount = int.Parse(dt.Rows[0].ItemArray[8].ToString());
                }
                Employee employee = new Employee(int.Parse(dt.Rows[0].ItemArray[0].ToString()),
                     dt.Rows[0].ItemArray[1].ToString(), dt.Rows[0].ItemArray[2].ToString(),
                     dt.Rows[0].ItemArray[3].ToString(), dt.Rows[0].ItemArray[4].ToString(),
                     DateTime.Parse(dt.Rows[0].ItemArray[5].ToString()),
                     int.Parse(dt.Rows[0].ItemArray[6].ToString()), DateTime.Parse(dt.Rows[0].ItemArray[7].ToString()),
                     idAccount, Convert.FromBase64String(dt.Rows[0].ItemArray[9].ToString()));
                return employee;
            }
            catch
            {
                return new Employee();
            }
            finally
            {
                CloseConnection();
            }
        }
        public int GetMaxId()
        {
            try
            {
                OpenConnection();
                string queryString = "select max(idEmployee) from Employee";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataReader rdr = command.ExecuteReader();
                rdr.Read();
                int maxId = int.Parse(rdr.GetString(0));
                return maxId;
            }
            catch
            {
                return 0;
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<Employee> FindByName(string name)
        {
            try
            {
                OpenConnection();
                string queryString = @"select * from Employee where name like  ""%" + name + "%\" and isDeleted = 0";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dt = new DataTable();
                List<Employee> employeeList = new List<Employee>();
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Employee employee = new Employee(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                        dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(),
                        dt.Rows[i].ItemArray[3].ToString(), dt.Rows[i].ItemArray[4].ToString(),
                        DateTime.Parse(dt.Rows[i].ItemArray[5].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[6].ToString()), DateTime.Parse(dt.Rows[i].ItemArray[7].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[8].ToString()), Convert.FromBase64String(dt.Rows[i].ItemArray[9].ToString()));
                    employeeList.Add(employee);
                }
                return employeeList;
            }
            catch
            {
                return new List<Employee>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool IsPosition(string idPosition)
        {
            try
            {
                OpenConnection();
                string queryString = "select * from Employee where idPosition=@idPosition";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idPosition", idPosition);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                return dataTable.Rows.Count >= 1;
            }
            catch
            {
                return true;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
