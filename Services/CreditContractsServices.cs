using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;
using WpfApp1.Models.Enums;
using WpfApp1.ViewModels;
using WpfApp1.Security;

namespace WpfApp1.Services
{
    public class CreditContractsServices : BaseService
    {
        public ServicesResult<List<ViewSeller>> ListSellers()
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var listSeller = db.Sellers.
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
                using (var db = new SevicellDbContext())
                {
                    var listCustumer = db.Customers.
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
                using (var db = new SevicellDbContext())
                {
                    var frecuenciesDb = db.Frequencies.Select(x => new viewFrecuency
                    {
                        id = x.Id,
                        name = x.Code
                    }).ToList();

                    if (frecuenciesDb.Count == 0)
                        return ServicesResult<List<viewFrecuency>>.Ok(frecuenciesDb, "No hay frecuencias registradas.");

                    return ServicesResult<List<viewFrecuency>>.Ok(frecuenciesDb, "Datos obtenidos exitosamente.");
                }
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
                using (var db = new SevicellDbContext())
                {
                    var phonesUnit = db.ProductUnits.Where(x => x.Status == EProductUnitStatus.Disponible.ToDbValue())
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
            }
            catch (Exception ex)
            {
                return ServicesResult<List<viewProductUnit>>.Fail("Error al obtener el listado de Frecuencias.");
            }
        }


        public async Task<ServicesResult<List<creditContractsDTO>>> ListCreditContractsDtoAsync()
        {
            try
            {
                using var db = new SevicellDbContext();

                var contracts = await db.Contracts
                    .Select(x => new creditContractsDTO
                    {
                        ContractNumber = x.Id,
                        ProductName = x.ProductUnit.Product.Name,
                        CustomerName = $"{x.Client.Name} {x.Client.LastName}",
                        //CustomerPhone = x.Client.Phone,          // NUEVO: para poder filtrar por teléfono
                        PriceSales = x.SalePrice,
                        DownPayment = x.DownPayment,
                        Status = x.Status.Code,
                        CreatedAt = x.CreatedAt ?? DateTime.Now,
                        Balance = x.PendingBalance,
                        TotalDebt = x.TotalDebt ?? 0,
                        SellerName = $"{x.Seller.FirstName} {x.Seller.LastName}",
                        empresa = x.Seller.Company,
                        DniSeller = x.Seller.Dni,
                        DniCustomer = x.Client.Dni,
                        CustomerAddress = x.Client.Address,
                        Brand = x.ProductUnit.Product.Brand.Name,
                        Imei = x.ProductUnit.Imei,
                        Imei2 = x.ProductUnit.Imei2,
                        colour = x.ProductUnit.Colour,
                        model = x.ProductUnit.Model
                    })
                    .ToListAsync();

                return ServicesResult<List<creditContractsDTO>>.Ok(contracts,
                    contracts.Count == 0 ? "No hay contratos registrados." : "Datos obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<creditContractsDTO>>.Fail("Error al obtener los contratos: " + ex.Message);
            }
        }
        public ServicesResult<bool> ValidateContract(ContractCreateDto dto)
        {
            var errors = new List<string>();

            if (dto.ClientId <= 0) errors.Add("Debe seleccionar un cliente.");
            if (dto.SellerId <= 0) errors.Add("Debe seleccionar un vendedor.");
            if (dto.ProductUnitId <= 0) errors.Add("Debe seleccionar un producto.");
            if (dto.SalePrice <= 0) errors.Add("El precio de venta debe ser mayor a cero.");
            if (dto.interes <= 0) errors.Add("El interes debe ser mayor a cero.");
            if (dto.DownPayment < 0) errors.Add("El enganche no puede ser negativo.");
            if (dto.DownPayment >= dto.SalePrice) errors.Add("El enganche no puede ser mayor o igual al precio de venta.");
            if (dto.InstallmentCount <= 0) errors.Add("El número de cuotas debe ser mayor a cero.");
            if (dto.FirstDueDate < DateTime.Today) errors.Add("La fecha de la primera cuota no puede ser anterior a hoy.");
            if (dto.FrequencyId <= 0) errors.Add("Debe seleccionar una frecuencia.");

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
                        TotalDebt = calc.TotalToPay,
                        FirstDueDate = DateOnly.FromDateTime(calc.Installments.First().DueDate),
                        LastDueDate = DateOnly.FromDateTime(calc.Installments.Last().DueDate),
                        //LateInterestRate = dto.interes,
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

        public async Task<ServicesResult<List<InstallmentPreview>>> GetDebtInstalmentAsync(int contractNumber)
        {
            if (contractNumber <= 0)
                return ServicesResult<List<InstallmentPreview>>.Fail("Número de contrato inválido.");

            try
            {
                using var db = new SevicellDbContext();

                var result = await db.DebtInstallments
                    .Where(x => x.ContractId == contractNumber)
                    .OrderBy(x => x.DueDate)
                    .Select(x => new InstallmentPreview
                    {
                        InstallmentNumber = x.InstallmentNumber,
                        ExpectedAmount = x.ExpectedAmount,
                        DueDate = x.DueDate.ToDateTime(TimeOnly.MinValue),
                        PaidAmount = x.PaidAmount ?? 0,
                        PaymentDate = x.PaymentDate,
                        Status = x.Status.Code
                    })
                    .ToListAsync();

                return ServicesResult<List<InstallmentPreview>>.Ok(result, "Listado obtenido correctamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<InstallmentPreview>>.Fail("Error al obtener los pagos: " + ex.Message);
            }
        }
        public async Task<ServicesResult<ReciboPagoCuotaDto>> RegisterPaymentAsync(int installmentId, decimal amount, DateTime paymentDate, string? receivedBy = null, string? notes = null)
        {
            using (var db = new SevicellDbContext())
            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    var installment = await db.DebtInstallments.FirstOrDefaultAsync(x => x.Id == installmentId);
                    if (installment == null)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<ReciboPagoCuotaDto>.Fail("La cuota no existe.");
                    }
                    if (installment.StatusId == (int)InstallmentStatus.Paid
                        || installment.StatusId == (int)InstallmentStatus.PaidLate
                        || installment.StatusId == (int)InstallmentStatus.Waived)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<ReciboPagoCuotaDto>.Fail("Esta cuota ya fue pagada.");
                    }


                    var saldoActual = installment.ExpectedAmount - (installment.PaidAmount ?? 0);
                    if (amount <= 0 || amount > saldoActual)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<ReciboPagoCuotaDto>.Fail(
                            $"Monto inválido. El saldo pendiente de esta cuota es {saldoActual:N2}.");
                    }
                    db.InstallmentPayments.Add(new InstallmentPayment
                    {
                        InstallmentId = installment.Id,
                        Amount = amount,
                        PaymentDate = paymentDate,
                        ReceivedBy = receivedBy ?? SessionManager.loggedInUser.Name ?? "sistema",
                        Notes = notes
                    });

                    var nuevoPaidAmount = (installment.PaidAmount ?? 0) + amount;
                    var nuevoSaldo = installment.ExpectedAmount - nuevoPaidAmount;
                    bool pagoTarde = paymentDate.Date > installment.DueDate.ToDateTime(TimeOnly.MinValue).Date;

                    installment.PaidAmount = nuevoPaidAmount;
                    installment.PaymentDate = DateOnly.FromDateTime(paymentDate);
                    installment.StatusId = nuevoSaldo <= 0
                                   ? (int)(pagoTarde ? InstallmentStatus.PaidLate : InstallmentStatus.Paid)
                                   : (int)InstallmentStatus.Partial;

                    await db.SaveChangesAsync();
                    await ActualizarEstadoContratoAsync(installment.ContractId, db);
                    await SaveAuditAsync(AuditAction.Update, "DebtInstallment", installment.Id.ToString(),
                        nuevoSaldo <= 0 ? "Se registró el pago de la cuota" : "Se registró un abono parcial a la cuota", db);
                    await db.SaveChangesAsync();

                    // A partir de aquí armamos el recibo, ANTES del commit,
                    // para asegurarnos que todo lo que preguntemos ya refleja el pago recién guardado.
                    var contrato = await db.Contracts
                        .Include(c => c.Client)
                        .Include(c => c.ProductUnit)
                            .ThenInclude(pu => pu.Product)
                        .FirstOrDefaultAsync(c => c.Id == installment.ContractId);

                    if (contrato == null)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<ReciboPagoCuotaDto>.Fail("No se encontró el contrato asociado a la cuota.");
                    }

                    var todasLasCuotas = await db.DebtInstallments
                        .Where(x => x.ContractId == installment.ContractId)
                        .OrderBy(x => x.DueDate)
                        .ToListAsync();

                    var saldoPendienteContrato = todasLasCuotas
                       .Where(x => x.StatusId != (int)InstallmentStatus.Paid
                                && x.StatusId != (int)InstallmentStatus.PaidLate
                                && x.StatusId != (int)InstallmentStatus.Waived)
                       .Sum(x => x.ExpectedAmount - (x.PaidAmount ?? 0));

                    var proximaCuota = todasLasCuotas
                     .Where(x => x.StatusId != (int)InstallmentStatus.Paid
                              && x.StatusId != (int)InstallmentStatus.PaidLate
                              && x.StatusId != (int)InstallmentStatus.Waived)
                     .OrderBy(x => x.DueDate)
                     .FirstOrDefault();

                    var numeroCuotaPagada = todasLasCuotas.FindIndex(x => x.Id == installment.Id) + 1;

                    var recibo = new ReciboPagoCuotaDto
                    {
                        NumeroRecibo = installment.Id.ToString(),   // o un consecutivo propio si ya manejas uno
                        FechaPago = paymentDate,
                        ContractNumber = contrato.Id,
                        NombreCliente = $"{contrato.Client.Name} {contrato.Client.LastName}",
                        NombreProducto = contrato.ProductUnit.Product.Name,
                        NumeroCuota = numeroCuotaPagada,
                        TotalCuotas = todasLasCuotas.Count,
                        MontoPagado = amount,
                        SaldoCuota = nuevoSaldo > 0 ? nuevoSaldo : null,   // null si la cuota quedó saldada
                        SaldoPendiente = saldoPendienteContrato,
                        ProximaFechaPago = proximaCuota?.DueDate.ToDateTime(TimeOnly.MinValue),
                        RecibidoPor = receivedBy ?? Environment.UserName
                    };

                    await transaction.CommitAsync();
                    return ServicesResult<ReciboPagoCuotaDto>.Ok(recibo,
                        nuevoSaldo <= 0 ? "Pago registrado exitosamente." : "Abono registrado exitosamente.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServicesResult<ReciboPagoCuotaDto>.Fail("Error al registrar el pago: " + ex.Message);
                }
            }
        }
        public async Task<ServicesResult<List<CobroItemDto>>> GetCobrosDashboardAsync()
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var hoy = DateOnly.FromDateTime(DateTime.Today);
                    var limiteSemana = hoy.AddDays(7);

                    var data = await db.DebtInstallments
                        .Where(x => x.StatusId == (int)InstallmentStatus.Pending
                                 || x.StatusId == (int)InstallmentStatus.Overdue
                                 || x.StatusId == (int)InstallmentStatus.Partial)
                        .Where(x => x.DueDate <= limiteSemana)
                        .Select(x => new CobroItemDto
                        {
                            InstallmentId = x.Id,
                            ContractId = x.ContractId,
                            ContractNumber = x.Contract.Id.ToString(),
                            ClientName = x.Contract.Client.Name + " " + x.Contract.Client.LastName,
                            ProductName = x.Contract.ProductUnit.Product.Name,
                            InstallmentNumber = x.InstallmentNumber,
                            ExpectedAmount = x.ExpectedAmount,
                            PaidAmount = x.PaidAmount ?? 0,          // nuevo, para mostrar cuánto lleva abonado
                            DueDate = x.DueDate.ToDateTime(TimeOnly.MinValue),
                            DaysOverdue = x.DueDate < hoy ? hoy.DayNumber - x.DueDate.DayNumber : 0,
                            IsPartial = x.StatusId == (int)InstallmentStatus.Partial
                        })
                        .ToListAsync();

                    foreach (var item in data)
                    {
                        if (item.DaysOverdue > 0) item.UrgencyGroup = "OVERDUE";
                        else if (item.DueDate.Date == DateTime.Today) item.UrgencyGroup = "TODAY";
                        else item.UrgencyGroup = "WEEK";
                    }

                    return ServicesResult<List<CobroItemDto>>.Ok(data, "Datos obtenidos exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<List<CobroItemDto>>.Fail("Error al obtener las cuotas pendientes: " + ex.Message);
            }
        }

        private async Task ActualizarEstadoContratoAsync(int contractId, SevicellDbContext db)
        {
            var cuotas = await db.DebtInstallments.Where(x => x.ContractId == contractId).ToListAsync();
            var contract = await db.Contracts.FirstOrDefaultAsync(x => x.Id == contractId);
            if (contract == null) return;

            bool todasPagadas = cuotas.All(x => x.StatusId == (int)InstallmentStatus.Paid
                                              || x.StatusId == (int)InstallmentStatus.PaidLate
                                              || x.StatusId == (int)InstallmentStatus.Waived);

            bool tieneVencidas = cuotas.Any(x =>
                (x.StatusId == (int)InstallmentStatus.Pending
                 || x.StatusId == (int)InstallmentStatus.Overdue
                 || x.StatusId == (int)InstallmentStatus.Partial)
                && x.DueDate.ToDateTime(TimeOnly.MinValue) < DateTime.Today);

            contract.PendingBalance = cuotas
                .Where(x => x.StatusId != (int)InstallmentStatus.Paid
                         && x.StatusId != (int)InstallmentStatus.PaidLate
                         && x.StatusId != (int)InstallmentStatus.Waived)
                .Sum(x => x.ExpectedAmount - (x.PaidAmount ?? 0)); // antes sumaba ExpectedAmount completo, ignorando abonos

            if (todasPagadas)
                contract.StatusId = (int)EContractStatus.Completed;
            else if (tieneVencidas)
                contract.StatusId = (int)EContractStatus.Overdue;
            else
                contract.StatusId = (int)EContractStatus.Active;
        }        
        public async Task ActualizarVencimientosAsync()
        {
            using var db = new SevicellDbContext();
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            var contractIds = await db.DebtInstallments
                .Where(x => x.DueDate < hoy
                         && (x.StatusId == (int)InstallmentStatus.Pending
                          || x.StatusId == (int)InstallmentStatus.Partial))
                .Select(x => x.ContractId)
                .Distinct()
                .ToListAsync();

            foreach (var id in contractIds)
                await ActualizarEstadoContratoAsync(id, db);

            await db.SaveChangesAsync();
        }
        public async Task<ServicesResult<ReciboAdelantoDto>> RegisterAdvancePaymentAsync(
        int contractId, decimal amount, DateTime paymentDate, string? receivedBy = null, string? notes = null)
        {
            using (var db = new SevicellDbContext())
            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    var cuotasPendientes = await db.DebtInstallments
                        .Where(x => x.ContractId == contractId
                                 && x.StatusId != (int)InstallmentStatus.Paid
                                 && x.StatusId != (int)InstallmentStatus.PaidLate
                                 && x.StatusId != (int)InstallmentStatus.Waived)
                        .OrderBy(x => x.DueDate)
                        .ToListAsync();

                    if (cuotasPendientes.Count == 0)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<ReciboAdelantoDto>.Fail("Este contrato no tiene cuotas pendientes.");
                    }

                    var saldoTotalContrato = cuotasPendientes.Sum(x => x.ExpectedAmount - (x.PaidAmount ?? 0));
                    if (amount <= 0 || amount > saldoTotalContrato)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<ReciboAdelantoDto>.Fail(
                            $"Monto inválido. El saldo total pendiente del contrato es {saldoTotalContrato:N2}.");
                    }

                    var montoRestante = amount;
                    var detalleAplicado = new List<DetalleCuotaPagadaDto>();
                    var usuario = receivedBy ?? SessionManager.loggedInUser?.Name ?? "sistema";

                    foreach (var cuota in cuotasPendientes)
                    {
                        if (montoRestante <= 0) break;

                        var saldoCuota = cuota.ExpectedAmount - (cuota.PaidAmount ?? 0);
                        var aplicado = Math.Min(montoRestante, saldoCuota);

                        db.InstallmentPayments.Add(new InstallmentPayment
                        {
                            InstallmentId = cuota.Id,
                            Amount = aplicado,
                            PaymentDate = paymentDate,
                            ReceivedBy = usuario,
                            Notes = notes
                        });

                        var nuevoPaidAmount = (cuota.PaidAmount ?? 0) + aplicado;
                        var nuevoSaldoCuota = cuota.ExpectedAmount - nuevoPaidAmount;
                        bool pagoTarde = paymentDate.Date > cuota.DueDate.ToDateTime(TimeOnly.MinValue).Date;

                        cuota.PaidAmount = nuevoPaidAmount;
                        cuota.PaymentDate = DateOnly.FromDateTime(paymentDate);
                        cuota.StatusId = nuevoSaldoCuota <= 0
                            ? (int)(pagoTarde ? InstallmentStatus.PaidLate : InstallmentStatus.Paid)
                            : (int)InstallmentStatus.Partial;

                        detalleAplicado.Add(new DetalleCuotaPagadaDto
                        {
                            InstallmentNumber = cuota.InstallmentNumber,
                            MontoAplicado = aplicado,
                            QuedoSaldada = nuevoSaldoCuota <= 0
                        });

                        montoRestante -= aplicado;
                    }

                    await db.SaveChangesAsync();
                    await ActualizarEstadoContratoAsync(contractId, db);
                    await SaveAuditAsync(AuditAction.Update, "Contract", contractId.ToString(),
                        $"Se registró un adelanto de pago de {amount:N2}, aplicado a {detalleAplicado.Count} cuota(s)", db);
                    await db.SaveChangesAsync();

                    var contrato = await db.Contracts
                        .Include(c => c.Client)
                        .Include(c => c.ProductUnit).ThenInclude(pu => pu.Product)
                        .FirstOrDefaultAsync(c => c.Id == contractId);

                    var todasLasCuotas = await db.DebtInstallments
                        .Where(x => x.ContractId == contractId)
                        .ToListAsync();

                    var saldoPendienteContrato = todasLasCuotas
                        .Where(x => x.StatusId != (int)InstallmentStatus.Paid
                                 && x.StatusId != (int)InstallmentStatus.PaidLate
                                 && x.StatusId != (int)InstallmentStatus.Waived)
                        .Sum(x => x.ExpectedAmount - (x.PaidAmount ?? 0));

                    var recibo = new ReciboAdelantoDto
                    {
                        FechaPago = paymentDate,
                        ContractNumber = contrato.Id,
                        NombreCliente = $"{contrato.Client.Name} {contrato.Client.LastName}",
                        NombreProducto = contrato.ProductUnit.Product.Name,
                        MontoTotalAbonado = amount,
                        CuotasAplicadas = detalleAplicado,
                        SaldoPendienteContrato = saldoPendienteContrato,
                        RecibidoPor = usuario
                    };

                    await transaction.CommitAsync();
                    return ServicesResult<ReciboAdelantoDto>.Ok(recibo,
                        $"Adelanto registrado, aplicado a {detalleAplicado.Count} cuota(s).");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ServicesResult<ReciboAdelantoDto>.Fail("Error al registrar el adelanto: " + ex.Message);
                }
            }
        }

    }
}