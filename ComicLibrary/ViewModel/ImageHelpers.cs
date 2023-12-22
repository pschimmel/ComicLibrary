using System.IO;
using System.Windows.Media.Imaging;

namespace ComicLibrary.ViewModel
{
  internal static class ImageHelpers
  {
    public static byte[] ToArray(BitmapImage image, bool isPng)
    {
      byte[] data = null;

      if (image != null)
      {
        BitmapEncoder encoder = isPng ? new PngBitmapEncoder() : new JpegBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(image));

        using var ms = new MemoryStream();
        encoder.Save(ms);
        data = ms.ToArray();
      }

      return data;
    }

    public static BitmapImage ToImage(byte[] array)
    {
      if (array == null || array.Length == 0)
        return null;
      using var ms = new MemoryStream(array);
      var image = new BitmapImage();
      image.BeginInit();
      image.CacheOption = BitmapCacheOption.OnLoad;
      image.StreamSource = ms;
      image.EndInit();
      return image;
    }
  }
}