using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool UpdateGoods(GoodsType goodsType)
        {
            try
            {
                conn.Open();
                string query = "update Goods set name=@name, profitPercentage = @profitPercentage,price =@price, unit=@unit" +
                    "where idGoodsType =" + goodsType.IdGoodsType.ToString();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", goodsType.Name);
                cmd.Parameters.AddWithValue("@profitPercentage", goodsType.ProfitPercentage);
                cmd.Parameters.AddWithValue("@unit", goodsType.Unit);
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
                conn.Close();
            }
        }
        public bool InsertGoods(GoodsType goodsType)
        {
            try
            {
                conn.Open();
                string query = "Insert into Goods(idGoodsType, name, profitPercentage, unit) " +
                    "values(@idGoodsType, @name, @profitPercentage,@unit)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idGoodsType", goodsType.IdGoodsType);
                cmd.Parameters.AddWithValue("@name", goodsType.Name);
                cmd.Parameters.AddWithValue("@profitPercentage", goodsType.ProfitPercentage);
                cmd.Parameters.AddWithValue("@unit", goodsType.Unit);
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
                conn.Close();
            }
        }

        public GoodsType GetGoodsTypeWithId(int id)
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
                if(dt.Rows.Count == 1)
                {
                    type = new GoodsType(id, dt.Rows[0].ItemArray[1].ToString(), double.Parse(dt.Rows[0].ItemArray[2].ToString()), dt.Rows[0].ItemArray[3].ToString());
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
        public DataTable LoadData()
        {
            try
            {
                DataTable dt = new DataTable();
                conn.Open();
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
                conn.Close();
            }
        }
    }
}
