namespace ComicLibrary.ViewModel
{
  public class AddLibraryViewModel : ES.Tools.Core.MVVM.ViewModel, ILibraryViewModel
  {
    public AddLibraryViewModel()
    {
    }

    public string Name => Properties.Resources.Add;
  }
}