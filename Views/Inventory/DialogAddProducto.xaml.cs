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

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para DialogAddProducto.xaml
    /// </summary>
    public partial class DialogAddProducto : Window
    {
        public DialogAddProducto(Product product = null )
        {
            InitializeComponent();
        }

    
    }
}
