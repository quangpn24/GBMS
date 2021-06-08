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
        private long importPrice;

        public int IdStockReceipt { get => idStockReceipt; set => idStockReceipt = value; }
        public int IdGoods { get => idGoods; set => idGoods = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public long ImportPrice { get => importPrice; set => importPrice = value; }

        public StockReceiptInfo(int idStockReceipt, int idGoods, int quantity, long importPrice)
        {
            this.idStockReceipt = idStockReceipt;
            this.idGoods = idGoods;
            this.quantity = quantity;
            this.importPrice = importPrice;
        }
    }
}
