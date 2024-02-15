using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
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
    private const string LanguagesKey = "Languages";
    private const string LanguageKey = "Language";
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
    private const string CurrencyKey = "Currency";
    private const string ComicCountKey = "ComicCount";
    private const string GradingTypeKey = "GradingType";
    private const string PurchasePriceKey = "PurchasePrice";
    private const string EstimatedValueKey = "EstimatedValue";
    private const string CreatedKey = "Created";
    private const string ModifiedKey = "Modified";

    #endregion

    #region Loading

    public static Globals LoadGlobals(string filePath = null)
    {
      filePath ??= Settings.Instance.GlobalsFilePath;

      var globals = new Globals();

      if (!File.Exists(filePath))
        return globals;

      try
      {
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
                    ReadOptionAttribute(country, countryAttribute);
                  }

                  if (!string.IsNullOrWhiteSpace(country.Name) && country.ID != Guid.Empty)
                    globals.Countries.Add(country);
                }
              }
            }
            else if (globalsChildNode.Name == LanguagesKey)
            {
              var languagesNode = globalsChildNode;
              foreach (XmlNode languageNode in languagesNode.ChildNodes)
              {
                if (languageNode.Name == LanguageKey)
                {
                  var language = new Language();

                  foreach (XmlAttribute languageAttribute in languageNode.Attributes)
                  {
                    ReadOptionAttribute(language, languageAttribute);
                  }

                  if (!string.IsNullOrWhiteSpace(language.Name) && language.ID != Guid.Empty)
                    globals.Languages.Add(language);
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
                    ReadOptionAttribute(publisher, publisherAttribute);
                  }

                  if (!string.IsNullOrWhiteSpace(publisher.Name) && publisher.ID != Guid.Empty)
                    globals.Publishers.Add(publisher);
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format(Properties.Resources.CannotLoadFileMessage, filePath) + Environment.NewLine + ex.Message);
      }

      return globals;
    }

    private static void ReadOptionAttribute(IOption option, XmlAttribute optionAttribute)
    {
      if (optionAttribute.Name == NameKey)
      {
        option.Name = optionAttribute.InnerText;
      }
      else ReadEntityAttribute(option, optionAttribute);
    }

    private static void ReadEntityAttribute(IEntity entity, XmlAttribute entityAttribute)
    {
      if (entityAttribute.Name == IDKey)
      {
        entity.ID = new Guid(entityAttribute.InnerText);
      }
      else if (entityAttribute.Name == CreatedKey && DateTime.TryParse(entityAttribute.Value, out DateTime created))
      {
        entity.CreatedDate = created;
      }
      else if (entityAttribute.Name == ModifiedKey && DateTime.TryParse(entityAttribute.Value, out DateTime modified))
      {
        entity.ModifiedDate = modified;
      }
    }

    public static IEnumerable<Library> LoadLibraries(string filePath = null)
    {
      filePath ??= Settings.Instance.LibrariesFilePath;
      var libraries = new List<Library>();

      if (!File.Exists(filePath))
        return libraries;

      try
      {
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
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format(Properties.Resources.CannotLoadFileMessage, filePath) + Environment.NewLine + ex.Message);
      }

      return libraries;
    }

    public static ActiveLibrary LoadActiveLibrary(string filePath)
    {
      if (!File.Exists(filePath))
        return new ActiveLibrary();

      //  EventService.Instance.Publish("StartProgress", 0.0);
      var library = new ActiveLibrary();

      try
      {
        var xml = new XmlDocument();
        xml.Load(filePath);

        ReadLibrary(library, xml);
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format(Properties.Resources.CannotLoadFileMessage, filePath) + Environment.NewLine + ex.Message);
      }

      // EventService.Instance.Publish("EndProgress", 100.0);
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
        int counter = 0;

        foreach (XmlNode comicNode in comicsNode.ChildNodes)
        {
          if (comicNode.Name == ComicKey)
          {
            var comic = new Comic();

            foreach (XmlAttribute comicAttribute in comicNode.Attributes)
            {
              ReadEntityAttribute(comic, comicAttribute);
            }

            foreach (XmlNode comicChildNode in comicNode.ChildNodes)
            {
              if (comicChildNode.Name == SeriesKey)
              {
                comic.Series = comicChildNode.InnerText;
              }
              else if (comicChildNode.Name == YearKey && int.TryParse(comicChildNode.InnerText, CultureInfo.InvariantCulture, out int year))
              {
                comic.Year = year;
              }
              else if (comicChildNode.Name == ConditionKey && double.TryParse(comicChildNode.InnerText, CultureInfo.InvariantCulture, out double gradingNumber))
              {
                // CGC is currently the only grading service supported 
                // If we ever want to add something else, it must be added here
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
              else if (comicChildNode.Name == LanguageKey && Guid.TryParse(comicChildNode.InnerText, out Guid languageID))
              {
                comic.Language = Globals.Instance.Languages.FirstOrDefault(x => x.ID == languageID);
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
              else if (comicChildNode.Name == PurchasePriceKey && double.TryParse(comicChildNode.InnerText, CultureInfo.InvariantCulture, out double price))
              {
                comic.PurchasePrice = price;
              }
              else if (comicChildNode.Name == EstimatedValueKey && double.TryParse(comicChildNode.InnerText, CultureInfo.InvariantCulture, out double estimate))
              {
                comic.EstimatedValue = estimate;
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

          counter++;

          // EventService.Instance.Publish("Progress", counter / comicsNode.ChildNodes.Count * 100.0);
        }
      }
    }

    #endregion

    #region Saving

    public static void SaveGlobals(Globals globals, string filePath = null)
    {
      Settings.Instance.EnsureLibrariesPathExists();
      filePath ??= Settings.Instance.GlobalsFilePath;

      if (File.Exists(filePath) && Settings.Instance.CreateBackupWhenSaving)
      {
        string backupPath = filePath + ".bak";

        try
        {
          File.Copy(filePath, backupPath, true);
        }
        catch (Exception ex)
        {
          MessageBox.Show(string.Format(Properties.Resources.CannotSaveFileMessage, backupPath) + Environment.NewLine + ex.Message);
        }
      }

      try
      {
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

          foreach (var country in globals.Countries.Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.ID != Guid.Empty).OrderBy(x => x.Name))
          {
            AppendChildWithAttributes(countriesNode,
                                      CountryKey,
                                      (IDKey, country.ID.ToString()),
                                      (NameKey, country.Name),
                                      (CreatedKey, country.CreatedDate.ToString("s")),
                                      (ModifiedKey, country.ModifiedDate.ToString("s")));
          }
        }

        if (globals.Languages.Count != 0)
        {
          var languegesNode = xml.CreateElement(LanguagesKey);
          globalsNode.AppendChild(languegesNode);

          foreach (var language in globals.Languages.Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.ID != Guid.Empty).OrderBy(x => x.Name))
          {
            AppendChildWithAttributes(languegesNode,
                                      LanguageKey,
                                      (IDKey, language.ID.ToString()),
                                      (NameKey, language.Name),
                                      (CreatedKey, language.CreatedDate.ToString("s")),
                                      (ModifiedKey, language.ModifiedDate.ToString("s")));
          }
        }

        if (globals.Publishers.Count != 0)
        {
          var publishersNode = xml.CreateElement(PublishersKey);
          globalsNode.AppendChild(publishersNode);

          foreach (var publisher in globals.Publishers.Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.ID != Guid.Empty).OrderBy(x => x.Name))
          {
            AppendChildWithAttributes(publishersNode,
                                      PublisherKey,
                                      (IDKey, publisher.ID.ToString()),
                                      (NameKey, publisher.Name),
                                      (CreatedKey, publisher.CreatedDate.ToString("s")),
                                      (ModifiedKey, publisher.ModifiedDate.ToString("s")));
          }
        }

        xml.Save(filePath);
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format(Properties.Resources.CannotSaveFileMessage, filePath) + Environment.NewLine + ex.Message);
      }
    }

    public static void SaveLibraries(IEnumerable<Library> libraries, string filePath = null)
    {
      Settings.Instance.EnsureLibrariesPathExists();
      filePath ??= Settings.Instance.LibrariesFilePath;

      if (File.Exists(filePath) && Settings.Instance.CreateBackupWhenSaving)
      {
        string backupPath = filePath + ".bak";
        File.Copy(filePath, backupPath, true);
      }

      var xml = new XmlDocument();
      xml.CreateXmlDeclaration("1.0", "iso-8859-1", "yes");

      var librariesNode = xml.CreateElement(LibrariesKey);
      xml.AppendChild(librariesNode);

      foreach (var library in libraries.Where(x => !string.IsNullOrWhiteSpace(x.Name)).OrderBy(x => x.Name))
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
      Settings.Instance.EnsureLibrariesPathExists();
      if (File.Exists(filePath) && Settings.Instance.CreateBackupWhenSaving)
      {
        string backupPath = filePath + ".bak";

        try
        {
          File.Copy(filePath, backupPath, true);
        }
        catch (Exception ex)
        {
          MessageBox.Show(string.Format(Properties.Resources.CannotSaveFileMessage, backupPath) + Environment.NewLine + ex.Message);
        }
      }

      try
      {
        var xml = new XmlDocument();
        xml.CreateXmlDeclaration("1.0", "iso-8859-1", "yes");
        WriteLibrary(library, xml);
        xml.Save(filePath);
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format(Properties.Resources.CannotSaveFileMessage, filePath) + Environment.NewLine + ex.Message);
      }
    }

    private static void WriteLibrary(ActiveLibrary library, XmlDocument xml)
    {
      var libraryNode = xml.CreateElement(LibraryKey);
      xml.AppendChild(libraryNode);

      AppendAttributes(libraryNode, (SaveDateKey, DateTime.Now.ToString("o")), // Uses ISO 8601 format
                                    (CurrencyKey, Settings.Instance.CurrencySymbol));
      WriteComics(library, libraryNode);
    }

    private static void WriteComics(ActiveLibrary library, XmlElement libraryNode)
    {
      if (library.Comics.Count != 0)
      {
        var xml = libraryNode.OwnerDocument;
        var comicsNode = xml.CreateElement(ComicsKey);
        libraryNode.AppendChild(comicsNode);

        foreach (var comic in library.Comics.Where(x => !string.IsNullOrWhiteSpace(x.Series) && x.ID != Guid.Empty).OrderBy(x => x.Series).ThenBy(x => x.IssueNumber))
        {
          var comicNode = AppendChildWithChildren(comicsNode,
                                                  ComicKey,
                                                  (SeriesKey, comic.Series),
                                                  (TitleKey, comic.Title),
                                                  (YearKey, comic.Year?.ToString(CultureInfo.InvariantCulture)),
                                                  (IssueNumberKey, comic.IssueNumber?.ToString(CultureInfo.InvariantCulture)),
                                                  (CommentKey, comic.Comment),
                                                  (CountryKey, comic.Country?.ID.ToString()),
                                                  (LanguageKey, comic.Language?.ID.ToString()),
                                                  (PublisherKey, comic.Publisher?.ID.ToString()),
                                                  (CollectorsEditionKey, comic.CollectorsEdition == true ? true.ToString() : ""),
                                                  (LimitedEditionKey, comic.LimitedEdition == true ? true.ToString() : ""),
                                                  (PurchasePriceKey, comic.PurchasePrice.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0:F2}", comic.PurchasePrice.Value) : null),
                                                  (EstimatedValueKey, comic.EstimatedValue.HasValue ? string.Format(CultureInfo.InvariantCulture, "{0:F2}", comic.EstimatedValue.Value) : null));

          comicNode.AppendAttributes((IDKey, comic.ID.ToString()),
                                     (CreatedKey, comic.CreatedDate.ToString("s")),
                                     (ModifiedKey, comic.ModifiedDate.ToString("s")));

          var gradingNode = xml.CreateElement(ConditionKey);
          gradingNode.InnerText = comic.Condition.Number.ToString(CultureInfo.InvariantCulture);
          gradingNode.AppendAttributes((GradingTypeKey, "CGC")); // CGC is currently the only grading supported
          comicNode.AppendChild(gradingNode);

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

    public static XmlNode AppendChildWithAttributes(this XmlNode parentElement, string name, params (string Key, string Value)[] attributes)
    {
      var xml = parentElement.OwnerDocument;
      var child = xml.CreateElement(name);

      child.AppendAttributes(attributes);

      return parentElement.AppendChild(child);
    }

    private static void AppendAttributes(this XmlNode element, params (string Key, string Value)[] attributes)
    {
      if (attributes != null)
      {
        var xml = element.OwnerDocument;

        foreach (var (Key, Value) in attributes)
        {
          if (!string.IsNullOrWhiteSpace(Value))
          {
            var childAttribute = xml.CreateAttribute(Key);
            childAttribute.InnerText = Value;
            element.Attributes.Append(childAttribute);
          }
        }
      }
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

    public static string GetValidFileName(string libraryName)
    {
      string fileName = libraryName;

      foreach (char c in Path.GetInvalidFileNameChars())
      {
        fileName = fileName.Replace(c, '_');
      }

      return fileName;
    }

    #endregion
  }
}