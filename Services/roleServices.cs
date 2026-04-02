using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Data;
using WpfApp1.Models;
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
                    return db.Roles.ToList();
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
    }
}
