using System.Collections.Generic;

namespace ComicLibrary.Model.Entities
{
  public class Globals
  {
    private static readonly Lazy<Globals> _lazyGlobals = new(Load());

    public static Globals Instance => _lazyGlobals.Value;

    public HashSet<Publisher> Publishers { get; set; } = [];

    public HashSet<Country> Countries { get; set; } = [];

    public HashSet<Language> Languages { get; set; } = [];

    public string DefaultGradingScale = "CGC";

    public static Globals Load()
    {
      return FileHelper.LoadGlobals();
    }

    public static void Save(Globals globals = null)
    {
      globals ??= Instance;
      FileHelper.SaveGlobals(globals);
    }
  }
}
