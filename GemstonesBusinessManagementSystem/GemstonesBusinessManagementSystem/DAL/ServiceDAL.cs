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
                OpenConnection();
                string queryString = @"SELECT * FROM Service WHERE isDeleted=0;";
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
            return dt;
        }
        public List<Service> GetActivedServices()
        {
            DataTable dt = new DataTable();
            List<Service> services = new List<Service>();
            try
            {
                OpenConnection();
                string queryString = @"SELECT * FROM Service WHERE isActive=1 ;";

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
                Service service = new Service(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), long.Parse(dt.Rows[i].ItemArray[2].ToString()), 0,
                    0);
                if (dt.Rows[i].ItemArray[3].ToString() == "True")
                    service.IsActive = 1;
                if (dt.Rows[i].ItemArray[4].ToString() == "True")
                    service.IsDeleted = 1;
                services.Add(service);
            }
            return services;
        }
        public List<Service> GetActivedServicesByName(string name)
        {
            DataTable dt = new DataTable();
            List<Service> services = new List<Service>();
            try
            {
                OpenConnection();
                string queryString = @"SELECT * FROM Service WHERE isActive=1 and name LIKE  ""%" + name + "%\";";

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
                Service service = new Service(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), long.Parse(dt.Rows[i].ItemArray[2].ToString()), 0,
                    0);
                if (dt.Rows[i].ItemArray[3].ToString() == "True")
                    service.IsActive = 1;
                if (dt.Rows[i].ItemArray[4].ToString() == "True")
                    service.IsDeleted = 1;
                services.Add(service);
            }
            return services;
        }
        public List<Service> ConvertDBToList()
        {
            DataTable dt = new DataTable();
            List<Service> services = new List<Service>();
            try
            {
                OpenConnection();
                string queryString = @"SELECT * FROM Service WHERE isDeleted=0 ;";

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
                Service service = new Service(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), long.Parse(dt.Rows[i].ItemArray[2].ToString()), 0,
                    0);
                if (dt.Rows[i].ItemArray[3].ToString() == "True")
                    service.IsActive = 1;
                if (dt.Rows[i].ItemArray[4].ToString() == "True")
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
                OpenConnection();
                string queryString = @"SELECT * FROM Service WHERE name LIKE  ""%" + name + "%\" and isDeleted=0;";
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
                Service service = new Service(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                    dt.Rows[i].ItemArray[1].ToString(), long.Parse(dt.Rows[i].ItemArray[2].ToString()),
                    0, 0);
                if (dt.Rows[i].ItemArray[3].ToString() == "True")
                    service.IsActive = 1;
                if (dt.Rows[i].ItemArray[4].ToString() == "True")
                    service.IsDeleted = 1;
                services.Add(service);
            }
            return services;
        }
        public Service FindById(string idService)
        {

            try
            {
                OpenConnection();
                string queryString = @"SELECT * FROM Service WHERE idService= " + idService;
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                Service service = new Service(int.Parse(dt.Rows[0].ItemArray[0].ToString()),
                    dt.Rows[0].ItemArray[1].ToString(), long.Parse(dt.Rows[0].ItemArray[2].ToString()),
                    0, 0);
                if (dt.Rows[0].ItemArray[3].ToString() == "True")
                    service.IsActive = 1;
                if (dt.Rows[0].ItemArray[4].ToString() == "True")
                    service.IsDeleted = 1;
                return service;
            }
            catch
            {
                return new Service();
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool Add(Service service)
        {
            try
            {
                OpenConnection();
                string queryString = "insert into Service(idService, name, price,isActive,isDeleted) values(@idService, @name, @price,@isActive,@isDeleted);";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idService", service.IdService.ToString());
                command.Parameters.AddWithValue("@name", service.Name.ToString());
                command.Parameters.AddWithValue("@price", service.Price.ToString());
                command.Parameters.AddWithValue("@isDeleted", "0");
                command.Parameters.AddWithValue("@isActive", service.IsActive.ToString());
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
        public int FindMaxId()
        {
            int res = 0;
            try
            {
                OpenConnection();
                string queryString = "SELECT MAX(idService) from Service ; ";

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
                CloseConnection();
            }
            return res;
        }
        public bool RestoreData()
        {
            try
            {
                OpenConnection();
                string queryString = "UPDATE Service set isDeleted = 0; ";

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
                CloseConnection();
            }
        }
        public bool Delete(string idService)
        {
            try
            {
                OpenConnection();
                string queryString = "UPDATE Service SET isDeleted = 1,isActive=0 WHERE idService = @idService; ";
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
                CloseConnection();
            }
        }
        public bool Update(Service service)
        {
            try
            {
                OpenConnection();
                string queryString = "UPDATE Service " +
                                        "SET name = @name, price=@price,isDeleted=@isDeleted, isActive=@isActive  where idService = @idService; ";
                MySqlCommand command = new MySqlCommand(queryString, conn);

                command.Parameters.AddWithValue("@idService", service.IdService.ToString());
                command.Parameters.AddWithValue("@name", service.Name.ToString());
                command.Parameters.AddWithValue("@price", service.Price.ToString());
                command.Parameters.AddWithValue("@isDeleted", service.IsDeleted.ToString());
                command.Parameters.AddWithValue("@isActive", service.IsActive.ToString());
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
        public bool IsExisted(string name)
        {
            try
            {
                OpenConnection();
                string queryString = "SELECT * from Service WHERE name=@name; ";

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
                CloseConnection();
            }
        }
        public bool IsExistServiceName(string serviceName)
        {
            try
            {
                OpenConnection();
                string query = @"select * from Service where name = '" + serviceName + "'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
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
