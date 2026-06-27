using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Security;
using WpfApp1.ViewModels;
using WpfApp1.Views.Inventory;

namespace WpfApp1.Services
{
    public class movementInventoryService
    {
        /// <summary>
        /// Consulta el historial utilizando LINQ para mayor legibilidad y mantenibilidad.
        /// </summary>
        public async Task<ServicesResult<List<InventoryAuditDto>>> GetMovementHistory()
        {
            var result = new ServicesResult<List<InventoryAuditDto>>();

            try
            {
                using (var db = new DBSevicellContext())
                {
                    // LINQ Query: Proyectamos directamente la unión al DTO que tu Grid necesita
                    var historia = await (from m in db.InventoryMovements
                                          join p in db.Products on m.ProductId equals p.Id
                                          orderby m.MovementDate descending // Los más recientes primero
                                          select new InventoryAuditDto
                                          {
                                              Folio = m.Id,
                                              ProductoNombre = p.Name,
                                              Motivo = m.Description,
                                              Fecha = m.MovementDate,
                                              UsuarioResponsable = m.UserId.ToString(),

                                              // Evaluamos el tipo de movimiento directamente en la consulta
                                              EsIncremento = m.MovementType.ToUpper() == "ENTRADA",

                                              // Valores base temporales para Stock
                                              StockAnterior = 0,
                                              StockNuevo = 0,

                                              // Guardamos la cantidad pura para formatearla en memoria abajo
                                              CantidadConSigno = m.Quantity.ToString(),
                                          }).ToListAsync();

                    // Post-procesamiento rápido en memoria para añadir el signo (+ / -) según tu diseño UX
                    foreach (var item in historia)
                    {
                        item.CantidadConSigno = item.EsIncremento ? $"+{item.CantidadConSigno}" : $"-{item.CantidadConSigno}";
                    }
                    return ServicesResult<List<InventoryAuditDto>>.Ok(historia, "Historial cargado correctamente");
                    

                }
            }
            catch (Exception ex)
            {
                var message = $"Error al procesar: {ex.Message}";

                return ServicesResult<List<InventoryAuditDto>>.Fail("Historial cargado correctamente" + message);

            }

        }

        public async Task<ServicesResult<bool>> SaveMovement(InventoryMovementDto dto)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    // 1. Buscar el producto para actualizar su stock
                    var product = await db.Products.FirstOrDefaultAsync( x => x.Id == dto.ProductId);
                    if (product == null)
                        return ServicesResult<bool>.Fail("Producto no encontrado.");

                    // 2. Calcular el stock nuevo según el tipo de movimiento
                    bool esEntrada = dto.EsIncremento;
                    int stockAnterior = product.Stock ?? 0;

                    int stockNuevo = esEntrada
                        ? stockAnterior + dto.Cantidad
                        : stockAnterior - dto.Cantidad;


                    // 3. Validar que no quede negativo antes de tocar la BD
                    if (stockNuevo < 0)
                        return ServicesResult<bool>.Fail("El ajuste dejaría el stock en negativo.");

                    // 4. Actualizar el stock del producto
                    product.Stock = stockNuevo;
                    db.Products.Update(product);

                    // 5. Registrar el movimiento con todos los datos de auditoría

                    var movimiento = new Models.InventoryMovement
                    {
                        //ProductId = dto.ProductId,
                        MovementType = esEntrada ? "ENTRADA" : "SALIDA",
                        Quantity = dto.Cantidad,
                        Description = dto.Motivo,
                        MovementDate = DateTime.Now,
                        PreviousStock = stockAnterior,
                        NewStock = stockNuevo,
                        UserId = SessionManager.loggedInUser.RoleId
                    };
                    db.InventoryMovements.Add(movimiento);

                    //// 6. Un solo SaveChanges = una sola transacción atómica
                    //// Si falla cualquier cosa, EF revierte ambas operaciones
                    //await db.SaveChangesAsync();

                    return ServicesResult<bool>.Ok(true, "Movimiento guardado correctamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Fail("Error al guardar el movimiento. " + ex.Message);
            }
        }
    }
}
