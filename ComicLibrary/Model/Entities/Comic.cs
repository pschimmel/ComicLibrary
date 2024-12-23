﻿using System.Collections.Generic;
using ComicLibrary.Model.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ComicLibrary.Model.Entities
{
  public class Comic : Entity
  {
    public Comic()
    {
      Condition = ScaleHelper.DefaultScale.UnGraded;
    }

    public Comic(Comic selectedComic, bool? copyData = null)
      : this()
    {
      copyData ??= Settings.Instance.CopyDataFromSelectedComic;

      if (selectedComic != null && copyData == true)
      {
        Series = selectedComic.Series;
        Publisher = selectedComic.Publisher;
        Year = selectedComic.Year;
        Condition = selectedComic.Condition;
        GradingCertified = selectedComic.GradingCertified;
        Country = selectedComic.Country;
        Language = selectedComic.Language;
        IssueNumber = selectedComic.IssueNumber;
        Title = selectedComic.Title;
        PurchasePrice = selectedComic.PurchasePrice;
        EstimatedValue = selectedComic.EstimatedValue;
        Comment = selectedComic.Comment;
        CollectorsEdition = selectedComic.CollectorsEdition;
        LimitedEdition = selectedComic.LimitedEdition;
        ImagesAsString = new List<string>(selectedComic.ImagesAsString);
        CreatedDate = DateTime.Now;
        ModifiedDate = DateTime.Now;
      }
    }

    public Publisher Publisher { get; set; }

    public string Series { get; set; }

    public string IssueNumber { get; set; }

    public string Title { get; set; }

    public Grade Condition { get; set; }
    public bool GradingCertified { get; set; }

    public int? Year { get; set; }

    public Country Country { get; set; }

    public Language Language { get; set; }

    public List<string> ImagesAsString { get; } = [];

    public bool LimitedEdition { get; set; }

    public bool CollectorsEdition { get; set; }

    public string Comment { get; set; }

    public double? PurchasePrice { get; set; }

    public double? EstimatedValue { get; set; }

    public Comic Copy()
    {
      return new Comic(this, true);
    }

    public string ToClipboardString()
    {
      return JsonConvert.SerializeObject(this, Formatting.Indented, new StringEnumConverter());
    }

    public static Comic FromClipboardString(string comicAsString)
    {
      return string.IsNullOrWhiteSpace(comicAsString) ? null : JsonConvert.DeserializeObject<Comic>(comicAsString);
    }
  }
}
