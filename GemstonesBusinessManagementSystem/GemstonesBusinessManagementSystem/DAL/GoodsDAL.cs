using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GemstonesBusinessManagementSystem.Models;
using System.Windows;

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
        public bool InsertOrUpdate(Goods goods, bool isUpdate)
        {
            try
            {
                conn.Open();
                string query;
                if (!isUpdate) // insert
                {
                    query = "Insert into Goods(idGoods, name, price, quantity, idGoodsType, imageFile, isDeleted) " +
                   "values(@idGoods, @name, @price, @quantity, @idGoodsType, @imageFile, @isDeleted)";
                }
                else
                {
                    query = "update Goods set name=@name, price =@price,quantity = @quantity,idGoodsType=@idGoodsType, imageFile=@imageFile, isDeleted =@isDeleted " +
                 "where idGoods = @idGoods";
                }
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idGoods", goods.IdGoods);
                cmd.Parameters.AddWithValue("@name", goods.Name);
                cmd.Parameters.AddWithValue("@price", goods.ImportPrice);
                cmd.Parameters.AddWithValue("@quantity", goods.Quantity);
                cmd.Parameters.AddWithValue("@idGoodsType", goods.IdGoodsType);
                cmd.Parameters.AddWithValue("@imageFile", Convert.ToBase64String(goods.ImageFile));
                cmd.Parameters.AddWithValue("@isDeleted", goods.IsDeleted);
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
        public DataTable GetActive()
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

        public bool InactivateOrReActivate(int idGoodsType, bool isActive)
        {
            try
            {
                conn.Open();
                string query = "update Goods set isDeleted = @isActive " +
                 "where idGoodsType = @idGoodsType";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idGoodsType", idGoodsType.ToString());
                if(isActive)
                {
                    cmd.Parameters.AddWithValue("@isActive", "0");
                }    
                else
                {
                    cmd.Parameters.AddWithValue("@isActive", "1");
                }                
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

        public Goods GetById(string idGoods) // lấy thông tin hàng hóa khi biết id 
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

        public DataTable GetByidGoodsType(int idGoodsType)
        {
            try
            {
                conn.Open();
                string queryString = "select * from Goods where idGoodsType = " + idGoodsType.ToString();

                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
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
                string queryString = "select max(idGoods) from Goods";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (string.IsNullOrEmpty(dt.Rows[0].ItemArray[0].ToString()))
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


        public DataTable SearchByName(string name)
        {
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                string query = @"select * from Goods where name like  ""%" + name + "%\" and isDeleted = 0";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                adapter.Fill(dt);
                return dt;
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

        public bool IsExistGoodsType(int idGoodsType)
        {
            try
            {
                conn.Open();
                string queryString = "select count(*) from Goods where isDeleted = 0 and idGoodsType = " + idGoodsType.ToString();
                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return int.Parse(dataTable.Rows[0].ItemArray[0].ToString()) > 0;
                
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
    }
}
