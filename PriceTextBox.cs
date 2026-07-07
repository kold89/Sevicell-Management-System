using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public class PriceTextBox : TextBox
    {
        public PriceTextBox()
        {
            DataObject.AddPastingHandler(this, AlCopyText);
            LostFocus += PriceTexBox_LostFocus;
            GotFocus += PriceTexBox_GotFocus;
        }

        // Solo permite dígitos y un único punto decimal
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            string textoFuturo = ObtenerTextoFuturo(e.Text);
            e.Handled = !EsFormatoValido(textoFuturo);
            base.OnPreviewTextInput(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
            base.OnPreviewKeyDown(e);
        }

        private void AlCopyText(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string texto = (string)e.DataObject.GetData(typeof(string));
                // Permite pegar valores ya formateados con comas de miles, ej: 2,000.00
                string limpio = texto.Replace(",", "");
                if (!Regex.IsMatch(limpio, @"^[0-9]*\.?[0-9]*$"))
                    e.CancelCommand();
            }
            else e.CancelCommand();
        }

        // Al obtener foco, muestra el número "limpio" (sin comas) para facilitar edición
        private void PriceTexBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                Text = valor.ToString("0.##", CultureInfo.InvariantCulture);
                CaretIndex = Text.Length;
            }
        }

        // Al perder foco, formatea con separador de miles y 2 decimales: 2,000.00
        private void PriceTexBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                Text = "0.00";
                return;
            }

            if (decimal.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                Text = valor.ToString("#,##0.00", CultureInfo.InvariantCulture);
            }
        }

        // Valida que el texto resultante tenga máximo un punto decimal
        private bool EsFormatoValido(string texto)
        {
            return Regex.IsMatch(texto, @"^[0-9]*\.?[0-9]*$");
        }

        // Simula cómo quedaría el texto si se acepta la entrada actual
        private string ObtenerTextoFuturo(string textoNuevo)
        {
            int inicioSeleccion = SelectionStart;
            int longitudSeleccion = SelectionLength;

            string textoActual = Text.Remove(inicioSeleccion, longitudSeleccion);
            return textoActual.Insert(inicioSeleccion, textoNuevo);
        }

        // Propiedad útil para leer el valor como decimal directamente
        public decimal ValorDecimal
        {
            get
            {
                decimal.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor);
                return valor;
            }
            set
            {
                Text = value.ToString("#,##0.00", CultureInfo.InvariantCulture);
            }
        }
  
    }
}
