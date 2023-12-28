﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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

      Comics = [];
      Comics.CollectionChanged += Comics_CollectionChanged;

      foreach (var comic in library.Comics)
      {
        Comics.Add(new ComicViewModel(comic, Name, Publishers, Countries));
      }

      libraryTemplate.ComicCount = Comics.Count;
      _isDirty = false;
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

    public bool IsDirty => _isDirty || Comics.Any(x => x.IsDirty);

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
    }

    private void Comic_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(ComicViewModel.IsDirty))
        OnPropertyChanged(nameof(IsDirty));
    }

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