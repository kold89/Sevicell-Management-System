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

namespace WpfApp1.Views.Credit
{
    /// <summary>
    /// Lógica de interacción para AddOrEditCustumer.xaml
    /// </summary>
    public partial class AddOrEditCustumer : Window
    {
        private readonly CustomerServices customerServices = new CustomerServices();
        public AddOrEditCustumer()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Por favor, ingrese Nombre del Cliente.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFirstName.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Por favor, ingrese apellidos.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
           
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Por favor, ingrese el teléfono.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDNI.Text))
            {
                MessageBox.Show("Por favor, ingrese el número de identidad.",
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

        private  async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnSave.IsEnabled = false;
                if (!ValidateFields()) return;

                    var customer = new Customer();
                    customer.Name = txtFirstName.Text;
                    customer.LastName = txtLastName.Text;
                    customer.Email = txtEmail.Text;
                    customer.Phone = txtPhone.Text;
                    customer.Dni = txtDNI.Text;
                    customer.Address = txtAddres.Text;

                   var result = await customerServices.RegisterCustomerAsync(customer);

                if (result.Success)
                    MessageBox.Show("cliente registrado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    MessageBox.Show("Error al registrar el cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
