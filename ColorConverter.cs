using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Jpp.Ironstone.Structures
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Color.FromRgb(0, 0, 0);

            Autodesk.AutoCAD.Colors.Color pickedColor = (Autodesk.AutoCAD.Colors.Color)value;
            return pickedColor.ColorValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Autodesk.AutoCAD.Colors.Color.FromRgb(0, 0, 0);

            Color pickedColor = (Color)value;
            return Autodesk.AutoCAD.Colors.Color.FromColor(pickedColor);

        }

    }
}