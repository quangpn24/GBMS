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
    class PositionDetailDAL : Connection
    {
        private static PositionDetailDAL instance;

        public static PositionDetailDAL Instance
        {
            get { if (instance == null) instance = new PositionDetailDAL(); return PositionDetailDAL.instance; }
            private set { PositionDetailDAL.instance = value; }
        }
        public bool InsertOrUpdate(PositionDetail positionDetail, bool isUpdating = false)
        {
            try
            {
                OpenConnection();
                string query = "";
                if (isUpdating)
                {
                    query = "update PositionDetail set IsPermitted = @IsPermitted " +
                        "where IdEmployeePosition = @IdEmployeePosition and IdPermission = @IdPermission";

                }
                else
                {
                    query = "insert into PositionDetail (IdEmployeePosition,IdPermission,IsPermitted) " +
                        "values(@IdEmployeePosition,@IdPermission,@IsPermitted)";
                }
                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@IdEmployeePosition", positionDetail.IdEmployeePosition);
                cmd.Parameters.AddWithValue("@IdPermission", positionDetail.IdPermission);
                cmd.Parameters.AddWithValue("@IsPermitted", positionDetail.IsPermitted);

                return cmd.ExecuteNonQuery() == 1;
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
        public List<PositionDetail> GetListByPosition(int idPosition)
        {
            try
            {
                OpenConnection();

                string queryStr = String.Format("select * from PositionDetail where idEmployeePosition = {0} " +
                    "order by idPermission", idPosition);
                MySqlCommand cmd = new MySqlCommand(queryStr, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dataReader);

                List<PositionDetail> positionDetails = new List<PositionDetail>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PositionDetail permission = new PositionDetail(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[1].ToString()), bool.Parse(dt.Rows[i].ItemArray[2].ToString()));
                    positionDetails.Add(permission);
                }
                return positionDetails;
            }
            catch
            {
                return new List<PositionDetail>();
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}