using System.Collections;
using System.ComponentModel;

namespace ComicLibrary.View.Comparers
{
  public interface ICustomComparer : IComparer
  {
    ListSortDirection SortDirection { get; set; }

    string PropertyName { get; set; }
  }
}
