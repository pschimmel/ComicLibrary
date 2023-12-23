using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ComicLibrary.Model;
using ComicLibrary.Model.Entities;
using ES.Tools.Core.MVVM;

namespace ComicLibrary.ViewModel
{
  public class ActiveLibraryViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    #region Fields

    private readonly LibraryViewModel _libraryTemplate;
    private ComicViewModel _selectedComic;
    private string _searchText;
    private ActionCommand _addComicCommand;
    private ActionCommand _removeComicCommand;
    private ActionCommand _saveLibraryCommand;
    private ActionCommand _clearSearchTextCommand;

    #endregion

    #region Constructor

    public ActiveLibraryViewModel(ActiveLibrary library, LibraryViewModel libraryTemplate)
    {
      _libraryTemplate = libraryTemplate;
      Conditions = Grade.Grades;
      Publishers =
      [
        new EmptyOptionItemViewModel<Publisher>(),
        .. Globals.Instance.Publishers.OrderBy(x => x.Name).Select(x => new OptionItemViewModel<Publisher>(x))
      ];
      Countries =
      [
        new EmptyOptionItemViewModel<Country>(),
        .. Globals.Instance.Countries.OrderBy(x => x.Name).Select(x => new OptionItemViewModel<Country>(x))
      ];
      Comics = new ObservableCollection<ComicViewModel>(library.Comics.Select(x => new ComicViewModel(x, Name, Publishers, Countries)));
      libraryTemplate.ComicCount = Comics.Count;
    }

    #endregion

    #region Properties

    public string Name => _libraryTemplate.Name;

    public IEnumerable<IOptionItemViewModel<Publisher>> Publishers { get; }

    public IEnumerable<IOptionItemViewModel<Country>> Countries { get; }

    public IEnumerable<Grade> Conditions { get; }

    public ObservableCollection<ComicViewModel> Comics { get; }

    public ComicViewModel SelectedComic
    {
      get => _selectedComic;
      set
      {
        if (_selectedComic != value)
        {
          _selectedComic = value;
          OnPropertyChanged(nameof(SelectedComic));
          _removeComicCommand.RaiseCanExecuteChanged();
        }
      }
    }

    public string Path => _libraryTemplate.GetFilePath();

    public string SearchText
    {
      get => _searchText;
      set
      {
        if (_searchText != value)
        {
          _searchText = value;
          OnPropertyChanged(nameof(SearchText));
          if (!string.IsNullOrWhiteSpace(SearchText))
            SelectedComic = null;
          ApplyFilter();
        }
      }
    }

    #endregion

    #region Commands

    #region Add Comic

    public ICommand AddComicCommand => _addComicCommand ??= new ActionCommand(AddComic);

    private void AddComic()
    {
      var comic = new Comic(SelectedComic?.ToModel());
      var comicVM = new ComicViewModel(comic, Name, Publishers, Countries);
      Comics.Add(comicVM);
      SelectedComic = comicVM;
      _libraryTemplate.ComicCount = Comics.Count;
    }

    #endregion

    #region Remove Comic

    public ICommand RemoveComicCommand => _removeComicCommand ??= new ActionCommand(RemoveComic, CanRemoveComic);

    private void RemoveComic()
    {
      Comics.Remove(SelectedComic);
      SelectedComic = null;
      _libraryTemplate.ComicCount = Comics.Count;
    }

    private bool CanRemoveComic()
    {
      return SelectedComic != null;
    }

    #endregion

    #region Save Library 

    public ICommand SaveLibraryCommand => _saveLibraryCommand ??= new ActionCommand(SaveLibrary);

    private void SaveLibrary()
    {
      FileHelper.SaveActiveLibrary(ToModel(), Path);
    }

    #endregion

    #region Clear Search Text

    public ICommand ClearSearchTextCommand => _clearSearchTextCommand ??= new ActionCommand(ClearSearchText);

    private void ClearSearchText()
    {
      SearchText = "";
    }

    #endregion

    #endregion

    #region Public Methods

    public ActiveLibrary ToModel()
    {
      return new ActiveLibrary
      {
        Comics = new HashSet<Comic>(Comics.Select(x => x.ToModel())),
        SaveDate = DateTime.Now
      };
    }

    #endregion

    #region Private Methods

    private void ApplyFilter()
    {
      var view = Comics.GetView();
      view.Filter = string.IsNullOrWhiteSpace(SearchText) || SearchText.Length > 2
                    ? null
                    : x =>
                    {
                      if (x is not ComicViewModel c)
                        return false;

                      if (!string.IsNullOrWhiteSpace(c.Series))
                        return c.Series.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);

                      if (!string.IsNullOrWhiteSpace(c.Title))
                        return c.Title.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);

                      return false;
                    };
    }

    #endregion
  }
}