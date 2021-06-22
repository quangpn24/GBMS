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

                string queryStr = "Select * from Employee where idEmployee != 0";
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

                string queryStr = "select * from Employee where isDeleted = false and idEmployee != 0";
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
                        "(idEmployee,name,gender,phonenumber,address,dateofBirth,idPosition,startingdate,imageFile,isDeleted) " +
                        "values(@idEmployee,@name,@gender,@phoneNumber,@address,@dateofBirth,@idPosition,@startingdate,@imageFile,@isDeleted)";
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
            finally
            {
                CloseConnection();
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
                MySqlDataReader reader = command.ExecuteReader();
                reader.Read();
                int idAccount = -1;
                int idPosition = 0;
                byte[] imgArr = null;
                if (!reader.IsDBNull(8))
                {
                    idAccount = int.Parse(reader.GetString(8));
                }
                if (!reader.IsDBNull(6))
                {
                    idPosition = int.Parse(reader.GetString(6));
                }
                if (!reader.IsDBNull(9))
                {
                    imgArr = Convert.FromBase64String(reader.GetString(9));
                }
                Employee employee = new Employee(int.Parse(reader.GetString(0)), reader.GetString(1),
                    reader.GetString(2), reader.GetString(3), reader.GetString(4),
                     DateTime.Parse(reader.GetString(5)), idPosition, DateTime.Parse(reader.GetString(7)),
                     idAccount, imgArr);
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
                string queryString = @"select * from Employee where name like  ""%" + name + "%\" and isDeleted = 0 and idEmployee != 0";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dt = new DataTable();
                List<Employee> employeeList = new List<Employee>();
                adapter.Fill(dt);
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
        public bool UpdateIdAccount(int idAccount, int idEmployee)
        {
            try
            {
                OpenConnection();
                string query = "update employee set idAccount = @idAccount where idEmployee = @idEmployee";

                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@idAccount", idAccount.ToString());
                command.Parameters.AddWithValue("@idEmployee", idEmployee.ToString());
                return command.ExecuteNonQuery() == 1;
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
        public List<Employee> GetEmployeeNonAccount()   // lấy list nhân viên không có Account
        {
            List<Employee> employees = new List<Employee>();

            OpenConnection();
            string query = "select * from Employee where idAccount is null";

            MySqlCommand command = new MySqlCommand(query, conn);
            command.ExecuteNonQuery();
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            MySqlDataReader dataReader = command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dataReader);

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

        public Employee GetByIdAccount(string idAccount)
        {
            try
            {
                OpenConnection();
                string query = "select * from Employee where idAccount = " + idAccount;

                MySqlCommand command = new MySqlCommand(query, conn);
                command.ExecuteNonQuery();
                MySqlDataReader reader = command.ExecuteReader();
                reader.Read();
                int idPosition = 0;
                byte[] imgArr = null;
                if (!reader.IsDBNull(6))
                {
                    idPosition = int.Parse(reader.GetString(6));
                }
                if (!reader.IsDBNull(9))
                {
                    imgArr = Convert.FromBase64String(reader.GetString(9));
                }
                Employee employee = new Employee(int.Parse(reader.GetString(0)), reader.GetString(1),
                    reader.GetString(2), reader.GetString(3), reader.GetString(4),
                     DateTime.Parse(reader.GetString(5)), idPosition, DateTime.Parse(reader.GetString(7)),
                     int.Parse(idAccount), imgArr);
                return employee;
            }
            catch
            {
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool UpdateUserInfo(Employee employee)
        {
            try
            {
                OpenConnection();
                string query = "update Employee set name=@name,gender=@gender,phonenumber=@phonenumber,address=@address," +
                        "dateofBirth=@dateofBirth, imageFile=@imageFile where idEmployee=" + employee.IdEmployee;

                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@name", employee.Name);
                cmd.Parameters.AddWithValue("@gender", employee.Gender);
                cmd.Parameters.AddWithValue("@phoneNumber", employee.PhoneNumber);
                cmd.Parameters.AddWithValue("@address", employee.Address);
                cmd.Parameters.AddWithValue("@dateofBirth", employee.DateOfBirth);
                cmd.Parameters.AddWithValue("@imageFile", Convert.ToBase64String(employee.ImageFile));

                int row = cmd.ExecuteNonQuery();
                if (row != 1)
                {
                    throw new Exception();
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
