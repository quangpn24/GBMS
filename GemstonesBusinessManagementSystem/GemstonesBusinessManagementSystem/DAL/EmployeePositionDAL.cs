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
    class EmployeePositionDAL : Connection
    {
        private static EmployeePositionDAL instance;

        public static EmployeePositionDAL Instance
        {
            get { if (instance == null) instance = new EmployeePositionDAL(); return EmployeePositionDAL.instance; }
            private set { EmployeePositionDAL.instance = value; }
        }

        private EmployeePositionDAL()
        {
        }

        public List<EmployeePosition> GetList()
        {
            try
            {
                OpenConnection();

                string queryStr = "select * from EmployeePosition where isDeleted = 0";
                MySqlCommand cmd = new MySqlCommand(queryStr, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dataReader);

                List<EmployeePosition> positionList = new List<EmployeePosition>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    EmployeePosition position = new EmployeePosition(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                        dt.Rows[i].ItemArray[1].ToString(), long.Parse(dt.Rows[i].ItemArray[2].ToString()),
                        long.Parse(dt.Rows[i].ItemArray[3].ToString()), long.Parse(dt.Rows[i].ItemArray[4].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[5].ToString()));

                    positionList.Add(position);
                }
                return positionList;
            }
            catch
            {
                return new List<EmployeePosition>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public void InsertOrUpdate(EmployeePosition position, bool isUpdating = false)
        {
            try
            {
                OpenConnection();
                string query = "";
                if (isUpdating)
                {
                    query = "update EmployeePosition set position=@position, salaryBase=@salaryBase, " +
                        "moneyPerShift=@moneyPerShift, moneyPerFault=@moneyPerFault, standardWorkDays=@standardWorkDays " +
                        "where idEmployeePosition = " + position.IdEmployeePosition;

                }
                else
                {
                    query = "insert into EmployeePosition " +
                        "(idEmployeePosition, position, salaryBase, moneyPerShift, moneyPerFault, standardWorkDays) " +
                        "values(@idEmployeePosition, @position, @salaryBase, @moneyPerShift, @moneyPerFault, @standardWorkDays)";
                }
                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@idEmployeePosition", position.IdEmployeePosition);
                cmd.Parameters.AddWithValue("@position", position.Position);
                cmd.Parameters.AddWithValue("@salaryBase", position.SalaryBase);
                cmd.Parameters.AddWithValue("@moneyPerShift", position.MoneyPerShift);
                cmd.Parameters.AddWithValue("@moneyPerFault", position.MoneyPerFault);
                cmd.Parameters.AddWithValue("@standardWorkDays", position.StandardWorkDays);

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
                string query = "update EmployeePosition set isDeleted = true where idEmployeePosition = " + id;
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
        public EmployeePosition GetById(int id)
        {
            try
            {
                OpenConnection();
                string queryString = "select * from EmployeePosition where idEmployeePosition = " + id;

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                EmployeePosition position = new EmployeePosition(int.Parse(dt.Rows[0].ItemArray[0].ToString()),
                        dt.Rows[0].ItemArray[1].ToString(), long.Parse(dt.Rows[0].ItemArray[2].ToString()),
                        long.Parse(dt.Rows[0].ItemArray[3].ToString()), long.Parse(dt.Rows[0].ItemArray[4].ToString()),
                        int.Parse(dt.Rows[0].ItemArray[5].ToString()));
                return position;
            }
            catch
            {
                return new EmployeePosition();
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
                string queryString = "select max(idEmployeePosition) from EmployeePosition";

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
        public bool IsExisted(string position)
        {
            try
            {
                OpenConnection();
                string queryString = "select * from EmployeePosition where position=@position";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@position", position);
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
