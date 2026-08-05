using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;
using WpfApp1.Models.Enums;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public class CreditContractsServices : BaseService
    {
        public ServicesResult<List<ViewSeller>> ListSellers()
        {
            try
            {
                var listSeller = _db.Sellers.
                  Select(x => new ViewSeller
                  {
                      id = x.Id,
                      name = ($"{x.FirstName} {x.LastName}"),
                      dni = x.Dni,
                      company = x.Company,
                      phone = x.Phone
                  }).ToList();

                if (listSeller.Count == 0)
                    return ServicesResult<List<ViewSeller>>.Ok(listSeller, "No hay vendedores registrados.");

                return ServicesResult<List<ViewSeller>>.Ok(listSeller, "Datos obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<ViewSeller>>.Fail("Error al obtener el listado de vendedores.");
            }
        }
        public ServicesResult<List<viewCustumers>> ListCustomers()
        {
            try
            {
                var listCustumer = _db.Customers.
                       Select(x => new viewCustumers
                       {
                           id = x.Id,
                           name = ($"{x.Name} {x.LastName}"),
                           direction = x.Address,
                           phone = x.Phone,
                           dni = x.Dni,
                           email = x.Email
                       }).ToList();

                if (listCustumer.Count == 0)
                    return ServicesResult<List<viewCustumers>>.Ok(listCustumer, "No hay clientes registrados.");

                return ServicesResult<List<viewCustumers>>.Ok(listCustumer, "Datos obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<viewCustumers>>.Fail("Error al obtener el listado de clientes.");
            }
        }
        public ServicesResult<List<viewFrecuency>> ListFrecuency()
        {
            try
            {
                var frecuenciesDb = _db.Frequencies.Select(x => new viewFrecuency
                {
                    id = x.Id,
                    name = x.Code
                }).ToList();

                if (frecuenciesDb.Count == 0)
                    return ServicesResult<List<viewFrecuency>>.Ok(frecuenciesDb, "No hay frecuencias registradas.");

                return ServicesResult<List<viewFrecuency>>.Ok(frecuenciesDb, "Datos obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<viewFrecuency>>.Fail("Error al obtener el listado de Frecuencias.");
            }
        }
        public ServicesResult<List<viewProductUnit>> ListProductsUnit()
        {
            try
            {
                var phonesUnit = _db.ProductUnits.Where(x => x.Status == EProductUnitStatus.Disponible.ToDbValue())
                    .Select(x => new viewProductUnit
                {
                    id = x.Id,
                    name = $"{x.Product.Name} IMEI {x.Imei}",
                    IMEI = x.Imei,
                    IMEI2 = x.Imei2,
                    priceSales = x.Product.SalePrice ?? 0,
                    description = x.Product.ProductDescription ?? ""
                }).ToList();

                if (phonesUnit.Count == 0)
                    return ServicesResult<List<viewProductUnit>>.Ok(phonesUnit, "No hay frecuencias registradas.");

                return ServicesResult<List<viewProductUnit>>.Ok(phonesUnit, "Datos obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<viewProductUnit>>.Fail("Error al obtener el listado de Frecuencias.");
            }
        }
        public ServicesResult<bool> ValidateContract(ContractCreateDto dto)
        {
            var errors = new List<string>();

            if (dto.ClientId <= 0) errors.Add("Debe seleccionar un cliente.");
            if (dto.SellerId <= 0) errors.Add("Debe seleccionar un vendedor.");
            if (dto.ProductUnitId <= 0) errors.Add("Debe seleccionar un producto.");
            if (dto.SalePrice <= 0) errors.Add("El precio de venta debe ser mayor a cero.");
            if (dto.DownPayment < 0) errors.Add("El enganche no puede ser negativo.");
            if (dto.DownPayment >= dto.SalePrice) errors.Add("El enganche no puede ser mayor o igual al precio de venta.");
            if (dto.InstallmentCount <= 0) errors.Add("El número de cuotas debe ser mayor a cero.");
            if (dto.FirstDueDate < DateTime.Today) errors.Add("La fecha de la primera cuota no puede ser anterior a hoy.");

            using (var db = new SevicellDbContext())
            {
                var unit = db.ProductUnits.FirstOrDefault(x => x.Id == dto.ProductUnitId);
                if (unit == null)
                    errors.Add("El producto seleccionado no existe.");
                else if (unit.Status != EProductUnitStatus.Disponible.ToDbValue())
                    errors.Add("El producto seleccionado ya no está disponible.");
            }

            if (errors.Count > 0)
                return ServicesResult<bool>.Fail(string.Join(" ", errors));

            return ServicesResult<bool>.Ok(true, "Validación exitosa.");
        }
        public async Task<ServicesResult<bool>> SaveContractAsync(ContractCreateDto dto, ContractCalculationResult calc)
        {
            using (var db = new SevicellDbContext())
            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    var contract = new Contract
                    {
                        ClientId = dto.ClientId,
                        ProductUnitId = dto.ProductUnitId,
                        SellerId = dto.SellerId,
                        CreatedAt = DateTime.Now,
                        SalePrice = dto.SalePrice,
                        DownPayment = dto.DownPayment,
                        FinancedBalance = calc.FinancedBalance,
                        InstallmentCount = dto.InstallmentCount,
                        InstallmentAmount = calc.InstallmentAmount,
                        FrequencyId = dto.FrequencyId,
                        FirstDueDate = DateOnly.FromDateTime(calc.Installments.First().DueDate),
                        LastDueDate = DateOnly.FromDateTime(calc.Installments.Last().DueDate),
                        LateInterestRate = dto.LateInterestRate,
                        StatusId = (int)EContractStatus.Pending,
                        PendingBalance = calc.TotalToPay,
                        Notes = dto.Notes
                    };

                    db.Contracts.Add(contract);
                    await db.SaveChangesAsync(); // necesitamos el contract.Id generado antes de crear las cuotas

                    foreach (var inst in calc.Installments)
                    {
                        db.DebtInstallments.Add(new DebtInstallment
                        {
                            ContractId = contract.Id,
                            InstallmentNumber = inst.InstallmentNumber,
                            DueDate = DateOnly.FromDateTime(inst.DueDate),
                            ExpectedAmount = inst.ExpectedAmount,
                            StatusId = (int)InstallmentStatus.Pending
                        });
                    }

                    var unit = await db.ProductUnits.FirstOrDefaultAsync(x => x.Id == dto.ProductUnitId);
                    if (unit == null)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<bool>.Fail("El producto ya no está disponible.");
                    }
                    unit.Status = EProductUnitStatus.Vendido.ToDbValue();

                    await SaveAuditAsync(AuditAction.Create, "Contract", contract.Id.ToString(), "Se creó un nuevo contrato de crédito", db);
                    await db.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return ServicesResult<bool>.Ok(true, "Contrato creado exitosamente.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServicesResult<bool>.Fail("Error al guardar el contrato: " + ex.Message);
                }
            }
        }
    }
}

