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
     
        public bool DisableUser(int id)
        {
            try
            {
                bool isDisable = false;
                using (var db = new DBSevicellContext())
                {
                    var user = db.Users.FirstOrDefault(x => x.Id == id);
                    if (user != null)
                    {
                        user.Status = false;
                        user.UpdatedAt = DateTime.Now;
                        db.SaveChanges();
                        isDisable = true;
                    }

                    return isDisable;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool EnableUser(int id)
        {
            try
            {
                bool isEnable = false;
                using (var db = new DBSevicellContext())
                {
                    var user = db.Users.FirstOrDefault(x => x.Id == id);
                    if (user != null)
                    {
                        user.Status = true;
                        user.UpdatedAt = DateTime.Now;
                        db.SaveChanges();
                        isEnable = true;
                    }

                    return isEnable;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<ViewUserDto> GetUserForDGrid()
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    return db.Users.Select(x => new ViewUserDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        lastName = x.LastName,
                        profile = x.Username,
                        status = (bool)x.Status ? "Activo" : "Deshabilitado",
                    }).ToList();
                }
            }
            catch (Exception ex) 
            {
                return new List<ViewUserDto>();
            }     
        }
    }
}
