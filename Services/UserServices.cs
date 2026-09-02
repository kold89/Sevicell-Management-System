using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;
using WpfApp1.Data;
using Microsoft.EntityFrameworkCore;
using WpfApp1.ViewModels;


namespace WpfApp1.Services
{
    public class UserServices : BaseService
    {
        public ServicesResult<User?> ValidateUser(string username, string password)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var user = db.Users.FirstOrDefault(x => x.Username == username
                        && x.Password == password
                        && x.Status == true);

                    if (user == null)
                    {
                        return ServicesResult<User?>.Fail($"Error al validar el usuario {username}");
                    }

                    return ServicesResult<User?>.Ok(user, "Usuario validado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<User?>.Fail($"Error inesperado al validar el usuario {username}");
            }
        }

        public User? SearchUser(int id)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var item = db.Users.FirstOrDefault(x => x.Id == id);
                    return item;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ServicesResult<User>> RegisterUserAsync(User user)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    user.Password = Security.Security.HashPassword(user.Password);

                    db.Users.Add(user);
                    await SaveAuditAsync(AuditAction.Create, "Users", user.Id.ToString(), "Se creo un nuevo usuario", db);
                    await db.SaveChangesAsync();

                    return ServicesResult<User>.Ok(user, "Usuario creado exitosamente.");
                }       
            }
            catch (Exception ex)
            {
                return ServicesResult<User>.Fail("Error inesperado al registrar al usuario. " + ex.Message);
            }
        }

        public async Task<ServicesResult<User>> UpdateUserAsync(User user, bool IsPasswordUpdated)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    if (IsPasswordUpdated) user.Password = Security.Security.HashPassword(user.Password);

                    db.Users.Update(user);
                    await SaveAuditAsync(AuditAction.Update, "Users", user.Id.ToString(), "Se actualizo el usuario " + user.Username, db);
                    await db.SaveChangesAsync();

                    return ServicesResult<User>.Ok(user, "Usuario editado exitosamente.");
                }           
            }
            catch (Exception ex)
            {
                return ServicesResult<User>.Fail("Error inesperado al editar al usuario. " + ex.Message);
            }
        }

        public async Task<ServicesResult<bool>> ChangeStatusUserAsync(int id, bool newStatus)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    string statusUser = newStatus ? "habilitado" : "deshabilitado";

                    var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
                    if (user == null)
                    {
                        return ServicesResult<bool>.Fail($"Error el usuario no pudo ser {statusUser}");
                    }

                    user.Status = newStatus;
                    user.UpdatedAt = DateTime.Now;
                    await db.SaveChangesAsync();

                    return ServicesResult<bool>.Ok(true, $"Usuario {user.Name} {statusUser} con exito."); 
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Fail("Sucedio un error inesperado.");
            }
        }

        public async Task<ServicesResult<List<ViewUserDto>>> GetUserForDGridAsync()
        {
            try
            {
                using(var db = new SevicellDbContext())
                {
                    var data = await db.Users.Select(x => new ViewUserDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        lastName = x.LastName,
                        profile = x.Username,
                        status = (bool)x.Status ? "Activo" : "Deshabilitado",
                    }).ToListAsync();

                    if (data.Count == 0)
                    {
                        return ServicesResult<List<ViewUserDto>>.Ok(data, "No se encontraron datos.");
                    }

                    return ServicesResult<List<ViewUserDto>>.Ok(data, "Usuarios obtenidos exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<List<ViewUserDto>>.Fail("Error inesperado " + ex.Message);
            }
        }
    }
}
