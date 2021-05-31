using GemstonesBusinessManagementSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.DAL
{
    class BillServiceDAL : Connection
    {
        private static BillServiceDAL instance;

        public static BillServiceDAL Instance
        {
            get { if (instance == null) instance = new BillServiceDAL(); return BillServiceDAL.instance; }
            private set { BillServiceDAL.instance = value; }
        }
        private BillServiceDAL()
        {

        }
        public int GetMaxId()
        {
            int res = 0;
            try
            {
                OpenConnection();
                string queryString = @"SELECT MAX(idBillService) FROM BillService";

                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                res = int.Parse(dataTable.Rows[0].ItemArray[0].ToString());
            }
            catch
            {

            }
            finally
            {
                CloseConnection();
            }
            return res;
        }
        public BillService GetBillService(string idBillService)
        {
            try
            {
                OpenConnection();
                string queryString = "SELECT * FROM BillService WHERE idBillService = " + idBillService;

                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                BillService res;
                if (string.IsNullOrEmpty(dataTable.Rows[0].ItemArray[1].ToString()))
                {
                    res = new BillService(int.Parse(idBillService), 1, DateTime.Parse(dataTable.Rows[0].ItemArray[2].ToString()),
                        float.Parse(dataTable.Rows[0].ItemArray[3].ToString()), float.Parse(dataTable.Rows[0].ItemArray[4].ToString()),
                        int.Parse(dataTable.Rows[0].ItemArray[5].ToString()), int.Parse(dataTable.Rows[0].ItemArray[6].ToString()));
                }
                else
                {
                    res = new BillService(int.Parse(idBillService), int.Parse(dataTable.Rows[0].ItemArray[1].ToString()),
                        DateTime.Parse(dataTable.Rows[0].ItemArray[2].ToString()), float.Parse(dataTable.Rows[0].ItemArray[3].ToString()),
                        float.Parse(dataTable.Rows[0].ItemArray[4].ToString()), int.Parse(dataTable.Rows[0].ItemArray[5].ToString()),
                        int.Parse(dataTable.Rows[0].ItemArray[6].ToString()));
                }
                return res;
            }
            catch
            {
                return new BillService();
            }
            finally
            {
                CloseConnection();
            }
        }
        public List<BillService> GetByDate(DateTime startDate, DateTime endDate)
        {
            try
            {
                OpenConnection();
                string start = startDate.ToString("yyyy-MM-dd");
                string end = endDate.AddDays(1).ToString("yyyy-MM-dd");
                string queryString = String.Format("SELECT * FROM BillService WHERE createdDate >= '{0}' and createdDate<='{1}';  ", start, end);

                MySqlCommand command = new MySqlCommand(queryString, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                List<BillService> res = new List<BillService>();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    BillService temp;
                    if (string.IsNullOrEmpty(dataTable.Rows[i].ItemArray[1].ToString()))
                    {
                        temp = new BillService(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), 1, DateTime.Parse(dataTable.Rows[i].ItemArray[2].ToString()),
                            float.Parse(dataTable.Rows[i].ItemArray[3].ToString()), float.Parse(dataTable.Rows[i].ItemArray[4].ToString()),
                            int.Parse(dataTable.Rows[i].ItemArray[5].ToString()), int.Parse(dataTable.Rows[i].ItemArray[6].ToString()));
                    }
                    else
                    {
                        temp = new BillService(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), int.Parse(dataTable.Rows[i].ItemArray[1].ToString()),
                            DateTime.Parse(dataTable.Rows[i].ItemArray[2].ToString()), float.Parse(dataTable.Rows[i].ItemArray[3].ToString()),
                            float.Parse(dataTable.Rows[i].ItemArray[4].ToString()), int.Parse(dataTable.Rows[i].ItemArray[5].ToString()),
                            int.Parse(dataTable.Rows[i].ItemArray[6].ToString()));
                    }
                    res.Add(temp);
                }
                return res;
            }
            catch
            {
                return new List<BillService>();
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool Update(BillService billService)
        {
            try
            {
                OpenConnection();
                string queryString = "UPDATE BillService SET createdDate=@createdDate,status=@status,total=@totalMoney,totalPaidMoney=@totalPaidMoney where idBillService=@idBillService";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idBillService", billService.IdBillService.ToString());
                command.Parameters.AddWithValue("@createdDate", billService.CreatedDate);
                command.Parameters.AddWithValue("@status", billService.Status);
                command.Parameters.AddWithValue("@totalMoney", billService.Total);
                command.Parameters.AddWithValue("@totalPaidMoney", billService.TotalPaidMoney);
                command.ExecuteNonQuery();
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
        public bool Add(BillService billService)
        {
            try
            {
                OpenConnection();
                string queryString = "INSERT INTO BillService(idBillService, idAccount, createdDate,status,total,totalPaidMoney,idCustomer ) VALUES(@idBillService, @idAccount,@createdDate,@status,@total,@totalPaidMoney,@idCustomer)";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idBillService", billService.IdBillService.ToString());
                command.Parameters.AddWithValue("@idAccount", billService.IdAccount.ToString());
                command.Parameters.AddWithValue("@createdDate", billService.CreatedDate);
                command.Parameters.AddWithValue("@status", billService.Status);
                command.Parameters.AddWithValue("@total", billService.Total.ToString());
                command.Parameters.AddWithValue("@totalPaidMoney", billService.TotalPaidMoney.ToString());
                command.Parameters.AddWithValue("@idCustomer", billService.IdCustomer.ToString());
                command.ExecuteNonQuery();
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
        public bool Delete(string idBillService)
        {
            try
            {
                OpenConnection();
                string queryString = "DELETE FROM BillService WHERE idBillService=" + idBillService;
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
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
