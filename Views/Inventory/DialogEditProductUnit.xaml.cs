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

namespace WpfApp1.Views.Inventory
{
    /// <summary>
    /// Lógica de interacción para DialogEditProductUnit.xaml
    /// </summary>
    public partial class DialogEditProductUnit : Window
    {
        private readonly ProductUnit _ProductUnit = new ProductUnit();
        private readonly productUnitServices servicesUnit = new productUnitServices();
        public DialogEditProductUnit(ProductUnit productUnit = null)
        {
            InitializeComponent();
            _ProductUnit = productUnit;
            FillEditModal();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void FillEditModal()
        {
            TxtImei1.Text = _ProductUnit.Imei ?? string.Empty;
            TxtImei2.Text = _ProductUnit.Imei2 ?? string.Empty;
            TxtColor.Text = _ProductUnit.Colour ?? string.Empty;
            TxtModel.Text = _ProductUnit.Model ?? string.Empty;
        }
        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _ProductUnit.Imei = TxtImei1.Text;
            _ProductUnit.Imei2 = TxtImei2.Text;
            _ProductUnit.Colour = TxtColor.Text;
            _ProductUnit.Model = TxtModel.Text;

            var result = await servicesUnit.UpdateProductUnitAsync(_ProductUnit);
            if (result.Success) ToastService.ShowSuccess(result.Message);

            this.DialogResult = true;
            this.Close();
        }
    }
}
