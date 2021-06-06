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

        public int IdBill { get => idBill; set => idBill = value; }
        public int IdGoods { get => idGoods; set => idGoods = value; }
        public int Quantity { get => quantity; set => quantity = value; }

        public BillInfo()
        {
        }

        public BillInfo(int id, int idgoods, int qti)
        {
            this.idBill = id;
            this.idGoods = idgoods;
            this.quantity = qti;
        }
    }
}
