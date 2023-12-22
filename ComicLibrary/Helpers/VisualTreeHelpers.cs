using System.Windows;
using System.Windows.Media;

namespace ComicLibrary.Helpers
{
  public static class VisualTreeHelpers
  {
    public static T FindFirstVisualChild<T>(this DependencyObject dependencyObject, Func<T, bool> condition) where T : DependencyObject
    {
      if (dependencyObject == null)
        return default;

      if (dependencyObject is T t && condition.Invoke(t))
      {
        return (T)dependencyObject;
      }

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);
        var childItem = FindFirstVisualChild(child, condition);
        if (childItem != default)
          return childItem;
      }

      return default;
    }

  }
}
