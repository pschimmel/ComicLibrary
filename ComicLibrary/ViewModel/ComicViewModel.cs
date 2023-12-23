using System.Collections.Generic;
using System.Linq;
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
    private ActionCommand _editComicCommand;

    #endregion

    #region Constructor

    public ComicViewModel(Comic comic,
                          string libraryName,
                          IEnumerable<IOptionItemViewModel<Publisher>> publishers,
                          IEnumerable<IOptionItemViewModel<Country>> countries)
    {
      _libraryName = libraryName;
      _comic = comic;
      _publishers = publishers;
      _countries = countries;
      Series = comic.Series;
      Year = comic.Year;
      IssueNumber = comic.IssueNumber;
      Title = comic.Title;
      Condition = comic.Condition;
      Publisher = _publishers.FirstOrDefault(x => x.Option == comic.Publisher);
      Country = _countries.FirstOrDefault(x => x.Option == comic.Country);
    }

    #endregion

    #region Properties

    public string Series
    {
      get => _comic.Series;
      set => _comic.Series = value;
    }

    public int? Year
    {
      get => _comic.Year;
      set => _comic.Year = value;
    }

    public int? IssueNumber
    {
      get => _comic.IssueNumber;
      set => _comic.IssueNumber = value;
    }

    public string Title
    {
      get => _comic.Title;
      set => _comic.Title = value;
    }

    public Grade Condition
    {
      get => _comic.Condition;
      set => _comic.Condition = value;
    }

    public bool CollectorsEdition => _comic.CollectorsEdition;

    public bool LimitedEdition => _comic.LimitedEdition;

    public IOptionItemViewModel<Publisher> Publisher
    {
      get => _publishers.FirstOrDefault(x => x.Option == _comic.Publisher) ?? _publishers.Single(x => x.IsEmpty);
      set => _comic.Publisher = value.Option;
    }

    public IOptionItemViewModel<Country> Country
    {
      get => _countries.FirstOrDefault(x => x.Option == _comic.Country) ?? _countries.Single(x => x.IsEmpty);
      set => _comic.Country = value.Option;
    }

    public int ImagesCount => _comic.ImagesAsString.Count;

    #endregion

    #region Commands

    #region Edit Comic

    public ICommand EditComicCommand => _editComicCommand ??= new ActionCommand(EditComic);

    private void EditComic()
    {
      var vm = new EditComicViewModel(_comic, _libraryName);
      var view = ViewFactory.Instance.CreateView(vm);
      if (view.ShowDialog() == true)
        Refresh();
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
    }

    #endregion
  }
}
