using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class RepairServices
    {
       
            public async Task<List<RepairOrder>> GetReparacionesFiltradasAsync(string filtro)
            {
                using (var db = new DBSevicellContext()) 
                {
                    var query = db.RepairOrders
                        .Include(o => o.Device.Customer) 
                        .Include(o => o.Status)
                        .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(filtro))
                    {
                        filtro = filtro.ToLower();
                        query = query.Where(o =>
                            o.Id.ToString().Contains(filtro) ||
                            o.Device.Customer.Name.ToLower().Contains(filtro) ||
                            o.Device.Imei.Contains(filtro) ||
                            o.Device.Model.ToLower().Contains(filtro));
                    }

                    return await query.OrderByDescending(o => o.Id).ToListAsync();
                }
            }
        
    }
}
