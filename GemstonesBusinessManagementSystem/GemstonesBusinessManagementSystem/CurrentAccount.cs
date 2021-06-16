using GemstonesBusinessManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem
{
    public static class CurrentAccount
    {
        private static int idAccount;
        private static int idEmployee;
        private static int idPosition;
        private static string name;
        private static byte[] imageFile;
        private static List<PositionDetail> positionDetails;

        public static int IdAccount { get => idAccount; set => idAccount = value; }
        public static int IdEmployee { get => idEmployee; set => idEmployee = value; }
        public static int IdPosition { get => idPosition; set => idPosition = value; }
        public static string Name { get => name; set => name = value; }
        public static byte[] ImageFile { get => imageFile; set => imageFile = value; }
        internal static List<PositionDetail> PositionDetails { get => positionDetails; set => positionDetails = value; }
    }
}
