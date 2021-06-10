using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class Account
    {
        private int idAccount;
        private string username;
        private string password;
        private int type;

        public int IdAccount { get => idAccount; set => idAccount = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public int Type { get => type; set => type = value; }
        
        public Account() { }

        public Account(int idAccount, string username, string password, int type)
        {
            this.idAccount = idAccount;
            this.username = username;
            this.password = password;
            this.type = type;
        }
    }


}
