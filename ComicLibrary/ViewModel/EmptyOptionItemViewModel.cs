using ComicLibrary.Model.Entities;

namespace ComicLibrary.ViewModel
{
  public class EmptyOptionItemViewModel<T> : IOptionItemViewModel<T> where T : IOption
  {
    public string Name => "Select Item";

    public bool IsEmpty => true;

    public T Option => default;
  }
}
