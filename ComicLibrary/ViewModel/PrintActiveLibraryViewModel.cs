using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using ComicLibrary.Model.Config;
using ComicLibrary.ViewModel.Helpers;

namespace ComicLibrary.ViewModel
{
  public class PrintActiveLibraryViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    public PrintActiveLibraryViewModel(ActiveLibraryViewModel library)
    {
      FlowDocument flowDocument = CreateFlowDocument(library);
      Document = PrepareReport(flowDocument);
    }

    public IDocumentPaginatorSource Document { get; }

    private static FlowDocument CreateFlowDocument(ActiveLibraryViewModel library)
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

    private static FixedDocumentSequence PrepareReport(FlowDocument document)
    {
      // Use available size of document
      document.ColumnWidth = double.PositiveInfinity;

      var paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
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
