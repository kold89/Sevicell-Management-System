using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Security;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    class roleServices : BaseService
    {

        public ServicesResult<List<Role>>  GetListRoles()
        {
            try
            {
                using (var db = new DBSevicellContext())
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
                using (var db = new DBSevicellContext())
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
                using (var db = new DBSevicellContext())
                {
                    var rol = await db.Roles.FirstOrDefaultAsync(x => x.Id == id);
                    if (rol == null) 
                        return ServicesResult<bool>.Fail($"No se encontro el rol.");
                    
                    rol.Status = newStatus;
                    actionStatus = newStatus ? "habilitado" : "deshabilitado";
                    await db.SaveChangesAsync();

                    await db.AuditTables.AddAsync(new AuditTable
                    {
                        DateCreate = DateTime.Now,
                        UserId = SessionManager.loggedInUser.Id,
                        Accion = "UPDATE",
                        AffectedTable = "Roles",
                        ObjectId = id.ToString(),
                        Details = $"Rol {rol.Name} se ha cambiado su estado a {rol.Status}."
                    });
                    await db.SaveChangesAsync();

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
                using (var db = new DBSevicellContext())
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
                using (var db = new DBSevicellContext())
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
            // El Service es el DUEÑO del contexto y de la transacción
            using (var db = new DBSevicellContext())
            {
                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Usamos las funciones internas del mismo service
                        var nuevoRol = new Role { Name = name, Description = description, Status = true };
                        db.Roles.Add(nuevoRol);
                        await db.SaveChangesAsync(); // Guardamos para obtener el ID

                        // 2. Registramos permisos
                        foreach (var pId in permisosIds)
                        {
                            db.RolePermissions.Add(new RolePermission
                            {
                                RoleId = nuevoRol.Id,
                                PermissionId = pId,
                                DateCreation = DateTime.Now,
                            });
                        }

                        // 3. Registramos Auditoría 
                        db.AuditTables.Add(new AuditTable
                        {
                            DateCreate = DateTime.Now,
                            UserId = SessionManager.loggedInUser.Id,
                            Accion = "INSERT",
                            AffectedTable = "Roles",
                            ObjectId = nuevoRol.Id.ToString(),
                            Details = $"Rol {name} creado con {permisosIds.Count} permisos."
                        });

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

        public List<int> GetIdsPermisosPorRol(int rolId)
        {
            using (var db = new DBSevicellContext())
            {
                return db.RolePermissions
                         .Where(rp => rp.RoleId == rolId)
                         .Select(rp => rp.PermissionId)
                         .ToList();
            }
        }

        public async Task<ServicesResult<bool>> ActualizarRolCompletoAsync(int rolId, string name, string description, List<int> nuevosPermisosIds)
        {
            using (var db = new DBSevicellContext())
            {
                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. Buscar el rol existente
                        var rolDb = await db.Roles.FindAsync(rolId);
                        if (rolDb == null)
                            return ServicesResult<bool>.Fail("Error, rol no encontrado");

                        // 2. Actualizar datos básicos
                        rolDb.Name = name;
                        rolDb.Description = description;

                        // 3. Sincronizar Permisos (Borrar los actuales e insertar los nuevos)
                        var permisosViejos = db.RolePermissions.Where(rp => rp.RoleId == rolId);
                        db.RolePermissions.RemoveRange(permisosViejos);

                        foreach (var pId in nuevosPermisosIds)
                        {
                            db.RolePermissions.Add(new RolePermission
                            {
                                RoleId = rolId,
                                PermissionId = pId,
                                DateCreation = DateTime.Now
                            });
                        }

                        // 4. Registrar Auditoría de la Edición
                        db.AuditTables.Add(new AuditTable
                        {
                            DateCreate = DateTime.Now,
                            UserId = SessionManager.loggedInUser.Id,
                            Accion = "UPDATE",
                            AffectedTable = "Roles",
                            ObjectId = rolId.ToString(),
                            Details = $"Rol modificado: {name}. Ahora tiene {nuevosPermisosIds.Count} permisos."
                        });

                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return ServicesResult<bool>.Ok(true, "Datos actualizados con exito.");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<bool>.Fail("Error inesperado. " + ex.Message);
                    }
                }
            }
        }

    }
}
