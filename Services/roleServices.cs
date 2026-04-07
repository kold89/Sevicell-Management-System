using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Security;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    class roleServices
    {

        public List<Role> GetRoles()
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    return db.Roles.Where(x => x.Status == true).ToList();
                }
            }
            catch (Exception ex)
            {
                return new List<Role>();
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

        public async Task<bool> RegistrarNuevoRolCompletoAsync(string name, string description, List<int> permisosIds)
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

                        // 3. Registramos Auditoría (Incluso podrías llamar a un AuditService aquí)
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
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        // Aquí podrías usar un Logger para guardar el error en un archivo
                        return false;
                    }
                }
            }
        }

    }
}
