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
        public List<Customer> ConvertDBToList()
        {
            DataTable dt = new DataTable();
            List<Customer> customers = new List<Customer>();
            try
            {
                OpenConnection();
                string queryString = "SELECT * FROM Customer";

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
                int a = int.Parse(dt.Rows[i].ItemArray[6].ToString());
                Customer customer = new Customer(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(), dt.Rows[i].ItemArray[3].ToString(),
                    dt.Rows[i].ItemArray[4].ToString(), double.Parse(dt.Rows[i].ItemArray[5].ToString()), int.Parse(dt.Rows[i].ItemArray[6].ToString()));
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
                string queryString = @"SELECT * FROM Customer WHERE customerName LIKE ""%" + name + "%\"";
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
                    dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(), dt.Rows[i].ItemArray[3].ToString(),
                    dt.Rows[i].ItemArray[4].ToString(), double.Parse(dt.Rows[i].ItemArray[5].ToString()), int.Parse(dt.Rows[i].ItemArray[6].ToString()));
                customers.Add(customer);
            }
            return customers;
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
                    dt.Rows[0].ItemArray[1].ToString(), dt.Rows[0].ItemArray[2].ToString(), dt.Rows[0].ItemArray[3].ToString(),
                    dt.Rows[0].ItemArray[4].ToString(), double.Parse(dt.Rows[0].ItemArray[5].ToString()), int.Parse(dt.Rows[0].ItemArray[6].ToString()));
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
    }
}
