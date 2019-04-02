using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Converters
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BooleanToBrushConverter : IValueConverter
    {
        private readonly Color trueColor;
        private readonly Color falseColor;

        public BooleanToBrushConverter(Color trueColor, Color falseColor)
        {
            this.trueColor = trueColor;
            this.falseColor = falseColor;
        }

        public object Convert(object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var color = new Color();

            if (value is bool booleanValue)
                color = booleanValue ? trueColor : falseColor;

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return false;
        }
    }
}
