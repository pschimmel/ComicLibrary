using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace ComicLibrary.View.Behaviors
{
  public class SingleClickEditingBehavior : Behavior<DataGrid>
  {
    private readonly RoutedEventHandler handler;

    public SingleClickEditingBehavior()
    {
      handler = new RoutedEventHandler(DataGridCell_Selected);
    }

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.AddHandler(DataGridCell.SelectedEvent, handler, true);
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.RemoveHandler(DataGridCell.SelectedEvent, handler);
    }

    private void DataGridCell_Selected(object sender, RoutedEventArgs e)
    {
      // Lookup for the source to be DataGridCell
      if (e.OriginalSource.GetType() == typeof(DataGridCell))
      {
        // Starts the Edit on the row;
        DataGrid dataGrid = (DataGrid)sender;
        dataGrid.BeginEdit(e);
      }
    }
  }
}
