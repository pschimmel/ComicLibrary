using System.Collections.Generic;
using System.Globalization;
using ComicLibrary.Model.Entities;

namespace ComicLibrary.ViewModel
{
  public class ChartViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    private ChartType _chartType;
    private bool _showRelative;
    private readonly ActiveLibrary _library;

    public ChartViewModel(ActiveLibrary library)
    {
      _library = library;
      ChartType = ChartType.Year;
    }

    public static ChartType[] ChartTypes => Enum.GetValues<ChartType>();

    public ChartType ChartType
    {
      get => _chartType;
      set
      {
        _chartType = value;

        Header = _chartType switch
        {
          ChartType.Year => Properties.Resources.Year,
          ChartType.Grading => Properties.Resources.Grading,
          ChartType.Price => Properties.Resources.PurchasePrice,
          ChartType.Value => Properties.Resources.EstimatedValue,
          _ => throw new NotImplementedException()
        };

        OnPropertyChanged(nameof(ChartType));
        OnPropertyChanged(nameof(Header));
        GenerateItems();
      }
    }

    public string Header { get; private set; }

    public Dictionary<string, int> Items { get; private set; }

    public int MaxValue { get; private set; }

    public bool ShowRelative
    {
      get => _showRelative;
      set
      {
        _showRelative = value;
        OnPropertyChanged(nameof(ShowRelative));
      }
    }

    private IEnumerable<Comic> Selector(IEnumerable<Comic> comics)
    {
      return _chartType switch
      {
        ChartType.Year => comics.Where(x => x.Year != null).OrderBy(x => x.Year),
        ChartType.Grading => comics.Where(x => x.Condition != null).OrderBy(x => x.Condition.Number),
        ChartType.Price => comics.Where(x => x.PurchasePrice != null).OrderBy(x => x.PurchasePrice),
        ChartType.Value => comics.Where(x => x.EstimatedValue != null).OrderBy(x => x.EstimatedValue),
        _ => throw new NotImplementedException()
      };
    }

    private string GetValue(Comic comic)
    {
      return _chartType switch
      {
        ChartType.Year => comic.Year.ToString(),
        ChartType.Grading => comic.Condition?.Number.ToString("F1", CultureInfo.InvariantCulture),
        ChartType.Price => Math.Round(comic.PurchasePrice ?? 0.0).ToString("F2", CultureInfo.InvariantCulture),
        ChartType.Value => Math.Round(comic.EstimatedValue ?? 0.0).ToString("F2", CultureInfo.InvariantCulture),
        _ => throw new NotImplementedException()
      };
    }

    private void GenerateItems()
    {
      Items = [];
      int max = 0;

      foreach (var comic in Selector(_library.Comics))
      {
        string valueAsString = GetValue(comic);

        if (!string.IsNullOrWhiteSpace(valueAsString))
        {
          if (Items.TryGetValue(valueAsString, out int value))
          {
            Items[valueAsString] = ++value;
          }
          else
          {
            Items.Add(valueAsString, 1);
            value = 1;
          }

          max = Math.Max(max, value);
        }
      }

      MaxValue = max;
      OnPropertyChanged(nameof(Items));
      OnPropertyChanged(nameof(MaxValue));
    }
  }
}
