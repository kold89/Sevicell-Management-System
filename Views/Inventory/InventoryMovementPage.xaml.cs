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
        // Servicios
        private readonly ProductsServices productService = new ProductsServices();
        // NOTA: Asumo que tienes un servicio para el historial, si no, puedes adaptarlo a tu arquitectura
        private readonly movementInventoryService inventoryService = new movementInventoryService();

        public event PropertyChangedEventHandler? PropertyChanged;

        // Listas maestras en memoria para filtrado rápido local
        private List<ProductsDto> _allProducts = new List<ProductsDto>();
        private List<InventoryAuditDto> _allAuditLogs = new List<InventoryAuditDto>();

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

                    // Dispara la carga de datos correspondiente a la pestaña activa sin congelar la UI
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

                    // El buscador filtra la pestaña que esté actualmente activa
                    FiltrarDatosSegunPestaña();
                }
            }
        }

        public InventoryMovement()
        {
            InitializeComponent();
            this.DataContext = this;

            // Forzar la carga inicial de la Pestaña 1
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
                    ServicesResult<List<InventoryAuditDto>> auditResult = await inventoryService.GetMovementHistory();
                    if (auditResult.Success)
                    {
                        _allAuditLogs = auditResult.Data ?? new List<InventoryAuditDto>();
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
            var boton = sender as Button;
            if (boton != null && boton.DataContext is ProductsDto productoSeleccionado)
            {
                // 1. Instanciar la ventana modal
                DiallogMovementInventory modalReajustar = new DiallogMovementInventory();
                modalReajustar.Owner = Window.GetWindow(this);

                // 2. PASAR INFORMACIÓN A LA MODAL:
                // Para que tu modal sepa qué producto va a ajustar, puedes pasarle el objeto a su constructor 
                // o usar una propiedad pública que crees dentro de 'DiallogMovementInventory'. Ejemplo:
                // modalReajustar.SelectedProduct = productoSeleccionado;

                // 3. Mostrar la modal y evaluar el resultado del guardado en SQL
                bool? result = modalReajustar.ShowDialog();
                if (result == true)
                {
                    // Si se guardó con éxito en la base de datos, refrescamos la pestaña actual
                    _ = LoadData(_tipoDataSeleccionado);
                }
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

    #region DTO Auxiliar para el Historial (Si no lo tienes creado)
    /// <summary>
    /// Estructura de datos requerida por los Bindings de tu segunda tabla (dgAuditoria)
    /// </summary>
    public class InventoryAuditDto
    {
        public int Folio { get; set; }
        public string ProductoNombre { get; set; }
        public int StockAnterior { get; set; }
        public string CantidadConSigno { get; set; }
        public bool EsIncremento { get; set; }
        public int StockNuevo { get; set; }
        public string Motivo { get; set; }
        public DateTime? Fecha { get; set; }
        public string UsuarioResponsable { get; set; }
    }
    #endregion
}