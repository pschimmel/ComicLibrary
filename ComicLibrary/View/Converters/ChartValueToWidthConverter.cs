using System.Globalization;
using System.Windows.Data;

namespace ComicLibrary.View.Converters
{
  internal class ChartValueToWidthConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values[0] is not int valueAsInt) throw new ArgumentException("Value 1 needs to be of type integer.");
      if (values[1] is not int maxAsInt) throw new ArgumentException("Value 2 needs to be of type integer.");
      if (values[2] is not bool showRelative) throw new ArgumentException("Value 3 needs to be of type bool.");
      if (values[3] is not double availableWidth) throw new ArgumentException("Value 4 needs to be of type double.");

      return !showRelative
             ? valueAsInt
             : Math.Max(valueAsInt, (double)((availableWidth - 50) / maxAsInt * valueAsInt));
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
