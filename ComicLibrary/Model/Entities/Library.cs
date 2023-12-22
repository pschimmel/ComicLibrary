using System.IO;
using ComicLibrary.Model.Config;

namespace ComicLibrary.Model.Entities
{
  public class Library : IComparable, IComparable<Library>
  {
    public string Name { get; set; }

    public string FileName { get; set; }

    public string ImageAsString { get; set; }

    internal static string GenerateFileName(string libraryName)
    {
      string filePath = Path.Combine(Settings.Instance.LibrariesPath, Path.ChangeExtension(libraryName, "xml"));
      int i = 0;

      while (File.Exists(filePath))
      {
        filePath = Path.Combine(Settings.Instance.LibrariesPath, Path.ChangeExtension(libraryName + "_" + i, "xml"));
        i++;
      }

      return Path.GetFileName(filePath);
    }

    public int CompareTo(object obj)
    {
      return obj is not Library other ? -1 : CompareTo(other);
    }

    public int CompareTo(Library other)
    {
      return string.Compare(Name, other.Name);
    }
  }
}
