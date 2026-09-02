using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Inventory
{

    /// <summary>
    /// Lógica de interacción para BrandAndCategory.xaml
    /// </summary>
    /// 
    public partial class BrandAndCategory : Page, INotifyPropertyChanged
    {
        private readonly BrandAndCategoryServices brandAndCategoryServices = new BrandAndCategoryServices();

        public event PropertyChangedEventHandler? PropertyChanged;

        private int _tipoDataSeleccionado;
        public int TipoDataSeleccionado
        {
            get => _tipoDataSeleccionado;
            set
            {
                if (_tipoDataSeleccionado != value)
                {
                    _tipoDataSeleccionado = value;

                    OnPropertyChanged();

                    LoadData(_tipoDataSeleccionado);
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BrandAndCategory()
        {
            InitializeComponent();
           
            this.DataContext = this;
            TipoDataSeleccionado = 1;
            
        }

        private void LoadData(int typeData)
        {
            var brands = new ServicesResult<List<Brand>>();
            var categories = new ServicesResult<List<Category>>();
            switch (typeData) {
                case 1:
                    categories = brandAndCategoryServices.ListCategories();
                    if (!categories.Success)
                    {
                        dgCategories.ItemsSource = null;
                        MessageBox.Show(brands.Message);
                    }
                    else
                    {
                        dgCategories.ItemsSource = categories.Data.Select(x => new ViewBrandOrCategoryDTO
                        {
                            id = x.Id,
                            name = x.Name,
                            status = x.Status == true ? "Activo" : "Deshabilitado"
                        }).ToList();
                    }
                    break;
                case 2:
                    brands = brandAndCategoryServices.ListBrands();
                    if (!brands.Success)
                    {
                        dgBrands.ItemsSource = null;
                        MessageBox.Show(brands.Message);

                    }
                    else
                    {
                        dgBrands.ItemsSource = brands.Data.Select(x => new ViewBrandOrCategoryDTO
                        {
                            id = x.Id,
                            name = x.Name,
                            status = x.Status  ? "Activo" : "Deshabilitado"
                        }).ToList();
                    }
                    break;
             default:
                    break;
                 
            }
        }

        private async void btnSaveCategory_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = txtCategoryName.Text.Trim();
            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Debe ingresar el nombre de la categoría.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCategoryName.Focus();
                return;
            }
            Category newCategory = new Category();
            newCategory.Name = categoryName;
            newCategory.Status = true;

            var result = await brandAndCategoryServices.AddCategory(newCategory);

            if (result.Success)
                ToastService.ShowSuccess(result.Message);

            txtCategoryName.Text = string.Empty;
            txtCategoryName.Focus();
            LoadData(TipoDataSeleccionado);

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

        private void btnClearBrand_Click(object sender, RoutedEventArgs e)
        {
            txtBrandName.Text = string.Empty;
            txtBrandName.Focus();
        }

        private async void btnSaveBrand_Click(object sender, RoutedEventArgs e)
        {
            string brandName = txtBrandName.Text.Trim();
            if (string.IsNullOrEmpty(brandName))
            {
                MessageBox.Show("Debe ingresar el nombre de la marca.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
                txtBrandName.Focus();
                return;
            }

            Brand newBrand = new Brand();
            newBrand.Name = brandName;
            newBrand.Status = true;

            var result = await brandAndCategoryServices.AddBrand(newBrand);

            if (result.Success)
                ToastService.ShowSuccess(result.Message);
            txtBrandName.Text = string.Empty;
            txtBrandName.Focus();
            LoadData(TipoDataSeleccionado);
        }

        private void BtnEditBrand_Click(object sender, RoutedEventArgs e)
        {
            var selectedBrand = (ViewBrandOrCategoryDTO)dgBrands.SelectedItem;

            if (selectedBrand != null)
            {
                var brand = brandAndCategoryServices.GetBrandForEdit(selectedBrand.id);
                var win = new DialogBrandsCategory(brand.Data);
                win.Owner = Window.GetWindow(this);

                if (win.ShowDialog() == true)
                {
                    LoadData(TipoDataSeleccionado);
                }
            }

        }

        private async void BtnDisableBrand_Click(object sender, RoutedEventArgs e)
        {
            var selectedBrand = (ViewBrandOrCategoryDTO)dgBrands.SelectedItem;

            if (selectedBrand != null)
            {
                bool newStatus = selectedBrand.status == "Activo" ? false : true;
                string actionStatus = selectedBrand.status == "Activo" ? "dar de baja" : "habilitar";

                var msjDisable = MessageBox.Show($"¿Está seguro que desea {actionStatus} a la marca {selectedBrand.name}?",
                            "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (msjDisable == MessageBoxResult.Yes)
                {
                    var result = await brandAndCategoryServices.ChangeStatusBrandAsync(selectedBrand.id, newStatus);

                    if (result.Success)
                    {
                        LoadData(_tipoDataSeleccionado);
                        MessageBox.Show(result.Message);
                    }
                }
            }
        }

        private async void BtnDisableCategory_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = (ViewBrandOrCategoryDTO)dgCategories.SelectedItem;

            if (selectedCategory != null)
            {
                bool newStatus = selectedCategory.status == "Activo" ? false : true;
                string actionStatus = selectedCategory.status == "Activo" ? "dar de baja" : "habilitar";

                var msjDisable = MessageBox.Show($"¿Está seguro que desea {actionStatus} a la categoría {selectedCategory.name}?",
                            "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (msjDisable == MessageBoxResult.Yes)
                {
                    var result = await brandAndCategoryServices.ChangeStatusCategoryAsync(selectedCategory.id, newStatus);

                    if (result.Success)
                    {
                        LoadData(_tipoDataSeleccionado);
                        ToastService.ShowSuccess(result.Message);
                    }
                }
            }
        }

        private void BtnEditCategory_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategory = (ViewBrandOrCategoryDTO)dgCategories.SelectedItem;

            if (selectedCategory != null)
            {
                var category = brandAndCategoryServices.GetCategoryForEdit(selectedCategory.id);
                var win = new DialogBrandsCategory(category.Data);
                win.Owner = Window.GetWindow(this);

                if (win.ShowDialog() == true)
                {
                    LoadData(TipoDataSeleccionado);
                }
            }
        }
    }
}
