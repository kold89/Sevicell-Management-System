using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.Converters
{
    public class BoolToClienteTipoConverter
    {
        private static readonly BrushConverter _brushConverter = new BrushConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool esRegistrado = value is bool b && b;
            string modo = parameter as string;

            if (modo == "Background")
            {
                return (Brush)_brushConverter.ConvertFromString(esRegistrado ? "#EFF6FF" : "#F1F5F9");
            }

            if (modo == "Foreground")
            {
                return (Brush)_brushConverter.ConvertFromString(esRegistrado ? "#1E40AF" : "#64748B");
            }

            return esRegistrado ? "Registrado" : "Mostrador";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
