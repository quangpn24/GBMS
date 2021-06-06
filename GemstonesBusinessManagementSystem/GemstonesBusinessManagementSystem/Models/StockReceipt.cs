using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class StockReceipt
    {
        private int id;
        private int idAccount;
        private int idSupplier;
        private DateTime date;
        private long totalMoney;
        

        public int Id { get => id; set => id = value; }
        public int IdAccount { get => idAccount; set => idAccount = value; }
        public int IdSupplier { get => idSupplier; set => idSupplier = value; }
        public DateTime Date { get => date; set => date = value; }
        public long TotalMoney { get => totalMoney; set => totalMoney = value; }

        public StockReceipt(int id , int idAccount, DateTime date, long total, int idSupplier)
        {
            this.id = id;
            this.idAccount = idAccount;
            this.date = date;
            this.totalMoney = total;
            this.idSupplier = idSupplier;
        }
        public StockReceipt() { }
    }
}
