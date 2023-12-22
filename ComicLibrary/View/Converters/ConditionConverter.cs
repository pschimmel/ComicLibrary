using System.Globalization;
using System.Windows.Data;
using ComicLibrary.Model.Entities;

namespace ComicLibrary.View.Converters
{
  internal class ConditionConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Condition condition)
      {
        return condition switch
        {
          Condition.Excellent => "1 - Excellent",
          Condition.VeryGood => "2 - Very Good",
          Condition.Good => "3 - Good",
          Condition.Medium => "4 - Medium",
          Condition.Bad => "5 - Bad",
          _ => throw new ArgumentException("Unknown Condition value"),
        };
      }

      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
