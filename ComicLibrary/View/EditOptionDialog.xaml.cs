using System.Windows;
using System.Windows.Threading;
using ComicLibrary.Helpers;
using ES.Tools.Core.MVVM;

namespace ComicLibrary.View
{
  /// <summary>
  /// Interaction logic for EditOptionDialog.xaml
  /// </summary>
  public partial class EditOptionDialog : Window, IView
  {
    public EditOptionDialog()
    {
      InitializeComponent();
      Loaded += EditOptionDialog_Loaded;
    }

    private void EditOptionDialog_Loaded(object sender, RoutedEventArgs e)
    {
      Loaded -= EditOptionDialog_Loaded;

      Dispatcher.BeginInvoke(() =>
      {
        var firstFocusableChild = (Content as FrameworkElement)?.FindFirstVisualChild<FrameworkElement>(x => x.Focusable);
        firstFocusableChild?.Focus();
      }, DispatcherPriority.ContextIdle);
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}
