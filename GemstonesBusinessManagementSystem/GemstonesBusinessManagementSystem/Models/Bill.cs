using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class Bill
    {
        private int idBill;
        private int idAccount;
        private DateTime invoiceDate;
        private string status;
        private long totalMoney;
        private int idCustomer;
        private string note;

        public int IdBill { get => idBill; set => idBill = value; }
        public int IdAccount { get => idAccount; set => idAccount = value; }
        public DateTime InvoiceDate { get => invoiceDate; set => invoiceDate = value; }
        public string Status { get => status; set => status = value; }
        public long TotalMoney { get => totalMoney; set => totalMoney = value; }
        public int IdCustomer { get => idCustomer; set => idCustomer = value; }
        public string Note { get => note; set => note = value; }

        public Bill()
        {
        }

        public Bill(int id, int idAccount, DateTime date, string status, 
            long total, int idCustomer, string note)
        {
            this.idBill = id;
            this.idAccount = idAccount;
            this.invoiceDate = date;
            this.status = status;
            this.totalMoney = total;
            this.idCustomer = idCustomer;
            this.note = note;
        }
    }
}
