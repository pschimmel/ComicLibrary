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
    private IEnumerable<IOptionItemViewModel<Publisher>> _publishers;
    private IEnumerable<IOptionItemViewModel<Country>> _countries;
    private IEnumerable<IOptionItemViewModel<Language>> _languages;
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
      _isDirty = false;
    }

    public void SetOptionLists(IEnumerable<IOptionItemViewModel<Publisher>> publishers,
                               IEnumerable<IOptionItemViewModel<Country>> countries,
                               IEnumerable<IOptionItemViewModel<Language>> languages)
    {
      // Preserve the currently selected options where possible. If the current
      // model option is no longer present in the new list, clear it so the UI
      // can fall back to the empty option.
      var previousPublisher = _comic?.Publisher;
      var previousCountry = _comic?.Country;
      var previousLanguage = _comic?.Language;

      _publishers = publishers;
      _countries = countries;
      _languages = languages;

      // If the previous selected publisher is not contained in the new list,
      // clear it so the getter can resolve to the empty option. If it is
      // contained, leave the model value intact to preserve selection.
      if (previousPublisher != null)
      {
        var found = _publishers?.FirstOrDefault(x => x.Option != null && x.Option.ID == previousPublisher.ID);
        if (found == null)
        {
          _comic.Publisher = null;
        }
        else
        {
          // Replace the model's Publisher reference with the canonical instance
          // from the globals/options list so subsequent comparisons work by reference.
          _comic.Publisher = found.Option;
        }
      }

      if (previousCountry != null)
      {
        var found = _countries?.FirstOrDefault(x => x.Option != null && x.Option.ID == previousCountry.ID);
        if (found == null)
        {
          _comic.Country = null;
        }
        else
        {
          _comic.Country = found.Option;
        }
      }

      if (previousLanguage != null)
      {
        var found = _languages?.FirstOrDefault(x => x.Option != null && x.Option.ID == previousLanguage.ID);
        if (found == null)
        {
          _comic.Language = null;
        }
        else
        {
          _comic.Language = found.Option;
        }
      }

      // Refresh option dependent properties so UI updates to new lists
      OnPropertyChanged(nameof(Publisher));
      OnPropertyChanged(nameof(Country));
      OnPropertyChanged(nameof(Language));
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

    public string IssueNumber
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

    public string StorageLocation
    {
      get => _comic.StorageLocation;
      set
      {
        if (_comic.StorageLocation != value)
        {
          _comic.StorageLocation = value;
          OnPropertyChanged(nameof(StorageLocation));
          IsDirty = true;
        }
      }
    }

    public string CoverVariant
    {
      get => _comic.CoverVariant;
      set
      {
        if (_comic.CoverVariant != value)
        {
          _comic.CoverVariant = value;
          OnPropertyChanged(nameof(CoverVariant));
          IsDirty = true;
        }
      }
    }

    public IEnumerable<Grade> Conditions
    {
      get
      {
        var scale = ScaleHelper.Scales.FirstOrDefault(x => x.Name == _comic.Condition.Scale) ?? ScaleHelper.DefaultScale;
        return scale != null ? scale.Grades.OrderBy(x => x.Number) : (IEnumerable<Grade>)null;
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

    public bool GradingCertified => _comic.GradingCertified;

    public bool CollectorsEdition => _comic.CollectorsEdition;

    public bool LimitedEdition => _comic.LimitedEdition;

    public double? PurchasePrice => _comic.PurchasePrice;

    public double? EstimatedValue => _comic.EstimatedValue;

    public string Comment => _comic.Comment;

    public IOptionItemViewModel<Publisher> Publisher
    {
      get => _publishers?.FirstOrDefault(x => x.Option != null && _comic.Publisher != null && x.Option.ID == _comic.Publisher.ID)
             ?? _publishers?.SingleOrDefault(x => x.IsEmpty);
      set
      {
        var newOption = value?.Option;
        if (!Equals(_comic.Publisher, newOption))
        {
          _comic.Publisher = newOption;
          OnPropertyChanged(nameof(Publisher));
          IsDirty = true;
        }
      }
    }

    public IOptionItemViewModel<Country> Country
    {
      get => _countries?.FirstOrDefault(x => x.Option != null && _comic.Country != null && x.Option.ID == _comic.Country.ID)
             ?? _countries?.SingleOrDefault(x => x.IsEmpty);
      set
      {
        var newOption = value?.Option;
        if (!Equals(_comic.Country, newOption))
        {
          _comic.Country = newOption;
          OnPropertyChanged(nameof(Country));
          IsDirty = true;
        }
      }
    }

    public IOptionItemViewModel<Language> Language
    {
      get => _languages?.FirstOrDefault(x => x.Option != null && _comic.Language != null && x.Option.ID == _comic.Language.ID)
             ?? _languages?.SingleOrDefault(x => x.IsEmpty);
      set
      {
        var newOption = value?.Option;
        if (!Equals(_comic.Language, newOption))
        {
          _comic.Language = newOption;
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

    public void Refresh()
    {
      OnPropertyChanged(nameof(Series));
      OnPropertyChanged(nameof(Year));
      OnPropertyChanged(nameof(IssueNumber));
      OnPropertyChanged(nameof(CoverVariant));
      OnPropertyChanged(nameof(Title));
      OnPropertyChanged(nameof(StorageLocation));
      OnPropertyChanged(nameof(Condition));
      OnPropertyChanged(nameof(GradingCertified));
      OnPropertyChanged(nameof(Publisher));
      OnPropertyChanged(nameof(Country));
      OnPropertyChanged(nameof(Comment));
      OnPropertyChanged(nameof(CollectorsEdition));
      OnPropertyChanged(nameof(LimitedEdition));
      OnPropertyChanged(nameof(ImagesCount));
      OnPropertyChanged(nameof(PurchasePrice));
      OnPropertyChanged(nameof(EstimatedValue));
    }

    #endregion
  }
}
