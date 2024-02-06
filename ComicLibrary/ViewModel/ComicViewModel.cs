using System.Collections.Generic;
using System.Windows.Input;
using ComicLibrary.Model.Entities;
using ES.Tools.Core.MVVM;

namespace ComicLibrary.ViewModel
{
  public class ComicViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    #region Fields

    private readonly Comic _comic;
    private readonly string _libraryName;
    private readonly IEnumerable<IOptionItemViewModel<Publisher>> _publishers;
    private readonly IEnumerable<IOptionItemViewModel<Country>> _countries;
    private readonly IEnumerable<IOptionItemViewModel<Language>> _languages;
    private ActionCommand _editComicCommand;
    private bool _isDirty;

    #endregion

    #region Constructor

    public ComicViewModel(Comic comic,
                          string libraryName,
                          IEnumerable<IOptionItemViewModel<Publisher>> publishers,
                          IEnumerable<IOptionItemViewModel<Country>> countries,
                          IEnumerable<IOptionItemViewModel<Language>> languages)
    {
      _libraryName = libraryName;
      _comic = comic;
      _publishers = publishers;
      _countries = countries;
      _languages = languages;
      Series = comic.Series;
      Year = comic.Year;
      IssueNumber = comic.IssueNumber;
      Title = comic.Title;
      Condition = comic.Condition;
      Publisher = _publishers.FirstOrDefault(x => x.Option == comic.Publisher);
      Country = _countries.FirstOrDefault(x => x.Option == comic.Country);
      _isDirty = false;
    }

    #endregion

    #region Properties

    public string Series
    {
      get => _comic.Series;
      set
      {
        if (_comic.Series != value)
        {
          _comic.Series = value;
          OnPropertyChanged(nameof(Series));
          IsDirty = true;
        }
      }
    }

    public int? Year
    {
      get => _comic.Year;
      set
      {
        if (_comic.Year != value)
        {
          _comic.Year = value;
          OnPropertyChanged(nameof(Year));
          IsDirty = true;
        }
      }
    }

    public int? IssueNumber
    {
      get => _comic.IssueNumber;
      set
      {
        if (_comic.IssueNumber != value)
        {
          _comic.IssueNumber = value;
          OnPropertyChanged(nameof(IssueNumber));
          IsDirty = true;
        }
      }
    }

    public string Title
    {
      get => _comic.Title;
      set
      {
        if (_comic.Title != value)
        {
          _comic.Title = value;
          OnPropertyChanged(nameof(Title));
          IsDirty = true;
        }
      }
    }

    public Grade Condition
    {
      get => _comic.Condition;
      set
      {
        if (_comic.Condition != value)
        {
          _comic.Condition = value;
          OnPropertyChanged(nameof(Condition));
          IsDirty = true;
        }
      }
    }

    public bool CollectorsEdition => _comic.CollectorsEdition;

    public bool LimitedEdition => _comic.LimitedEdition;

    public double? PurchasePrice => _comic.PurchasePrice;

    public double? EstimatedValue => _comic.EstimatedValue;

    public IOptionItemViewModel<Publisher> Publisher
    {
      get => _publishers.FirstOrDefault(x => x.Option == _comic.Publisher) ?? _publishers.Single(x => x.IsEmpty);
      set
      {
        if (_comic.Publisher != value.Option)
        {
          _comic.Publisher = value.Option;
          OnPropertyChanged(nameof(Publisher));
          IsDirty = true;
        }
      }
    }

    public IOptionItemViewModel<Country> Country
    {
      get => _countries.FirstOrDefault(x => x.Option == _comic.Country) ?? _countries.Single(x => x.IsEmpty);
      set
      {
        if (_comic.Country != value.Option)
        {
          _comic.Country = value.Option;
          OnPropertyChanged(nameof(Country));
          IsDirty = true;
        }
      }
    }

    public IOptionItemViewModel<Language> Language
    {
      get => _languages.FirstOrDefault(x => x.Option == _comic.Language) ?? _languages.Single(x => x.IsEmpty);
      set
      {
        if (_comic.Language != value.Option)
        {
          _comic.Language = value.Option;
          OnPropertyChanged(nameof(Language));
          IsDirty = true;
        }
      }
    }

    public int ImagesCount => _comic.ImagesAsString.Count;

    public bool IsDirty
    {
      get => _isDirty;
      set
      {
        if (value == true)
        {
          // Whenever we try to set the IsDirty flag, we know that a change happened and we can update the change date
          _comic.ModifiedDate = DateTime.Now;
        }

        if (_isDirty != value)
        {
          _isDirty = value;
          OnPropertyChanged(nameof(IsDirty));
        }
      }
    }

    #endregion

    #region Commands

    #region Edit Comic

    public ICommand EditComicCommand => _editComicCommand ??= new ActionCommand(EditComic);

    private void EditComic()
    {
      var vm = new EditComicViewModel(_comic, _libraryName);
      var view = ViewFactory.Instance.CreateView(vm);
      if (view.ShowDialog() == true)
      {
        Refresh();
        IsDirty = true;
      }
    }

    #endregion

    #endregion

    #region Public Methods

    public Comic ToModel()
    {
      return _comic;
    }

    #endregion

    #region Private Methods

    private void Refresh()
    {
      OnPropertyChanged(nameof(Series));
      OnPropertyChanged(nameof(Year));
      OnPropertyChanged(nameof(IssueNumber));
      OnPropertyChanged(nameof(Title));
      OnPropertyChanged(nameof(Condition));
      OnPropertyChanged(nameof(Publisher));
      OnPropertyChanged(nameof(Country));
      OnPropertyChanged(nameof(CollectorsEdition));
      OnPropertyChanged(nameof(LimitedEdition));
      OnPropertyChanged(nameof(ImagesCount));
      OnPropertyChanged(nameof(PurchasePrice));
      OnPropertyChanged(nameof(EstimatedValue));
    }

    #endregion
  }
}
