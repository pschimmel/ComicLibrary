using System.Windows;
using System.Windows.Controls;
using ComicLibrary.View;
using ComicLibrary.ViewModel;
using ES.Tools.Core.MVVM;

namespace ComicLibrary
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      // Automatically select all text when a TextBox is focused.
      EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler(TextBox_GotFocus));

      // Register combination of VMs and Views in the ViewFactory.
      ViewFactory.Instance.Register<GetNameViewModel, GetNameDialog>();
      ViewFactory.Instance.Register<EditComicViewModel, EditComicDialog>();
      ViewFactory.Instance.Register<EditOptionsViewModel, EditOptionsDialog>();
      ViewFactory.Instance.Register<EditOptionViewModel, EditOptionDialog>();
      ViewFactory.Instance.Register<GradePickerViewModel, GradePickerDialog>();
      ViewFactory.Instance.Register<MoveToLibraryViewModel, MoveToLibraryDialog>();
      ViewFactory.Instance.Register<PrintActiveLibraryViewModel, PrintActiveLibraryDialog>();
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      (sender as TextBox).SelectAll();
    }
  }
}
