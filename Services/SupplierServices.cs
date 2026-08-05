using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public class SupplierServices : BaseService
    {

        public async Task<ServicesResult<List<SupplierDto>>> listSupplierForGrid()
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var listSupplier = await db.Suppliers.Select(x => new SupplierDto
                    {
                        id = x.Id,
                        name = x.Name ?? "Nombre no registrado",
                        phone = x.Phone ?? "Número no registrado",
                        email = x.Email ?? "Sin  correo",
                        address = x.Address ?? "Sin dirección"

                    }).ToListAsync();

                    return ServicesResult<List<SupplierDto>>.Ok(listSupplier, "Datos obtenidos exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<List<SupplierDto>>.Fail("Error al consultar los proveedores");
            }

        }

        public ServicesResult<Supplier> GetSupplierById(int id)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var supplier = db.Suppliers.FirstOrDefault(x => x.Id == id);
                    return ServicesResult<Supplier>.Ok(supplier, "Datos obtenidos exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<Supplier>.Fail("Error al obtener los datos.");
            }

        }

        public ServicesResult<SupplierDto> SearchSupplierById(int id)
        {
            try 
            {
                using (var db = new  SevicellDbContext())
                {
                    var supplier = db.Suppliers.Where(x => x.Id == id).Select(x => new SupplierDto
                    {
                        id = x.Id,
                        name = x.Name,
                        phone = x.Phone,
                        address = x.Address,
                        email = x.Email,
                        cta = x.AccountNumber                        
                    }).FirstOrDefault();
                    return ServicesResult<SupplierDto>.Ok(supplier, "Datos obtenidos exitosamente.");
                }
            } catch (Exception ex)
            {
                return ServicesResult<SupplierDto>.Fail("Error al obtener los datos.");
            }
        }

        public async Task<ServicesResult<bool>> RegisterSupplierAsync(Supplier supplierToSave)
        {
            try
            {
                using (var db = new SevicellDbContext()) 
                {
                    db.Add(supplierToSave);
                    await db.SaveChangesAsync();

                    await SaveAuditAsync(AuditAction.Create, "Supplier", $"Proveedor Id {supplierToSave.Id}", "Se creo un nuevo proveedor.");
                    return ServicesResult<bool>.Ok(true, "Proveedor registrado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Fail("Error al registrar la información.");
            }
        }

        public async Task<ServicesResult<Supplier>> UpdateSupplierAsync(Supplier supplier)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    db.Update(supplier);
                    await SaveAuditAsync(AuditAction.Update,
                        "Supplier",
                        $"Proveedor con id {supplier.Id}",
                        "Se actualizaron datos",
                        db);

                    await db.SaveChangesAsync();
                    return ServicesResult<Supplier>.Ok(supplier, "Datos actualizados exitosamente.");
                }
            }
            catch (Exception ex) {
                return ServicesResult<Supplier>.Fail("Error al ingresar los datos.");

            }
        }
    }
}
