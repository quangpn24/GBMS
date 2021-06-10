using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class Authorizations
    {
        private string authKey;

        public string AuthKey { get => authKey; set => authKey = value; }
        public Authorizations()
        {

        }
        public Authorizations(string authKey)
        {
            this.authKey = authKey;
        }
    }
}
