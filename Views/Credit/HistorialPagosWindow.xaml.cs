using Microsoft.EntityFrameworkCore;
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
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Views.Credit
{
    /// <summary>
    /// Lógica de interacción para HistorialPagosWindow.xaml
    /// </summary>
    public partial class HistorialPagosWindow : Window
    {
        public class PaymentHistoryItemDto
        {
            public DateTime PaymentDate { get; set; }
            public int InstallmentNumber { get; set; }
            public decimal Amount { get; set; }
            public string ReceivedBy { get; set; }
            public string? Notes { get; set; }
        }

        private readonly CreditContractsServices _service = new();
        private readonly int _contractId;

        public HistorialPagosWindow(int contractId)
        {
            InitializeComponent();
            _contractId = contractId;
            TxtTitulo.Text = $"Histórico de pagos — Contrato #{contractId}";
            Loaded += async (s, e) => await CargarHistorialAsync();
        }

        private async Task CargarHistorialAsync()
        {
            var result = await GetPaymentHistoryAsync(_contractId);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            DgHistorial.ItemsSource = result.Data;

            if (result.Data.Count == 0)
            {
                MessageBox.Show("Este contrato aún no tiene pagos registrados.", "Sin pagos",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async Task<ServicesResult<List<PaymentHistoryItemDto>>> GetPaymentHistoryAsync(int contractId)
        {
            try
            {
                using (var db = new SevicellDbContext())
                {
                    var historial = await db.InstallmentPayments
                        .Where(p => p.Installment.ContractId == contractId)
                        .OrderByDescending(p => p.PaymentDate)
                        .Select(p => new PaymentHistoryItemDto
                        {
                            PaymentDate = p.PaymentDate,
                            InstallmentNumber = p.Installment.InstallmentNumber,
                            Amount = p.Amount,
                            ReceivedBy = p.ReceivedBy,
                            Notes = p.Notes
                        })
                        .ToListAsync();

                    return ServicesResult<List<PaymentHistoryItemDto>>.Ok(historial, "Historial obtenido correctamente.");
                }
            }
            catch (Exception ex)
            {
                return ServicesResult<List<PaymentHistoryItemDto>>.Fail("Error al obtener el historial: " + ex.Message);
            }
        }
    }
}
