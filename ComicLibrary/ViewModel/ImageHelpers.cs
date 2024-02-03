using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ComicLibrary.ViewModel
{
  internal static class ImageHelpers
  {
    public static byte[] LoadImage(string filePath)
    {
      try
      {
        bool isPng = Path.GetExtension(filePath).Equals(".png", StringComparison.CurrentCultureIgnoreCase);
        var bitmap = new BitmapImage(new Uri(filePath, UriKind.Absolute));
        return ImageToByteArray(bitmap, isPng);
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format(Properties.Resources.CannotLoadFileMessage, filePath) + Environment.NewLine + ex.Message);
      }
      return null;
    }

    public static void SaveImage(BitmapSource image, string filePath, bool isPng)
    {
      try
      {
        using var fileStream = new FileStream(filePath, FileMode.Create);
        BitmapEncoder encoder = isPng ? new PngBitmapEncoder() : new JpegBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(image));
        encoder.Save(fileStream);
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format(Properties.Resources.CannotLoadFileMessage, filePath) + Environment.NewLine + ex.Message);
      }
    }

    public static byte[] ImageToByteArray(BitmapImage image, bool isPng)
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

    public static BitmapImage ByteArrayToImage(byte[] array)
    {
      if (array == null || array.Length == 0)
        return null;
      using var ms = new MemoryStream(array);
      var image = new BitmapImage();
      image.BeginInit();
      image.CacheOption = BitmapCacheOption.OnLoad;
      image.StreamSource = ms;
      image.EndInit();
      image.Freeze();
      return image;
    }
  }
}