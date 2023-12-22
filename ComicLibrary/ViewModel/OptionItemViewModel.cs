using ComicLibrary.Model.Entities;

namespace ComicLibrary.ViewModel
{
  public class OptionItemViewModel<T> : IOptionItemViewModel<T> where T : IOption
  {
    public OptionItemViewModel(T option)
    {
      Option = option;
      Name = option.Name;
    }

    public string Name { get; }

    public bool IsEmpty => false;

    public T Option { get; }
  }
}
