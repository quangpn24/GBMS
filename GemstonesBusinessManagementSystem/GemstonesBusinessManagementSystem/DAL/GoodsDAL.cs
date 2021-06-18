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
                OpenConnection();
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
                CloseConnection();
            }
        }
        public bool InsertOrUpdate(Goods goods, bool isUpdate)
        {
            try
            {
                OpenConnection();
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
                    query = "update Goods set name=@name, price =@price,quantity = @quantity,idGoodsType=@idGoodsType, imageFile=@imageFile, isDeleted =@isDeleted" +
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
                CloseConnection();
            }
        }
        public DataTable GetActive()
        {
            try
            {
                DataTable dt = new DataTable();
                OpenConnection();
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
                CloseConnection();
            }
        }

        public bool InactivateOrReActivate(int idGoodsType, bool isActive)
        {
            try
            {
                OpenConnection();
                string query = "update Goods set isDeleted = @isActive " +
                 "where idGoodsType = @idGoodsType";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idGoodsType", idGoodsType.ToString());
                if (isActive)
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
                CloseConnection();
            }
        }
        public List<Goods> GetList()
        {
            List<Goods> goodsList = new List<Goods>();
            try
            {
                OpenConnection();
                string queryStr = "select * from Goods where isDeleted = false";
                MySqlCommand cmd = new MySqlCommand(queryStr, conn);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dataReader);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Goods goods = new Goods(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                        dt.Rows[i].ItemArray[1].ToString(), long.Parse(dt.Rows[i].ItemArray[2].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[3].ToString()), int.Parse(dt.Rows[i].ItemArray[4].ToString()),
                        Convert.FromBase64String(dt.Rows[i].ItemArray[5].ToString()), bool.Parse(dt.Rows[i].ItemArray[6].ToString()));
                    goodsList.Add(goods);
                }
            }
            catch
            {
            }
            return goodsList;
        }
        public Goods GetById(string idGoods) // lấy thông tin hàng hóa khi biết id 
        {
            try
            {
                OpenConnection();
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
                CloseConnection();
            }
        }

        public DataTable GetByidGoodsType(int idGoodsType)
        {
            try
            {
                OpenConnection();
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
                CloseConnection();
            }
        }
        public int GetMaxId()
        {
            try
            {
                OpenConnection();
                string queryString = "select max(idGoods) from Goods";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
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


        public DataTable SearchByName(string name)
        {
            DataTable dt = new DataTable();
            try
            {
                OpenConnection();
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
                CloseConnection();
            }
            return dt;
        }

        public bool UpdateQuantity(int id, int quantity)
        {
            try
            {
                OpenConnection();
                string query = "Update Goods set quantity = @quantity where idGoods = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@quantity", quantity);
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
        public int GetQuantityById(int id)
        {
            try
            {
                OpenConnection();
                string query = "select quantity from Goods where idGoods = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return int.Parse(reader.GetString(0));
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
        public List<Goods> FindByName(string name)
        {
            DataTable dt = new DataTable();
            List<Goods> goodsList = new List<Goods>();
            try
            {
                OpenConnection();
                string queryString = @"select * from Goods where name like  ""%" + name + "%\" and isDeleted = 0";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Goods goods = new Goods(int.Parse(dt.Rows[i].ItemArray[0].ToString()),
                        dt.Rows[i].ItemArray[1].ToString(), long.Parse(dt.Rows[i].ItemArray[2].ToString()),
                        int.Parse(dt.Rows[i].ItemArray[3].ToString()), int.Parse(dt.Rows[i].ItemArray[4].ToString()),
                        Convert.FromBase64String(dt.Rows[i].ItemArray[5].ToString()), bool.Parse(dt.Rows[i].ItemArray[6].ToString()));
                    goodsList.Add(goods);
                }
            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }
            return goodsList;
        }
        public Goods FindById(string idGoods) // lấy thông tin hàng hóa khi biết id 
        {
            try
            {
                OpenConnection();
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
                CloseConnection();
            }
        }
    }
}
