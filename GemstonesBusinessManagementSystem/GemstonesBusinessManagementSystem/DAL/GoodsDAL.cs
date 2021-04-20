using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GemstonesBusinessManagementSystem.Models;

namespace GemstonesBusinessManagementSystem.DAL
{
    class GoodsDAL : Connection
    {
        private static GoodsDAL instance;
        public static GoodsDAL Instance 
        {
            get
            {
                if (instance == null)
                    instance = new GoodsDAL();
                return instance;
            }
            private set
            {
                instance = value;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                conn.Open();
                string query = "update Goods set isDeleted = 1 " +
                 "where idGoods = @idGoods";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idGoods", id.ToString());
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
        public bool InsertOrUpdate(Goods goods, bool isUpdate)
        {
            try
            {
                conn.Open();
                string query;
                if(!isUpdate) // insert
                {
                     query = "Insert into Goods(idGoods, name, price, quantity, idGoodsType, imageFile, isDeleted) " +
                    "values(@idGoods, @name, @price, @quantity, @idGoodsType, @imageFile, @isDeleted)";
                }
                else
                {
                     query = "update Goods set name=@name, price =@price,quantity = @quantity,idGoodsType=@idGoodsType, imageFile=@imageFile, isDeleted =@isDeleted" +
                  "where idGoods = @idGoods";
                }
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idGoods", goods.IdGoods);
                cmd.Parameters.AddWithValue("@name", goods.Name);
                cmd.Parameters.AddWithValue("@price", goods.Price);
                cmd.Parameters.AddWithValue("@quantity", goods.Quantity);
                cmd.Parameters.AddWithValue("@idGoodsType", goods.IdGoodsType);
                cmd.Parameters.AddWithValue("@imageFile",Convert.ToBase64String(goods.ImageFile));
                cmd.Parameters.AddWithValue("@isDeleted", goods.IsDeleted);
                int rs = cmd.ExecuteNonQuery();
                if(rs == 1)
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
        public DataTable LoadData()
        {
            try
            {
                DataTable dt = new DataTable();
                conn.Open();
                string query = "select * from Goods where isDeleted = 0";
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

        public Goods GetGoods(string idGoods) // lấy thông tin hàng hóa khi biết id 
        {
            try
            {
                conn.Open();
                string queryString = "select * from Goods where idGoods = " + idGoods;

                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                Goods res = new Goods(int.Parse(idGoods), dataTable.Rows[0].ItemArray[1].ToString(),
                    long.Parse(dataTable.Rows[0].ItemArray[2].ToString()), int.Parse(dataTable.Rows[0].ItemArray[3].ToString()),
                    int.Parse(dataTable.Rows[0].ItemArray[4].ToString()), 
                    Convert.FromBase64String(dataTable.Rows[0].ItemArray[5].ToString()),
                    bool.Parse(dataTable.Rows[0].ItemArray[6].ToString()));
                return res;
            }
            catch
            {
                return new Goods();
            }
            finally
            {
                conn.Close();
            }
        }

        public Goods GetByGoodsType(string idGoodsType)
        {
            try
            {
                conn.Open();
                string queryString = "select * from Goods where idGoodsType = " + idGoodsType;

                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                Goods res = new Goods(int.Parse(dataTable.Rows[0].ItemArray[0].ToString()), dataTable.Rows[0].ItemArray[1].ToString(),
                    long.Parse(dataTable.Rows[0].ItemArray[2].ToString()), int.Parse(dataTable.Rows[0].ItemArray[3].ToString()),
                    int.Parse(dataTable.Rows[0].ItemArray[4].ToString()),
                    Convert.FromBase64String(dataTable.Rows[0].ItemArray[5].ToString()),
                    bool.Parse(dataTable.Rows[0].ItemArray[6].ToString()));
                return new Goods();
            }
            catch
            {
                return new Goods();
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
                string queryString = "select max(idGoods) from Goods";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count == 1)
                {
                    return int.Parse(dataTable.Rows[0].ItemArray[0].ToString());
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
    }
}
