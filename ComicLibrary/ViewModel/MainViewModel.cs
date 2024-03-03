using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ComicLibrary.Model;
using ComicLibrary.Model.Config;
using ComicLibrary.Model.Entities;
using ES.Tools.Core.MVVM;
using Microsoft.Win32;

namespace ComicLibrary.ViewModel
{
  public class MainViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    #region Fields

    private bool _showLibrariesOverlay;
    private ActiveLibraryViewModel _selectedLibrary;
    private ActionCommand _changeLibraryPathCommand;
    private ActionCommand _editCountriesCommand;
    private ActionCommand _editLanguagesCommand;
    private ActionCommand _editPublishersCommand;
    private ActionCommand _closeCommand;
    private ActionCommand _editLibrariesCommand;
    private ActionCommand _cancelEditLibrariesCommand;
    private ActionCommand<LibraryViewModel> _loadLibraryCommand;
    private ActionCommand _addLibraryCommand;
    private ActionCommand<LibraryViewModel> _changeLibraryImageCommand;
    private ActionCommand<LibraryViewModel> _removeLibraryImageCommand;
    private ActionCommand<LibraryViewModel> _exportLibraryImageCommand;
    private ActionCommand<LibraryViewModel> _removeLibraryCommand;

    #endregion

    #region Constructor

    public MainViewModel()
    {
      ActiveLibraries = [];
      ActiveLibraries.CollectionChanged += ActiveLibraries_CollectionChanged;
      Libraries = [];
      ShowLibrariesOverlay = true;
    }

    #endregion

    #region Properties

    public string LibrariesPath
    {
      get => Settings.Instance.LibrariesPath;
      set
      {
        Settings.Instance.LibrariesPath = value;
        OnPropertyChanged(nameof(LibrariesPath));
      }
    }

    public bool ShowLibrariesOverlay
    {
      get => _showLibrariesOverlay;
      set
      {
        if (_showLibrariesOverlay != value)
        {
          _showLibrariesOverlay = value;
          OnPropertyChanged(nameof(ShowLibrariesOverlay));

          if (_showLibrariesOverlay)
            LoadLibraries();
        }
      }
    }

    public string CurrencySymbol
    {
      get => Settings.Instance.CurrencySymbol;
      set
      {
        Settings.Instance.CurrencySymbol = value;
        OnPropertyChanged(nameof(CurrencySymbol));
      }
    }

    public ObservableCollection<ILibraryViewModel> Libraries { get; }

    public ObservableCollection<ActiveLibraryViewModel> ActiveLibraries { get; }

    public ActiveLibraryViewModel SelectedLibrary
    {
      get => _selectedLibrary;
      set
      {
        if (_selectedLibrary != value)
        {
          _selectedLibrary = value;
          OnPropertyChanged(nameof(SelectedLibrary));
        }
      }
    }

    public bool CopyDataFromSelectedComic
    {
      get => Settings.Instance.CopyDataFromSelectedComic;
      set
      {
        if (Settings.Instance.CopyDataFromSelectedComic != value)
        {
          Settings.Instance.CopyDataFromSelectedComic = value;
          OnPropertyChanged(nameof(CopyDataFromSelectedComic));
          Settings.Save();
        }
      }
    }

    public bool CreateBackupWhenSaving
    {
      get => Settings.Instance.CreateBackupWhenSaving;
      set
      {
        if (Settings.Instance.CreateBackupWhenSaving != value)
        {
          Settings.Instance.CreateBackupWhenSaving = value;
          OnPropertyChanged(nameof(CreateBackupWhenSaving));
          Settings.Save();
        }
      }
    }

    #endregion

    #region Commands

    #region Change Library Path

    public ICommand ChangeLibraryPathCommand => _changeLibraryPathCommand ??= new ActionCommand(ChangeLibraryPath);

    private void ChangeLibraryPath()
    {
      if (ActiveLibraries.Count > 0)
      {
        MessageBox.Show(Properties.Resources.CloseLibrariesToChangePathMessage, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      using var dialog = new System.Windows.Forms.FolderBrowserDialog
      {
        Description = Properties.Resources.SelectAFolderMessage,
        UseDescriptionForTitle = true,
        SelectedPath = LibrariesPath,
        ShowNewFolderButton = true
      };

      if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && dialog.SelectedPath != LibrariesPath)
      {
        var result = MessageBox.Show(Properties.Resources.MoveXMLFilesQuestion, Properties.Resources.Question, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        if (result == MessageBoxResult.Cancel)
          return;

        else if (result == MessageBoxResult.Yes)
        {
          var directory = new DirectoryInfo(LibrariesPath);

          if (directory.Exists)
          {
            foreach (var file in directory.GetFiles("*.xml"))
            {
              file.MoveTo(Path.Combine(dialog.SelectedPath, file.Name));
            }
          }
        }

        LibrariesPath = dialog.SelectedPath;
        Settings.Save();
      }
    }

    #endregion

    #region Edit Countries

    public ICommand EditCountriesCommand => _editCountriesCommand ??= new ActionCommand(EditCountries);

    private void EditCountries()
    {
      if (ActiveLibraries.Count > 0)
      {
        MessageBox.Show(Properties.Resources.CloseLibrariesToModifyOptionsMessage, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      var optionsVM = new EditOptionsViewModel(Globals.Instance.Countries.OrderBy(x => x.Name), typeof(Country), Properties.Resources.Countries, Properties.Resources.Country);
      var view = ViewFactory.Instance.CreateView(optionsVM);
      if (view.ShowDialog() == true)
      {
        Globals.Instance.Countries = new HashSet<Country>(optionsVM.Options.OfType<Country>());
        Globals.Save();
      }
    }

    #endregion

    #region Edit Languages

    public ICommand EditLanguagesCommand => _editLanguagesCommand ??= new ActionCommand(EditLanguages);

    private void EditLanguages()
    {
      if (ActiveLibraries.Count > 0)
      {
        MessageBox.Show(Properties.Resources.CloseLibrariesToModifyOptionsMessage, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      var optionsVM = new EditOptionsViewModel(Globals.Instance.Languages.OrderBy(x => x.Name), typeof(Language), Properties.Resources.Languages, Properties.Resources.Language);
      var view = ViewFactory.Instance.CreateView(optionsVM);
      if (view.ShowDialog() == true)
      {
        Globals.Instance.Languages = new HashSet<Language>(optionsVM.Options.OfType<Language>());
        Globals.Save();
      }
    }

    #endregion

    #region Edit Publishers

    public ICommand EditPublishersCommand => _editPublishersCommand ??= new ActionCommand(EditPublishers);

    private void EditPublishers()
    {
      if (ActiveLibraries.Count > 0)
      {
        MessageBox.Show(Properties.Resources.CloseLibrariesToModifyOptionsMessage, Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      var optionsVM = new EditOptionsViewModel(Globals.Instance.Publishers.OrderBy(x => x.Name), typeof(Publisher), Properties.Resources.Publishers, Properties.Resources.Publisher);
      var view = ViewFactory.Instance.CreateView(optionsVM);
      if (view.ShowDialog() == true)
      {
        Globals.Instance.Publishers = new HashSet<Publisher>(optionsVM.Options.OfType<Publisher>());
        Globals.Save();
      }
    }

    #endregion

    #region Close

    public ICommand CloseCommand => _closeCommand ??= new ActionCommand(Close, CanClose);

    private bool CanClose()
    {
      return true;
    }

    private void Close()
    {
      Application.Current.MainWindow.Close();
    }

    #endregion

    #region Edit Libraries

    public ICommand EditLibrariesCommand => _editLibrariesCommand ??= new ActionCommand(EditLibraries, CanEditLibraries);

    private bool CanEditLibraries()
    {
      return true;
    }

    private void EditLibraries()
    {
      ShowLibrariesOverlay = true;
    }

    #endregion

    #region Cancel Edit Libraries

    public ICommand CancelEditLibrariesCommand => _cancelEditLibrariesCommand ??= new ActionCommand(CancelEditLibraries, CanCancelEditLibraries);

    private bool CanCancelEditLibraries()
    {
      return true;
    }

    private void CancelEditLibraries()
    {
      ShowLibrariesOverlay = false;
    }

    #endregion

    #region Load Library

    public ICommand LoadLibraryCommand => _loadLibraryCommand ??= new ActionCommand<LibraryViewModel>(LoadLibrary, CanLoadLibrary);

    private bool CanLoadLibrary(LibraryViewModel libraryVM)
    {
      return ShowLibrariesOverlay;
    }

    private void LoadLibrary(LibraryViewModel libraryVM)
    {
      // If the library is already open, just switch to it
      foreach (var activeLibrary in ActiveLibraries)
      {
        if (activeLibrary.Name == libraryVM.Name && activeLibrary.Path == libraryVM.GetFilePath())
        {
          SelectedLibrary = activeLibrary;
          ShowLibrariesOverlay = false;
          return;
        }
      }

      var path = libraryVM.GetFilePath();
      var newLibrary = new ActiveLibraryViewModel(FileHelper.LoadActiveLibrary(path), libraryVM);
      ActiveLibraries.Add(newLibrary);
      SelectedLibrary = newLibrary;
      ShowLibrariesOverlay = false;
    }

    #endregion

    #region Add Library

    public ICommand AddLibraryCommand => _addLibraryCommand ??= new ActionCommand(AddLibrary);

    private void AddLibrary()
    {
      bool libraryNameOK(string name) => !string.IsNullOrWhiteSpace(name) && !Libraries.Any(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
      var vm = new GetNameViewModel(Properties.Resources.EnterNameOfLibrary, Properties.Resources.Name, null, libraryNameOK);
      var view = ViewFactory.Instance.CreateView(vm);

      if (view.ShowDialog() == true && !string.IsNullOrWhiteSpace(vm.Name))
      {

        var newLibrary = new Library
        {
          Name = vm.Name,
          FileName = Library.GenerateFileName(vm.Name)
        };

        var newLibraryVM = new LibraryViewModel(newLibrary);
        Libraries.Insert(Libraries.Count - 1, newLibraryVM);
        SaveLibraries();
      }
    }

    #endregion

    #region Change Library Image

    public ICommand ChangeLibraryImageCommand => _changeLibraryImageCommand ??= new ActionCommand<LibraryViewModel>(ChangeLibraryImage);

    private void ChangeLibraryImage(LibraryViewModel vm)
    {
      var dialog = new OpenFileDialog
      {
        Filter = $"{Properties.Resources.JpegFiles}|*.jpg|{Properties.Resources.PngFiles}|*.png",
        CheckFileExists = true
      };

      if (dialog.ShowDialog() == true && File.Exists(dialog.FileName))
      {
        vm.LoadImage(dialog.FileName);
        SaveLibraries();
      }
    }

    #endregion

    #region Remove Library Image

    public ICommand RemoveLibraryImageCommand => _removeLibraryImageCommand ??= new ActionCommand<LibraryViewModel>(RemoveLibraryImage);

    private void RemoveLibraryImage(LibraryViewModel vm)
    {
      vm.ClearImage();
    }

    #endregion

    #region Export Library Image

    public ICommand ExportLibraryImageCommand => _exportLibraryImageCommand ??= new ActionCommand<LibraryViewModel>(ExportLibraryImage, CanExportLibraryImage);

    private void ExportLibraryImage(LibraryViewModel vm)
    {
      var dialog = new SaveFileDialog
      {
        Filter = $"{Properties.Resources.JpegFiles}|*.jpg|{Properties.Resources.PngFiles}|*.png",
        FileName = FileHelper.GetValidFileName(Path.ChangeExtension(vm.Name, "jpg"))
      };

      if (dialog.ShowDialog() == true)
      {
        var filePath = Path.Combine(Path.GetDirectoryName(dialog.FileName), FileHelper.GetValidFileName(Path.GetFileName(dialog.FileName)));
        ImageHelpers.SaveImage((BitmapSource)vm.ComicImage.Image, filePath, Path.GetExtension(dialog.FileName) == ".png");
      }
    }

    private static bool CanExportLibraryImage(LibraryViewModel vm)
    {
      return vm?.ComicImage?.Image != null;
    }

    #endregion

    #region Remove Library

    public ICommand RemoveLibraryCommand => _removeLibraryCommand ??= new ActionCommand<LibraryViewModel>(RemoveLibrary);

    private void RemoveLibrary(LibraryViewModel vm)
    {
      if (MessageBox.Show(Properties.Resources.RemoveLibraryQuestion, Properties.Resources.Warning, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
      {
        Libraries.Remove(vm);
        var activeLibrary = ActiveLibraries.FirstOrDefault(x => x.Name == vm.Name);

        if (activeLibrary != null)
        {
          ActiveLibraries.Remove(activeLibrary);
          if (Settings.Instance.CreateBackupWhenSaving)
          {
            var backupPath = activeLibrary.Path + ".bak";
            File.Move(activeLibrary.Path, backupPath, true);
          }
        }
      }
    }

    #endregion

    #endregion

    #region Public Methods

    public bool OnClosing()
    {
      if (ActiveLibraries.Any(x => x.IsDirty))
      {
        var result = MessageBox.Show(Properties.Resources.SaveChangesQuestion, Properties.Resources.Question, MessageBoxButton.YesNoCancel);

        if (result == MessageBoxResult.Cancel)
          return false;

        if (result == MessageBoxResult.Yes)
        {
          Parallel.ForEach(ActiveLibraries.Where(x => x.IsDirty), library =>
          {
            var model = library.ToModel();
            FileHelper.SaveActiveLibrary(model, library.Path);
          });
        }
      }

      // Save the updated comic counter
      SaveLibraries();

      return true;
    }

    #endregion

    #region Private Methods

    private void LoadLibraries()
    {
      var libraries = FileHelper.LoadLibraries();

      // Add missing libraries
      foreach (var storedlibrary in libraries)
      {
        if (!Libraries.OfType<LibraryViewModel>().Any(x => Equals(x.ToModel(), storedlibrary)))
        {
          Libraries.Add(new LibraryViewModel(storedlibrary));
        }
      }

      // Remove libraries not stored anymore
      foreach (var currentLibrary in Libraries.OfType<LibraryViewModel>().ToArray())
      {
        if (!libraries.Any(x => Equals(x, currentLibrary.ToModel())))
        {
          Libraries.Remove(currentLibrary);
        }
      }

      var addLibrary = Libraries.OfType<AddLibraryViewModel>().FirstOrDefault() ?? new AddLibraryViewModel();

      // Make the entry to create a library the last entry
      Libraries.Remove(addLibrary);
      Libraries.Add(addLibrary);
    }

    private void SaveLibraries()
    {
      FileHelper.SaveLibraries(Libraries.OfType<LibraryViewModel>().Select(x => x.ToModel()));
    }

    private void ActiveLibraries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      foreach (var newLibrary in e.NewItems?.OfType<ActiveLibraryViewModel>() ?? [])
      {
        newLibrary.ClosingRequested += Library_ClosingRequested;
      }

      foreach (var oldLibrary in e.OldItems?.OfType<ActiveLibraryViewModel>() ?? [])
      {
        oldLibrary.ClosingRequested -= Library_ClosingRequested;
      }

      if (ActiveLibraries.Count == 0)
        ShowLibrariesOverlay = true;
    }

    private void Library_ClosingRequested(object sender, EventArgs e)
    {
      ActiveLibraries.Remove(sender as ActiveLibraryViewModel);
    }

    #endregion
  }
}
