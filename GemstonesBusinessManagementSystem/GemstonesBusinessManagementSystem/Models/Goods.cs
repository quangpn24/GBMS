using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class Goods
    {
        private int idGoods;
        private string name;
        private long price;
        private int quantity;
        private int idGoodsType;
        private byte[] imageFile;
        private bool isDeleted;
        public int IdGoods { get => idGoods; set => idGoods = value; }
        public string Name { get => name; set => name = value; }
        public long Price { get => price; set => price = value; }
        public int IdGoodsType { get => idGoodsType; set => idGoodsType = value; }
        public int Quantity { get => quantity; set => quantity = value; }

        public byte[] ImageFile { get => imageFile; set => imageFile = value; }
        public bool IsDeleted { get => isDeleted; set => isDeleted = value; }

        public Goods()
        {

        }
        public Goods(int id, string name, long price, int quantity, int idGoodsType, byte[] imageFile, bool isDeleted)
        {
            this.idGoods = id;
            this.name = name;
            this.price = price;
            this.quantity = quantity;
            this.idGoodsType = idGoodsType;
            this.imageFile = imageFile;
            this.isDeleted = isDeleted;
        }
    }
}
