using System.Windows;
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

      ViewFactory.Instance.Register<GetNameViewModel, GetNameDialog>();
      ViewFactory.Instance.Register<EditComicViewModel, EditComicDialog>();
      ViewFactory.Instance.Register<EditOptionsViewModel, EditOptionsDialog>();
      ViewFactory.Instance.Register<EditOptionViewModel, EditOptionDialog>();
      ViewFactory.Instance.Register<GradePickerViewModel, GradePickerDialog>();
      ViewFactory.Instance.Register<MoveToLibraryViewModel, MoveToLibraryDialog>();
    }
  }
}
