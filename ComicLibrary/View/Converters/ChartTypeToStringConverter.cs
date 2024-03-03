using System.Globalization;
using System.Windows.Data;
using ComicLibrary.ViewModel;

namespace ComicLibrary.View.Converters
{
  internal class ChartTypeToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is ChartType type
        ? type switch
        {
          ChartType.Year => Properties.Resources.Year,
          ChartType.Grading => Properties.Resources.Grading,
          ChartType.Price => Properties.Resources.PurchasePrice,
          ChartType.Value => Properties.Resources.EstimatedValue,
          _ => throw new NotImplementedException($"Wrong {nameof(ChartType)} {value}")
        }
        : throw new NotImplementedException($"Wrong {nameof(ChartType)} {value}");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
