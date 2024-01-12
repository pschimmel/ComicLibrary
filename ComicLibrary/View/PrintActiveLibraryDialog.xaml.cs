using System.Windows;
using ES.Tools.Core.MVVM;

namespace ComicLibrary.View
{
  /// <summary>
  /// Interaction logic for PrintActiveLibraryDialog.xaml
  /// </summary>
  public partial class PrintActiveLibraryDialog : Window, IView
  {
    public PrintActiveLibraryDialog()
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
  }
}
