using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.Views
{
    public partial class AuditList : Page
    {
        private bool _isLoading;

        public AuditList()
        {
            InitializeComponent();
            Loaded += async (_, _) => await LoadData(nuevaCarga: true);
        }

        private async Task LoadData(bool nuevaCarga = true)
        {
            // Evita cargas solapadas (clic rápido en las flechas)
            if (_isLoading) return;
            _isLoading = true;
            Paginador.IsEnabled = false;

            try
            {
                using var db = new SevicellDbContext();

                // 1. Total, para que el paginador calcule sus páginas
                int total = await db.AuditTables.CountAsync();

                // 2. Configurar ANTES de leer PaginaActual: el control la ajusta si ya no existe
                Paginador.Configurar(total, nuevaCarga ? 1 : Paginador.PaginaActual);

                int tam = Paginador.RegistrosPorPagina;
                int salto = (Paginador.PaginaActual - 1) * tam;

                // 3. Solo el segmento que toca
                var logs = await db.AuditTables
                    .AsNoTracking()
                    .OrderByDescending(x => x.DateCreate)
                    .ThenByDescending(x => x.Id)   // orden estable para Skip/Take
                    .Skip(salto)
                    .Take(tam)
                    .ToListAsync();

                dgAudit.ItemsSource = logs;

                Paginador.Visibility = total == 0 ? Visibility.Collapsed : Visibility.Visible;

                TxtResumen.Text = total == 0
                    ? "No hay registros de auditoría."
                    : $"Mostrando {salto + 1}–{salto + logs.Count} de {total} registro(s).";
                Paginador.Visibility = total == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)

            {
                TxtResumen.Text = string.Empty;
                ToastService.ShowError("Error al cargar los registros de auditoría: " + ex.Message);

            }
            finally
            {
                Paginador.IsEnabled = true;
                _isLoading = false;
            }
        }

        private async void Paginador_PaginaCambiada(object sender, EventArgs e)
        {
            await LoadData(nuevaCarga: false);
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void BtnSee_Click(object sender, RoutedEventArgs e)
        {
            // pendiente
        }
    }
}