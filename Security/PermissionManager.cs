using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Data;

namespace WpfApp1.Security
{
    public class PermissionManager
    {
        private static HashSet<string> _permisos = new();
        public static event Action? PermisosActualizados;
        public static async Task LoadPermissionAsync(int roleId)
        {
            using var db = new DBSevicellContext();

            var codes = await db.RolePermissions 
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission.Code) 
                .ToListAsync();

            _permisos = new HashSet<string>(codes);
            PermisosActualizados?.Invoke();
        }

        public static bool Puede(string permissionCode)
        {
            return _permisos.Contains(permissionCode);
        }

        public static void ClearPermission()
        {
            _permisos = new HashSet<string>();
        }
    }
}
