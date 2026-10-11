using System.Windows;

namespace WpfApp1.Views.Credit
{
    public partial class HistorialPagosWindow : Window
    {
        public HistorialPagosWindow(int contractId)
        {
            InitializeComponent();
            TxtTitulo.Text = $"Histórico de pagos — Contrato #{contractId}";

            Loaded += async (s, e) =>
            {
                if (!await Vista.CargarAsync(contractId))
                    Close();
            };
        }
    }
}