using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Data;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    internal class ProductsServices : BaseService
    {
        public async Task<ServicesResult<List<ProductsDto>>> listProducForGrid()
        {
            using (var db = new DBSevicellContext())
            {
                var data = await db.Products
                    .Join(db.Brands, 
                    p => p.BrandId, b => b.Id,
                    (p, b) => new {
                       product = p, 
                       brand = b
                    }
                    ).Join(db.Categories,
                    pb => pb.product.CategoryId, c => c.Id,
                    (pb, c) => new ProductsDto
                    {
                        id = pb.product.Id,
                        code = pb.product.Code,
                        name = pb.product.Name,
                        salesPrice = (double)pb.product.SalePrice,
                        stock = pb.product.Stock ?? 0,
                        minimunStock = pb.product.MinimumStock ?? 0,
                        Brand = pb.brand.Name,
                        category = c.Name,
                        description = pb.product.ProductDescription ?? "",
                        status = pb.product.Status == true ? "Activo" : "Inactivo"
                    }
                    ).ToListAsync();

                if (data.Count == 0)
                {
                    data = new List<ProductsDto>();
                    return ServicesResult<List<ProductsDto>>.Ok(data, "No se encontraron datos.");
                }

                return ServicesResult<List<ProductsDto>>.Ok(data, "productos obtenidos exitosamente.");
            }
        }

        public ServicesResult<ProductsDto> SearchProductById(int idProduct)
        {
            using (var db = new DBSevicellContext())
            {
                var data =  db.Products
                    .Where(x => x.Id == idProduct)
                    .Join(db.Brands,
                    p => p.BrandId, b => b.Id,
                    (p, b) => new {
                        product = p,
                        brand = b
                    }
                    ).Join(db.Categories,
                    pb => pb.product.CategoryId, c => c.Id,
                    (pb, c) => new ProductsDto
                    {
                        id = pb.product.Id,
                        code = pb.product.Code,
                        name = pb.product.Name,
                        salesPrice = (double)pb.product.SalePrice,
                        stock = pb.product.Stock ?? 0,
                        minimunStock = pb.product.MinimumStock ?? 0,
                        Brand = pb.brand.Name,
                        category = c.Name,
                        description = pb.product.ProductDescription ?? "",
                        status = pb.product.Status == true ? "Activo" : "Inactivo"
                    }).FirstOrDefault();

                if (data == null)
                {
                    data = new ProductsDto();
                    return ServicesResult<ProductsDto>.Ok(data, "No se encontraron datos.");
                }

                return ServicesResult<ProductsDto>.Ok(data, "producto obtenido exitosamente.");
            }
        }

    }
}
