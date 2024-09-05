using System.ComponentModel;
using ComicLibrary.ViewModel;

namespace ComicLibrary.View.Comparers
{
  public class CustomComparer : ICustomComparer
  {
    private readonly StringAsNumberComparer _stringComparer;

    public CustomComparer()
    {
      _stringComparer = new StringAsNumberComparer();
    }

    public ListSortDirection SortDirection { get; set; } = ListSortDirection.Ascending;

    public string PropertyName { get; set; }

    public int Compare(object x, object y)
    {
      if (x == null)
        return -1;
      if (y == null)
        return 1;

      var type = x.GetType();
      if (type != y.GetType())
        throw new NotSupportedException("Cannot compare two different types.");

      var property = type.GetProperty(PropertyName);
      var xValue = property.GetValue(x)?.ToString();
      var yValue = property.GetValue(y)?.ToString();

      int result = _stringComparer.Compare(xValue, yValue);
      if (SortDirection == ListSortDirection.Descending)
      {
        result = -result;
      }
      return result;
    }

    public int Compare(ComicViewModel x, ComicViewModel y)
    {
      return Compare((object)x, (object)y);
    }
  }
}
