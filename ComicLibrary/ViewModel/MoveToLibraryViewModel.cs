using System.Collections.Generic;
using System.Linq;
using ComicLibrary.Model.Entities;
using ES.Tools.Core.MVVM;

namespace ComicLibrary.ViewModel
{
  public class MoveToLibraryViewModel : ES.Tools.Core.MVVM.ViewModel, IViewModel
  {
    public MoveToLibraryViewModel(IEnumerable<Library> libraries)
    {
      Libraries = [.. libraries.OrderBy(x => x.Name)];

      if (Libraries.Count > 0)
        SelectedLibrary = Libraries.First();
    }

    public List<Library> Libraries { get; }

    public Library SelectedLibrary { get; set; }
  }
}