using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using ComicLibrary.Model.Config;
using ComicLibrary.ViewModel.Helpers;

namespace ComicLibrary.ViewModel
{
  public class PrintActiveLibraryViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    public enum ReportType { Report, List }

    public PrintActiveLibraryViewModel(ActiveLibraryViewModel library, ReportType type)
    {
      FlowDocument flowDocument = type == ReportType.Report ? CreateReport(library) : CreateList(library);
      Document = PrepareReport(flowDocument);
    }

    public IDocumentPaginatorSource Document { get; }

    private static FlowDocument CreateReport(ActiveLibraryViewModel library)
    {
      var document = new FlowDocument();

      // Add header
      document.AddHeader1(library.Name);

      // Add comics as table
      var table = document.AddTable(
        new GridLength(3, GridUnitType.Star), // Series
        new GridLength(1, GridUnitType.Star), // Year
        new GridLength(1, GridUnitType.Star), // IssueNumber
        new GridLength(3, GridUnitType.Star), // Title
        new GridLength(2, GridUnitType.Star), // Condition
        new GridLength(1, GridUnitType.Star)  // Price
      );

      table.AddHeaderRow(Properties.Resources.Series,
                         Properties.Resources.Year,
                         Properties.Resources.IssueNumber,
                         Properties.Resources.Title,
                         Properties.Resources.Condition,
                         Properties.Resources.PurchasePrice);

      foreach (var comic in library.Comics)
      {
        table.AddRow(new CellContent(comic.Series),
                     new CellContent(comic.Year?.ToString(), TextAlignment.Right),
                     new CellContent(comic.IssueNumber?.ToString(), TextAlignment.Right),
                     new CellContent(comic.Title),
                     new CellContent(comic.Condition.Name),
                     new CellContent(comic.PurchasePrice.HasValue ? $"{Settings.Instance.CurrencySymbol} {comic.PurchasePrice.Value:F2}" : null, TextAlignment.Right));
      }

      return document;
    }

    private static FlowDocument CreateList(ActiveLibraryViewModel library)
    {
      var document = new FlowDocument();

      // Add header
      document.AddHeader1(library.Name);

      // Add series 
      var series = library.Comics.Where(x => x.IssueNumber.HasValue && !string.IsNullOrWhiteSpace(x.Series))
                                 .Select(x => x.Series)
                                 .Distinct()
                                 .Order();

      foreach (var serie in series)
      {
        document.AddHeader2(serie);
        var listOfIssues = library.Comics.Where(x => x.IssueNumber.HasValue && x.Series == serie)
                                         .Select(x => x.IssueNumber.Value)
                                         .Order()
                                         .ToList();
        int? lastIssue = null;
        string text = "";

        for (int i = 0; i < listOfIssues.Count; i++)
        {
          var issue = listOfIssues[i];

          if (issue != lastIssue) // In case there are duplicates
          {
            if (lastIssue.HasValue && lastIssue + 1 == issue)
            {
              if (!text.EndsWith('-'))
                text += "-";

              if (i == listOfIssues.Count - 1)
                text += issue;
            }
            else
            {
              if (text.EndsWith('-'))
                text += lastIssue;

              if (text != "")
                text += ", ";

              text += issue;
            }
          }

          lastIssue = issue;
        }

        if (text.EndsWith('-'))
          text += lastIssue;

        document.AddParagraph(text);
      }

      return document;
    }

    private static FixedDocumentSequence PrepareReport(FlowDocument document)
    {
      // Use available size of document
      document.ColumnWidth = double.PositiveInfinity;

      var printQueue = LocalPrintServer.GetDefaultPrintQueue();
      var ticket = printQueue.DefaultPrintTicket;
      var pageMediaSize = ticket.PageMediaSize;
      var printableArea = printQueue.GetPrintCapabilities(ticket).PageImageableArea;

      // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
      var leftPadding = printableArea.OriginWidth;
      var topPadding = printableArea.OriginHeight;
      var rightPadding = pageMediaSize.Width - leftPadding - printableArea.ExtentWidth ?? 0.0;
      var bottomPadding = pageMediaSize.Height - topPadding - printableArea.ExtentHeight ?? 0.0;

      var minBorderPadding = 50.0;
      document.PagePadding = new Thickness(Math.Max(minBorderPadding, topPadding),
                                           Math.Max(minBorderPadding, leftPadding),
                                           Math.Max(minBorderPadding, rightPadding),
                                           Math.Max(minBorderPadding, bottomPadding));

      var paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
      paginator.PageSize = new Size(pageMediaSize.Width ?? 0, pageMediaSize.Height ?? 0);
      using var ms = new MemoryStream();
      var package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
      var packUri = new Uri("pack://temp.xps");
      PackageStore.RemovePackage(packUri);
      PackageStore.AddPackage(packUri, package);
      var xps = new XpsDocument(package, CompressionOption.NotCompressed, packUri.ToString());
      XpsDocument.CreateXpsDocumentWriter(xps).Write(paginator);
      return xps.GetFixedDocumentSequence();
    }
  }
}
