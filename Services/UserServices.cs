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
    class UserServices
    {
        public User? ValidateUser(string user, string password)
        {
            try {
                using (var db = new DBSevicellContext())
                {
                    return db.Users.FirstOrDefault(x => x.Username == user
                    && x.Password == password
                    && x.Status == true);
                }
            }
            catch (Exception ex) 
            {
                return null;
            } 
        }

        public User? SearchUser(int id)
        {
            try
            {
                using (var db = new DBSevicellContext())
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

        public void RegisterUser(User user)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    user.Password = Security.Security.HashPassword(user.Password);

                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
        }

        public void UpdateUser (User user, bool IsPasswordUpdated)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    if (IsPasswordUpdated) user.Password = Security.Security.HashPassword(user.Password);
                    
                    db.Users.Update(user);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            } 
        }
     

        public async Task<ServicesResult<bool>>  ChangeStatusUserAsync(int id, bool newStatus)
        {
            try
            {
                using (var db = new DBSevicellContext())
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

                    return ServicesResult<bool>.Ok(true, $"Usuario {user.Name} {statusUser} con exito."); ;
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
                using (var db = new DBSevicellContext())
                {
                    var data = await db.Users.Select(x => new ViewUserDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        lastName = x.LastName,
                        profile = x.Username,
                        status = (bool)x.Status ? "Activo" : "Deshabilitado",
                    }).ToListAsync();

                    if(data.Count == 0)
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
