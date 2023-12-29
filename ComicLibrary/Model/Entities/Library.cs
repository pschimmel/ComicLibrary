using System.IO;
using ComicLibrary.Model.Config;

namespace ComicLibrary.Model.Entities
{
  public class Library
  {
    public string Name { get; set; }

    public int ComicCount { get; set; }

    public string FileName { get; set; }

    public string ImageAsString { get; set; }

    internal static string GenerateFileName(string libraryName)
    {
      string fileName = FileHelper.GetValidFileName(libraryName);

      string filePath = Path.Combine(Settings.Instance.LibrariesPath, Path.ChangeExtension(fileName, "xml"));
      int i = 0;

      while (File.Exists(filePath))
      {
        filePath = Path.Combine(Settings.Instance.LibrariesPath, Path.ChangeExtension(fileName + "_" + i, "xml"));
        i++;
      }

      return Path.GetFileName(filePath);
    }

    public override int GetHashCode()
    {
      return new
      {
        Name,
        FileName
      }.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return obj is Library other
        && Equals(Name, other.Name)
        && Equals(FileName, other.FileName);
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
