using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class BillInfo
    {
        private int idBill;
        private int idGoods;
        private double price;
        private int quantity;
        private int status;

        public int IdBill { get => idBill; set => idBill = value; }
        public int IdGoods { get => idGoods; set => idGoods = value; }
        public double Price { get => price; set => price = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public int Status { get => status; set => status = value; }

        public BillInfo()
        {

        }
        public BillInfo(int idBill, int idGoods, double price, int quantity, int status)
        {
            this.idBill = idBill;
            this.idGoods = idGoods;
            this.price = price;
            this.quantity = quantity;
            this.status = status;
        }
    }
}
