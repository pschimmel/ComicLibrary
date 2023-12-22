using System.Windows;
using System.Windows.Threading;
using ComicLibrary.View.Helpers;
using ComicLibrary.ViewModel;
using ES.Tools.Core.MVVM;

namespace ComicLibrary.View
{
  /// <summary>
  /// Interaction logic for EditComicDialog.xaml
  /// </summary>
  public partial class EditComicDialog : Window, IView
  {
    public EditComicDialog()
    {
      InitializeComponent();
      Loaded += OptionView_Loaded;
    }

    private void OptionView_Loaded(object sender, RoutedEventArgs e)
    {
      Loaded -= OptionView_Loaded;

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
      (ViewModel as EditComicViewModel).ApplyChanges();
      DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}
