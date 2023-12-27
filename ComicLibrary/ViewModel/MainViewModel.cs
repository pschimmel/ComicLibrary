using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
    private string _libraryName;
    private ActionCommand _changeLibraryCommand;
    private ActionCommand _editCountriesCommand;
    private ActionCommand _editPublishersCommand;
    private ActionCommand _closeCommand;
    private ActionCommand _editLibrariesCommand;
    private ActionCommand _cancelEditLibrariesCommand;
    private ActionCommand<LibraryViewModel> _loadLibraryCommand;
    private ActionCommand<ActiveLibraryViewModel> _closeLibraryCommand;
    private ActionCommand _addLibraryCommand;
    private ActionCommand<LibraryViewModel> _changeLibraryImageCommand;
    private ActionCommand<LibraryViewModel> _removeLibraryCommand;

    #endregion

    #region Constructor

    public MainViewModel()
    {
      ActiveLibraries = [];
      Libraries = [];
      _showLibrariesOverlay = true;
      LoadLibraries();
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

          _addLibraryCommand.RaiseCanExecuteChanged();
        }
      }
    }

    public string LibraryName
    {
      get => _libraryName;
      set
      {
        if (_showLibrariesOverlay && _libraryName != value)
        {
          _libraryName = value;
          OnPropertyChanged(nameof(LibraryName));
          _addLibraryCommand.RaiseCanExecuteChanged();
        }
      }
    }

    public ObservableCollection<LibraryViewModel> Libraries { get; }

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

    #endregion

    #region Commands

    #region Change Library

    public ICommand ChangeLibraryCommand => _changeLibraryCommand ??= new ActionCommand(ChangeLibrary);

    private void ChangeLibrary()
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

    #region Close Library

    public ICommand CloseLibraryCommand => _closeLibraryCommand ??= new ActionCommand<ActiveLibraryViewModel>(CloseLibrary, CanCloseCommand);

    private void CloseLibrary(ActiveLibraryViewModel libraryVM)
    {
      var result = MessageBox.Show(Properties.Resources.SaveChangesQuestion, Properties.Resources.Question, MessageBoxButton.YesNoCancel);

      if (result == MessageBoxResult.Cancel)
        return;

      if (result == MessageBoxResult.Yes)
      {
        var model = libraryVM.ToModel();
        FileHelper.SaveActiveLibrary(model, libraryVM.Path);
      }

      ActiveLibraries.Remove(libraryVM);
    }

    private bool CanCloseCommand(ActiveLibraryViewModel libraryVM)
    {
      return libraryVM != null;
    }

    #endregion

    #region Add Library

    public ICommand AddLibraryCommand => _addLibraryCommand ??= new ActionCommand(AddLibrary, CanAddLibrary);

    private bool CanAddLibrary()
    {
      return ShowLibrariesOverlay && !Libraries.Any(x => string.Equals(x.Name, LibraryName, StringComparison.InvariantCultureIgnoreCase));
    }

    private void AddLibrary()
    {
      var newLibrary = new Library
      {
        Name = LibraryName,
        FileName = Library.GenerateFileName(LibraryName)
      };

      var newLibraryVM = new LibraryViewModel(newLibrary);
      Libraries.Add(newLibraryVM);
      SaveLibraries();
    }

    #endregion

    #region Change Library Image

    public ICommand ChangeLibraryImageCommand => _changeLibraryImageCommand ??= new ActionCommand<LibraryViewModel>(ChangeLibraryImage);

    private void ChangeLibraryImage(LibraryViewModel vm)
    {
      var dialog = new OpenFileDialog
      {
        Filter = $"{Properties.Resources.JpegFiles}|*.jpg|{Properties.Resources.PngFiles}|*.png"
      };

      if (dialog.ShowDialog() == true && File.Exists(dialog.FileName))
      {
        vm.LoadImage(dialog.FileName);
        SaveLibraries();
      }
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
          var backupPath = activeLibrary.Path + ".bak";
          File.Move(activeLibrary.Path, backupPath, true);
        }
      }
    }

    #endregion

    #endregion

    #region Public Methods

    public bool OnClosing()
    {
      if (ActiveLibraries.Any())
      {
        var result = MessageBox.Show(Properties.Resources.SaveChangesQuestion, Properties.Resources.Question, MessageBoxButton.YesNoCancel);

        if (result == MessageBoxResult.Cancel)
          return false;

        if (result == MessageBoxResult.Yes)
        {
          Parallel.ForEach(ActiveLibraries, library =>
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
        if (!Libraries.Any(x => Equals(x.ToModel(), storedlibrary)))
        {
          Libraries.Add(new LibraryViewModel(storedlibrary));
        }
      }

      // Remove libraries not stored anymore
      foreach (var currentLibrary in Libraries.ToArray())
      {
        if (!libraries.Any(x => Equals(x, currentLibrary.ToModel())))
        {
          Libraries.Remove(currentLibrary);
        }
      }
    }

    private void SaveLibraries()
    {
      FileHelper.SaveLibraries(Libraries.Select(x => x.ToModel()));
    }

    #endregion
  }
}
