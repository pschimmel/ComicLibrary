using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ComicLibrary.Model;
using ComicLibrary.Model.Config;
using ComicLibrary.Model.Entities;
using ES.Tools.Core.MVVM;

namespace ComicLibrary.ViewModel
{
  public class ActiveLibraryViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    #region Events

    public event EventHandler ClosingRequested;

    #endregion

    #region Fields

    private readonly LibraryViewModel _libraryTemplate;
    private ComicViewModel _selectedComic;
    private string _searchText;
    private ActionCommand _addComicCommand;
    private ActionCommand _removeComicCommand;
    private ActionCommand _saveLibraryCommand;
    private ActionCommand _clearSearchTextCommand;
    private ActionCommand _moveToLibraryCommand;
    private ActionCommand _renameSeriesCommand;
    private ActionCommand _printReportCommand;
    private ActionCommand _printListCommand;
    private ActionCommand _closeCommand;
    private bool _isDirty;

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

      Languages =
      [
        new EmptyOptionItemViewModel<Language>(),
        .. Globals.Instance.Languages.OrderBy(x => x.Name).Select(x => new OptionItemViewModel<Language>(x))
      ];

      Comics = [];
      Comics.CollectionChanged += Comics_CollectionChanged;

      foreach (var comic in library.Comics)
      {
        Comics.Add(new ComicViewModel(comic, Name, Publishers, Countries, Languages));
      }

      libraryTemplate.ComicCount = Comics.Count;
      _isDirty = false;
    }

    #endregion

    #region Properties

    public string Name => _libraryTemplate.Name;

    public ComicImageViewModel ComicImage => _libraryTemplate.ComicImage;

    public IEnumerable<IOptionItemViewModel<Publisher>> Publishers { get; }

    public IEnumerable<IOptionItemViewModel<Country>> Countries { get; }

    public IEnumerable<IOptionItemViewModel<Language>> Languages { get; }

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

    public bool IsDirty => _isDirty || Comics.Any(x => x.IsDirty);

    public double TotalPrice
    {
      get
      {
        var view = Comics.GetView();
        return view.OfType<ComicViewModel>()
                   .Sum(x => x.PurchasePrice ?? 0);
      }
    }

    public double TotalValue
    {
      get
      {
        var view = Comics.GetView();
        return view.OfType<ComicViewModel>()
                   .Sum(x => x.EstimatedValue ?? 0);
      }
    }

    #endregion

    #region Commands

    #region Add Comic

    public ICommand AddComicCommand => _addComicCommand ??= new ActionCommand(AddComic);

    private void AddComic()
    {
      var comic = new Comic(SelectedComic?.ToModel());
      var comicVM = new ComicViewModel(comic, Name, Publishers, Countries, Languages);

      if (SelectedComic != null)
      {
        var view = Comics.GetView();
        int index = view.OfType<ComicViewModel>().ToList().IndexOf(SelectedComic);

        if (index + 1 < Comics.Count - 1)
        {
          Comics.Insert(index, comicVM);
        }
        else
        {
          Comics.Add(comicVM);
        }
      }
      else
      {
        Comics.Add(comicVM);
      }

      SelectedComic = comicVM;
      _libraryTemplate.ComicCount = Comics.Count;
    }

    #endregion

    #region Remove Comic

    public ICommand RemoveComicCommand => _removeComicCommand ??= new ActionCommand(RemoveComic, CanRemoveComic);

    private void RemoveComic()
    {
      if (MessageBox.Show(Properties.Resources.RemoveComicQuestion, Properties.Resources.Question, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
      {
        Comics.Remove(SelectedComic);
        SelectedComic = null;
        _libraryTemplate.ComicCount = Comics.Count;
      }
    }

    private bool CanRemoveComic()
    {
      return SelectedComic != null;
    }

    #endregion

    #region Save Library 

    public ICommand SaveLibraryCommand => _saveLibraryCommand ??= new ActionCommand(SaveLibrary, CanSaveLibrary);

    private void SaveLibrary()
    {
      FileHelper.SaveActiveLibrary(ToModel(), Path);

      foreach (var comic in Comics)
      {
        comic.IsDirty = false;
      }

      _isDirty = false;
      OnPropertyChanged(nameof(IsDirty));
    }

    private bool CanSaveLibrary()
    {
      return IsDirty;
    }

    #endregion

    #region Clear Search Text

    public ICommand ClearSearchTextCommand => _clearSearchTextCommand ??= new ActionCommand(ClearSearchText);

    private void ClearSearchText()
    {
      SearchText = "";
    }

    #endregion

    #region Move to Library

    public ICommand MoveToLibraryCommand => _moveToLibraryCommand ??= new ActionCommand(MoveToLibrary, CanMoveToLibrary);

    private void MoveToLibrary()
    {
      var libraries = FileHelper.LoadLibraries();
      var vm = new MoveToLibraryViewModel(libraries.Where(x => x.Name != _libraryTemplate.Name));
      var view = ViewFactory.Instance.CreateView(vm);
      if (view.ShowDialog() == true && vm.SelectedLibrary != null)
      {
        var targetLibrary = FileHelper.LoadActiveLibrary(System.IO.Path.Combine(Settings.Instance.LibrariesPath, vm.SelectedLibrary.FileName));
        var newComic = SelectedComic.ToModel().Copy();
        targetLibrary.Comics.Add(newComic);
        vm.SelectedLibrary.ComicCount++;
        Comics.Remove(SelectedComic);
        SelectedComic = null;
        _libraryTemplate.ComicCount--;
        FileHelper.SaveActiveLibrary(targetLibrary, System.IO.Path.Combine(Settings.Instance.LibrariesPath, vm.SelectedLibrary.FileName));
        FileHelper.SaveLibraries(libraries); // This is to update the comic count
      }
    }

    private bool CanMoveToLibrary()
    {
      return SelectedComic != null;
    }

    #endregion

    #region Rename Series

    public ICommand RenameSeriesCommand => _renameSeriesCommand ??= new ActionCommand(RenameSeries, CanRenameSeries);

    private void RenameSeries()
    {
      var libraries = FileHelper.LoadLibraries();
      var vm = new GetNameViewModel(Properties.Resources.RenameSeries, Properties.Resources.Series, SelectedComic.Series, s => !string.IsNullOrWhiteSpace(s));
      var view = ViewFactory.Instance.CreateView(vm);

      if (view.ShowDialog() == true && !string.IsNullOrWhiteSpace(vm.Name))
      {
        var oldName = SelectedComic.Series;
        foreach (var c in Comics.ToList())
        {
          if (string.Equals(oldName, c.Series, StringComparison.InvariantCultureIgnoreCase))
          {
            c.Series = vm.Name;
          }
        }
      }
    }

    private bool CanRenameSeries()
    {
      return SelectedComic != null;
    }

    #endregion

    #region Print Report

    public ICommand PrintReportCommand => _printReportCommand ??= new ActionCommand(PrintReport);

    private void PrintReport()
    {
      var vm = new PrintActiveLibraryViewModel(this, PrintActiveLibraryViewModel.ReportType.Report);
      var view = ViewFactory.Instance.CreateView(vm);
      view.ShowDialog();
    }

    #endregion

    #region Print List

    public ICommand PrintListCommand => _printListCommand ??= new ActionCommand(PrintList);

    private void PrintList()
    {
      var vm = new PrintActiveLibraryViewModel(this, PrintActiveLibraryViewModel.ReportType.List);
      var view = ViewFactory.Instance.CreateView(vm);
      view.ShowDialog();
    }

    #endregion

    #region Close Library

    public ICommand CloseCommand => _closeCommand ??= new ActionCommand(Close, () => CanClose);

    private void Close()
    {
      if (IsDirty)
      {
        var result = MessageBox.Show(Properties.Resources.SaveChangesQuestion, Properties.Resources.Question, MessageBoxButton.YesNoCancel);

        if (result == MessageBoxResult.Cancel)
          return;

        if (result == MessageBoxResult.Yes)
        {
          var model = ToModel();
          FileHelper.SaveActiveLibrary(model, Path);
        }
      }

      ClosingRequested?.Invoke(this, EventArgs.Empty);
    }

    public static bool CanClose => true; // Needs to be public for AvalonDock binding

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

    private void Comics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        foreach (var oldItem in Comics)
        {
          oldItem.PropertyChanged += Comic_PropertyChanged;
        }
      }
      else
      {
        if (e.NewItems != null)
        {
          foreach (var newItem in e.NewItems.OfType<ComicViewModel>())
          {
            newItem.PropertyChanged += Comic_PropertyChanged;
          }
        }
        if (e.OldItems != null)
        {
          foreach (var oldItem in e.OldItems.OfType<ComicViewModel>())
          {
            oldItem.PropertyChanged -= Comic_PropertyChanged;
          }
        }
      }

      _isDirty = true;
      OnPropertyChanged(nameof(IsDirty));
      OnPropertyChanged(nameof(TotalPrice));
      OnPropertyChanged(nameof(TotalValue));
    }

    private void Comic_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case nameof(ComicViewModel.IsDirty):
          OnPropertyChanged(nameof(IsDirty));
          break;
        case nameof(ComicViewModel.PurchasePrice):
          OnPropertyChanged(nameof(TotalPrice));
          break;
        case nameof(ComicViewModel.EstimatedValue):
          OnPropertyChanged(nameof(TotalValue));
          break;
      }
    }

    private void ApplyFilter()
    {
      var view = Comics.GetView();
      view.Filter = string.IsNullOrWhiteSpace(SearchText) || SearchText.Length < 3
                    ? null
                    : x => x is ComicViewModel c && MatchesFilter(c, SearchText);
      OnPropertyChanged(nameof(TotalPrice));
      OnPropertyChanged(nameof(TotalValue));
    }

    private static bool MatchesFilter(ComicViewModel c, string searchText)
    {
      return (!string.IsNullOrWhiteSpace(c.Series) && c.Series.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)) ||
             (!string.IsNullOrWhiteSpace(c.Title)) && c.Title.Contains(searchText, StringComparison.InvariantCultureIgnoreCase);
    }

    #endregion
  }
}