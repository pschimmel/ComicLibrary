using System.ComponentModel;
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
  }
}