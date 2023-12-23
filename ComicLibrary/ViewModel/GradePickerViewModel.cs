using System.Collections.Generic;
using System.Linq;
using ComicLibrary.Model.Entities;

namespace ComicLibrary.ViewModel
{
  public class GradePickerViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    #region Constructor

    public GradePickerViewModel(Grade grade)
    {
      grade ??= Grade.Unrated;
      Grades = Grade.Grades.ToList().Select(x => new GradeViewModel(x, x.Equals(grade))).ToArray();
    }

    #endregion

    #region Properties

    public Grade SelectedGrade => Grades.Single(x => x.IsSelected).GetModel();

    public IEnumerable<GradeViewModel> Grades { get; }

    #endregion
  }
}
