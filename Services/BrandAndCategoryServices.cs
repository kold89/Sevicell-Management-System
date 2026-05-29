using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class BrandAndCategoryServices
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
    }
}
