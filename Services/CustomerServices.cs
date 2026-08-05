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
    public class CustomerServices : BaseService
    {
        public async Task<ServicesResult<List<custumerDTO>>> ListAllCustomersForGrid()
        {
            try
            {
                using (var db = new SevicellDbContext())
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

        public async Task<ServicesResult<bool>> RegisterCustomerAsync(Customer customer)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    db.Add(customer);
                    await db.SaveChangesAsync();

                    await SaveAuditAsync(AuditAction.Create, "Supplier", $"cliente Id {customer.Id}", "Se creo un nuevo cliente.");
                    return ServicesResult<bool>.Ok(true, "cliente registrado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Fail("Error al registrar la información.");
            }
        }
    }
}
