using System.Windows;
using System.Windows.Controls;

namespace ComicLibrary.View.Behaviors
{
  public class ScrollIntoViewBehavior
  {
    public static readonly DependencyProperty SelectingItemProperty = DependencyProperty.RegisterAttached("SelectingItem", typeof(object), typeof(ScrollIntoViewBehavior), new PropertyMetadata(null, OnSelectingItemChanged));

    public static object GetSelectingItem(DependencyObject target)
    {
      return target.GetValue(SelectingItemProperty);
    }

    public static void SetSelectingItem(DependencyObject target, object value)
    {
      target.SetValue(SelectingItemProperty, value);
    }

    static void OnSelectingItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender is not DataGrid grid || grid.SelectedItem == null)
        return;

      grid.Dispatcher.InvokeAsync(() =>
      {
        grid.UpdateLayout();
        grid.ScrollIntoView(grid.SelectedItem, null);
      });
    }
  }
}
