using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ComicLibrary.Model.Config
{
  internal class Settings
  {
    private const string ApplicationName = "Comic Library";
    private static readonly Lazy<Settings> _lazyConfiguration = new(Load());
    private const string _configFileName = $"{ApplicationName}.Config.json";
    private const string _librariesFileName = $"Libraries.xml";
    private const string _globalsFileName = $"Globals.xml";

    private Settings()
    {
      LibrariesPath = LibrariesPathDefault;
    }

    private static string LibrariesPathDefault => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ApplicationName);

    private static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

    public static Settings Instance => _lazyConfiguration.Value;

    public string LibrariesPath { get; set; }

    public bool CopyDataFromSelectedComic { get; set; } = true;

    public bool CreateBackupWhenSaving { get; set; } = true;

    public string CurrencySymbol { get; set; } = "€";

    public string LibrariesFilePath => Path.Combine(LibrariesPath, _librariesFileName);

    public string GlobalsFilePath => Path.Combine(LibrariesPath, _globalsFileName);

    public static Settings Load()
    {
      string configFilePath = Path.Combine(SettingsPath, _configFileName);

      if (File.Exists(configFilePath))
      {
        try
        {
          string configAsString = File.ReadAllText(configFilePath);
          return JsonConvert.DeserializeObject<Settings>(configAsString);
        }
        catch (Exception ex)
        {
          MessageBox.Show(string.Format(Properties.Resources.CannotLoadFileMessage, configFilePath) + Environment.NewLine + ex.Message);
        }
      }

      return new Settings();
    }

    public static void Save()
    {
      EnsureSettingsPathExists();
      string configFilePath = Path.Combine(SettingsPath, _configFileName);

      try
      {
        string configAsString = JsonConvert.SerializeObject(Instance, Formatting.Indented, new StringEnumConverter());
        File.WriteAllText(configFilePath, configAsString);
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format(Properties.Resources.CannotSaveFileMessage, configFilePath) + Environment.NewLine + ex.Message);
      }
    }

    private static void EnsureSettingsPathExists()
    {
      if (!Directory.Exists(SettingsPath))
      {
        try
        {
          Directory.CreateDirectory(SettingsPath);
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }
    }

    internal void EnsureLibrariesPathExists()
    {
      if (!Directory.Exists(LibrariesPath))
      {
        try
        {
          Directory.CreateDirectory(LibrariesPath);
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }
    }
  }
}
