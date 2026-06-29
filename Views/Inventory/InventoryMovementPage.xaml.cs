using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Services;
using WpfApp1.ViewModels; // Asegúrate de que aquí estén tus DTOs

namespace WpfApp1.Views.Inventory
{
    public partial class InventoryMovement : Page, INotifyPropertyChanged
    {
        private readonly ProductsServices productService = new ProductsServices();
        private readonly movementInventoryService inventoryService = new movementInventoryService();

        public event PropertyChangedEventHandler? PropertyChanged;

        private List<ProductsDto> _allProducts = new List<ProductsDto>();
        private List<InventoryAuditsDto> _allAuditLogs = new List<InventoryAuditsDto>();

        private int _tipoDataSeleccionado;
        public int TabItemSelected
        {
            get => _tipoDataSeleccionado;
            set
            {
                if (_tipoDataSeleccionado != value)
                {
                    _tipoDataSeleccionado = value;
                    OnPropertyChanged();

                    _ = LoadData(_tipoDataSeleccionado);
                }
            }
        }

        private string _productIdInput = string.Empty;
        public string ProductIdInput
        {
            get => _productIdInput;
            set
            {
                if (_productIdInput != value)
                {
                    _productIdInput = value;
                    OnPropertyChanged();

                    FiltrarDatosSegunPestaña();
                }
            }
        }

        public InventoryMovement()
        {
            InitializeComponent();
            this.DataContext = this;

            TabItemSelected = 1;
        }

        /// <summary>
        /// Centraliza la carga de datos asíncrona dependiendo de la pestaña seleccionada
        /// </summary>
        public async Task LoadData(int type)
        {
            try
            {
                if (type == 1) // Pestaña: Inventario de Repuestos
                {
                    ServicesResult<List<ProductsDto>> productResult = await productService.listProducForGrid();
                    if (productResult.Success)
                    {
                        _allProducts = productResult.Data ?? new List<ProductsDto>();
                        FiltrarDatosSegunPestaña();
                    }
                    else
                    {
                        _allProducts.Clear();
                        dgRepuestos.ItemsSource = null;
                        MessageBox.Show(productResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (type == 2) // Pestaña: Historial de Auditoría
                {
                    // Consumo de servicio de historial (mapeado a tu estructura de tabla)
                    ServicesResult<List<InventoryAuditsDto>> auditResult = await inventoryService.GetMovementHistory();
                    if (auditResult.Success)
                    {
                        _allAuditLogs = auditResult.Data ?? new List<InventoryAuditsDto>();
                        FiltrarDatosSegunPestaña();
                    }
                    else
                    {
                        _allAuditLogs.Clear();
                        dgAuditoria.ItemsSource = null;
                        MessageBox.Show(auditResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado al cargar datos: {ex.Message}", "Excepción", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ejecuta el filtro dinámico por texto en el DataGrid activo
        /// </summary>
        private void FiltrarDatosSegunPestaña()
        {
            string filtro = ProductIdInput?.Trim().ToLower() ?? string.Empty;

            if (TabItemSelected == 1) // Filtrar Repuestos
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    dgRepuestos.ItemsSource = _allProducts;
                }
                else
                {
                    dgRepuestos.ItemsSource = _allProducts.Where(p =>
                        (p.id != null && p.id.ToString().Contains(filtro)) ||
                        (p.name != null && p.name.ToLower().Contains(filtro)) ||
                        (p.category != null && p.category.ToLower().Contains(filtro))
                    ).ToList();
                }
            }
            else if (TabItemSelected == 2) // Filtrar Historial de Auditoría
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    dgAuditoria.ItemsSource = _allAuditLogs;
                }
                else
                {
                    dgAuditoria.ItemsSource = _allAuditLogs.Where(a =>
                        (a.Folio != null && a.Folio.ToString().Contains(filtro)) ||
                        (a.ProductoNombre != null && a.ProductoNombre.ToLower().Contains(filtro)) ||
                        (a.Motivo != null && a.Motivo.ToLower().Contains(filtro)) ||
                        (a.UsuarioResponsable != null && a.UsuarioResponsable.ToLower().Contains(filtro))
                    ).ToList();
                }
            }
        }

        private void BtnReajustarFila_Click(object sender, RoutedEventArgs e)
        {
            var botom = sender as Button;
            if (botom?.DataContext is not ProductsDto productsDto)
                return;

            var productSelected = productService.SearchProductById(productsDto.id);

            if (productSelected == null || !productSelected.Success)
                return;

            DiallogMovementInventory win = new DiallogMovementInventory(productSelected.Data);
                win.Owner = Window.GetWindow(this);
                bool? result = win.ShowDialog();

                if (result == true)
                {
                    _ = LoadData(_tipoDataSeleccionado);
                }
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}