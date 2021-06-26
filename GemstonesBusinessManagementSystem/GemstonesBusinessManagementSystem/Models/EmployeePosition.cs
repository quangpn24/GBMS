using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class EmployeePosition
    {
        private int idEmployeePosition;
        private string position;
        private long salaryBase;
        private long moneyPerShift;
        private long moneyPerFault;
        private int standardWorkDays;
        private bool isDeleted;

        public int IdEmployeePosition { get => idEmployeePosition; set => idEmployeePosition = value; }
        public string Position { get => position; set => position = value; }
        public long SalaryBase { get => salaryBase; set => salaryBase = value; }
        public long MoneyPerShift { get => moneyPerShift; set => moneyPerShift = value; }
        public long MoneyPerFault { get => moneyPerFault; set => moneyPerFault = value; }
        public int StandardWorkDays { get => standardWorkDays; set => standardWorkDays = value; }
        public bool IsDeleted { get => isDeleted; set => isDeleted = value; }

        public EmployeePosition()
        {
        }

        public EmployeePosition(int id, string pos, long salarybase, long shift, long fault, 
            int days, bool deleted = false)
        {
            idEmployeePosition = id;
            position = pos;
            salaryBase = salarybase;
            moneyPerShift = shift;
            moneyPerFault = fault;
            standardWorkDays = days;
            isDeleted = deleted;
        }
    }
}
