using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using ComicLibrary.View.Comparers;
using ES.Tools.Core.MVVM;
using Microsoft.Xaml.Behaviors;

namespace ComicLibrary.View.Behaviors
{
  /// <summary>
  /// Sort behavior to apply a custom sorting in a WPF DataGrid.
  /// </summary>
  //public class CustomSortBehavior
  //{
  //  public static readonly DependencyProperty CustomSorterProperty = DependencyProperty.RegisterAttached("CustomSorter", typeof(ICustomComparer), typeof(CustomSortBehavior));

  //  public static ICustomComparer GetCustomSorter(DataGridColumn gridColumn)
  //  {
  //    return (ICustomComparer)gridColumn.GetValue(CustomSorterProperty);
  //  }

  //  public static void SetCustomSorter(DataGridColumn gridColumn, ICustomComparer value)
  //  {
  //    gridColumn.SetValue(CustomSorterProperty, value);
  //  }

  //  public static readonly DependencyProperty AllowCustomSortProperty = DependencyProperty.RegisterAttached("AllowCustomSort", typeof(bool), typeof(CustomSortBehavior), new UIPropertyMetadata(false, OnAllowCustomSortChanged));

  //  public static bool GetAllowCustomSort(DataGrid grid)
  //  {
  //    return (bool)grid.GetValue(AllowCustomSortProperty);
  //  }

  //  public static void SetAllowCustomSort(DataGrid grid, bool value)
  //  {
  //    grid.SetValue(AllowCustomSortProperty, value);
  //  }

  //  private static void OnAllowCustomSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  //  {
  //    if (d is not DataGrid existing)
  //      return;

  //    var oldAllow = (bool)e.OldValue;
  //    var newAllow = (bool)e.NewValue;

  //    if (!oldAllow && newAllow)
  //    {
  //      existing.Sorting += HandleCustomSorting;
  //    }
  //    else
  //    {
  //      existing.Sorting -= HandleCustomSorting;
  //    }
  //  }

  //  private static void HandleCustomSorting(object sender, DataGridSortingEventArgs e)
  //  {
  //    if (sender is not DataGrid _dataGrid || !GetAllowCustomSort(_dataGrid))
  //      return;

  //    CollectionView view = ES.Tools.Core.MVVM.ViewModelExtensions.GetView(_dataGrid.ItemsSource);

  //    if (view == null)
  //      throw new Exception("The DataGrid's ItemsSource property must be of type, ListCollectionView");

  //    if (view is not ListCollectionView listCollectionView)
  //      throw new Exception("The DataGrid's ItemsSource property must be of type, ListCollectionView");

  //    // Sanity check
  //    var sorter = GetCustomSorter(e.Column);
  //    if (sorter == null)
  //      return;

  //    // The guts.
  //    e.Handled = true;

  //    var direction = e.Column.SortDirection == ListSortDirection.Ascending
  //                    ? ListSortDirection.Descending
  //                    : ListSortDirection.Ascending;

  //    e.Column.SortDirection = sorter.SortDirection = direction;
  //    listCollectionView.CustomSort = sorter;
  //  }
  //}

  public class CustomSortBehavior : Behavior<DataGrid>
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.Sorting += HandleCustomSorting;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.Sorting -= HandleCustomSorting;
      base.OnDetaching();
    }

    public ICustomComparer Comparer { get; set; }

    private void HandleCustomSorting(object sender, DataGridSortingEventArgs e)
    {
      if (sender is not DataGrid dataGrid || Comparer == null)
        return;

      if (e.Column.SortMemberPath != Comparer.PropertyName)
        return;

      CollectionView view = dataGrid.ItemsSource.GetView() ?? throw new Exception("The DataGrid's ItemsSource property must be of type, ListCollectionView");

      if (view is not ListCollectionView listCollectionView)
        throw new Exception("The DataGrid's ItemsSource property must be of type, ListCollectionView");

      e.Handled = true;

      var direction = e.Column.SortDirection == ListSortDirection.Ascending
                      ? ListSortDirection.Descending
                      : ListSortDirection.Ascending;

      e.Column.SortDirection = Comparer.SortDirection = direction;
      listCollectionView.CustomSort = Comparer;
    }
  }
}
