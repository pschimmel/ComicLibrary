using ComicLibrary.Model.Entities;

namespace ComicLibrary.ViewModel
{
  public interface IOptionItemViewModel<T> where T : IOption
  {
    string Name { get; }

    bool IsEmpty { get; }

    T Option { get; }
  }
}