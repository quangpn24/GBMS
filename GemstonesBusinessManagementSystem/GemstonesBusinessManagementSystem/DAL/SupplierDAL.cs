using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GemstonesBusinessManagementSystem.Models;
using System.Windows;

namespace GemstonesBusinessManagementSystem.DAL
{
    class SupplierDAL : Connection
    {
        private static SupplierDAL instance;

        public static SupplierDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new SupplierDAL();
                return instance;
            }
        }

        public int GetMaxId()
        {
            try
            {
                OpenConnection();
                string queryString = "select max(idSupplier) from Supplier";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (!string.IsNullOrEmpty(dt.Rows[0].ItemArray[0].ToString()))
                {
                    return int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return -1;
            }
            finally
            {
                CloseConnection();
            }
        }
        public string GetNameById(string id)
        {
            try
            {
                OpenConnection();
                string query = "select name from Supplier where idSupplier = " + id;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return reader.GetString(0);
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

        public DataTable GetAll()
        {
            try
            {
                conn.Open();
                string query = "select * from Supplier";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                cmd.ExecuteNonQuery();
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch
            {
                return new DataTable();
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable SearchByName(string name)
        {
            try
            {
                OpenConnection();
                string query = @"select * from Supplier where name like  ""%" + name + "%\"";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
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

        public bool IsExisted(string typeName)
        {
            try
            {
                OpenConnection();
                string queryString = string.Format("select * from GoodsType where name = '{0}'", typeName);

                MySqlCommand command = new MySqlCommand(queryString, conn);
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

        public bool InsertOrUpdate(Supplier supplier, bool isUpdate)
        {
            try
            {
                OpenConnection();
                string query;
                if (!isUpdate) // insert
                {
                    query = "Insert into Supplier(idSupplier, name, address, phoneNumber) " +
                    "values(@idSupplier, @name, @address,@phoneNumber)";
                }
                else // update
                {
                    query = "update Supplier set  name =@name,address = @address,phoneNumber=@phoneNumber" +
                 " where idSupplier = @idSupplier";
                }
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idSupplier", supplier.Id.ToString());
                cmd.Parameters.AddWithValue("@name", supplier.Name);
                cmd.Parameters.AddWithValue("@address", supplier.Address);
                cmd.Parameters.AddWithValue("@phoneNumber", supplier.PhoneNumber);
                int rs = cmd.ExecuteNonQuery();
                if (rs == 1)
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
