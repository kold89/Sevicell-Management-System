using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1.Security
{
    public static class SessionManager
    {
        public static User? loggedInUser {  get; set; }

        public static event Action? UserSessionChanged;
        public static void Logout()
        {
            loggedInUser = null;
        }
        public static string? GetProfile(int? id)
        {
            using(var db = new SevicellDbContext())
            {
                return db.Roles.FirstOrDefault(x => x.Id == id)?.Name;
            }
        }
        private static async Task GetUserAsync(int? id)
        {
            using (var db = new SevicellDbContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {  Logout();
                    return;
                }

                loggedInUser = user;
                UserSessionChanged?.Invoke();
            }
        }
        public static async Task RefreshIfUserChangedAsync(int idUser)
        {
            if (loggedInUser != null && idUser == loggedInUser.Id)
            {
                await GetUserAsync(idUser);
            }
        }
    }
}
