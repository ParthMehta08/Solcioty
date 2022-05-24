using DataAccessLayer;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class AccessbilityService
    {
        public static bool HasReadWritePermission(string RoleCode, string PermissionCode)
        {
            var _dbcontext = new SolciotyNewEntities();
            var hasReadWrite = false;
            hasReadWrite = _dbcontext.RolePermissionMappings.Any(e => e.RoleCode == RoleCode && e.PermissionCode == PermissionCode && (e.PermissionType == (int)PermissionType.ReadWrite || e.PermissionType == (int)PermissionType.Read));
            return hasReadWrite;
        }

        public static bool HasReadPermission(string RoleCode, string PermissionCode)
        {
            var _dbcontext = new SolciotyNewEntities();
            var hasReadWrite = false;
            hasReadWrite = _dbcontext.RolePermissionMappings.Any(e => e.RoleCode == RoleCode && e.PermissionCode == PermissionCode && e.PermissionType == (int)PermissionType.Read);
            return hasReadWrite;
        }

        public static PermissionType GetPermission(string RoleCode, string PermissionCode)
        {
            var _dbcontext = new SolciotyNewEntities();
            var permission = _dbcontext.RolePermissionMappings.FirstOrDefault(e => e.RoleCode == RoleCode && e.PermissionCode == PermissionCode);
            if (permission != null)
                return (PermissionType)permission.PermissionType;
            else
                return PermissionType.None;
        }
}
}
