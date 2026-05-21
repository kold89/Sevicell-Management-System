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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Data;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Views
{
    /// <summary>
    /// Lógica de interacción para AuditList.xaml
    /// </summary>
    public partial class AuditList : Page
    {
        private int paginaActual = 1;
        private int registrosPorPagina = 15; // Ajusta según el alto de tu pantalla
        private int totalRegistros = 0;

        public AuditList()
        {
            InitializeComponent();
            LoadData();
        }

        public async void LoadData()
        {
            try
            {
                using (var db = new DBSevicellContext())
                {
                    // 1. Obtener total para calcular páginas
                    totalRegistros = await db.AuditTables.CountAsync();
                    int totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                    // 2. Cargar solo el segmento (Paginación)
                    var logs = await db.AuditTables
                        .OrderByDescending(x => x.DateCreate)
                        .Skip((paginaActual - 1) * registrosPorPagina)
                        .Take(registrosPorPagina)
                        .ToListAsync();

                    dgAudit.ItemsSource = logs;

                    // 3. Actualizar Interfaz
                    lblPag.Text = $"Página {paginaActual} de {Math.Max(1, totalPaginas)}";

                    // Deshabilitar botones si no hay más páginas
                    BtnPreviusPage.IsEnabled = paginaActual > 1;
                    BtnPreviusPage.Opacity = BtnPreviusPage.IsEnabled ? 1 : 0.3;

                    BtnNextPage.IsEnabled = paginaActual < totalPaginas;
                    BtnNextPage.Opacity = BtnNextPage.IsEnabled ? 1 : 0.3;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar logs: " + ex.Message);
            }
        }
        private void BtnSee_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void BtnPreviusPage_Click(object sender, RoutedEventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                LoadData();
            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
            if (paginaActual < totalPaginas)
            {
                paginaActual++;
                LoadData();
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Buscar...";
                txtBuscar.Opacity = 0.5;
            }
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "Buscar...")
            {
                txtBuscar.Text = "";
                txtBuscar.Opacity = 1;
            }
        }
    }
}
