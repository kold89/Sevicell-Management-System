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

        private const int ID_ADMIN = 1;
        private const int ID_VENTAS = 2;

        public static bool isAdmin => loggedInUser?.RoleId == ID_ADMIN;
        public static bool isVentas => loggedInUser?.RoleId == ID_VENTAS;



        public static void Logout()
        {
            loggedInUser = null;
        }
    }
}
