using System.Windows;
using ES.Tools.Core.MVVM;

namespace ComicLibrary.View
{
  /// <summary>
  /// Interaction logic for EditOptionsDialog.xaml
  /// </summary>
  public partial class EditOptionsDialog : Window, IView
  {
    public EditOptionsDialog()
    {
      InitializeComponent();
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
