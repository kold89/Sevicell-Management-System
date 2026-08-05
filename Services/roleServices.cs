using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Security;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public class roleServices : BaseService
    {
        public ServicesResult<List<Role>>  GetListRoles()
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var roles =  db.Roles.Where(x => x.Status == true).ToList();
                    if(roles.Count == 0)  return ServicesResult<List<Role>>.Fail("No se encontro ningun rol.");

                    return ServicesResult<List<Role>>.Ok(roles, "Roles obtenidos con exito");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<List<Role>>.Fail("Erro al buscar roles." + ex.Message);
            }
        }

        public ServicesResult<Role?> GetRolForId(int id)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var item = db.Roles.FirstOrDefault(x => x.Id == id);
                    if (item == null)
                    {
                        return ServicesResult<Role?>.Fail("No se encontro rol.");
                    }

                    return ServicesResult<Role?>.Ok(item, "Rol encontrado con exito.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<Role?>.Fail("Error inesperado. " + ex.Message);

            }
        }

        public async Task<ServicesResult<bool>> ChangeRoleStatusAsync(int id, bool newStatus)
        {
            string actionStatus = "";
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var rol = await db.Roles.FirstOrDefaultAsync(x => x.Id == id);
                    if (rol == null) 
                        return ServicesResult<bool>.Fail($"No se encontro el rol.");
                    
                    rol.Status = newStatus;
                    actionStatus = newStatus ? "habilitado" : "deshabilitado";
                    await db.SaveChangesAsync();

                    await SaveAuditAsync(AuditAction.Update, "Roles", id.ToString(), $"Rol {rol.Name} se ha cambiado su estado a {rol.Status}.");

                    return ServicesResult<bool>.Ok(true, $"El rol ha sido {actionStatus} correctamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Fail($"Error al cambiar estado" + ex.Message);
            }
        }

        public List<ViewRoleDto> GetRoleForDGrid()
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    return db.Roles.Select(x => new ViewRoleDto
                    {
                        id = x.Id,
                        name = x.Name,
                        description = x.Description,
                        status = (bool)x.Status ? "Activo" : "Deshabilitado",
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<ViewRoleDto>();
            }
        }

        public List<Permission> GetPermissions()
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    return db.Permissions.ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<Permission>();
            }
        }

        public async Task<ServicesResult<bool>> RegistrarNuevoRolCompletoAsync(string name, string description, List<int> permisosIds)
        {
            using (var db = new SevicellDbContext())
            {
                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var nuevoRol = new Role { Name = name, Description = description, Status = true };
                        db.Roles.Add(nuevoRol);
                        await db.SaveChangesAsync(); // Guarda para obtener el ID

                        // 2. Registramos permisos
                        var newPermisions = BuildListPermissions(nuevoRol.Id, permisosIds);
                        db.RolePermissions.AddRange(newPermisions);

                        string mensaje = $"Rol {name} creado con {permisosIds.Count} permisos.";
                        await SaveAuditAsync(AuditAction.Create, "Roles", nuevoRol.Id.ToString(), mensaje, db);

                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return ServicesResult<bool>.Ok(true, "Rol y permisos creados con exito");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<bool>.Fail("Error al crear el rol y permisos. " + ex.Message);
                    }
                }
            }
        }

        public ServicesResult<List<int>> GetIdsPermisosPorRol(int rolId)
        {
            try 
            {
                using (var db = new SevicellDbContext())
                {
                    var listPermissions = db.RolePermissions
                             .Where(rp => rp.RoleId == rolId)
                             .Select(rp => rp.PermissionId)
                             .ToList();
                    return ServicesResult<List<int>>.Ok(listPermissions, "Permisos obtenidos con exito.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<List<int>>.Fail("Error inesperado. "+ ex.Message);
            } 
        }

        public async Task<ServicesResult<bool>> ActualizarRolCompletoAsync(int rolId, string name, string description, List<int> nuevosPermisosIds)
        {
            using (var db = new SevicellDbContext())
            {
                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Buscar el rol existente
                        var rolDb = await db.Roles.FindAsync(rolId);
                        if (rolDb == null)
                            return ServicesResult<bool>.Fail("Error, rol no encontrado");

                        rolDb.Name = name;
                        rolDb.Description = description;

                        // 3. Sincronizar Permisos
                        var permisosViejos = db.RolePermissions.Where(x => x.RoleId == rolId);
                        db.RolePermissions.RemoveRange(permisosViejos);

                        var newPermissions = BuildListPermissions(rolId, nuevosPermisosIds);
                        db.RolePermissions.AddRange(newPermissions);

                        string mensaje = $"Rol modificado: {name}. Ahora tiene {nuevosPermisosIds.Count} permisos.";
                        await SaveAuditAsync(AuditAction.Update, "Roles", rolId.ToString(), mensaje, db);
                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return ServicesResult<bool>.Ok(true, "Datos actualizados con éxito.");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<bool>.Fail("Error inesperado. " + ex.Message);
                    }
                }
            }
        }

        private List<RolePermission> BuildListPermissions(int roleId, List<int> newPermissionsId)
        {
            if(newPermissionsId.Count == 0)
                return new List<RolePermission>();
            
            return newPermissionsId.Select(pId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = pId,
                DateCreation = DateTime.Now
            }).ToList();
        }
    }
}
