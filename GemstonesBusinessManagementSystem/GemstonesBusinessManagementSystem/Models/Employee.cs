using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class Employee
    {
		private int idEmployee;
		private int idAccount;
		private string name;
		private string gender;
		private string phoneNumber;
		private string address;
		private DateTime dateOfBirth;
		private string position;
		private DateTime startingDate;
		private byte[] imageFile;
		private bool isDeleted;

        public int IdEmployee { get => idEmployee; set => idEmployee = value; }
        public int IdAccount { get => idAccount; set => idAccount = value; }
        public string Name { get => name; set => name = value; }
        public string Gender { get => gender; set => gender = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        public string Address { get => address; set => address = value; }
        public DateTime DateOfBirth { get => dateOfBirth; set => dateOfBirth = value; }
        public string Position { get => position; set => position = value; }
        public DateTime StartingDate { get => startingDate; set => startingDate = value; }
        public byte[] ImageFile { get => imageFile; set => imageFile = value; }
        public bool IsDeleted { get => isDeleted; set => isDeleted = value; }

        public Employee()
        {

        }

        public Employee(int idEmployee, string name, string gender, 
            string phonenumber, string address, DateTime dateOfBirth,
            string position, DateTime startingdate, int idAccount,
            byte[] image, bool isdeleted = false)
        {
            this.idAccount = idAccount;
            this.IdEmployee = idEmployee;
            this.name = name;
            this.gender = gender;
            this.phoneNumber = phonenumber;
            this.address = address;
            this.dateOfBirth = dateOfBirth;
            this.position = position;
            this.startingDate = startingdate;
            this.imageFile = image;
            this.isDeleted = isdeleted;
        }
    }
}
