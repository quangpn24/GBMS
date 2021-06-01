using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GemstonesBusinessManagementSystem.Models;
using MySql.Data.MySqlClient;

namespace GemstonesBusinessManagementSystem.DAL
{
    class GoodsTypeDAL : Connection
    {
        private static GoodsTypeDAL instance;
        public static GoodsTypeDAL Instance
        {
            get
            {
                if (instance == null)
                    instance = new GoodsTypeDAL();
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        public bool IsActive(int id)
        {
            try
            {
                string query = "select isActive from GoodsType where idGoodsType = " + id.ToString();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return bool.Parse(dt.Rows[0].ItemArray[0].ToString());
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Clone();
            }
        }
        public bool InsertOrUpdate(GoodsType goodsType, bool isUpdate)
        {
            try
            {
                OpenConnection();
                string query;
                if (!isUpdate) // insert
                {
                    query = "Insert into GoodsType(idGoodsType, name, profitPercentage, unit, isActive) " +
                    "values(@idGoodsType, @name, @profitPercentage,@unit, @isActive)";
                }
                else // update
                {
                    query = "update GoodsType set  name =@name,profitPercentage = @profitPercentage,unit=@unit, isActive = @isActive" +
                 " where idGoodsType = @idGoodsType";
                }
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idGoodsType", goodsType.IdGoodsType);
                cmd.Parameters.AddWithValue("@name", goodsType.Name);
                cmd.Parameters.AddWithValue("@profitPercentage", goodsType.ProfitPercentage);
                cmd.Parameters.AddWithValue("@unit", goodsType.Unit);
                cmd.Parameters.AddWithValue("@isActive", goodsType.IsActive);
                int rs = cmd.ExecuteNonQuery();
                if (rs == 1)
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
        public GoodsType GetById(int id)
        {
            GoodsType type;
            try
            {
                OpenConnection();
                string query = "Select * from GoodsType where idGoodsType = " + id.ToString();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    type = new GoodsType(id, dt.Rows[0].ItemArray[1].ToString(),
                        double.Parse(dt.Rows[0].ItemArray[2].ToString()),
                        dt.Rows[0].ItemArray[3].ToString(), bool.Parse(dt.Rows[0].ItemArray[4].ToString()));
                    return type;
                }
                else
                {
                    return new GoodsType();
                }
            }
            catch
            {
                return new GoodsType();
            }
            finally
            {
                CloseConnection();
            }
        }
        public DataTable GetActive()
        {
            try
            {
                DataTable dt = new DataTable();
                OpenConnection();
                string query = "select * from GoodsType where isActive = 1 ";
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
        public DataTable GetAll()
        {
            try
            {
                DataTable dt = new DataTable();
                OpenConnection();
                string query = "select * from GoodsType";
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
        public int GetMaxId()
        {
            try
            {
                OpenConnection();
                string queryString = "select max(idGoodsType) from GoodsType";
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

        public bool InactivateOrReactivate(int id, bool isActive)
        {
            try
            {
                OpenConnection();
                string query = "update GoodsType set isActive = @isActive  where idGoodsType =  @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@isActive", isActive);
                cmd.Parameters.AddWithValue("@id", id.ToString());
                int rs = cmd.ExecuteNonQuery();
                return rs == 1;
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

        public GoodsType GetByIdGoods(int idGoods)
        {
            GoodsType type;
            try
            {
                OpenConnection();
                string query = "Select GT.idGoodsType, GT.name, GT.profitPercentage, GT.unit from GoodsType as GT "
                                + "inner join Goods on GT.idGoodsType = Goods.idGoodsType "
                                + "where GT.isActive = 1 and Goods.idGoods = " + idGoods.ToString();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    type = new GoodsType(int.Parse(dt.Rows[0].ItemArray[0].ToString()), dt.Rows[0].ItemArray[1].ToString(),
                        double.Parse(dt.Rows[0].ItemArray[2].ToString()),
                        dt.Rows[0].ItemArray[3].ToString(), true);
                    return type;
                }
                else
                {
                    return new GoodsType();
                }
            }
            catch
            {
                return new GoodsType();
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
