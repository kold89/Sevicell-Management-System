using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Services;
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Credit
{
    public partial class AdelantoPagoDialog : Window
    {
        private readonly int _contractId;
        private readonly List<InstallmentPreview> _cuotasPendientes;
        private readonly decimal _saldoTotal;

        public decimal Monto { get; private set; }

        public AdelantoPagoDialog(int contractId, string nombreCliente, List<InstallmentPreview> cuotasPendientes)
        {
            InitializeComponent();
            _contractId = contractId;
            _cuotasPendientes = cuotasPendientes.OrderBy(c => c.DueDate).ToList();
            _saldoTotal = _cuotasPendientes.Sum(c => c.SaldoPendiente);

            TxtSubtitulo.Text = $"Contrato #{contractId} — {nombreCliente}";
            TxtSaldoTotal.Text = $"L. {_saldoTotal:N2}";

            RenderCascada();
        }

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
                var saldada = aplicado >= cuota.SaldoPendiente - 0.01m;

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
            if (!TryGetMonto(out var monto) || monto <= 0 || monto > _saldoTotal) return;
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