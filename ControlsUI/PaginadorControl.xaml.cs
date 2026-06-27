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

namespace WpfApp1.ControlsUI
{
    /// <summary>
    /// Lógica de interacción para PaginadorControl.xaml
    /// </summary>
    public partial class PaginadorControl : UserControl
    {
        // Variables internas
        private int paginaActual = 1;
        private int totalPaginas = 1;
        private int registrosPorPagina = 10;

        // 2. Propiedad pública que expone el atributo hacia afuera
        public int RegistrosPorPagina
        {
            get => registrosPorPagina;
            set
            {
                if (value > 0)
                {
                    registrosPorPagina = value;
                }
            }
        }

        // Eventos personalizados que las otras ventanas pueden escuchar
        public event EventHandler PaginaCambiada;

        public PaginadorControl()
        {
            InitializeComponent();
            ActualizarEtiqueta();
        }

        // Propiedades públicas para controlar el paginador desde fuera
        public int PaginaActual
        {
            get => paginaActual;
            set
            {
                if (value >= 1 && value <= totalPaginas)
                {
                    paginaActual = value;
                    ActualizarEtiqueta();
                }
            }
        }

        public int TotalPaginas
        {
            get => totalPaginas;
            set
            {
                if (value >= 1)
                {
                    totalPaginas = value;
                    if (paginaActual > totalPaginas) paginaActual = totalPaginas;
                    ActualizarEtiqueta();
                }
            }
        }

        private void ActualizarEtiqueta()
        {
            // Cambia el texto del TextBlock interno de forma automática
            lblPag.Text = $"Página {paginaActual} de {totalPaginas}";

            // Deshabilitar botones de forma inteligente si no hay más páginas
            BtnPrevious.IsEnabled = paginaActual > 1;
            BtnNext.IsEnabled = paginaActual < totalPaginas;
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                ActualizarEtiqueta();
                PaginaCambiada?.Invoke(this, EventArgs.Empty); // Avisa que cambió la página
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (paginaActual < totalPaginas)
            {
                paginaActual++;
                ActualizarEtiqueta();
                PaginaCambiada?.Invoke(this, EventArgs.Empty); // Avisa que cambió la página
            }
        }
    }
}
