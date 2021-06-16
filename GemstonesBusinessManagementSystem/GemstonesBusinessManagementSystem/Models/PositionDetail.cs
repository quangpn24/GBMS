using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class PositionDetail
    {
        private int idEmployeePosition;
        private int idPermission;
        private bool isPermitted;

        public int IdEmployeePosition { get => idEmployeePosition; set => idEmployeePosition = value; }
        public int IdPermission { get => idPermission; set => idPermission = value; }
        public bool IsPermitted { get => isPermitted; set => isPermitted = value; }

        public PositionDetail()
        {
        }

        public PositionDetail(int idPos, int idPer, bool isPermit)
        {
            this.idEmployeePosition = idPos;
            this.idPermission = idPer;
            this.isPermitted = isPermit;
        }
    }
}
