using System.ComponentModel;
using System.Windows.Controls;
using ComicLibrary.ViewModel;

namespace ComicLibrary
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private MainViewModel VM => DataContext as MainViewModel;

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      if (e.Cancel)
        return;

      if (VM.OnClosing() == false)
        e.Cancel = true;
    }

    private void DataGridCell_Selected(object sender, System.Windows.RoutedEventArgs e)
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