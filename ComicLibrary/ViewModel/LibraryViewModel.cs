using System.Diagnostics;
using System.IO;
using ComicLibrary.Model.Config;
using ComicLibrary.Model.Entities;

namespace ComicLibrary.ViewModel
{
  public class LibraryViewModel : ES.Tools.Core.MVVM.ViewModel, ILibraryViewModel
  {
    #region Fields

    private readonly Library _library;
    private ComicImageViewModel _comicImage;

    #endregion

    #region Constructor

    public LibraryViewModel(Library library)
    {
      _library = library;
      _comicImage = string.IsNullOrWhiteSpace(_library.ImageAsString) ? null : new ComicImageViewModel(_library.ImageAsString);
    }

    #endregion

    #region Properties

    public string Name => _library.Name;

    public int ComicCount
    {
      get => _library.ComicCount;
      set
      {
        if (_library.ComicCount != value)
        {
          _library.ComicCount = value;
          OnPropertyChanged(nameof(ComicCount));
        }
      }
    }

    public ComicImageViewModel ComicImage
    {
      get => _comicImage;
      private set
      {
        _comicImage = value;
        OnPropertyChanged(nameof(ComicImage));
      }
    }

    #endregion

    #region Public Methods

    public string GetFilePath()
    {
      return Path.Combine(Settings.Instance.LibrariesPath, _library.FileName);
    }

    internal Library ToModel()
    {
      _library.ImageAsString = ComicImage?.ToString();
      return _library;
    }

    public void LoadImage(string filePath)
    {
      if (File.Exists(filePath))
      {
        try
        {
          var imageAsByteArray = ImageHelpers.LoadImage(filePath);
          if (imageAsByteArray != null)
          {
            ComicImage = new ComicImageViewModel(imageAsByteArray);
          }
        }
        catch (Exception ex)
        {
          Debug.Fail("Cannot read image: " + ex.Message);
        }
      }
    }

    public void ClearImage()
    {
      ComicImage = null;
    }

    #endregion
  }
}