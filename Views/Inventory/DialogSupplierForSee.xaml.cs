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
using WpfApp1.ViewModels;

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para DialogSupplierForSee.xaml
    /// </summary>
    public partial class DialogSupplierForSee : Window
    {
        private readonly SupplierDto _supplier;
        public DialogSupplierForSee(SupplierDto supplier)
        {
            InitializeComponent();
            _supplier  = supplier;
            FillDialog();
        }
        public void FillDialog()
        {
            txtCode.Text =  Convert.ToString(_supplier.id);
            txtName.Text = _supplier.name;
            txtEmail.Text = _supplier.email;
            txtPhone.Text = _supplier.phone;
            txtAddres.Text = _supplier.address;
            txtCta.Text = _supplier.cta;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
