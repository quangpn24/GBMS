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
        public void InsertOrUpdate(MembershipsType membership, bool isUpdating = false)
        {
            try
            {
                OpenConnection();
                string query = "";
                if (isUpdating)
                {
                    query = "update membershipsType set membership=@membership, target=@target "
                       + "where idMembershipsType = " + membership.IdMembershipsType;

                }
                else
                {
                    query = "insert into membershipsType " +
                        "(idMembershipsType, membership, target) " +
                        "values(@idMembershipsType, @membership, @target)";
                }
                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@idMembershipsType", membership.IdMembershipsType);
                cmd.Parameters.AddWithValue("@membership", membership.Membership);
                cmd.Parameters.AddWithValue("@target", membership.Target);

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
        public bool Delete(string id)
        {
            try
            {
                OpenConnection();
                string query = "delete from membershipsType where idMembershipsType = " + id;
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
        public bool IsExisted(string membership)
        {
            try
            {
                OpenConnection();
                string queryString = "select * from membershipsType where membership=@membership";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@membership", membership);
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
                    MembershipsType type = new MembershipsType(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(), double.Parse(dt.Rows[i].ItemArray[2].ToString()));
                    memberships.Add(type);
                }
            }
            catch
            {
                return new List<MembershipsType>();
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
                type = new MembershipsType(int.Parse(dt.Rows[0].ItemArray[0].ToString()), dt.Rows[0].ItemArray[1].ToString(), double.Parse(dt.Rows[0].ItemArray[2].ToString()));
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
        public int GetMaxId()
        {
            try
            {
                OpenConnection();
                string queryString = "select max(idMembershipsType) from membershipsType";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataReader rdr = command.ExecuteReader();
                rdr.Read();
                return int.Parse(rdr.GetString(0));
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
    }
}
