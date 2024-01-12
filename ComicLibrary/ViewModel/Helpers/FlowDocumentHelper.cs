using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ComicLibrary.ViewModel.Helpers
{
  public static class FlowDocumentHelper
  {
    public const double Header1Size = 26.0;
    public const double Header2Size = 14.0;
    public const double DefaultFontSize = 12.0;

    public static void AddHeader1(this FlowDocument doc, string header)
    {
      var p = new Paragraph(new Run(header))
      {
        FontSize = Header1Size,
        TextAlignment = TextAlignment.Left
      };

      doc.Blocks.Add(p);
    }

    public static void AddHeader2(this FlowDocument doc, string header)
    {
      var p = new Paragraph(new Run(header))
      {
        FontSize = Header2Size,
        TextAlignment = TextAlignment.Left
      };

      doc.Blocks.Add(p);
    }

    public static Table AddTable(this FlowDocument doc)
    {
      // Create the Table...
      var table = new Table
      {
        // Set some global formatting properties for the table.
        CellSpacing = 10,
        Background = Brushes.White,
      };

      // ...and add it to the FlowDocument Blocks collection.
      doc.Blocks.Add(table);
      return table;
    }

    public static void AddHeaderRow(this Table table, params string[] cellContents)
    {
      // Create columns and add them to the table's Columns collection.
      int numberOfColumns = cellContents.Length;

      for (int x = 0; x < numberOfColumns; x++)
      {
        table.Columns.Add(new TableColumn());
      }

      // Create and add an empty TableRowGroup to hold the table's Rows.
      var rowGroup = new TableRowGroup();
      table.RowGroups.Add(rowGroup);

      // Add the (title) row.
      var row = new TableRow();
      rowGroup.Rows.Add(row);

      // Global formatting for the title row.
      row.Background = Brushes.Silver;
      row.FontSize = DefaultFontSize;
      row.FontWeight = FontWeights.Bold;

      // Add the header row with content.
      foreach (var cellContent in cellContents)
      {
        var p = new Paragraph(new Run(cellContent))
        {
          TextAlignment = TextAlignment.Left
        };

        row.Cells.Add(new TableCell(p));
      }
    }

    public static void AddRow(this Table table, params string[] cellContents)
    {
      if (cellContents.Length != table.Columns.Count)
        throw new ArgumentException("The number of cells does not match the number of columns.");

      // Create and add an empty TableRowGroup to hold the table's Rows.
      var rowGroup = new TableRowGroup();
      table.RowGroups.Add(rowGroup);

      // Add the (title) row.
      var row = new TableRow();
      rowGroup.Rows.Add(row);

      // Global formatting for the title row.
      row.FontSize = DefaultFontSize;

      // Add the header row with content.
      foreach (var cellContent in cellContents)
      {
        var p = new Paragraph(new Run(cellContent))
        {
          TextAlignment = TextAlignment.Left
        };

        row.Cells.Add(new TableCell(p));
      }
    }
  }
}
