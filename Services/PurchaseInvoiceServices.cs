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
    class PurchaseInvoiceServices : BaseService
    {
        public async Task<ServicesResult<bool>> SaveInvoiceAsync(

           string invoiceNumber,
           bool invoiceNumberExist,
           int? supplierId,
           string? supplierNameCasual,
           DateTime createdAt,

           List<(int ProductId, int Quantity, decimal PurchasePrice)> details)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                decimal total = details.Sum(d => d.Quantity * d.PurchasePrice);

                var invoice = new PurchaseInvoice
                {
                    InvoiceNumber = invoiceNumber,
                    InoviceNumberExist = invoiceNumberExist,
                    SupplierId = supplierId,
                    SupplierNameCasual = supplierNameCasual,
                    CreatedAt = createdAt,
                    SubTotal = total,
                    Tax = 0,
                    TotalAmount = total
                };

                _db.PurchaseInvoices.Add(invoice);
                await _db.SaveChangesAsync(); 

                foreach (var d in details)
                {
                    var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == d.ProductId);
                    if (product == null)
                    {
                        await transaction.RollbackAsync();
                        return ServicesResult<bool>.Fail($"El producto con Id {d.ProductId} no existe.");
                    }

                    _db.PurchaseDetails.Add(new PurchaseDetail
                    {
                        PurchaseInvoiceId = invoice.Id,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        PurchasePrice = d.PurchasePrice,
                        TaxPercent = 0,
                        DiscountPercent = 0
                    });

                    product.Stock += d.Quantity; 
                }

                await SaveAuditAsync(
                    AuditAction.Create,
                    "PurchaseInvoice",
                    invoiceNumber,
                    $"Factura de compra registrada con {details.Count} producto(s) por un total de {total:N2}.",
                    _db);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServicesResult<bool>.Ok(true, "Factura guardada correctamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServicesResult<bool>.Fail("Error al guardar la factura: " + ex.Message);
            }
        }


        /// <summary>
        /// Obtiene el listado de facturas de compra, aplicando los filtros recibidos.
        /// </summary>
        public async Task<ServicesResult<List<PurchaseInvoiceListDto>>> GetInvoicesAsync(InvoiceFilterDto filter)
        {
            try
            {
                var query = _db.PurchaseInvoices
                    .Include(x => x.Supplier) // 👉 ajustar nombre de la propiedad de navegación si es distinto
                    .AsQueryable();

                if (filter.DateFrom.HasValue)
                    query = query.Where(x => x.CreatedAt >= filter.DateFrom.Value);

                if (filter.DateTo.HasValue)
                    query = query.Where(x => x.CreatedAt <= filter.DateTo.Value.AddDays(1).AddTicks(-1));

                if (filter.SupplierId.HasValue && filter.SupplierId.Value > 0)
                    query = query.Where(x => x.SupplierId == filter.SupplierId.Value);

                if (!string.IsNullOrWhiteSpace(filter.InvoiceNumber))
                    query = query.Where(x => x.InvoiceNumber.Contains(filter.InvoiceNumber));

                var result = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new PurchaseInvoiceListDto
                    {
                        Id = x.Id,
                        InvoiceNumber = x.InvoiceNumber,
                        CreatedAt = x.CreatedAt ?? DateTime.Today,
                        SupplierDisplay = x.InoviceNumberExist == true ? x.Supplier.Name : x.SupplierNameCasual,
                        IsFormal = x.InoviceNumberExist == true,
                        TotalAmount = x.TotalAmount ?? 0
                    })
                    .ToListAsync();

                return ServicesResult<List<PurchaseInvoiceListDto>>.Ok(result, "Listado obtenido correctamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<List<PurchaseInvoiceListDto>>.Fail("Error al obtener las facturas: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el detalle completo (cabecera + líneas de producto) de una factura puntual.
        /// </summary>
        public async Task<ServicesResult<PurchaseInvoiceDetailViewDto>> GetInvoiceDetailAsync(int invoiceId)
        {
            try
            {
                var invoice = await _db.PurchaseInvoices
                    .Include(x => x.Supplier)
                    .Include(x => x.PurchaseDetails) // 👉 ajustar nombre de la propiedad de navegación del detalle
                        .ThenInclude(d => d.Product) // 👉 para traer el nombre del producto
                    .FirstOrDefaultAsync(x => x.Id == invoiceId);

                if (invoice == null)
                    return ServicesResult<PurchaseInvoiceDetailViewDto>.Fail("La factura no existe.");

                var dto = new PurchaseInvoiceDetailViewDto
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    CreatedAt = invoice.CreatedAt ?? DateTime.Today,
                    SupplierDisplay = invoice.InoviceNumberExist == true ? invoice.Supplier.Name : invoice.SupplierNameCasual,
                    IsFormal = invoice.InoviceNumberExist == true,
                    SubTotal = invoice.SubTotal ?? 0,
                    Tax = invoice.Tax ?? 0,
                    TotalAmount = invoice.TotalAmount ?? 0,
                    Lines = invoice.PurchaseDetails.Select(d => new PurchaseDetailLineDto
                    {
                        ProductCode = d.Product.Code,
                        ProductName = d.Product.Name,
                        Quantity = d.Quantity ?? 0,
                        PurchasePrice = d.PurchasePrice ?? 0,
                    }).ToList()
                };

                return ServicesResult<PurchaseInvoiceDetailViewDto>.Ok(dto, "Detalle obtenido correctamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<PurchaseInvoiceDetailViewDto>.Fail("Error al obtener el detalle: " + ex.Message);
            }
        }
    }
}
