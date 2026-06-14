using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
{
    public class PriceTexBox : TextBox
    {
        public PriceTexBox() {
            DataObject.AddPastingHandler(this, AlCopyText);
        }

    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            // 1. Si intenta poner un punto decimal, revisar que no exista ya otro
            if (e.Text == ".")
            {
                e.Handled = this.Text.Contains(".");
                base.OnPreviewTextInput(e);
                return;
            }

            // 2. Verificar que sea un número del 0 al 9
            if (!Regex.IsMatch(e.Text, "[0-9]"))
            {
                e.Handled = true;
                base.OnPreviewTextInput(e);
                return;
            }

            // 3. REGLA MAESTRA: Validar el límite de 2 decimales
            // Simulamos cómo quedaría el texto si dejamos pasar este número
            int indicePunto = this.Text.IndexOf('.');
            if (indicePunto != -1) // Si ya existe un punto decimal en la caja
            {
                // Si el cursor está colocado después del punto decimal...
                if (this.SelectionStart > indicePunto)
                {
                    string[] partes = this.Text.Split('.');
                    // Si ya tiene 2 o más dígitos en la parte decimal, bloqueamos
                    if (partes[1].Length >= 2 && this.SelectionLength == 0)
                    {
                        e.Handled = true;
                    }
                }
            }

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
                // Valida que el texto copiado sea un decimal válido de máximo 2 dígitos decimales
                if (!Regex.IsMatch(texto, @"^[0-9]+(\.[0-9]{1,2})?$")) e.CancelCommand();
            }
            else e.CancelCommand();
        }
    }
}
