using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;
using WpfApp1.ViewModels;
using WpfApp1.Views.Inventory;

namespace WpfApp1.Services
{
    public class productUnitServices : BaseService
    {
        public ServicesResult<ProductUnit> SearchProductUnitById(int id)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var product = db.ProductUnits.Where(x => x.Id == id).FirstOrDefault();
                    return ServicesResult<ProductUnit>.Ok(product, "Unidad obtenida con exito.");
                       
                }            
            } catch (Exception ex) 
            {
                return ServicesResult<ProductUnit>.Fail("Error al obtener el product." + ex.Message);
            }       
        }

        public async Task<ServicesResult<List<Product>>> GetProductsSerialized()
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var productos = await db.Products
                         .Where(p => p.IsSerialized)
                         .OrderBy(p => p.Name)
                         .ToListAsync();
                    return ServicesResult<List<Product>>.Ok(productos, "Productos obtenidos exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<List<Product>>.Fail("Error al obtener los datos." + ex.Message);
            }
        }

        public async Task<ServicesResult<bool>> UpdateProductUnitAsync(ProductUnit product)
        {

            using (var db = new SevicellDbContext())
            {
                db.ProductUnits.Update(product);
                await SaveAuditAsync(AuditAction.Update, "ProductsUnit", product.Id.ToString(), "Se actualizo el producto ", db);
                await db.SaveChangesAsync();

                return ServicesResult<bool>.Ok(true, "Producto actualizado correctamente.");
            }
        }
    }
}
