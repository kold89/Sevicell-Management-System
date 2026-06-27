using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para AddOrEditSupplier.xaml
    /// </summary>
    public partial class AddOrEditSupplier : Window
    {
        private readonly SupplierServices SupplierServices = new SupplierServices();
        private readonly Supplier? _supplier;

        public AddOrEditSupplier(Supplier supplier = null)
        {
            InitializeComponent();
            _supplier = supplier;

            if (_supplier != null)
            {
                FillEditModal();
            }
        }

        public void FillEditModal()
        {
            txtTitle.Text = "Editar Proveedor";

            txtName.Text = _supplier.Name;
            txtEmail.Text = _supplier?.Email;
            txtPhone.Text = _supplier?.Phone;
            txtCta.Text = "Pendiente crear en base de datos.";
            txtAddres.Text = _supplier.Address;
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Por favor, ingrese Nombre del proveedor.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Por favor, ingrese correo electronico.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            //valida contraseña
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Por favor, ingrese el teléfono.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            //valida Usuario
            if (string.IsNullOrWhiteSpace(txtAddres.Text))
            {
                MessageBox.Show("Por favor, ingrese una dirección.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnSave.IsEnabled = false;
                if (!ValidateFields()) return;

                if (_supplier == null)
                {
                    var supplier = new Supplier();
                    supplier.Name = txtName.Text;
                    supplier.Email = txtEmail.Text;
                    supplier.Phone = txtPhone.Text;
                    supplier.AccountNumber = txtCta.Text;
                    supplier.Address = txtAddres.Text;

                    var result = await SupplierServices.RegisterSupplierAsync(supplier);

                    if (result.Success)
                        MessageBox.Show("Proveedor registrado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                    {
                        MessageBox.Show("Error al registrar el Proveedor.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    _supplier.Name = txtName.Text;
                    _supplier.Email = txtEmail.Text;
                    _supplier.Phone = txtPhone.Text;
                    _supplier.Address = txtAddres.Text;
                    _supplier.AccountNumber = txtCta.Text;
                    var update = await SupplierServices.UpdateSupplierAsync(_supplier);

                    if (update.Success)
                        MessageBox.Show("Proveedor editado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar los datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnSave.IsEnabled = true;
            }

        }
    }
}
