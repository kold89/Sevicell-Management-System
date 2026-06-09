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
using System.Windows.Shapes;
using WpfApp1.Models;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Inventory
{
   
    /// <summary>
    /// Lógica de interacción para DialogBrandsCategory.xaml
    /// </summary>
    public partial class DialogBrandsCategory : Window
    {
        private readonly Brand dtoBrand = new Brand();
        private readonly Category dtoCategory = new Category();
        private readonly BrandAndCategoryServices brandAndCategoryServices = new BrandAndCategoryServices();
        private readonly int typeData;
        public DialogBrandsCategory(Category category)
        {
            InitializeComponent();
            LoadStatus();

            dtoCategory = category;
            typeData = 1;
            txtNombre.Text = category.Name;
            cbStatus.SelectedIndex = category.Status == true ? 1 : 2;
        }

        public DialogBrandsCategory(Brand brand)
        {
            InitializeComponent();
            LoadStatus();

            dtoBrand = brand;
            typeData = 2;
            txtNombre.Text = brand.Name;
            cbStatus.SelectedIndex = brand.Status == true ? 1 : 2;
        }

        private void LoadStatus()
        {
            try
            {
                cbStatus.Items.Clear();
                cbStatus.Items.Add(new { Id = 0, Name = "--Selecciones un Rol--" });
                cbStatus.Items.Add(new { Id = 1, Name = "--Activo--" });
                cbStatus.Items.Add(new { Id = 2, Name = "--Deshabilitado--" });

                cbStatus.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado " + ex.Message);
            }

        }

        public bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNombre.Focus();
                return false;
            }
            if (cbStatus.SelectedValue == null || (int)cbStatus.SelectedValue == 0)
            {
                MessageBox.Show("Por favor, seleccione un estado válido.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;


            if (typeData == 1)
            {
                dtoCategory.Name = txtNombre.Text;
                dtoCategory.Status = (int)cbStatus.SelectedValue == 1 ? true : false;


                var result = await brandAndCategoryServices.updateCategoryAsync(dtoCategory);

                if (result.Success)
                    MessageBox.Show("Marca Editada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {

                dtoBrand.Name = txtNombre.Text;
                dtoBrand.Status = (int)cbStatus.SelectedValue == 1 ? true : false;

                var result = await brandAndCategoryServices.updateBrandAsync(dtoBrand);

                if (result.Success)
                    MessageBox.Show("Marca Editada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            this.DialogResult = true;
            this.Close();

        }
    }
}
