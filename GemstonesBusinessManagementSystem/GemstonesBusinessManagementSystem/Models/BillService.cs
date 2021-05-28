using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class BillService
    {
        private int idBillService;
        private int idAccount;
        private DateTime createdDate;
        private float total;
        private float totalPaidMoney;
        private int idCustomer;
        private int status;

        public int IdBillService { get => idBillService; set => idBillService = value; }
        public int IdAccount { get => idAccount; set => idAccount = value; }
        public DateTime CreatedDate { get => createdDate; set => createdDate = value; }
        public float Total { get => total; set => total = value; }
        public float TotalPaidMoney { get => totalPaidMoney; set => totalPaidMoney = value; }
        public int IdCustomer { get => idCustomer; set => idCustomer = value; }
        public int Status { get => status; set => status = value; }
        public BillService()
        {

        }

        public BillService(int idBillService, int idAccount, DateTime createdDate, float total, float totalPaidMoney, int idCustomer, int status)
        {
            this.idBillService = idBillService;
            this.idAccount = idAccount;
            this.createdDate = createdDate;
            this.total = total;
            this.totalPaidMoney = totalPaidMoney;
            this.idCustomer = idCustomer;
            this.status = status;
        }

    }
}
