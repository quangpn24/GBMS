using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class Supplier
    {
        private int id;
        public int Id { get => id; set => id = value; }
        private string name;
        public string Name { get => name; set => name = value; }
        private string address;
        public string Address { get => address; set => address = value; }
        private string phoneNumber;
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }

        public Supplier(int id, string name, string address, string phoneNumber)
        {
            this.id = id;
            this.name = name;
            this.address = address;
            this.phoneNumber = phoneNumber;
        }
        public Supplier() { }

    }
}
