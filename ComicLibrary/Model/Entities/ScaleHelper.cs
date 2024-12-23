using System.Collections.Generic;

namespace ComicLibrary.Model.Entities
{
  public class ScaleHelper
  {
    private static readonly Lazy<ScaleHelper> _lazyScaleHelper = new(() => new ScaleHelper());
    private readonly IEnumerable<GradingScale> _scales;

    private ScaleHelper()
    {
      _scales = GradingScaleFileHelper.GetScales();
      foreach (var scale in _scales)
      {
        if (!scale.Grades.Any(x => x.Number == -1.0))
        {
          // Add ungraded item
          scale.Grades.Insert(0, scale.UnGraded);
        }
      }
    }

    public static IEnumerable<GradingScale> Scales => _lazyScaleHelper.Value._scales;

    public static GradingScale DefaultScale => Scales.FirstOrDefault(x => x.Name == Globals.Instance.DefaultGradingScale);
  }
}
