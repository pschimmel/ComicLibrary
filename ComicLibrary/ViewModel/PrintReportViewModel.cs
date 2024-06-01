using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using ComicLibrary.Model.Config;
using ComicLibrary.ViewModel.Helpers;

namespace ComicLibrary.ViewModel
{
  public class PrintReportViewModel : PrintActiveLibraryViewModel
  {
    private readonly PrintColumns _printColumns;

    [Flags]
    public enum PrintColumns
    {
      Series = 1,
      Year = 2,
      IssueNumber = 4,
      Title = 8,
      Condition = 16,
      PurchasePrice = 32,
      Default = Series | Year | IssueNumber | Title | Condition
    }

    public PrintReportViewModel(PrintColumns printColumns)
      : base()
    {
      _printColumns = printColumns;
    }

    protected override FlowDocument CreateReport(ActiveLibraryViewModel library)
    {
      var document = new FlowDocument();

      // Add header
      document.AddHeader1(library.Name);

      var columns = new List<GridLength>();
      var headers = new List<string>();

      // Series
      if (_printColumns.HasFlag(PrintColumns.Series))
      {
        columns.Add(new GridLength(3, GridUnitType.Star));
        headers.Add(Properties.Resources.Series);
      }

      // Year
      if (_printColumns.HasFlag(PrintColumns.Year))
      {
        columns.Add(new GridLength(1, GridUnitType.Star));
        headers.Add(Properties.Resources.Year);
      }

      // IssueNumber
      if (_printColumns.HasFlag(PrintColumns.IssueNumber))
      {
        columns.Add(new GridLength(1, GridUnitType.Star));
        headers.Add(Properties.Resources.IssueNumber);
      }

      // Title
      if (_printColumns.HasFlag(PrintColumns.Title))
      {
        columns.Add(new GridLength(3, GridUnitType.Star));
        headers.Add(Properties.Resources.Title);
      }

      // Condition
      if (_printColumns.HasFlag(PrintColumns.Condition))
      {
        columns.Add(new GridLength(2, GridUnitType.Star));
        headers.Add(Properties.Resources.Condition);
      }

      // Price
      if (_printColumns.HasFlag(PrintColumns.PurchasePrice))
      {
        columns.Add(new GridLength(1, GridUnitType.Star));
        headers.Add(Properties.Resources.PurchasePrice);
      }

      // Add comics as table
      var table = document.AddTable(columns.ToArray());
      table.AddHeaderRow(headers.ToArray());

      foreach (var comic in library.Comics)
      {
        var content = new List<CellContent>();

        // Series
        if (_printColumns.HasFlag(PrintColumns.Series))
        {
          content.Add(new CellContent(comic.Series));
        }

        // Year
        if (_printColumns.HasFlag(PrintColumns.Year))
        {
          content.Add(new CellContent(comic.Year?.ToString(), TextAlignment.Right));
        }

        // IssueNumber
        if (_printColumns.HasFlag(PrintColumns.IssueNumber))
        {
          content.Add(new CellContent(comic.IssueNumber?.ToString(), TextAlignment.Right));
        }

        // Title
        if (_printColumns.HasFlag(PrintColumns.Title))
        {
          content.Add(new CellContent(comic.Title));
        }

        // Condition
        if (_printColumns.HasFlag(PrintColumns.Condition))
        {
          content.Add(new CellContent(comic.Condition.Name));
        }

        // Price
        if (_printColumns.HasFlag(PrintColumns.PurchasePrice))
        {
          content.Add(new CellContent(comic.PurchasePrice.HasValue ? $"{Settings.Instance.CurrencySymbol} {comic.PurchasePrice.Value:F2}" : null, TextAlignment.Right));
        }

        table.AddRow(content.ToArray());
      }

      return document;
    }
  }
}
