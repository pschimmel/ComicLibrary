using System.IO;
using System.Windows.Media.Imaging;
using ComicLibrary.Model.Config;
using ComicLibrary.Model.Entities;

namespace ComicLibrary.ViewModel
{
  public class LibraryViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    private readonly Library _library;
    private ComicImageViewModel _comicImage;

    public LibraryViewModel(Library library)
    {
      _library = library;
      _comicImage = new ComicImageViewModel(_library.ImageAsString);
    }

    public string Name => _library.Name;

    public int ComicCount
    {
      get => _library.ComicCount;
      set => _library.ComicCount = value;
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
        bool isPng = Path.GetExtension(filePath).Equals(".png", StringComparison.CurrentCultureIgnoreCase);
        var bitmap = new BitmapImage(new Uri(filePath, UriKind.Absolute));
        ComicImage = new ComicImageViewModel(ImageHelpers.ToArray(bitmap, isPng));
      }
    }
  }
}