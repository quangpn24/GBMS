using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class BillServiceInfo
    {
        private int idBillService;
        private int idService;
        private int quantity;
        private float paidMoney;
        private int status;
        private DateTime deliveryDate;

        public int IdBillService { get => idBillService; set => idBillService = value; }
        public int IdService { get => idService; set => idService = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public float PaidMoney { get => paidMoney; set => paidMoney = value; }
        public int Status { get => status; set => status = value; }
        public DateTime DeliveryDate { get => deliveryDate; set => deliveryDate = value; }

        public BillServiceInfo()
        {

        }
        public BillServiceInfo(int idBillService, int idService, int quantity, float paidMoney, int status, DateTime deliveryDate)
        {
            this.idBillService = idBillService;
            this.idService = idService;
            this.quantity = quantity;
            this.paidMoney = paidMoney;
            this.status = status;
            this.deliveryDate = deliveryDate;
        }
    }
}
