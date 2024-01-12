using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
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
      var table = document.AddTable();
      table.AddHeaderRow(Properties.Resources.Series,
                         Properties.Resources.Year,
                         Properties.Resources.IssueNumber,
                         Properties.Resources.Title,
                         Properties.Resources.Condition);

      foreach (var comic in library.Comics)
      {
        table.AddRow(comic.Series,
                     comic.Year?.ToString() ?? "",
                     comic.IssueNumber?.ToString() ?? "",
                     comic.Title,
                     comic.Condition.Name);
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
