using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.Models
{
    class Permission
    {
        private int idPermission;
        private string permissionName;

        public int IdPermission { get => idPermission; set => idPermission = value; }
        public string PermissionName { get => permissionName; set => permissionName = value; }

        public Permission()
        {
        }

        public Permission(int id, string name)
        {
            this.idPermission = id;
            this.permissionName = name;
        }
    }
}
