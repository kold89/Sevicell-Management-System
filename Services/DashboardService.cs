using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;
using WpfApp1.Models.Enums;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public class DashboardService : BaseService
    {
        public async Task<ServicesResult<DashboardDto>> ObtenerResumenAsync()
        {
            try
            {
                using var db = new SevicellDbContext();
                var dto = new DashboardDto();

                await CargarContratosAsync(db, dto);   // Paso 3
                // Paso 4: ventas
                // Paso 5: reparaciones
                // Paso 6: stock
                await CargarStockAsync(db, dto);        // Paso 6
                // Paso 7: listos para entrega

                return ServicesResult<DashboardDto>.Ok(dto, "Datos obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                return ServicesResult<DashboardDto>.Fail("Error al obtener el resumen: " + ex.Message);
            }
        }

        // ---------- Paso 3: contratos ----------
        private static async Task CargarContratosAsync(SevicellDbContext db, DashboardDto dto)
        {
            // Se guardan en variables locales para que EF pueda traducir la consulta a SQL
            int pendiente = (int)EContractStatus.Pending;
            int activo = (int)EContractStatus.Active;
            int vencido = (int)EContractStatus.Overdue;

            int cuotaPendiente = (int)InstallmentStatus.Pending;
            int cuotaVencida = (int)InstallmentStatus.Overdue;
            int cuotaParcial = (int)InstallmentStatus.Partial;

            var hoy = DateOnly.FromDateTime(DateTime.Today);

            dto.ContratosActivos = await db.Contracts
                .CountAsync(c => c.StatusId == pendiente
                              || c.StatusId == activo
                              || c.StatusId == vencido);

            dto.CuotasVencidas = await db.DebtInstallments
                .Where(x => x.DueDate < hoy)
                .Where(x => x.StatusId == cuotaPendiente
                         || x.StatusId == cuotaVencida
                         || x.StatusId == cuotaParcial)
                .Where(x => x.Contract.StatusId == pendiente
                         || x.Contract.StatusId == activo
                         || x.Contract.StatusId == vencido)
                .CountAsync();
        }
        // ---------- Paso 6: stock ----------
        private static async Task CargarStockAsync(SevicellDbContext db, DashboardDto dto)
        {
            dto.StockCritico = await db.Products
                .Where(p => p.Status == true)
                .Where(p => p.MinimumStock > 0 && (p.Stock ?? 0) <= p.MinimumStock)
                .CountAsync();
        }
    }
}