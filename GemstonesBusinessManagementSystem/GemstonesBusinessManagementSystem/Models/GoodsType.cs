using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class GoodsType
    {
        private int idGoodsType;
        private double profitPercentage;
        private string name;
        private string unit;

        public int IdGoodsType { get => idGoodsType; set => idGoodsType = value; }
        public double ProfitPercentage { get => profitPercentage; set => profitPercentage = value; }
        public string Name { get => name; set => name = value; }
        public string Unit { get => unit; set => unit = value; }

        public GoodsType()
        {

        }
        public GoodsType(int id, string name, double profitPercentage, string unit)
        {
            this.idGoodsType = id;
            this.name = name;
            this.profitPercentage = profitPercentage;
            this.unit = unit;
        }
    }
}
