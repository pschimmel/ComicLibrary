using System.Collections.Generic;

namespace ComicLibrary.Model.Entities
{
  public class Comic : Entity
  {
    private int? _year;
    private int? _issueNumber;

    public Comic()
    { }

    public Comic(Comic selectedComic)
    {
      if (selectedComic != null)
      {
        Series = selectedComic.Series;
        Publisher = selectedComic.Publisher;
        Year = selectedComic.Year;
        Condition = selectedComic.Condition;
        Country = selectedComic.Country;
      }
    }

    public Publisher Publisher { get; set; }

    public string Series { get; set; }

    public int? IssueNumber
    {
      get => _issueNumber;
      set
      {
        if (!value.HasValue || value >= 0)
          _issueNumber = value;
      }
    }

    public string Title { get; set; }

    public Condition Condition { get; set; }

    public int? Year
    {
      get => _year;
      set
      {
        if (!value.HasValue || (value > 1800 && value < DateTime.Now.Year + 10))
          _year = value;
      }
    }

    public Country Country { get; set; }

    public List<string> ImagesAsString { get; } = [];

    public bool LimitedEdition { get; set; }

    public bool CollectorsEdition { get; set; }

    public string Comment { get; set; }
  }
}
