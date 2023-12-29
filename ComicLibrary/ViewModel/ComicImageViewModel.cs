using System.Windows.Media;

namespace ComicLibrary.ViewModel
{
  public class ComicImageViewModel
  {
    private readonly byte[] _imageAsBytes;

    public ComicImageViewModel(string imageAsString)
    {
      _imageAsBytes = imageAsString == null ? null : Convert.FromBase64String(imageAsString);
    }

    public ComicImageViewModel(byte[] imageAsBytes)
    {
      _imageAsBytes = imageAsBytes;
    }

    public ImageSource Image => ImageHelpers.ByteArrayToImage(_imageAsBytes);

    public override string ToString()
    {
      return _imageAsBytes == null ? null : Convert.ToBase64String(_imageAsBytes);
    }
  }
}
