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
        private int quantity;
        private long price;

        public int IdBill { get => idBill; set => idBill = value; }
        public int IdGoods { get => idGoods; set => idGoods = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public long Price { get => price; set => price = value; }

        public BillInfo()
        {
        }

        public BillInfo(int id, int idgoods, int qti, long pri)
        {
            this.idBill = id;
            this.idGoods = idgoods;
            this.quantity = qti;
            this.price = pri;
        }
    }
}
