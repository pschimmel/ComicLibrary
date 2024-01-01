using ES.Tools.Core.MVVM;

namespace ComicLibrary.ViewModel
{
  public class GetNameViewModel : ES.Tools.Core.MVVM.ViewModel, IViewModel
  {
    private readonly Func<string, bool> _validation;
    private string _name;

    public GetNameViewModel(string title, string label, string name, Func<string, bool> validationDelegate)
    {
      Title = title;
      Label = label;
      _name = name;
      _validation = validationDelegate;
      Validate();
    }

    public bool ValidationOK { get; private set; }

    public string Title { get; }

    public string Label { get; }

    public string Name
    {
      get => _name;
      set
      {
        if (_name != value)
        {
          _name = value;
          OnPropertyChanged(nameof(Name));
          Validate();
        }
      }
    }

    public void Validate()
    {
      ValidationOK = _validation?.Invoke(Name) ?? true;
      OnPropertyChanged(nameof(ValidationOK));
    }
  }
}