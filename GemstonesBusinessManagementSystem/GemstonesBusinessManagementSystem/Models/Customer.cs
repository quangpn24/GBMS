using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class Customer
    {
        private int idCustomer;
        private string customerName;
        private string phoneNumber;
        private string idNumber;
        private long totalPrice;
        private int idMembership;
        private string address;

        public int IdCustomer { get => idCustomer; set => idCustomer = value; }
        public string CustomerName { get => customerName; set => customerName = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        public long TotalPrice { get => totalPrice; set => totalPrice = value; }
        public int IdMembership { get => idMembership; set => idMembership = value; }
        public string Address { get => address; set => address = value; }
        public string IdNumber { get => idNumber; set => idNumber = value; }

        public Customer()
        {

        }

        public Customer(int idCustomer, string customerName,
            string phoneNumber, string address, string idNumber, long totalPrice, int idMembership)
        {
            this.idCustomer = idCustomer;
            this.customerName = customerName;
            this.phoneNumber = phoneNumber;
            this.address = address;
            this.idNumber = idNumber;
            this.totalPrice = totalPrice;
            this.idMembership = idMembership;
        }
    }
}
