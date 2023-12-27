using ComicLibrary.Model.Entities;

namespace ComicLibrary.ViewModel
{
  public class EditOptionViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    public EditOptionViewModel(IOption option, string header)
    {
      Option = option;
      Header = header;
    }

    public IOption Option { get; }

    public string Header { get; }
  }
}
