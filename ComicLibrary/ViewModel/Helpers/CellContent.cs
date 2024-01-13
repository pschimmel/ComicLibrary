using System.Windows;

namespace ComicLibrary.ViewModel.Helpers
{
  public class CellContent
  {
    public CellContent(string text, TextAlignment alignment = TextAlignment.Left, int columnSpan = 1)
    {
      Text = text;
      Alignment = alignment;
      ColumnSpan = columnSpan;
    }

    public string Text { get; }

    public TextAlignment Alignment { get; }

    public int ColumnSpan { get; }
  }
}
