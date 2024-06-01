using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace ComicLibrary.ViewModel
{
  public abstract class PrintActiveLibraryViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    public PrintActiveLibraryViewModel()
    { }

    public void PrepareReport(ActiveLibraryViewModel library)
    {
      FlowDocument flowDocument = CreateReport(library);
      Document = PrepareReport(flowDocument);
    }

    public IDocumentPaginatorSource Document { get; private set; }

    protected abstract FlowDocument CreateReport(ActiveLibraryViewModel library);

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
