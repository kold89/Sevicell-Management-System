using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Services;

namespace WpfApp1.Views.Credit
{
    public partial class HistorialPagosView : UserControl
    {
        private readonly CreditContractsServices _service = new();

        public HistorialPagosView()
        {
            InitializeComponent();
        }

        /// <summary>Devuelve true si cargó bien (aunque no haya pagos).</summary>
        public async Task<bool> CargarAsync(int contractId)
        {
            var result = await _service.GetPaymentHistoryAsync(contractId);

            if (!result.Success || result.Data == null)
            {
                DgHistorial.ItemsSource = null;
                TxtVacio.Visibility = Visibility.Collapsed;
                ToastService.ShowError(result.Message);
                return false;
            }

            bool vacio = result.Data.Count == 0;
            DgHistorial.ItemsSource = result.Data;
            DgHistorial.Visibility = vacio ? Visibility.Collapsed : Visibility.Visible;
            TxtVacio.Visibility = vacio ? Visibility.Visible : Visibility.Collapsed;
            return true;
        }
    }
}