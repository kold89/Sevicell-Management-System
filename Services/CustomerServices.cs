using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public class CustomerServices
    {
        public async Task<ServicesResult<List<custumerDTO>>> ListAllCustomersForGrid()
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    var data = await db.Customers.Select( 
                        x  => new custumerDTO{
                        Id = x.Id,
                        Name = $"{x.Name ?? ""} {x.LastName ?? ""}".Trim(),
                        Address = x.Address ?? "sin datos",
                        Mail = x.Email ?? "Sin correo"
                        }).ToListAsync();

                    return ServicesResult<List<custumerDTO>>.Ok(data, "Datos obtenidos exitosamente.");

                }
            }
            catch (Exception ex)
            {
                return ServicesResult<List<custumerDTO>>.Fail("Error al obtener los datos.");

            }
        }
    }
}
