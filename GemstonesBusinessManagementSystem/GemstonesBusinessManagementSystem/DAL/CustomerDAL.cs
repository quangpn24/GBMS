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
    class CustomerDAL : Connection
    {
        private static CustomerDAL instance;

        public static CustomerDAL Instance
        {
            get { if (instance == null) instance = new CustomerDAL(); return CustomerDAL.instance; }
            private set { CustomerDAL.instance = value; }
        }

        private CustomerDAL()
        {

        }

        public DataTable LoadData()
        {
            try
            {
                DataTable dt = new DataTable();
                OpenConnection();
                string query = "select * from customer";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                return dt;
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


        public List<Customer> ConvertDBToList()
        {
            DataTable dt = new DataTable();
            List<Customer> customers = new List<Customer>();
            try
            {
                OpenConnection();
                string queryString = "select * from customer";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                adapter.Fill(dt);
            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                Customer customer = new Customer(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(), dt.Rows[i].ItemArray[3].ToString(),
                    int.Parse(dt.Rows[i].ItemArray[4].ToString()), long.Parse(dt.Rows[i].ItemArray[5].ToString()), int.Parse(dt.Rows[i].ItemArray[6].ToString()));
                customers.Add(customer);
            }
            return customers;
        }   

        public List<Customer>GetListByIdMembership(int id)
        {
            DataTable dt = new DataTable();
            List<Customer> customers = new List<Customer>();
            try
            {
                OpenConnection();
                string queryString = "select * from customer where idMembership = " + id;
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                adapter.Fill(dt);
            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Customer customer = new Customer(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(), (dt.Rows[i].ItemArray[3].ToString()),
                    int.Parse(dt.Rows[i].ItemArray[4].ToString()), long.Parse(dt.Rows[i].ItemArray[5].ToString()), int.Parse(dt.Rows[i].ItemArray[6].ToString()));
                customers.Add(customer);
            }
            return customers;
        }
        public List<Customer> FindByName(string name)
        {
            DataTable dt = new DataTable();
            List<Customer> customers = new List<Customer>();
            try
            {
                OpenConnection();
                string queryString= @"select * from customer where customerName like ""%" + name + "%\"";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                adapter.Fill(dt);

            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Customer customer = new Customer(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(), (dt.Rows[i].ItemArray[3].ToString()),
                    int.Parse(dt.Rows[i].ItemArray[4].ToString()), long.Parse(dt.Rows[i].ItemArray[5].ToString()), int.Parse(dt.Rows[i].ItemArray[6].ToString()));
                customers.Add(customer);
            }
            return customers;
        }

        public int GetMaxId()
        {
            try
            {
                OpenConnection();
                string queryString = "select max(idCustomer) from Customer";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataReader rd = command.ExecuteReader();
                rd.Read();
                int maxId = int.Parse(rd.GetString(0));
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
        public bool IsExisted(string idNumber)  // kiểm tra thông tin số CMND
        {
            try
            {
                OpenConnection();
                string queryString = "select * from customer where idNumber=@idNumber;";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idNumber", idNumber);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count >= 1)
                    return true;
                else
                    return false;
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
        public bool AddOrUpdate(Customer customer, bool isUpdating)
        {
            try
            {
                OpenConnection();
                string queryString = "";
                if (!isUpdating)
                {
                    queryString = "insert into customer(idCustomer, customerName, phoneNumber, idNumber,totalPrice, idMembership, address)" +
                    "values(@idCustomer, @customerName, @phoneNumber, @idNumber,@totalPrice, @idMembership, @address);";
                }
                else
                {
                    queryString = "update Customer set customerName = @customerName, phoneNumber=@phoneNumber, idNumber=@idNumber," +
                        "totalPrice=@totalPrice, idMembership=@idMembership, address=@address where idCustomer = @idCustomer";
                }               
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idCustomer", customer.IdCustomer);
                command.Parameters.AddWithValue("@customerName", customer.CustomerName);
                command.Parameters.AddWithValue("@phoneNumber", customer.PhoneNumber);
                command.Parameters.AddWithValue("@idNumber", customer.IdCustomer);
                command.Parameters.AddWithValue("@totalPrice", customer.TotalPrice);
                command.Parameters.AddWithValue("@idMembership", customer.IdMembership);
                command.Parameters.AddWithValue("@address", customer.Address);
                int rs = command.ExecuteNonQuery();
                if(rs == 1)
                {
                    MessageBox.Show("Thành công!!!", "Thông báo");
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
        public bool Update(Customer customer)
        {
            try
            {
                OpenConnection();
                string queryString = "update customer set " +
                    "customerName = @customerName, phoneNumber = @phoneNumber, idNumber = @idNumber where idCustomer = @idCustomer; ";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idCustomer", customer.IdCustomer.ToString());
                command.Parameters.AddWithValue("@customerName", customer.CustomerName.ToString());
                command.Parameters.AddWithValue("@phoneNumber", customer.PhoneNumber.ToString());
                command.Parameters.AddWithValue("@idNumber", customer.IdCustomer.ToString());
                command.ExecuteNonQuery();
                return true;
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

        public Customer FindById(string idCustomer)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection();
                string queryString = @"SELECT * FROM Customer WHERE idCustomer=" + idCustomer;
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                adapter.Fill(dt);
                Customer customer = new Customer(int.Parse(dt.Rows[0].ItemArray[0].ToString()),
                    dt.Rows[0].ItemArray[1].ToString(), dt.Rows[0].ItemArray[2].ToString(), (dt.Rows[0].ItemArray[3].ToString()),
                    int.Parse(dt.Rows[0].ItemArray[4].ToString()), long.Parse(dt.Rows[0].ItemArray[5].ToString()), int.Parse(dt.Rows[0].ItemArray[6].ToString()));
                return customer;
            }
            catch
            {
                return new Customer();
            }
            finally
            {
                CloseConnection();
            }
        }
        public int CountPrice()  // tinh tong doanh thu cua tat ca khach hang
        {
            try
            {
                OpenConnection();
                string queryString = "select SUM(totalPrice) from gemstonesbusinessmanagementsystem.customer";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count == 1)
                {
                    return int.Parse(dataTable.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
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
        public bool IsMembership(string idMembership)
        {
            try
            {
                OpenConnection();
                string queryString = "select * from customer where idMembership=@idMembership";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idMembership", idMembership);
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
