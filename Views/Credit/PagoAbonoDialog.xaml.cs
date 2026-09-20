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

namespace WpfApp1.Views.Credit
{
    /// <summary>
    /// Lógica de interacción para PagoAbonoDialog.xaml
    /// </summary>
    public partial class PagoAbonoDialog : Window
    {
        private readonly decimal _saldoPendiente;
        private readonly bool _esPagoTotal;

        public decimal Monto { get; private set; }

        public PagoAbonoDialog(decimal saldoPendiente, bool esPagoTotal)
        {
            InitializeComponent();
            _saldoPendiente = saldoPendiente;
            _esPagoTotal = esPagoTotal;

            TxtSaldo.Text = saldoPendiente.ToString("N2");
            TxtTitulo.Text = esPagoTotal ? "Pago total" : "Pago parcial";
            TxtSubtitulo.Text = esPagoTotal
                ? "Se registrará el pago completo de esta cuota."
                : "Ingrese el monto que el cliente está abonando.";

            TxtMonto.Text = saldoPendiente.ToString("N2");
            if (esPagoTotal)
            {
                TxtMonto.IsEnabled = false;
            }
            else
            {
                TxtMonto.SelectAll();
                TxtMonto.Focus();
            }
        }

        private void TxtMonto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Solo dígitos y un punto decimal
            var regex = new Regex(@"^[0-9.]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void TxtMonto_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidarMonto();
        }

        private bool ValidarMonto()
        {
            TxtError.Visibility = Visibility.Collapsed;
            BtnConfirmar.IsEnabled = true;

            if (!decimal.TryParse(TxtMonto.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var monto))
            {
                MostrarError("Ingrese un monto válido.");
                return false;
            }
            if (monto <= 0)
            {
                MostrarError("El monto debe ser mayor a cero.");
                return false;
            }
            if (monto > _saldoPendiente)
            {
                MostrarError($"El monto no puede exceder el saldo pendiente ({_saldoPendiente:N2}).");
                return false;
            }
            return true;
        }

        private void MostrarError(string mensaje)
        {
            TxtError.Text = mensaje;
            TxtError.Visibility = Visibility.Visible;
            BtnConfirmar.IsEnabled = false;
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarMonto()) return;

            Monto = decimal.Parse(TxtMonto.Text, NumberStyles.Number, CultureInfo.InvariantCulture);
            DialogResult = true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
