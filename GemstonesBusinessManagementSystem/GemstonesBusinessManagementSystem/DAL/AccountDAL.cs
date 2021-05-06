using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using GemstonesBusinessManagementSystem.Models;
using MySql.Data.MySqlClient;

namespace GemstonesBusinessManagementSystem.DAL
{
    class AccountDAL : Connection
    {
        private static AccountDAL instance;

        public static AccountDAL Instance
        {
            get { if (instance == null) instance = new AccountDAL(); return AccountDAL.instance; }
            private set { AccountDAL.instance = value; }
        }
        public DataTable LoadData()
        {
            try
            {
                OpenConnection();
                string query = "select * from Account";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
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
        private AccountDAL()
        {

        }
        public bool LoginAdmin(string idAccount, string userName, string passWord, int type)
        {
            //passWord = Encryptor.Instance.Encrypt(passWord);

            string query = "SELECT * FROM Account WHERE idAccount = '" + idAccount + "' AND username = '" + userName + "' AND password = '" + passWord + "' and type " + type ;

            DataTable result = new DataTable();
            
            return result.Rows.Count > 0;
        }
        public List<Account> ConvertDBToList()
        {
            DataTable dt;
            List<Account> accounts = new List<Account>();
            try
            {
                dt = LoadData();
            }
            catch
            {
                dt = new DataTable();
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Account acc = new Account(int.Parse(dt.Rows[i].ItemArray[0].ToString()), dt.Rows[i].ItemArray[1].ToString(), dt.Rows[i].ItemArray[2].ToString(), int.Parse(dt.Rows[i].ItemArray[3].ToString()));
                accounts.Add(acc);
            }
            return accounts;
        }
        public bool AddintoDB(Account account)
        {
            try
            {
                OpenConnection();
                string query = "insert into account(idAccount, username, password, type) values(@idAccount, @username, @password, @type)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idAccount", account.IdAccount.ToString());
                cmd.Parameters.AddWithValue("@username", account.Username);
                cmd.Parameters.AddWithValue("@password", account.Password);
                cmd.Parameters.AddWithValue("@type", account.Type.ToString());
                cmd.ExecuteNonQuery();
                return true;
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
        public int SetNewID()
        {
            try
            {
                OpenConnection();
                string queryString = "select max(idAccount) from Account";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    return int.Parse(dataTable.Rows[0].ItemArray[0].ToString()) + 1;
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
                CloseConnection();
            }
        }
        //kiem tra ma xac thuc
        public bool isExistKey(string key)
        {
            try
            {
                OpenConnection();
                string queryString = "select * from Authorizations where authKey = " + key;
                MySqlCommand cmd = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
            finally
            {
                CloseConnection();
            }
        }
        public bool IsExistUsername(string username)
        {
            try
            {
                OpenConnection();
                string query = "select * from account where username  = '" + username + "'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                if(dt.Rows.Count > 0)
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
                CloseConnection();
            }
        }

        public bool UpdatePasswordByUsername(string username, string password)
        {
            try
            {
                OpenConnection();
                string query = "update Account set password=@password where username = @username";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.ExecuteNonQuery();
                return true;
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

    }
}
