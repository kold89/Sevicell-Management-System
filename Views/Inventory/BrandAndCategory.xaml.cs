using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Views.Inventory
{

    /// <summary>
    /// Lógica de interacción para BrandAndCategory.xaml
    /// </summary>
    /// 
    public partial class BrandAndCategory : Page
    {
        private readonly BrandAndCategoryServices brandAndCategoryServices = new BrandAndCategoryServices();

        public BrandAndCategory()
        {
            InitializeComponent();
            LoadData(2);
        }

        private void LoadData(int typeData)
        {
            var brands = new ServicesResult<List<Brand>>();
            var categories = new ServicesResult<List<Category>>();
            switch (typeData) { 
             case 1:
                    brands = brandAndCategoryServices.ListBrands();
                    if (!brands.Success)
                    {
                        dgBrands.ItemsSource = null;
                        MessageBox.Show(brands.Message);

                    }
                    else
                    {
                        dgBrands.ItemsSource = brands.Data.Select(x => new
                        {
                            id = x.Id,
                            name = x.Name,
                            status = x.Status  ? "Activo" : "Inactivo"
                        }).ToList();
                    }
                    break;
                case 2:
                    categories = brandAndCategoryServices.ListCategories();
                    if (!categories.Success)
                    {
                        dgCategories.ItemsSource = null;
                        MessageBox.Show(brands.Message);

                    }
                    else
                    {
                        dgCategories.ItemsSource = categories.Data.Select(x => new
                        {
                            id = x.Id,
                            name = x.Name,
                            status = x.Status == true ? "Activo" : "Inactivo"
                        }).ToList();
                    }
                    break;
             default:
                    break;
                 
            }
        }

        private void btnSaveCategory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void btnClearCategory_Click(object sender, RoutedEventArgs e)
        {
            txtCategoryName.Text = string.Empty;
            txtCategoryName.Focus();    
        }
    }
}
