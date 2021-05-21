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
                string query = "select isActive from GoodsType where isDeleted = false and idGoodsType = " + id.ToString();
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
        public bool Delete(int id)
        {
            try
            {
                conn.Open();
                string queryString = "update Goodstype set isDeleted = 1 where idGoodsType = " + id.ToString();
                MySqlCommand command = new MySqlCommand(queryString, conn);
                int rs = command.ExecuteNonQuery();
                return rs == 1;
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
        public bool InsertOrUpdate(GoodsType goodsType, bool isUpdate)
        {
            try
            {
                conn.Open();
                string query;
                if (!isUpdate) // insert
                {
                    query = "Insert into GoodsType(idGoodsType, name, profitPercentage, unit, isActive, isDeleted) " +
                    "values(@idGoodsType, @name, @profitPercentage,@unit, @isActive, 0)";
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
                conn.Close();
            }
        }
        public GoodsType GetById(int id)
        {
            GoodsType type;
            try
            {
                conn.Open();
                string query = "Select * from GoodsType where idGoodsType = " + id.ToString();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    type = new GoodsType(id, dt.Rows[0].ItemArray[1].ToString(),
                        double.Parse(dt.Rows[0].ItemArray[2].ToString()),
                        dt.Rows[0].ItemArray[3].ToString(), bool.Parse(dt.Rows[0].ItemArray[4].ToString()), false);
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
                conn.Close();
            }
        }
        public DataTable GetActive()
        {
            try
            {
                DataTable dt = new DataTable();
                conn.Open();
                string query = "select * from GoodsType where isActive = 1 and isDeleted = 0";
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
                conn.Close();
            }
        }
        public DataTable GetInactive()
        {
            try
            {
                DataTable dt = new DataTable();
                conn.Open();
                string query = "select * from GoodsType where isActive = 0 and isDeleted = 0";
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
                conn.Close();
            }
        }
        public DataTable GetAll()
        {
            try
            {
                DataTable dt = new DataTable();
                conn.Open();
                string query = "select * from GoodsType where isDeleted = 0";
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
                conn.Close();
            }
        }
        public int GetMaxId()
        {
            try
            {
                conn.Open();
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
                conn.Close();
            }
        }

        public bool InactivateOrReactivate(int id, bool isActive)
        {
            try
            {
                conn.Open();
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
                conn.Close();
            }
        }
        public double GetProfitPercentage(int id)
        {
            try
            {
                conn.Open();
                string queryString = "select profitPercentage from GoodsType where idGoodsType = " + id.ToString();
                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count == 1)
                {
                    return double.Parse(dataTable.Rows[0].ItemArray[0].ToString());
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }

        public bool IsExisted(string typeName)
        {
            try
            {
                conn.Open();
                string queryString = string.Format("select 8 from GoodsType where name = '{0}'", typeName);

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
                conn.Close();
            }
        }

        public GoodsType GetByIdGoods(int idGoods)
        {
            GoodsType type;
            try
            {
                conn.Open();
                string query = "Select GT.idGoodsType, GT.name, GT.profitPercentage, GT.unit from GoodsType as GT "
                                + "inner join Goods on GT.idGoodsType = Goods.idGoodsType "
                                + "where GT.isActive = 1 and GT.isDeleted = 0 and Goods.idGoods = " + idGoods.ToString();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    type = new GoodsType(int.Parse(dt.Rows[0].ItemArray[0].ToString()), dt.Rows[0].ItemArray[1].ToString(),
                        double.Parse(dt.Rows[0].ItemArray[2].ToString()),
                        dt.Rows[0].ItemArray[3].ToString(), true, false);
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
                conn.Close();
            }
        }
    }
}
