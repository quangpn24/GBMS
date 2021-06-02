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
    class MembershipsTypeDAL : Connection
    {
        private static MembershipsTypeDAL instance;

        public static MembershipsTypeDAL Instance
        {
            get { if (instance == null) instance = new MembershipsTypeDAL(); return MembershipsTypeDAL.instance; }
            private set { MembershipsTypeDAL.instance = value; }
        }

        private MembershipsTypeDAL()
        {

        }
        public List<MembershipsType> GetList()
        {
            List<MembershipsType> memberships = new List<MembershipsType>();
            try
            {
                OpenConnection();

                string query = "select * from membershipsType";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dataReader);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    MembershipsType type = new MembershipsType(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString());
                    memberships.Add(type);
                }
            }
            catch
            {

            }
            return memberships;
        }
        public DataTable GetActive()
        {
            try
            {
                DataTable dt = new DataTable();
                OpenConnection();
                string query = "select * from MembershipsType";
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
        public MembershipsType GetById(int id)
        {
            MembershipsType type = new MembershipsType();
            try
            {
                OpenConnection();
                string queryString = "select * from MembershipsType where idMembershipsType = " + id;

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                type = new MembershipsType(int.Parse(dt.Rows[0].ItemArray[0].ToString()), dt.Rows[0].ItemArray[1].ToString());
            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }
            return type;
        }
    }
}
