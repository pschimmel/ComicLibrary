using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using ComicLibrary.Model.Config;
using ComicLibrary.Model.Entities;

namespace ComicLibrary.Model
{
  public static class FileHelper
  {
    #region Constants

    private const string GlobalsKey = "Globals";
    private const string LibrariesKey = "Libraries";
    private const string LibraryKey = "Library";
    private const string PublishersKey = "Publishers";
    private const string PublisherKey = "Publisher";
    private const string CountriesKey = "Countries";
    private const string CountryKey = "Country";
    private const string IDKey = "ID";
    private const string NameKey = "Name";
    private const string TitleKey = "Title";
    private const string FileNameKey = "FileName";
    private const string SeriesKey = "Series";
    private const string ComicsKey = "Comics";
    private const string ComicKey = "Comic";
    private const string YearKey = "Year";
    private const string ImagesKey = "Images";
    private const string ImageKey = "Image";
    private const string ConditionKey = "Condition";
    private const string IssueNumberKey = "IssueNumber";
    private const string LimitedEditionKey = "LimitedEdition";
    private const string CollectorsEditionKey = "CollectorsEdition";
    private const string CommentKey = "Comment";
    private const string SaveDateKey = "SaveDate";
    private const string ComicCountKey = "ComicCount";

    #endregion

    #region Loading

    public static Globals LoadGlobals(string filePath = null)
    {
      filePath ??= Settings.Instance.GlobalsFilePath;

      var globals = new Globals();

      if (!File.Exists(filePath))
        return globals;

      var xml = new XmlDocument();
      xml.Load(filePath);

      if (xml.FirstChild.Name == GlobalsKey)
      {
        var globalsNode = xml.FirstChild;

        foreach (XmlNode globalsChildNode in globalsNode.ChildNodes)
        {
          if (globalsChildNode.Name == CountriesKey)
          {
            var countriesNode = globalsChildNode;
            foreach (XmlNode countryNode in countriesNode.ChildNodes)
            {
              if (countryNode.Name == CountryKey)
              {
                var country = new Country();

                foreach (XmlAttribute countryAttribute in countryNode.Attributes)
                {
                  if (countryAttribute.Name == IDKey)
                  {
                    country.ID = new Guid(countryAttribute.InnerText);
                  }
                  else if (countryAttribute.Name == NameKey)
                  {
                    country.Name = countryAttribute.InnerText;
                  }
                }

                if (!string.IsNullOrWhiteSpace(country.Name) && country.ID != Guid.Empty)
                  globals.Countries.Add(country);
              }
            }
          }
          else if (globalsChildNode.Name == PublishersKey)
          {
            var publishersNode = globalsChildNode;
            foreach (XmlNode publisherNode in publishersNode.ChildNodes)
            {
              if (publisherNode.Name == PublisherKey)
              {
                var publisher = new Publisher();

                foreach (XmlAttribute publisherAttribute in publisherNode.Attributes)
                {
                  if (publisherAttribute.Name == IDKey)
                  {
                    publisher.ID = new Guid(publisherAttribute.InnerText);
                  }
                  else if (publisherAttribute.Name == NameKey)
                  {
                    publisher.Name = publisherAttribute.InnerText;
                  }
                }

                if (!string.IsNullOrWhiteSpace(publisher.Name) && publisher.ID != Guid.Empty)
                  globals.Publishers.Add(publisher);
              }
            }
          }
        }
      }

      return globals;
    }

    public static IEnumerable<Library> LoadLibraries(string filePath = null)
    {
      filePath ??= Settings.Instance.LibrariesFilePath;
      var libraries = new List<Library>();

      if (!File.Exists(filePath))
        return libraries;

      var xml = new XmlDocument();
      xml.Load(filePath);

      if (xml.FirstChild.Name == LibrariesKey)
      {
        var librariesNode = xml.FirstChild;

        foreach (XmlNode libraryNode in librariesNode.ChildNodes)
        {
          if (libraryNode.Name == LibraryKey)
          {
            var library = new Library();

            foreach (XmlAttribute categoryAttribute in libraryNode.Attributes)
            {
              if (categoryAttribute.Name == NameKey)
              {
                library.Name = categoryAttribute.InnerText;
              }
              else if (categoryAttribute.Name == FileNameKey)
              {
                library.FileName = categoryAttribute.InnerText;
              }
              else if (categoryAttribute.Name == ImageKey)
              {
                library.ImageAsString = categoryAttribute.InnerText;
              }
              else if (categoryAttribute.Name == ComicCountKey && int.TryParse(categoryAttribute.InnerText, out int comicCount))
              {
                library.ComicCount = comicCount;
              }
            }

            if (!string.IsNullOrWhiteSpace(library.FileName) && !string.IsNullOrWhiteSpace(library.Name))
              libraries.Add(library);
          }
        }
      }

      return libraries;
    }

    public static ActiveLibrary LoadActiveLibrary(string filePath)
    {
      if (!File.Exists(filePath))
        return new ActiveLibrary();

      var xml = new XmlDocument();
      xml.Load(filePath);
      var library = new ActiveLibrary();

      ReadLibrary(library, xml);
      return library;
    }

    private static void ReadLibrary(ActiveLibrary library, XmlDocument xml)
    {
      if (xml.FirstChild.Name == LibraryKey)
      {
        var libraryNode = xml.FirstChild;
        foreach (XmlAttribute libraryAttribute in libraryNode.Attributes)
        {
          if (libraryAttribute.Name == SaveDateKey)
          {
            library.SaveDate = DateTime.Parse(libraryAttribute.InnerText);
          }
        }

        ReadComics(library, libraryNode);
      }
    }

    private static void ReadComics(ActiveLibrary library, XmlNode libraryNode)
    {
      if (libraryNode.ChildNodes.Count > 0 && libraryNode.ChildNodes[0].Name == ComicsKey)
      {
        var comicsNode = libraryNode.ChildNodes[0];

        foreach (XmlNode comicNode in comicsNode.ChildNodes)
        {
          if (comicNode.Name == ComicKey)
          {
            var comic = new Comic();

            foreach (XmlNode comicChildNode in comicNode.ChildNodes)
            {
              if (comicChildNode.Name == IDKey)
              {
                comic.ID = new Guid(comicChildNode.InnerText);
              }
              else if (comicChildNode.Name == SeriesKey)
              {
                comic.Series = comicChildNode.InnerText;
              }
              else if (comicChildNode.Name == YearKey && int.TryParse(comicChildNode.InnerText, CultureInfo.InvariantCulture, out int year))
              {
                comic.Year = year;
              }
              else if (comicChildNode.Name == ConditionKey && double.TryParse(comicChildNode.InnerText, CultureInfo.InvariantCulture, out double gradingNumber))
              {
                var grading = Grade.Grades.FirstOrDefault(x => x.Number == gradingNumber);

                if (grading != null)
                  comic.Condition = grading;
              }
              else if (comicChildNode.Name == TitleKey)
              {
                comic.Title = comicChildNode.InnerText;
              }
              else if (comicChildNode.Name == IssueNumberKey && int.TryParse(comicChildNode.InnerText, CultureInfo.InvariantCulture, out int issueNumber))
              {
                comic.IssueNumber = issueNumber;
              }
              else if (comicChildNode.Name == CommentKey)
              {
                comic.Comment = comicChildNode.InnerText;
              }
              else if (comicChildNode.Name == CountryKey && Guid.TryParse(comicChildNode.InnerText, out Guid countryID))
              {
                comic.Country = Globals.Instance.Countries.FirstOrDefault(x => x.ID == countryID);
              }
              else if (comicChildNode.Name == PublisherKey && Guid.TryParse(comicChildNode.InnerText, out Guid publisherID))
              {
                comic.Publisher = Globals.Instance.Publishers.FirstOrDefault(x => x.ID == publisherID);
              }
              else if (comicChildNode.Name == LimitedEditionKey && bool.TryParse(comicChildNode.InnerText, out bool limitedEdition))
              {
                comic.LimitedEdition = limitedEdition;
              }
              else if (comicChildNode.Name == CollectorsEditionKey && bool.TryParse(comicChildNode.InnerText, out bool collectorsEdition))
              {
                comic.CollectorsEdition = collectorsEdition;
              }
              else if (comicChildNode.Name == ImagesKey)
              {
                foreach (XmlNode imageNode in comicChildNode.ChildNodes)
                {
                  if (imageNode.Name == ImageKey)
                  {
                    comic.ImagesAsString.Add(imageNode.InnerText);
                  }
                }
              }
            }

            if (!string.IsNullOrWhiteSpace(comic.Series) && comic.ID != Guid.Empty)
            {
              library.Comics.Add(comic);
            }
          }
        }
      }
    }

    #endregion

    #region Saving

    public static void SaveGlobals(Globals globals, string filePath = null)
    {
      filePath ??= Settings.Instance.GlobalsFilePath;

      if (File.Exists(filePath))
      {
        string backupPath = filePath + ".bak";
        File.Copy(filePath, backupPath, true);
      }

      var xml = new XmlDocument();
      xml.CreateXmlDeclaration("1.0", "iso-8859-1", "yes");

      var globalsNode = xml.CreateElement(GlobalsKey);
      var childAttribute = xml.CreateAttribute(SaveDateKey);
      childAttribute.InnerText = DateTime.Now.ToString("o"); // Uses ISO 8601 format
      globalsNode.Attributes.Append(childAttribute);
      xml.AppendChild(globalsNode);

      if (globals.Countries.Count != 0)
      {
        var countriesNode = xml.CreateElement(CountriesKey);
        globalsNode.AppendChild(countriesNode);

        foreach (var country in globals.Countries.OrderBy(x => x.Name))
        {
          AppendChildWithAttributes(countriesNode,
                                    CountryKey,
                                    (IDKey, country.ID.ToString()),
                                    (NameKey, country.Name));
        }
      }

      if (globals.Publishers.Count != 0)
      {
        var publishersNode = xml.CreateElement(PublishersKey);
        globalsNode.AppendChild(publishersNode);

        foreach (var publisher in globals.Publishers.OrderBy(x => x.Name))
        {
          AppendChildWithAttributes(publishersNode,
                                    PublisherKey,
                                    (IDKey, publisher.ID.ToString()),
                                    (NameKey, publisher.Name));
        }
      }

      xml.Save(filePath);
    }

    public static void SaveLibraries(IEnumerable<Library> libraries, string filePath = null)
    {
      filePath ??= Settings.Instance.LibrariesFilePath;

      if (File.Exists(filePath))
      {
        string backupPath = filePath + ".bak";
        File.Copy(filePath, backupPath, true);
      }

      var xml = new XmlDocument();
      xml.CreateXmlDeclaration("1.0", "iso-8859-1", "yes");

      var librariesNode = xml.CreateElement(LibrariesKey);
      xml.AppendChild(librariesNode);

      foreach (var library in libraries.OrderBy(x => x.Name))
      {
        AppendChildWithAttributes(librariesNode,
                                  LibraryKey,
                                  (NameKey, library.Name),
                                  (FileNameKey, library.FileName),
                                  (ComicCountKey, library.ComicCount.ToString()),
                                  (ImageKey, library.ImageAsString));
      }

      xml.Save(filePath);
    }

    public static void SaveActiveLibrary(ActiveLibrary library, string filePath)
    {
      if (File.Exists(filePath))
      {
        string backupPath = filePath + ".bak";
        File.Copy(filePath, backupPath, true);
      }

      var xml = new XmlDocument();
      xml.CreateXmlDeclaration("1.0", "iso-8859-1", "yes");
      WriteLibrary(library, xml);
      xml.Save(filePath);
    }

    private static void WriteLibrary(ActiveLibrary library, XmlDocument xml)
    {
      var libraryNode = xml.CreateElement(LibraryKey);
      var childAttribute = xml.CreateAttribute(SaveDateKey);
      childAttribute.InnerText = DateTime.Now.ToString("o"); // Uses ISO 8601 format
      libraryNode.Attributes.Append(childAttribute);
      xml.AppendChild(libraryNode);
      WriteComics(library, libraryNode);
    }

    private static void WriteComics(ActiveLibrary library, XmlElement libraryNode)
    {
      if (library.Comics.Count != 0)
      {
        var xml = libraryNode.OwnerDocument;
        var comicsNode = xml.CreateElement(ComicsKey);
        libraryNode.AppendChild(comicsNode);

        foreach (var comic in library.Comics.OrderBy(x => x.Publisher).ThenBy(x => x.Series).ThenBy(x => x.Year).ThenBy(x => x.IssueNumber))
        {
          var comicNode = AppendChildWithChildren(comicsNode,
                                                  ComicKey,
                                                  (IDKey, comic.ID.ToString()),
                                                  (SeriesKey, comic.Series),
                                                  (TitleKey, comic.Title),
                                                  (YearKey, comic.Year?.ToString(CultureInfo.InvariantCulture)),
                                                  (IssueNumberKey, comic.IssueNumber?.ToString(CultureInfo.InvariantCulture)),
                                                  (CommentKey, comic.Comment),
                                                  (ConditionKey, comic.Condition.Number.ToString(CultureInfo.InvariantCulture)),
                                                  (CountryKey, comic.Country?.ID.ToString()),
                                                  (PublisherKey, comic.Publisher?.ID.ToString()),
                                                  (CollectorsEditionKey, comic.CollectorsEdition == true ? true.ToString() : ""),
                                                  (LimitedEditionKey, comic.LimitedEdition == true ? true.ToString() : ""));
          if (comic.ImagesAsString.Count > 0)
          {
            var imagesNode = xml.CreateElement(ImagesKey);
            comicNode.AppendChild(imagesNode);

            foreach (var image in comic.ImagesAsString)
            {
              var imageNode = xml.CreateElement(ImageKey);
              imageNode.InnerText = image;
              imagesNode.AppendChild(imageNode);
            }
          }

          comicsNode.AppendChild(comicNode);
        }
      }
    }

    public static XmlNode AppendChildWithAttributes(this XmlElement parentElement, string name, params (string Key, string Value)[] attributes)
    {
      var xml = parentElement.OwnerDocument;
      var child = xml.CreateElement(name);

      foreach (var (Key, Value) in attributes)
      {
        if (!string.IsNullOrWhiteSpace(Value))
        {
          var childAttribute = xml.CreateAttribute(Key);
          childAttribute.InnerText = Value;
          child.Attributes.Append(childAttribute);
        }
      }

      return parentElement.AppendChild(child);
    }

    public static XmlNode AppendChildWithChildren(this XmlElement parentElement, string name, params (string Key, string Value)[] children)
    {
      var xml = parentElement.OwnerDocument;
      var child = xml.CreateElement(name);

      foreach (var (Key, Value) in children)
      {
        if (!string.IsNullOrWhiteSpace(Value))
        {
          var childNode = xml.CreateElement(Key);
          childNode.InnerText = Value;
          child.AppendChild(childNode);
        }
      }

      return parentElement.AppendChild(child);
    }

    #endregion
  }
}