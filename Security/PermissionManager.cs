using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Security
{
    /// <summary>
    /// Clase encargada de manejar los permisos activos del usuario logueado.
    /// </summary>
    public class PermissionManager
    {
        private static HashSet<string> _permisos = new();
        public static event Action? PermisosActualizados;
        private static int? _currentRoleId;

        public static async Task LoadPermissionAsync(int roleId)
        {
            using var db = new SevicellDbContext();
            {
                var codes = await db.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission.Code)
                .ToListAsync();

                _permisos = new HashSet<string>(codes);
                _currentRoleId = roleId;
                PermisosActualizados?.Invoke();
            }         
        }
        /// <summary>
        /// Valida si el usuario tiene asignado dicho permiso.
        /// </summary>
        /// <param name="permissionCode"></param>
        /// <returns></returns>
        public static bool Puede(string permissionCode)
        {
            return _permisos.Contains(permissionCode);
        }

        public static void ClearPermission()
        {
            _permisos = new HashSet<string>();
            _currentRoleId = null;
        }
        public static async Task RefreshIfCurrentUserAsync(int editedRoleId)
        {
            if (_currentRoleId.HasValue && editedRoleId == _currentRoleId.Value)
            {
                await LoadPermissionAsync(editedRoleId);
            }
        }
        protected ServicesResult<T>? ValidarPermiso<T>(string permissionCode)
        {
            if (!PermissionManager.Puede(permissionCode))
                return ServicesResult<T>.Fail("No tiene permisos para realizar esta acción.");

            return null; 
        }
        /*
         * FORMA DE USO DE VALIDAR PERMISO.
         * public async Task<ServicesResult<bool>> SaveInvoiceAsync(...)
        {
            var permisoError = ValidarPermiso<bool>("PURCHASES_CREATE");
            if (permisoError != null) return permisoError;

            using var transaction = await _db.Database.BeginTransactionAsync();
            // ... tu lógica existente, sin cambios
        }*/
    }
}
