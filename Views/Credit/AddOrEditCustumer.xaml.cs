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
using System.Xml.Linq;
using WpfApp1.Models;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Credit
{
    /// <summary>
    /// Lógica de interacción para AddOrEditCustumer.xaml
    /// </summary>
    public partial class AddOrEditCustumer : Window
    {
        private readonly CustomerServices customerServices = new CustomerServices();
        private readonly int? _customerId;
        private bool IsEditMode => _customerId.HasValue;

        public AddOrEditCustumer(custumerDTO? customer = null)
        {
            InitializeComponent();

            if (customer != null)
            {
                _customerId = customer.Id;
                Title = "Editar cliente";
                txtTitle.Text = "Editar Cliente";
                Loaded += async (_, _) => await LoadCustomerAsync();
            }
            else
            {
                Title = "Nuevo cliente";
            }
        }

        private async Task LoadCustomerAsync()
        {
            try
            {
                var result = await customerServices.GetCustomerByIdAsync(_customerId!.Value);
                if (!result.Success || result.Data == null)
                {
                    MessageBox.Show("No se pudo cargar el cliente.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    DialogResult = false;
                    return;
                }

                var c = result.Data;
                txtFirstName.Text = c.Name;
                txtLastName.Text = c.LastName;
                txtEmail.Text = c.Email;
                txtPhone.Text = c.Phone;
                txtDNI.Text = c.Dni;
                txtAddres.Text = c.Address;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el cliente: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private bool ValidateFields()
        {
            return Require(txtFirstName, "el nombre del cliente")
                && Require(txtLastName, "los apellidos")
                && Require(txtPhone, "el teléfono")
                && Require(txtDNI, "el número de identidad")
                && Require(txtAddres, "una dirección");
        }

        private static bool Require(TextBox box, string what)
        {
            if (!string.IsNullOrWhiteSpace(box.Text)) return true;

            MessageBox.Show($"Por favor, ingrese {what}.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            box.Focus();
            return false;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            BtnSave.IsEnabled = false;
            try
            {
                var customer = new Customer
                {
                    Id = _customerId ?? 0,
                    Name = txtFirstName.Text.Trim().ToUpper(),
                    LastName = txtLastName.Text.Trim().ToUpper(),
                    Email = txtEmail.Text?.Trim() ?? "",
                    Phone = txtPhone.Text.Trim(),
                    Dni = txtDNI.Text.Trim(),
                    Address = txtAddres.Text.Trim()
                };

                var result = IsEditMode
                    ? await customerServices.UpdateCustomerAsync(customer)
                    : await customerServices.RegisterCustomerAsync(customer);

                if (!result.Success)
                {
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(result.Message)
                            ? "No se pudo guardar el cliente."
                            : result.Message,
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // el modal queda abierto con los datos
                }

                MessageBox.Show(
                    IsEditMode ? "Cliente actualizado con éxito." : "Cliente registrado con éxito.",
                    "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true; // esto ya cierra la ventana, no hace falta Close()
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar los datos: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnSave.IsEnabled = true;
            }
        }
    }
}
