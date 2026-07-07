using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1.Converters
{
    public class BoolToTipoConverter : IValueConverter
    {
        private static readonly BrushConverter _brushConverter = new BrushConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool esFormal = value is bool b && b;
            string modo = parameter as string;

            if (modo == "Background")
            {
                return (Brush)_brushConverter.ConvertFromString(esFormal ? "#EFF6FF" : "#FFFBEB");
            }

            if (modo == "Foreground")
            {
                return (Brush)_brushConverter.ConvertFromString(esFormal ? "#1E40AF" : "#B45309");
            }

            return esFormal ? "Formal" : "Informal";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
