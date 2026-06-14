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
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para InventoryMovement.xaml
    /// </summary>
    public partial class InventoryMovement : Page, INotifyPropertyChanged
    {
        private readonly ProductsServices productService = new ProductsServices();   
        public event PropertyChangedEventHandler? PropertyChanged;

        private int _tipoDataSeleccionado;
        public int TabItemSelected
        {
            get => _tipoDataSeleccionado;
            set
            {
                if (_tipoDataSeleccionado != value)
                {
                    _tipoDataSeleccionado = value;

                    // Notifica al XAML que la propiedad cambió
                    OnPropertyChanged();

                    // ¡AQUÍ SE DISPARA TU MÉTODO AUTOMÁTICAMENTE!
                    LoadData(_tipoDataSeleccionado);
                }
            }
        }

        public InventoryMovement()
        {
            InitializeComponent();
            this.DataContext = this;
            TabItemSelected = 1;
        }
        public async Task LoadData(int type)
        {
            var data = productService.listProducForGrid();
            ServicesResult<List<ProductsDto>> productDto = await productService.listProducForGrid();
            if (productDto.Success)
            {
                dgRepuestos.ItemsSource = productDto.Data;
            }
            else
            {
                dgRepuestos.ItemsSource = null;
                MessageBox.Show(productDto.Message);
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void BtnReajustarFila_Click(object sender, RoutedEventArgs e)
        {
            DiallogMovementInventory modalReajustar = new DiallogMovementInventory();
            modalReajustar.Owner = Window.GetWindow(this);
            bool? resutl = modalReajustar.ShowDialog();
            if (resutl == true)
            {
                LoadData(_tipoDataSeleccionado);
            }
        }
    }
}
