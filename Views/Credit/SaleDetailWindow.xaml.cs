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

namespace WpfApp1.Views.Credit
{
    /// <summary>
    /// Lógica de interacción para SaleDetailWindow.xaml
    /// </summary>
    public partial class SaleDetailWindow : Window
    {
        public SaleDetailWindow(string numeroContrato, string clienteNombre,string details, DateTime? fechaInicio, string dispositivo, string estado, string imei)
        {
            InitializeComponent();

            TxtNumeroContrato.Text = numeroContrato;
            TxtCliente.Text = clienteNombre;
            txtDetailsCliente.Text = details;
            TxtFechaInicio.Text = fechaInicio?.ToString("dd/MM/yyyy");
            TxtDispositivo.Text = dispositivo;
            TxtImei.Text = imei;
            TxtIniciales.Text = ObtenerIniciales(clienteNombre);
        }
        private void BtnCerrar_Click(object sender, RoutedEventArgs e) => Close();

        private string ObtenerIniciales(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                return "?";

            var partes = nombreCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length == 1)
                return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();

            return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
        }
    }
}

