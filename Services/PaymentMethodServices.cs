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
    class PaymentMethodServices
    {
        public async Task<ServicesResult<List<PaymentMethod>>> ListAllPaymentMethods()
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var data = await db.PaymentMethods.ToListAsync();

                    return ServicesResult<List<PaymentMethod>>.Ok(data, "Datos obtenidos exitosamente.");

                }
            }
            catch (Exception ex)
            {
                return ServicesResult<List<PaymentMethod>>.Fail("Error al obtener los datos.");

            }
        }
    }
}
