using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class MembershipsType
    {
        private int idMembershipsType;
        private string membership;
        private long target;

        public int IdMembershipsType { get => idMembershipsType; set => idMembershipsType = value; }
        public string Membership { get => membership; set => membership = value; }
        public long Target { get => target; set => target = value; }

        public MembershipsType()
        {

        }
        public MembershipsType(int idMembershipsType, string membership, long target)
        {
            this.idMembershipsType = idMembershipsType;
            this.membership = membership;
            this.target = target;
        }
    }
}
