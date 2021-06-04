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
        private double totalPrice;
        private string address;
        private int idMembership;
        public int IdCustomer { get => idCustomer; set => idCustomer = value; }
        public string CustomerName { get => customerName; set => customerName = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        public string IdNumber { get => idNumber; set => idNumber = value; }
        public double TotalPrice { get => totalPrice; set => totalPrice = value; }
        public string Address { get => address; set => address = value; }
        public int IdMembership { get => idMembership; set => idMembership = value; }

        public Customer()
        {

        }

        public Customer(int idCustomer, string customerName,
            string phoneNumber, string address, string idNumber, double totalPrice, int idMembership)
        {
            this.idCustomer = idCustomer;
            this.customerName = customerName;
            this.phoneNumber = phoneNumber;
            this.idNumber = idNumber;
            this.totalPrice = totalPrice;
            this.address = address;
            this.idMembership = idMembership;
        }
    }
}