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
    class ParameterDAL : Connection
    {

        // id 1: Prepayment PerCent
        //     2: Store Name
        //     3: Store Address
        //     4: Store Phone Number
        //     5: Store Email
        private static ParameterDAL instance;

        public static ParameterDAL Instance
        {
            get { if (instance == null) instance = new ParameterDAL(); return ParameterDAL.instance; }
            private set { ParameterDAL.instance = value; }
        }
        private ParameterDAL()
        {

        }
        public List<Parameter> GetData()
        {
            try
            {
                OpenConnection();
                string query = "select * from Parameter";
                List<Parameter> res = new List<Parameter>();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string value = null;
                    if (!reader.IsDBNull(2))
                        value = reader.GetString(2);
                    res.Add(new Parameter(int.Parse(reader.GetString(0)), reader.GetString(1), value));
                }
                return res;
            }
            catch
            {
                return new List<Parameter>();
            }
            finally
            {
                CloseConnection();
            }
        }

        public Parameter GetPrepayment()
        {
            try
            {
                OpenConnection();
                string query = "select * from Parameter where idParameter = 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return new Parameter(int.Parse(reader.GetString(0)), reader.GetString(1), reader.GetString(2));
                
            }
            catch
            {
                return new Parameter();
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool UpdatePrepayment(string value)
        {
            try
            {
                OpenConnection();
                string query = "Update Parameter set value = @value where idParameter = 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@value", value);
                return cmd.ExecuteNonQuery() == 1;
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
        public bool UpdateStoreInfo(int id , string value)
        {
            try
            {
                OpenConnection();
                string query = "Update Parameter set value = @value where idParameter = " + id;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@value", value);
                return cmd.ExecuteNonQuery() == 1;
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
