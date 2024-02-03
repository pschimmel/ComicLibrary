using System.Windows.Data;
using ComicLibrary.ViewModel;

namespace ComicLibrary.View.Converters
{
  public class ActiveDocumentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value is ActiveLibraryViewModel ? value : Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value is ActiveLibraryViewModel ? value : Binding.DoNothing;
    }
  }
}
