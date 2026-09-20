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
using WpfApp1.Services;

namespace WpfApp1.Views.Credit
{
    /// <summary>
    /// Lógica de interacción para CuotasWindow.xaml
    /// </summary>
    public partial class CuotasWindow : Window
    {
        private readonly CreditContractsServices _service = new();
        private readonly int _contractId;

        public CuotasWindow(int contractId)
        {
            InitializeComponent();
            _contractId = contractId;
            TxtTitulo.Text = $"Cuotas — Contrato #{contractId}";
            Loaded += async (s, e) => await CargarCuotasAsync();
        }

        private async Task CargarCuotasAsync()
        {
            var result = await _service.GetDebtInstalmentAsync(_contractId);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            DgCuotas.ItemsSource = result.Data;
        }
    }
}
