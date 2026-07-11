using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Markup;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.ViewModels;
using WpfApp1.Views.Inventory;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    (p, b) => new
                    {
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
                    ).OrderByDescending(x => x.id).ToListAsync();

                if (data.Count == 0)
                {
                    data = new List<ProductsDto>();
                    return ServicesResult<List<ProductsDto>>.Ok(data, "No se encontraron datos.");
                }

                return ServicesResult<List<ProductsDto>>.Ok(data, "productos obtenidos exitosamente.");
            }
        }

        public async Task<ServicesResult<List<ProductsDto>>> ListAllProducts()
        {
            try 
            {
                using (var db = new SevicellDbContext())
                {
                    var products = await db.Products.Select( x => new ProductsDto
                    {
                        id = x.Id,
                        code = x.Code,
                        name = x.Name,
                        salesPrice =  Convert.ToDouble(x.SalePrice),
                        stock = x.Stock ?? 0
                    }).ToListAsync();

                    return ServicesResult<List<ProductsDto>>.Ok(products, "Productos obtenidos exitosamente.");
                }
            }
            catch (Exception ex) 
            {
                return ServicesResult<List<ProductsDto>>.Fail("Error al obtener el listado de productos.");
            }
        }
        public Product GetProductById(int id)
        {
            using (var db = new DBSevicellContext())
            {
                return db.Products.Where(x => x.Id == id).FirstOrDefault();
            }
        }

        public ServicesResult<ProductsDto> SearchProductById(int idProduct)
        {
            using (var db = new DBSevicellContext())
            {
                var data = db.Products
                    .Where(x => x.Id == idProduct)
                    .Join(db.Brands,
                    p => p.BrandId, b => b.Id,
                    (p, b) => new
                    {
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

        public ServicesResult<(List<Brand>, List<Category>)> listCategoryAndBrand()
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    List<Brand> brands = new List<Brand>();
                    List<Category> categories = new List<Category>();

                    brands = db.Brands.Where(x => x.Status == true).ToList();
                    categories = db.Categories.Where(x => x.Status == true).ToList();

                    var data = (brands, categories);
                    return ServicesResult<(List<Brand>, List<Category>)>.Ok(data, "Categorias y marcas obtenidas exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<(List<Brand>, List<Category>)>.Fail("Error al recuperar la información.");

            }
        }

        public async Task<ServicesResult<bool>> RegisterProductAsync(Product product)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    db.Products.Add(product);
                    await SaveAuditAsync(AuditAction.Create, "Products", product.Id.ToString(), "Se creo un nuevo producto", db);
                    await db.SaveChangesAsync();

                    return ServicesResult<bool>.Ok(true, "Producto creado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Fail("Error inesperado al registrar el producto. " + ex.Message);
            }
        }

        public async Task<ServicesResult<bool>> UpdateProductAsync(Product product)
        {

            using (var db = new DBSevicellContext())
            {
                db.Products.Update(product);
                await SaveAuditAsync(AuditAction.Update, "Products", product.Id.ToString(), "Se actualizo el producto " , db);
                await db.SaveChangesAsync();

                return ServicesResult<bool>.Ok(true, "Producto actualizado correctamente.");
            }

        }
    }
}
