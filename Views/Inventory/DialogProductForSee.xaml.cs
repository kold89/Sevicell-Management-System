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
    /// Lógica de interacción para DialogProductForSee.xaml
    /// </summary>
    public partial class DialogProductForSee : Window
    {
        private readonly ProductsDto productsDto;
        public DialogProductForSee(ProductsDto selectedProduct)
        {
            InitializeComponent();
            productsDto = selectedProduct;
            FillDialog();
        }

        public void FillDialog()
        {
            txtCode.Text = productsDto.code;
            txtName.Text = productsDto.name;
            txtSalesPrice.Text = Convert.ToString(productsDto.salesPrice);
            txtStock.Text = Convert.ToString(productsDto.stock);
            txtBrand.Text = productsDto.Brand;
            txtCategory.Text = productsDto.category;
            txtDescription.Text = productsDto.description;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
