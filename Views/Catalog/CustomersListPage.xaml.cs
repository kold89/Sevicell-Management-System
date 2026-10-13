using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Services;
using WpfApp1.ViewModels;
using WpfApp1.Views.Credit;

namespace WpfApp1.Views.Catalog
{
    public partial class CustomersListPage : Page
    {
        private readonly CustomerServices _customerServices = new CustomerServices();
        private List<custumerDTO> _allCustomers = new();   // todos los clientes
        private List<custumerDTO> _filtered = new();       // después del buscador
        private bool _updatingPager;                       // evita rebotes del evento del paginador

        public CustomersListPage()
        {
            InitializeComponent();
            Loaded += async (_, _) => await LoadCustomersAsync();
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var result = await _customerServices.ListAllCustomersForGrid();
                if (!result.Success)
                {
                    ToastService.ShowInfo(result.Message);
                    return;
                }

                _allCustomers = result.Data?.ToList() ?? new List<custumerDTO>();
                ApplyFilter(resetPage: false);   // conserva la página después de editar
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Error inesperado: " + ex.Message);
            }
        }

        private void ApplyFilter(bool resetPage)
        {
            string term = Normalize(txtBuscar.Text);

            IEnumerable<custumerDTO> query = _allCustomers;
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = _allCustomers.Where(c =>
                    Normalize(c.Name).Contains(term) ||
                    Normalize(c.Mail).Contains(term) ||
                    Normalize(c.Phone).Contains(term) ||
                    Normalize(c.Address).Contains(term) ||
                    c.Id.ToString().Contains(term));
            }

            _filtered = query.ToList();

            // Calcula una página válida antes de configurar el paginador
            int size = Paginador.RegistrosPorPagina;
            int totalPages = Math.Max(1, (int)Math.Ceiling(_filtered.Count / (double)size));
            int page = resetPage ? 1 : Math.Min(Paginador.PaginaActual, totalPages);
            if (page < 1) page = 1;

            _updatingPager = true;
            Paginador.Configurar(_filtered.Count, page);
            _updatingPager = false;

            ShowPage();
        }

        private void ShowPage()
        {
            int size = Paginador.RegistrosPorPagina;
            int skip = (Paginador.PaginaActual - 1) * size;

            var pageItems = _filtered.Skip(skip).Take(size).ToList();
            dgCustomers.ItemsSource = pageItems;

            bool empty = _filtered.Count == 0;
            pnlEmpty.Visibility = empty ? Visibility.Visible : Visibility.Collapsed;
            Paginador.Visibility = empty ? Visibility.Collapsed : Visibility.Visible;

            if (empty)
            {
                TxtResumen.Text = string.Empty;
                txtEmpty.Text = _allCustomers.Count == 0
                    ? "Aún no hay clientes registrados."
                    : "Ningún cliente coincide con la búsqueda.";
            }
            else
            {
                TxtResumen.Text = $"Mostrando {skip + 1}–{skip + pageItems.Count} de {_filtered.Count} cliente(s).";
            }
        }

        private void Paginador_PaginaCambiada(object sender, EventArgs e)
        {
            if (_updatingPager) return;
            ShowPage();
        }

        private static string Normalize(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            string decomposed = text.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char ch in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            ApplyFilter(resetPage: true);   // buscar siempre vuelve a la página 1
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private async void BtnAddCustomers_Click(object sender, RoutedEventArgs e)
        {
            var modal = new AddOrEditCustumer { Owner = Window.GetWindow(this) };
            if (modal.ShowDialog() == true)
                await LoadCustomersAsync();
        }

        private async void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { DataContext: custumerDTO customer }) return;
            await OpenEditAsync(customer);
        }

        private async void dgCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCustomers.SelectedItem is custumerDTO customer &&
                e.OriginalSource is DependencyObject src &&
                FindParent<DataGridRow>(src) != null)
            {
                await OpenEditAsync(customer);
            }
        }

        private async Task OpenEditAsync(custumerDTO customer)
        {
            var modal = new AddOrEditCustumer(customer) { Owner = Window.GetWindow(this) };
            if (modal.ShowDialog() == true)
                await LoadCustomersAsync();
        }

        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T match) return match;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
    }
}