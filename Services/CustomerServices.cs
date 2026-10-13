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
                        Mail = x.Email ?? "Sin correo",
                        Phone = x.Phone ?? "-"
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
                    bool dniRepetido = await db.Customers.AnyAsync(c => c.Dni == customer.Dni);
                    if (dniRepetido)
                        return ServicesResult<bool>.Fail("Ya existe un cliente con ese número de identidad.");

                    db.Add(customer);
                    await db.SaveChangesAsync();

                    await SaveAuditAsync(AuditAction.Create, "Customer", $"cliente Id {customer.Id}", "Se creó un nuevo cliente.");
                    return ServicesResult<bool>.Ok(true, "Cliente registrado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Fail("Error al registrar la información.");
            }
        }
        public async Task<ServicesResult<Customer>> GetCustomerByIdAsync(int id)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var customer = await db.Customers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (customer == null)
                        return ServicesResult<Customer>.Fail("El cliente no existe.");

                    return ServicesResult<Customer>.Ok(customer, "Cliente obtenido exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<Customer>.Fail("Error al obtener el cliente.");
            }
        }

        public async Task<ServicesResult<bool>> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var existing = await db.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
                    if (existing == null)
                        return ServicesResult<bool>.Fail("El cliente no existe.");

                    bool dniRepetido = await db.Customers
                        .AnyAsync(c => c.Dni == customer.Dni && c.Id != customer.Id);
                    if (dniRepetido)
                        return ServicesResult<bool>.Fail("Ya existe otro cliente con ese número de identidad.");

                    existing.Name = customer.Name;
                    existing.LastName = customer.LastName;
                    existing.Email = customer.Email;
                    existing.Phone = customer.Phone;
                    existing.Dni = customer.Dni;
                    existing.Address = customer.Address;

                    await db.SaveChangesAsync();

                    await SaveAuditAsync(AuditAction.Update, "Customer", $"cliente Id {existing.Id}", "Se actualizó un cliente.");
                    return ServicesResult<bool>.Ok(true, "Cliente actualizado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Fail("Error al actualizar la información.");
            }
        }
    }
}
