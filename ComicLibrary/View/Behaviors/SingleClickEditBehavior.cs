using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ES.Tools.UI;
using Microsoft.Xaml.Behaviors;

namespace ComicLibrary.View.Behaviors
{
  public class SingleClickEditBehavior : Behavior<DataGrid>
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      var style = new Style(typeof(DataGridCell));
      style.Setters.Add(new EventSetter(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(DataGridCell_PreviewMouseLeftButtonDown)));
      style.Setters.Add(new EventSetter(UIElement.PreviewTextInputEvent, new TextCompositionEventHandler(DataGridCell_PreviewTextInput)));
      AssociatedObject.CellStyle = style;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.CellStyle = null;
    }
    private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      DataGridCell cell = sender as DataGridCell;
      AssociatedObject.SelectedItem = cell.DataContext;
      GridColumnFastEdit(cell, e);
    }

    private void DataGridCell_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      DataGridCell cell = sender as DataGridCell;
      GridColumnFastEdit(cell, e);
    }

    private void GridColumnFastEdit(DataGridCell cell, RoutedEventArgs e)
    {
      DataGrid dataGrid = AssociatedObject;
      dataGrid.BeginEdit(e);

      if (cell == null || cell.IsEditing || cell.IsReadOnly)
        return;

      if (!cell.IsFocused)
      {
        cell.Focus();
      }

      InvokeContent(cell, e, dataGrid);
    }

    private static void InvokeContent(DataGridCell cell, RoutedEventArgs e, DataGrid dataGrid)
    {
      dataGrid.BeginEdit(e);

      // Check for CheckBox
      var checkBox = cell.GetVisualChild<CheckBox>();
      if (checkBox != null)
      {
        checkBox.IsChecked = !checkBox.IsChecked;
        return;
      }

      // Check for ComboBox
      var comboBox = cell.GetVisualChild<ComboBox>();
      if (comboBox != null && !comboBox.IsEditable)
      {
        DispatcherWrapper.Default.InvokeIfRequired(() => comboBox.IsDropDownOpen = true, DispatcherPriority.Background);
        return;
      }

      // Check for Button
      var button = cell.GetVisualChild<Button>();
      if (button != null && button.Command is ICommand command)
      {
        command.Execute(null);
      }
    }
  }
}
