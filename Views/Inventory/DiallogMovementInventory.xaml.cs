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
    /// Lógica de interacción para DiallogMovementInventory.xaml
    /// </summary>
    public partial class DiallogMovementInventory : Window
    {
        private readonly ProductsDto _productDto;
        private readonly movementInventoryService serviceMovement = new movementInventoryService();

        public DiallogMovementInventory(ProductsDto product)
        {
            InitializeComponent();
            _productDto = product;
            FillData();
            
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FillData()
        {
            txtProduct.Text = _productDto.name;
            LoadCbType();
        }
        private void LoadCbType()
        {
            try
            {
                var movement = new List<movement>();
                movement.Add(new movement { Id = 0, Name = "--Seleccione el movimiento.--" });
                movement.Add(new movement { Id = 1, Name = "Incrementar (+)." });
                movement.Add(new movement { Id = 2, Name = "Disminuir (-)." });

                cbType.ItemsSource = movement;
                cbType.DisplayMemberPath = "Name";
                cbType.SelectedValuePath = "Id";
                cbType.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado " + ex.Message);
            }
        }

        public bool ValidateFields()
        {
            // Validar Nombre
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("La justificación es obligatorio.", "Validación",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDescription.Focus();
                return false;
            }
            //Valida apellidos
            if (string.IsNullOrWhiteSpace(txtCant.Text))
            {
                MessageBox.Show("Por favor, ingrese la cantidad.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCant.Focus();
                return false;
            }
            if (cbType.SelectedValue == null || (int)cbType.SelectedValue == 0)
            {
                MessageBox.Show("Por favor, seleccione un tipo de ajuste válida.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            //valida Usuario
            return true;
        }

        public class movement
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            var movement = new InventoryMovementDto();
            movement.ProductId = _productDto.id;
            movement.Cantidad = Convert.ToInt32(txtCant.Text);
            movement.Motivo = txtDescription.Text;
            movement.EsIncremento = (int)cbType.SelectedValue == 1 ? true : false;


            var result = await serviceMovement.SaveMovement(movement);
            if (result.Success)
            {
                MessageBox.Show("Movimiento registrado exitosamente.",
                            "Información", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
