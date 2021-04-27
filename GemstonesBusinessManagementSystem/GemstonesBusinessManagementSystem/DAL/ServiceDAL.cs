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
    class ServiceDAL : Connection
    {
        private static ServiceDAL instance;

        public static ServiceDAL Instance
        {
            get { if (instance == null) instance = new ServiceDAL(); return ServiceDAL.instance; }
            private set { ServiceDAL.instance = value; }
        }

        private ServiceDAL()
        {

        }
        public DataTable GetServices()
        {
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                string queryString = @"SELECT * FROM gemstonesbusinessmanagementsystem.service WHERE isDeleted=0;";
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
                conn.Close();
            }
            return dt;
        }
        public List<Service> ConvertDBToList()
        {
            DataTable dt = new DataTable();
            List<Service> services = new List<Service>();
            try
            {
                conn.Open();
                string queryString = @"SELECT * FROM gemstonesbusinessmanagementsystem.service WHERE isDeleted=0 ;";

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
                conn.Close();
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Service service = new Service(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), long.Parse(dt.Rows[i].ItemArray[2].ToString()), int.Parse(dt.Rows[i].ItemArray[3].ToString()), 0,
                    0);
                if (dt.Rows[i].ItemArray[4].ToString() == "True")
                    service.IsActived = 1;
                if (dt.Rows[i].ItemArray[5].ToString() == "True")
                    service.IsDeleted = 1;
                services.Add(service);
            }
            return services;
        }
        public List<Service> FindByName(string name)
        {
            DataTable dt = new DataTable();
            List<Service> services = new List<Service>();
            try
            {
                conn.Open();
                string queryString = @"SELECT * FROM gemstonesbusinessmanagementsystem.service WHERE name LIKE  ""%" + name + "%\" and isDeleted=0;";
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
                conn.Close();
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Service service = new Service(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), long.Parse(dt.Rows[i].ItemArray[2].ToString()), int.Parse(dt.Rows[i].ItemArray[3].ToString()),
                    0, 0);
                if (dt.Rows[i].ItemArray[4].ToString() == "True")
                    service.IsActived = 1;
                if (dt.Rows[i].ItemArray[5].ToString() == "True")
                    service.IsDeleted = 1;
                services.Add(service);
            }
            return services;
        }
        public bool Add(Service service)
        {
            try
            {
                conn.Open();
                string queryString = "insert into gemstonesbusinessmanagementsystem.service(idService, name, price,numberOfHired,isActived,isDeleted) values(@idService, @name, @price,@numberOfHired,@isActived,@isDeleted);";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idService", service.IdService.ToString());
                command.Parameters.AddWithValue("@name", service.Name.ToString());
                command.Parameters.AddWithValue("@price", service.Price.ToString());
                command.Parameters.AddWithValue("@numberOfHired", service.NumberOfHired.ToString());
                command.Parameters.AddWithValue("@isDeleted", "0");
                command.Parameters.AddWithValue("@isActived", service.IsActived.ToString());
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public int FindMaxId()
        {
            int res = 0;
            try
            {
                conn.Open();
                string queryString = "SELECT MAX(idService) from gemstonesbusinessmanagementsystem.service ; ";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                res = int.Parse(dataTable.Rows[0].ItemArray[0].ToString());
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
            return res;
        }
        public bool RestoreData()
        {
            try
            {
                conn.Open();
                string queryString = "UPDATE gemstonesbusinessmanagementsystem.service set isDeleted = 0; ";

                MySqlCommand command = new MySqlCommand(queryString, conn);

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public bool Delete(string idService)
        {
            try
            {
                conn.Open();
                string queryString = "UPDATE gemstonesbusinessmanagementsystem.service SET isDeleted = 1,isActived=0 WHERE idService = @idService; ";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idService", idService);
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public bool Update(Service service)
        {
            try
            {
                conn.Open();
                string queryString = "UPDATE gemstonesbusinessmanagementsystem.service " +
                                        "SET name = @name, price=@price,numberOfHired=@numberOfHired,isDeleted=@isDeleted, isActived=@isActived  where idService = @idService; ";
                MySqlCommand command = new MySqlCommand(queryString, conn);

                command.Parameters.AddWithValue("@idService", service.IdService.ToString());
                command.Parameters.AddWithValue("@name", service.Name.ToString());
                command.Parameters.AddWithValue("@price", service.Price.ToString());
                command.Parameters.AddWithValue("@numberOfHired", service.NumberOfHired.ToString());
                command.Parameters.AddWithValue("@isDeleted", service.IsDeleted.ToString());
                command.Parameters.AddWithValue("@isActived", service.IsActived.ToString());
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        public bool IsExisted(string name)
        {
            try
            {
                conn.Open();
                string queryString = "SELECT * from gemstonesbusinessmanagementsystem.service WHERE name=@name; ";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@name", name);
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
                conn.Close();
            }
        }
    }
}
