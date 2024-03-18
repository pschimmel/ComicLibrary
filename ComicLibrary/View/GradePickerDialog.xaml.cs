using System.Windows;
using System.Windows.Threading;
using ComicLibrary.View.Helpers;
using ComicLibrary.ViewModel;
using ES.Tools.Core.MVVM;

namespace ComicLibrary.View
{
  /// <summary>
  /// Interaction logic for GradePickerDialog.xaml
  /// </summary>
  public partial class GradePickerDialog : Window, IView
  {
    public GradePickerDialog()
    {
      InitializeComponent();
      Loaded += OptionView_Loaded;
    }

    private void OptionView_Loaded(object sender, RoutedEventArgs e)
    {
      Loaded -= OptionView_Loaded;

      Dispatcher.BeginInvoke(() =>
      {
        for (int i = 0; i < gradesControl.Items.Count; i++)
        {
          if (gradesControl.Items[i] is GradeViewModel vm && vm.IsSelected)
          {
            FrameworkElement uiElement = (FrameworkElement)gradesControl.ItemContainerGenerator.ContainerFromIndex(i);
            uiElement.BringIntoView();
          }
        }

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
