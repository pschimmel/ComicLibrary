using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ComicLibrary.Model.Config
{
  internal class Settings
  {
    private const string ApplicationName = "ComicLibrary";
    private static readonly Lazy<Settings> _lazyConfiguration = new(Load());
    private const string _configFileName = $"{ApplicationName}.Config.json";
    private const string _librariesFileName = $"Libraries.xml";
    private const string _globalsFileName = $"Globals.xml";

    private Settings()
    {
      LibrariesPath = ApplicationPath;
      if (!Directory.Exists(ApplicationPath))
        Directory.CreateDirectory(ApplicationPath);
    }

    private static string ApplicationPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

    public static Settings Instance => _lazyConfiguration.Value;

    public string LibrariesPath { get; set; }

    public string LibrariesFilePath => Path.Combine(LibrariesPath, _librariesFileName);

    public string GlobalsFilePath => Path.Combine(LibrariesPath, _globalsFileName);

    public static Settings Load()
    {
      string configFilePath = Path.Combine(ApplicationPath, _configFileName);

      if (File.Exists(configFilePath))
      {
        string configAsString = File.ReadAllText(configFilePath);
        return JsonConvert.DeserializeObject<Settings>(configAsString);
      }

      return new Settings();
    }

    public static void Save()
    {
      string configFilePath = Path.Combine(ApplicationPath, _configFileName);
      string configAsString = JsonConvert.SerializeObject(Instance, Formatting.Indented, new StringEnumConverter());
      File.WriteAllText(configFilePath, configAsString);
    }
  }
}
