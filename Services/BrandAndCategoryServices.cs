using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.ViewModels;

namespace WpfApp1.Services
{
    public class BrandAndCategoryServices : BaseService
    {
        public ServicesResult<List<Brand>> ListBrands()
        {
            try
            {
                using (var context = new DBSevicellContext())
                {
                    var listBrand = context.Brands.ToList();
                    return ServicesResult<List<Brand>>.Ok(listBrand, "Lista obtenida con exito");
                }

            }
            catch (Exception ex) {
                return ServicesResult<List<Brand>>.Fail("Error al obtener las marcas. " + ex.Message);
            }
        }

        public ServicesResult<List<Category>> ListCategories()
        {
            try
            {
                using (var context = new DBSevicellContext())
                {
                    var list = context.Categories.ToList();
                    return ServicesResult<List<Category>>.Ok(list, "Lista obtenida con exito");
                }

            }
            catch (Exception ex)
            {
                return ServicesResult<List<Category>>.Fail("Error al obtener las categorias. " + ex.Message);
            }
        }


        public async Task<ServicesResult<Category>> AddCategory(Category newCategory)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {

                    db.Categories.Add(newCategory);
                    await SaveAuditAsync(AuditAction.Create, "Category", newCategory.Id.ToString(), "Se creo un nueva Categoría", db);
                    await db.SaveChangesAsync();

                    return ServicesResult<Category>.Ok(newCategory, "Categoría creada exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<Category>.Fail("Error al guardar la Categoría." + ex.Message);
            }
        }

        public async Task<ServicesResult<Brand>> AddBrand(Brand newBrand)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {

                    db.Brands.Add(newBrand);
                    await SaveAuditAsync(AuditAction.Create, "Brand", newBrand.Id.ToString(), "Se creo un nueva marca", db);
                    await db.SaveChangesAsync();

                    return ServicesResult<Brand>.Ok(newBrand, "Categoría creada exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<Brand>.Fail("Error al guardar la Marca." + ex.Message);
            }
        }

        public async Task<ServicesResult<bool>> ChangeStatusBrandAsync(int id, bool newStatus)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    string status = newStatus ? "habilitado" : "deshabilitado";

                    var brand = await db.Brands.FirstOrDefaultAsync(x => x.Id == id);
                    if (brand == null)
                    {
                        return ServicesResult<bool>.Fail($"Error el usuario no pudo ser {status}");
                    }

                    brand.Status = newStatus;
                    await db.SaveChangesAsync();

                    return ServicesResult<bool>.Ok(true, $"Usuario {brand.Name} {status} con exito."); ;
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Fail("Sucedio un error inesperado.");
            }
        }

        public async Task<ServicesResult<bool>> ChangeStatusCategoryAsync(int id, bool newStatus)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    string status = newStatus ? "habilitado" : "deshabilitado";

                    var category = await db.Categories.FirstOrDefaultAsync(x => x.Id == id);
                    if (category == null)
                    {
                        return ServicesResult<bool>.Fail($"Error no se pudo cambiar el estado.");
                    }

                    category.Status = newStatus;
                    await db.SaveChangesAsync();

                    return ServicesResult<bool>.Ok(true, $"Categoria {category.Name} guardada con exito."); ;
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<bool>.Fail("Sucedio un error inesperado.");
            }
        }

        public async Task<ServicesResult<Brand>> UpdateBrand(Brand brand)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    db.Brands.Update(brand);
                    await SaveAuditAsync(AuditAction.Update, "Marcas", brand.Id.ToString(), "Se actualizo el usuario " + brand.Name, db);
                    await db.SaveChangesAsync();

                    return ServicesResult<Brand>.Ok(brand, "Marca creado exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<Brand>.Fail("Error inesperado al registrar al Marca. " + ex.Message);
            }
        }

        public ServicesResult<Category> GetCategoryForEdit(int id)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                            
                    var itemCategory = db.Categories.Where(x => x.Id == id).FirstOrDefault();
                    
                    return ServicesResult<Category>.Ok(itemCategory, "Datos obtenidos de forma exitosa.");

                }
            }
            catch (Exception ex)
            {
                return ServicesResult<Category>.Fail("Error al procesar los datos.");

            }
        }
        public ServicesResult<Brand> GetBrandForEdit(int id)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {

                    var itemBrand = db.Brands.Where(x => x.Id == id).FirstOrDefault();

                    return ServicesResult<Brand>.Ok(itemBrand, "Datos obtenidos de forma exitosa.");

                }
            }
            catch (Exception ex)
            {
                return ServicesResult<Brand>.Fail("Error al procesar los datos.");

            }
        }
        public async Task<ServicesResult<Brand>> updateBrandAsync(Brand brand)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {

                    db.Brands.Update(brand);
                    await SaveAuditAsync(AuditAction.Update, "Brand", brand.Id.ToString(), "Se actualizaron los datos del registro.", db);
                    await db.SaveChangesAsync();

                    return ServicesResult<Brand>.Ok(brand, "Marca editada exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<Brand>.Fail("Error inesperado al editar la marca. " + ex.Message);
            }
        }

        public async Task<ServicesResult<Category>> updateCategoryAsync(Category category)
        {
            try
            {
                using (var db = new DBSevicellContext())
                {

                    db.Categories.Update(category);
                    await SaveAuditAsync(AuditAction.Update, "Category", category.Id.ToString(), "Se actualizaron los datos del registro.", db);
                    await db.SaveChangesAsync();

                    return ServicesResult<Category>.Ok(category, "Marca editada exitosamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<Category>.Fail("Error inesperado al editar la marca. " + ex.Message);
            }
        }
    }
}
