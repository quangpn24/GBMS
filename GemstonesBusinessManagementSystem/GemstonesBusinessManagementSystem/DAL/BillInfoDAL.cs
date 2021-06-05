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
    class BillInfoDAL : Connection
    {
        private static BillInfoDAL instance;

        public static BillInfoDAL Instance
        {
            get { if (instance == null) instance = new BillInfoDAL(); return BillInfoDAL.instance; }
            private set { BillInfoDAL.instance = value; }
        }
        private BillInfoDAL()
        {

        }
    }
}
