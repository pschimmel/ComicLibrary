﻿using System.Linq;
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
    public static readonly FontFamily DefaultFontFamily = new("Arial");

    /// <summary>
    /// Adds a large header 
    /// </summary>
    public static void AddHeader1(this FlowDocument doc, string header)
    {
      var p = new Paragraph(new Bold(new Run(header)))
      {
        FontSize = Header1Size,
        TextAlignment = TextAlignment.Left,
        FontFamily = DefaultFontFamily
      };

      doc.Blocks.Add(p);
    }

    /// <summary>
    /// Adds a smaller header 
    /// </summary>
    public static void AddHeader2(this FlowDocument doc, string header)
    {
      var p = new Paragraph(new Bold(new Run(header)))
      {
        FontSize = Header2Size,
        TextAlignment = TextAlignment.Left,
        FontFamily = DefaultFontFamily
      };

      doc.Blocks.Add(p);
    }

    /// <summary>
    /// Adds a text 
    /// </summary>
    public static void AddParagraph(this FlowDocument doc, string header)
    {
      var p = new Paragraph(new Run(header))
      {
        FontSize = DefaultFontSize,
        TextAlignment = TextAlignment.Justify,
        FontFamily = DefaultFontFamily
      };

      doc.Blocks.Add(p);
    }

    public static Table AddTable(this FlowDocument doc, params GridLength[] columnWidths)
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

      // Create columns and add them to the table's Columns collection.
      int numberOfColumns = columnWidths.Length;

      for (int i = 0; i < numberOfColumns; i++)
      {
        table.Columns.Add(new TableColumn { Width = columnWidths[i] });
      }

      return table;
    }

    /// <summary>
    /// Add header row to table.
    /// </summary>
    public static void AddHeaderRow(this Table table, params string[] cellContents)
    {
      table.AddHeaderRow(cellContents.ToList().Select(x => new CellContent(x)).ToArray());
    }

    /// <summary>
    /// Add header row to table.
    /// </summary>
    public static void AddHeaderRow(this Table table, params CellContent[] cellContents)
    {
      static void formatter(TableRow row)
      {
        // Global formatting for the title row.
        row.Background = Brushes.Silver;
        row.FontSize = DefaultFontSize;
        row.FontWeight = FontWeights.Bold;
        row.FontFamily = DefaultFontFamily;
      }

      table.AddRow(formatter, cellContents);
    }

    /// <summary>
    /// Add standard table row to table.
    /// </summary>
    public static void AddRow(this Table table, params string[] cellContents)
    {
      table.AddRow(cellContents.ToList().Select(x => new CellContent(x)).ToArray());
    }

    /// <summary>
    /// Add standard table row to table.
    /// </summary>
    public static void AddRow(this Table table, params CellContent[] cellContents)
    {
      static void formatter(TableRow row)
      {
        row.FontSize = DefaultFontSize;
        row.FontFamily = DefaultFontFamily;
      }

      table.AddRow(formatter, cellContents);
    }

    private static void AddRow(this Table table, Action<TableRow> formatter, params CellContent[] cellContents)
    {
      if (cellContents.Length != table.Columns.Count)
        throw new ArgumentException("The number of cells does not match the number of columns.");

      // Create and add an empty TableRowGroup to hold the table's Rows.
      var rowGroup = new TableRowGroup();
      table.RowGroups.Add(rowGroup);

      // Add the (title) row.
      var row = new TableRow();
      formatter.Invoke(row);
      rowGroup.Rows.Add(row);

      // Add the header row with content.
      foreach (var cellContent in cellContents)
      {
        var p = new Paragraph(new Run(cellContent.Text))
        {
          TextAlignment = cellContent.Alignment
        };

        row.Cells.Add(new TableCell(p) { ColumnSpan = cellContent.ColumnSpan });
      }
    }
  }
}
