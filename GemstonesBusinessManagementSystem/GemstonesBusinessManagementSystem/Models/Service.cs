using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class Service
    {
        private int idService;

        public int IdService { get => idService; set => idService = value; }

        private string name;

        public string Name { get => name; set => name = value; }

        private long price;

        public long Price { get => price; set => price = value; }

        private int isDeleted;

        public int IsDeleted { get => isDeleted; set => isDeleted = value; }

        private int isActived;

        public int IsActived { get => isActived; set => isActived = value; }

        public Service()
        {

        }
        public Service(int idService, string name, long price, int isActived, int isDeleted)
        {
            this.idService = idService;
            this.name = name;
            this.price = price;
            this.isDeleted = isDeleted;
            this.isActived = isActived;
        }
    }
}
