using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    class SalesInvoiceServices : BaseService
    {
        public async Task<ServicesResult<bool>> SaveInvoiceAsync(
            string invoiceNumber,
            int? customerId,
            int? paymentMethodId,
            DateTime createdAt,
            decimal headerDiscount,
            List<(int ProductId, int Quantity, decimal UnitPrice)> details)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                decimal subtotal = details.Sum(d => d.Quantity * d.UnitPrice);
                decimal total = subtotal - headerDiscount;

                var invoice = new SalesInvoice
                {
                    InvoiceNumber = invoiceNumber,
                    CreatedAt = createdAt,
                    SubTotal = subtotal,
                    Discount = headerDiscount,
                    Tax = 0,
                    TotalAmount = total,
                    CustomerId = customerId,
                    PaymentMethodId = paymentMethodId,
                    Status = true,
                    RepairOrderId = null
                };

                _db.SalesInvoices.Add(invoice);
                await _db.SaveChangesAsync();

                foreach (var d in details)
                {
                    var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == d.ProductId);
                    if (product == null)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<bool>.Fail($"El producto con Id {d.ProductId} no existe.");
                    }

                    if (product.Stock < d.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<bool>.Fail(
                            $"Stock insuficiente para '{product.Name}'. Disponible: {product.Stock}, solicitado: {d.Quantity}.");
                    }

                    _db.SalesDetails.Add(new SalesDetail
                    {
                        SalesInvoiceId = invoice.Id,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        TaxPercent = 0,
                        DiscountPercent = 0
                    });

                    product.Stock -= d.Quantity;
                }

                await SaveAuditAsync(
                    AuditAction.Create,
                    "SalesInvoice",
                    invoiceNumber,
                    $"Venta registrada con {details.Count} producto(s) por un total de {total:N2}.",
                    _db);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServicesResult<bool>.Ok(true, "Venta guardada correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServicesResult<bool>.Fail("Error al guardar la venta: " + ex.Message);
            }
        }

        public ServicesResult<List<SalesInvoiceTypeDto>> GetTypeSalesInvoice()
        {
            try
            {
                var data = new List<SalesInvoiceTypeDto>();

                data.Add( new SalesInvoiceTypeDto{ Id = 0, Name = "--Todos--" });
                data.Add( new SalesInvoiceTypeDto{ Id = 1, Name = "Formal" });
                data.Add( new SalesInvoiceTypeDto { Id = 2, Name = "Informal" });

                return ServicesResult<List<SalesInvoiceTypeDto>>.Ok(data, "Datos obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<SalesInvoiceTypeDto>>.Fail("Error al obtener los datos.");
            }
        }
        /// <summary>
        /// Obtiene el listado de las ventas del dia seleccionado.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        //public async Task<ServicesResult<List<SalesInvoiceListDto>>> GetListSaleInvoiceAsync(SalesInvoiceFilterDto filter)
        //{
        //    try
        //    {
        //        var nextDayStart = filter.DateTo.Value.AddDays(1).AddTicks(-1);
        //        var result = await _db.SalesInvoices.Include(x => x.Customer)
        //            .Where(x => x.CreatedAt >= filter.DateFrom 
        //        && x.CreatedAt < nextDayStart).
        //        Select(x => new SalesInvoiceListDto
        //        {
        //            Id = x.Id,
        //            InvoiceNumber = x.InvoiceNumber,
        //            CustomerDisplay = x.Customer != null ? x.Customer.Name : "Cliente de mostrador",
        //            CreatedAt =x.CreatedAt,
        //            TotalAmount = x.TotalAmount,
        //            IsRegisteredCustomer = x.CustomerId != null,
        //        }).ToListAsync();

        //        if (result.Count == 0)
        //        {
        //            return ServicesResult<List<SalesInvoiceListDto>>.Fail("Error no se encontraron ventas.");
        //        }
        //        return ServicesResult<List<SalesInvoiceListDto>>.Ok(result, "Listado obtenido correctamente.");

        //    }
        //    catch (Exception ex)
        //    {
        //        return ServicesResult<List<SalesInvoiceListDto>>.Fail("Error al obtener las ventas: " + ex.Message);
        //    }
        //}
        /// <summary>
        /// Obtiene el listado de ventas, aplicando los filtros recibidos.
        /// </summary>
        public async Task<ServicesResult<List<SalesInvoiceListDto>>> GetInvoicesAsync(SalesInvoiceFilterDto filter)
        {
            try
            {
                var query = _db.SalesInvoices
                    .Include(x => x.Customer) 
                    .AsQueryable();

                if (filter.DateFrom.HasValue)
                    query = query.Where(x => x.CreatedAt >= filter.DateFrom.Value);

                if (filter.DateTo.HasValue)
                    query = query.Where(x => x.CreatedAt < filter.DateTo.Value.AddDays(1).AddTicks(-1));

                if (filter.TypeFilter == 1)
                    query = query.Where(x => x.CustomerId != null);
                else if (filter.TypeFilter == 2)
                    query = query.Where(x => x.CustomerId == null);

                if (filter.PaymentMethodId.HasValue && filter.PaymentMethodId.Value > 0)
                    query = query.Where(x => x.PaymentMethodId == filter.PaymentMethodId.Value);

                if (!string.IsNullOrWhiteSpace(filter.InvoiceNumber))
                    query = query.Where(x => x.InvoiceNumber.Contains(filter.InvoiceNumber));
                var sql = query.ToQueryString();
                var result = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new SalesInvoiceListDto
                    {
                        Id = x.Id,
                        InvoiceNumber = x.InvoiceNumber,
                        CreatedAt = x.CreatedAt,
                        CustomerDisplay = x.Customer != null ? x.Customer.Name : "Cliente de mostrador", // 👉 ajustar campo de nombre
                        IsRegisteredCustomer = x.CustomerId != null,
                        TotalAmount = x.TotalAmount
                    })
                    .ToListAsync();

                return ServicesResult<List<SalesInvoiceListDto>>.Ok(result, "Listado obtenido correctamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<SalesInvoiceListDto>>.Fail("Error al obtener las ventas: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el detalle completo de una venta puntual.
        /// </summary>
        public async Task<ServicesResult<SalesInvoiceDetailsViewDTO>> GetInvoiceDetailAsync(int invoiceId)
        {
            try
            {
                var invoice = await _db.SalesInvoices
                    .Include(x => x.Customer)
                    .Include(x => x.PaymentMethod)     // 👉 ajustar nombre de la propiedad de navegación
                    .Include(x => x.SalesDetails)           // 👉 ajustar nombre de la colección de detalle
                        .ThenInclude(d => d.Product)
                    .FirstOrDefaultAsync(x => x.Id == invoiceId);

                if (invoice == null)
                    return ServicesResult<SalesInvoiceDetailsViewDTO>.Fail("La venta no existe.");

                var dto = new SalesInvoiceDetailsViewDTO
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    CreatedAt = invoice.CreatedAt,
                    CustomerDisplay = invoice.Customer != null ? invoice.Customer.Name : "Cliente de mostrador",
                    IsRegisteredCustomer = invoice.CustomerId != null,
                    PaymentMethodName = invoice.PaymentMethod != null ? invoice.PaymentMethod.Name : "N/D",
                    SubTotal = invoice.SubTotal ?? 0,
                    Discount = invoice.Discount ?? 0,
                    Tax = invoice.Tax ?? 0,
                    TotalAmount = invoice.TotalAmount ?? 0,
                    Lines = invoice.SalesDetails.Select(d => new SalesDetailLineDto
                    {
                        ProductCode = d.Product.Code,   // 👉 ajustar si en tu entidad Product es "code" en minúscula
                        ProductName = d.Product.Name,
                        Quantity = d.Quantity ?? 0,
                        UnitPrice = d.UnitPrice ?? 0
                    }).ToList()
                };
                
                return ServicesResult<SalesInvoiceDetailsViewDTO>.Ok(dto, "Detalle obtenido correctamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<SalesInvoiceDetailsViewDTO>.Fail("Error al obtener el detalle: " + ex.Message);
            }
        }

        /// <summary>
        /// Nivel 1: resumen de ventas agrupado por día.
        /// </summary>
        public async Task<ServicesResult<List<DailySalesSummaryDto>>> GetDailySalesSummaryAsync(DateTime? dateFrom, DateTime? dateTo, int? paymentMethodId = null)
        {
            try
            {
                var query = _db.SalesInvoices.AsQueryable();

                if (dateFrom.HasValue)
                    query = query.Where(x => x.CreatedAt >= dateFrom.Value);

                if (dateTo.HasValue)
                    query = query.Where(x => x.CreatedAt <= dateTo.Value.AddDays(1).AddTicks(-1));

                if (paymentMethodId.HasValue && paymentMethodId.Value > 0)
                    query = query.Where(x => x.PaymentMethodId == paymentMethodId.Value);
                var sql = query.ToQueryString();
                var result = await query
                    .GroupBy(x => x.CreatedAt!.Value.Date)
                    .Select(g => new DailySalesSummaryDto
                    {
                        Date = g.Key,
                        InvoiceCount = g.Count(),
                        TotalAmount = g.Sum(x => x.TotalAmount) ?? 0
                    })
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();

                return ServicesResult<List<DailySalesSummaryDto>>.Ok(result, "Resumen obtenido correctamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<DailySalesSummaryDto>>.Fail("Error al obtener el resumen: " + ex.Message);
            }
        }

        /// <summary>
        /// Nivel 2: facturas de un día específico.
        /// </summary>
        public async Task<ServicesResult<List<SalesInvoiceListDto>>> GetInvoicesByDayAsync(DateTime day, int? paymentMethodId = null)
        {
            var filter = new SalesInvoiceFilterDto
            {
                DateFrom = day.Date,
                DateTo = day.Date,
                PaymentMethodId = paymentMethodId,
            };

            return await GetInvoicesAsync(filter);
        }

        /// <summary>
        /// Métricas de resumen para las tarjetas superiores: total en efectivo,
        /// días distintos con venta, y promedio de venta por día.
        /// </summary>
        public async Task<ServicesResult<SalesSummaryMetricsDto>> GetSalesSummaryMetricsAsync(DateTime? dateFrom, DateTime? dateTo, int? paymentMethodId = null)
        {
            try
            {
                var query = _db.SalesInvoices
                    .Include(x => x.PaymentMethod) 
                    .AsQueryable();

                if (dateFrom.HasValue)
                    query = query.Where(x => x.CreatedAt >= dateFrom.Value);

                if (dateTo.HasValue)
                    query = query.Where(x => x.CreatedAt <= dateTo.Value.AddDays(1).AddTicks(-1));

                if (paymentMethodId.HasValue && paymentMethodId.Value > 0)
                    query = query.Where(x => x.PaymentMethodId == paymentMethodId.Value);

                var invoices = await query.ToListAsync();

                decimal? cashTotal = invoices
                    .Where(x => paymentMethodId == 0 ||
                               (x.PaymentMethod != null && x.PaymentMethod.Id == paymentMethodId))
                    .Sum(x => x.TotalAmount);

                int distinctDays = invoices.Select(x => x.CreatedAt!.Value.Date).Distinct().Count();

                decimal? totalGeneral = invoices.Sum(x => x.TotalAmount);
                decimal? averagePerDay = distinctDays > 0 ? totalGeneral / distinctDays : 0;

                var dto = new SalesSummaryMetricsDto
                {
                    CashTotal = cashTotal ?? 0,
                    DistinctDaysCount = distinctDays,
                    AveragePerDay = averagePerDay ?? 0
                };

                return ServicesResult<SalesSummaryMetricsDto>.Ok(dto, "Métricas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<SalesSummaryMetricsDto>.Fail("Error al obtener las métricas: " + ex.Message);
            }
        }

        /// <summary>
        /// Años disponibles (con al menos una venta), para llenar el filtro de Año.
        /// </summary>
        public async Task<ServicesResult<List<int>>> GetAvailableYearsAsync()
        {
            try
            {
                var years = await _db.SalesInvoices
                    .Select(x => x.CreatedAt!.Value.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToListAsync();

                if (years.Count == 0)
                    years.Add(DateTime.Today.Year);

                return ServicesResult<List<int>>.Ok(years, "Años obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<int>>.Fail("Error al obtener los años: " + ex.Message);
            }
        }
    }
}
