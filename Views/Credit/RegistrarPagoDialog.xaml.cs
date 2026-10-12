using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Credit
{
    /// <summary>
    /// Lógica de interacción para RegistrarPagoDialog.xaml
    /// </summary>
    public partial class RegistrarPagoDialog : Window
    {
        
        private readonly int _contractId;
        private readonly List<InstallmentPreview> _cuotasPendientes;
        private readonly decimal _saldoTotal;

        public RegistrarPagoDialog(int contractId, string nombreCliente, List<InstallmentPreview> cuotasPendientes)
        {
            InitializeComponent();
            _contractId = contractId;
            _cuotasPendientes = cuotasPendientes.OrderBy(c => c.DueDate).ToList();
            _saldoTotal = _cuotasPendientes.Sum(c => c.SaldoPendiente);

            TxtSubtitulo.Text = $"Contrato #{contractId} — {nombreCliente}";
            TxtSaldoTotal.Text = $"L. {_saldoTotal:N2}";

            TxtMonto.Text = _cuotasPendientes.First().SaldoPendiente
                .ToString("0.00", CultureInfo.InvariantCulture);
            RenderCascada();
            Loaded += (s, e) => { TxtMonto.Focus(); TxtMonto.SelectAll(); };
        }
        private void BtnCuota_Click(object sender, RoutedEventArgs e) =>
             TxtMonto.Text = _cuotasPendientes.First().SaldoPendiente.ToString("0.00", CultureInfo.InvariantCulture);

        private void BtnTodo_Click(object sender, RoutedEventArgs e) =>
            TxtMonto.Text = _saldoTotal.ToString("0.00", CultureInfo.InvariantCulture);
        public decimal Monto { get; private set; }
        private void TxtMonto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9.]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void TxtMonto_TextChanged(object sender, TextChangedEventArgs e)
        {
            RenderCascada();
        }

        private bool TryGetMonto(out decimal monto)
        {
            return decimal.TryParse(TxtMonto.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out monto);
        }

        private void RenderCascada()
        {
            TxtError.Visibility = Visibility.Collapsed;
            BtnConfirmar.IsEnabled = true;
            IcCuotas.ItemsSource = null;

            if (!TryGetMonto(out var monto) || monto <= 0)
            {
                MostrarError("Ingrese un monto válido mayor a cero.");
                return;
            }
            if (decimal.Round(monto, 2) != monto)
            {
                MostrarError("Use como máximo dos decimales.");
                return;
            }
            if (monto > _saldoTotal)
            {
                MostrarError($"El monto excede el saldo total del contrato ({_saldoTotal:N2}).");
                return;
            }

            var restante = monto;
            var filas = new List<FilaCascadaVm>();

            foreach (var cuota in _cuotasPendientes)
            {
                if (restante <= 0) break;

                var aplicado = Math.Min(restante, cuota.SaldoPendiente);
                restante -= aplicado;
                var saldada = aplicado >= cuota.SaldoPendiente;

                filas.Add(new FilaCascadaVm
                {
                    Etiqueta = $"Cuota {cuota.InstallmentNumber}",
                    MontoTexto = $"L. {aplicado:N2}",
                    EstadoTexto = saldada ? "saldada" : "parcial",
                    EstadoColor = new SolidColorBrush(saldada
                        ? (Color)ColorConverter.ConvertFromString("#059669")
                        : (Color)ColorConverter.ConvertFromString("#D97706"))
                });
            }

            IcCuotas.ItemsSource = filas;
        }

        private void MostrarError(string mensaje)
        {
            TxtError.Text = mensaje;
            TxtError.Visibility = Visibility.Visible;
            BtnConfirmar.IsEnabled = false;
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetMonto(out var monto) || monto <= 0 || monto > _saldoTotal
                || decimal.Round(monto, 2) != monto) return;

            Monto = monto;
            DialogResult = true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private class FilaCascadaVm
        {
            public string Etiqueta { get; set; }
            public string MontoTexto { get; set; }
            public string EstadoTexto { get; set; }
            public SolidColorBrush EstadoColor { get; set; }
        }

    }
}
