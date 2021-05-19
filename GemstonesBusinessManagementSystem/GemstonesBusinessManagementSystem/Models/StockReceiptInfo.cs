using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class StockReceiptInfo
    {
        private int idStockReceipt;
        private int idGoods;
        private int quantity;

        public int IdStockReceipt { get => idStockReceipt; set => idStockReceipt = value; }
        public int IdGoods { get => idGoods; set => idGoods = value; }
        public int Quantity { get => quantity; set => quantity = value; }

        public StockReceiptInfo(int idStockReceipt, int idGoods, int quantity)
        {
            this.idStockReceipt = idStockReceipt;
            this.idGoods = idGoods;
            this.quantity = quantity;
        }
    }
}
