using DocumentFormat.OpenXml.Packaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.Models.Enums;
using WpfApp1.ViewModels;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

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
        public ServicesResult<List<creditContractsDTO>> ListCreditContractsDto()
        {
            try
            {
                var contracts = _db.Contracts
                    .Select(x => new creditContractsDTO
                    {
                        ContractNumber = x.Id,
                        ProductName = x.ProductUnit.Product.Name,
                        CustomerName = $"{x.Client.Name} {x.Client.LastName}",
                        PriceSales = x.SalePrice,
                        DownPayment = x.DownPayment,
                        Status = x.Status.Code,
                        CreatedAt = x.CreatedAt ?? DateTime.Now,
                        Balance = x.PendingBalance,

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
                    }).ToList();

                if (contracts.Count == 0)
                    return ServicesResult<List<creditContractsDTO>>.Ok(contracts, "No hay frecuencias registradas.");

                return ServicesResult<List<creditContractsDTO>>.Ok(contracts, "Datos obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<creditContractsDTO>>.Fail("Error al obtener el listado de Frecuencias.");
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

        public async Task<ServicesResult<List<InstallmentPreview>>> GetDebtInstalmentAsync(int contractNumber)
        {
            try
            {
                var query = _db.DebtInstallments
                    .Include(x => x.Status)
                    .AsQueryable();
                
                if (contractNumber != null && contractNumber > 0)
                    query = query.Where(x => x.ContractId == contractNumber);
                var sql = query.ToQueryString();
                var result = await query
                    .OrderBy(x => x.DueDate)
                    .Select(x => new InstallmentPreview
                    {
                        InstallmentNumber = x.InstallmentNumber,
                        ExpectedAmount = x.ExpectedAmount,
                        DueDate = x.DueDate.ToDateTime(TimeOnly.MinValue),
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

        public async Task<ServicesResult<ReciboPagoCuotaDto>> RegisterPaymentAsync(int installmentId, DateTime paymentDate)
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
                    if (installment.StatusId == (int)InstallmentStatus.Paid || installment.StatusId == (int)InstallmentStatus.PaidLate)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<ReciboPagoCuotaDto>.Fail("Esta cuota ya fue pagada.");
                    }

                    bool pagoTarde = paymentDate.Date > installment.DueDate.ToDateTime(TimeOnly.MinValue).Date;
                    installment.PaidAmount = installment.ExpectedAmount;
                    installment.PaymentDate = DateOnly.FromDateTime(paymentDate);
                    installment.StatusId = pagoTarde ? (int)InstallmentStatus.PaidLate : (int)InstallmentStatus.Paid;

                    await db.SaveChangesAsync();
                    await ActualizarEstadoContratoAsync(installment.ContractId, db);
                    await SaveAuditAsync(AuditAction.Update, "DebtInstallment", installment.Id.ToString(), "Se registró el pago de la cuota", db);
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

                    var saldoPendiente = todasLasCuotas
                        .Where(x => x.StatusId != (int)InstallmentStatus.Paid && x.StatusId != (int)InstallmentStatus.PaidLate)
                        .Sum(x => x.ExpectedAmount);

                    var proximaCuota = todasLasCuotas
                        .Where(x => x.StatusId != (int)InstallmentStatus.Paid && x.StatusId != (int)InstallmentStatus.PaidLate)
                        .OrderBy(x => x.DueDate)
                        .FirstOrDefault();

                    var numeroCuotaPagada = todasLasCuotas.FindIndex(x => x.Id == installment.Id) + 1;

                    var recibo = new ReciboPagoCuotaDto
                    {
                        NumeroRecibo = installment.Id.ToString(),   // o un consecutivo propio si ya manejas uno
                        FechaPago = paymentDate,
                        ContractNumber = contrato.Id,   // ajusta al nombre real
                        NombreCliente = $"{contrato.Client.Name} {contrato.Client.LastName}",
                        NombreProducto = contrato.ProductUnit.Product.Name, // ajusta
                        NumeroCuota = numeroCuotaPagada,
                        TotalCuotas = todasLasCuotas.Count,
                        MontoPagado = installment.PaidAmount ?? 0,
                        SaldoPendiente = saldoPendiente,
                        ProximaFechaPago = proximaCuota?.DueDate.ToDateTime(TimeOnly.MinValue),
                        RecibidoPor = Environment.UserName  // o tu usuario de sesión real
                    };

                    await transaction.CommitAsync();
                    return ServicesResult<ReciboPagoCuotaDto>.Ok(recibo, "Pago registrado exitosamente.");
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
                                 || x.StatusId == (int)InstallmentStatus.Overdue)
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
                            DueDate = x.DueDate.ToDateTime(TimeOnly.MinValue),
                            DaysOverdue = x.DueDate < hoy ? hoy.DayNumber - x.DueDate.DayNumber : 0
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

            bool todasPagadas = cuotas.All(x => x.StatusId == (int)InstallmentStatus.Paid || x.StatusId == (int)InstallmentStatus.PaidLate);
            bool tieneVencidas = cuotas.Any(x => x.StatusId == (int)InstallmentStatus.Overdue
                                               || (x.StatusId == (int)InstallmentStatus.Pending && x.DueDate.ToDateTime(TimeOnly.MinValue) < DateTime.Today));

            contract.PendingBalance = cuotas.Where(x => x.StatusId != (int)InstallmentStatus.Paid && x.StatusId != (int)InstallmentStatus.PaidLate)
                                             .Sum(x => x.ExpectedAmount);

            if (todasPagadas)
                contract.StatusId = (int)EContractStatus.Completed;
            else if (tieneVencidas)
                contract.StatusId = (int)EContractStatus.Overdue;
            else
                contract.StatusId = (int)EContractStatus.Active;
        }

        public void GenerarContratoDocumento(TemplateContractDto datos)
        {
            if (datos == null) return;

            string rutaPlantilla = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plantillas", "Contrato.docx");
            string carpetaDestino = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ContratosGenerados");

            if (!File.Exists(rutaPlantilla))
            {
                MessageBox.Show($"No se encontró la plantilla del contrato en la ruta:\n{rutaPlantilla}",
                                "Error de Configuración", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (!Directory.Exists(carpetaDestino))
                    Directory.CreateDirectory(carpetaDestino);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string clienteSeguro = string.Concat((datos.NombreComprador ?? "Cliente").Split(Path.GetInvalidFileNameChars()));
                string rutaArchivoFinal = Path.Combine(carpetaDestino, $"Contrato_{clienteSeguro}_{timestamp}.docx");

                File.Copy(rutaPlantilla, rutaArchivoFinal, overwrite: true);

                var valores = new Dictionary<string, string>
                {
                    ["{NombreVendedor}"] = datos.NombreVendedor ?? "",
                    ["{Identidad}"] = datos.DniVendedor ?? "",
                    ["{Empresa}"] = datos.empresa ?? "",
                    ["{NombreComprador}"] = datos.NombreComprador ?? "",
                    ["{DniComprador}"] = datos.DniComprador ?? "",
                    ["{Residencia}"] = datos.DomicilioComprador ?? "",
                    ["{Producto}"] = datos.Articulo ?? "",
                    ["{Marca}"] = datos.Marca ?? "",
                    ["{Modelo}"] = datos.Modelo ?? "",
                    ["{color}"] = datos.Color ?? "",
                    ["{IMEI}"] = datos.Imei ?? "",
                    ["{IMEI2}"] = datos.Imei2 ?? "",
                    ["{PrecioTotal}"] = datos.PrecioTotal.ToString("N2"),
                    ["{Enganche}"] = datos.Prima.ToString("N2"),
                    ["{SaldoFinanciado}"] = datos.SaldoFinanciado.ToString("N2"),
                    ["{CantidadCuotas}"] = datos.CantidadCuotas.ToString(),
                    ["{montoCuotas}"] = datos.ValorCuota.ToString("N2"),
                    ["{FrecuenciaPago}"] = datos.FrecuenciaPago ?? "",
                    ["{diaPago}"] = datos.FechaInicio.Day.ToString("00"),
                    ["{primerPago}"] = datos.FechaInicio.ToString("dd 'de' MMMM 'del' yyyy"),
                    ["{ultimoPago}"] = datos.FechaFin.ToString("dd 'de' MMMM 'del' yyyy"),
                    ["{Municipio}"] = datos.Municipio ?? "",
                    ["{Departamento}"] = datos.Departamento ?? "",
                    ["{Anio}"] = datos.FechaFirma.ToString("dd 'días del mes de' MMMM 'del año' yyyy"),
                };

                using (WordprocessingDocument documento = WordprocessingDocument.Open(rutaArchivoFinal, true))
                {
                    ReemplazarPlaceholders(documento, valores);
                    documento.MainDocumentPart.Document.Save();
                }

                MessageBoxResult abrir = MessageBox.Show($"Contrato generado para {datos.NombreComprador}.\n\n¿Desea abrir el archivo ahora?",
                                                         "Operación Exitosa", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (abrir == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(rutaArchivoFinal) { UseShellExecute = true });
                }
            }
            catch (IOException)
            {
                MessageBox.Show("El archivo destino está abierto en Microsoft Word. Por favor ciérralo e intenta de nuevo.",
                                "Archivo Bloqueado", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error general al procesar el documento:\n{ex.Message}",
                                "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void ReemplazarPlaceholders(WordprocessingDocument documento, Dictionary<string, string> valores)
        {
            var body = documento.MainDocumentPart.Document.Body;

            foreach (var paragraph in body.Descendants<Paragraph>())
            {
                ReemplazarEnParrafo(paragraph, valores);
            }
        }

        private static void ReemplazarEnParrafo(Paragraph paragraph, Dictionary<string, string> valores)
        {
            bool huboReemplazo = true;

            while (huboReemplazo)
            {
                huboReemplazo = false;

                var runs = paragraph.Descendants<Run>().ToList();
                if (runs.Count == 0) break;

                var textoPorRun = runs.Select(r => string.Concat(r.Elements<Text>().Select(t => t.Text))).ToList();
                string textoCompleto = string.Concat(textoPorRun);

                var coincidencia = valores.Keys
                    .Select(k => new { Key = k, Index = textoCompleto.IndexOf(k, StringComparison.Ordinal) })
                    .Where(x => x.Index >= 0)
                    .OrderBy(x => x.Index)
                    .FirstOrDefault();

                if (coincidencia == null) break; 

                string valorNuevo = valores[coincidencia.Key];
                int inicio = coincidencia.Index;
                int fin = inicio + coincidencia.Key.Length;

                int cursor = 0, runInicio = -1, runFin = -1, offsetInicio = 0, offsetFin = 0;
                for (int i = 0; i < textoPorRun.Count; i++)
                {
                    int largo = textoPorRun[i].Length;
                    int finRun = cursor + largo;

                    if (runInicio == -1 && inicio < finRun)
                    {
                        runInicio = i;
                        offsetInicio = inicio - cursor;
                    }
                    if (fin <= finRun)
                    {
                        runFin = i;
                        offsetFin = fin - cursor;
                        break;
                    }
                    cursor = finRun;
                }

                if (runInicio == -1 || runFin == -1) break; 

                if (runInicio == runFin)
                {
                    string texto = textoPorRun[runInicio];
                    string nuevo = texto.Substring(0, offsetInicio) + valorNuevo + texto.Substring(offsetFin);
                    SetRunText(runs[runInicio], nuevo);
                }
                else
                {
                    string textoInicio = textoPorRun[runInicio];
                    string antes = textoInicio.Substring(0, offsetInicio);
                    SetRunText(runs[runInicio], antes + valorNuevo); 

                    for (int i = runInicio + 1; i < runFin; i++)
                        SetRunText(runs[i], ""); 

                    string textoFin = textoPorRun[runFin];
                    string despues = textoFin.Substring(offsetFin);
                    SetRunText(runs[runFin], despues); 
                }

                huboReemplazo = true; 
            }
        }

        private static void SetRunText(Run run, string nuevoTexto)
        {
            run.RemoveAllChildren<Text>();
            run.AppendChild(new Text(nuevoTexto) { Space = DocumentFormat.OpenXml.SpaceProcessingModeValues.Preserve });
        }
    }
}

