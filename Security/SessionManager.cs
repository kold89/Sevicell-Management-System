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
        
        public static void Logout()
        {
            loggedInUser = null;
        }
    }
}
