using System.Collections.Generic;
using System.Diagnostics;
using ComicLibrary.Model.Entities;

namespace ComicLibrary.ViewModel
{
  public class GradePickerViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    #region Fields

    private GradingScale _selectedScale;

    #endregion

    #region Constructor

    public GradePickerViewModel(Grade grade, GradingScale scale)
    {
      Debug.Assert(grade == null || grade.Scale == scale.Name);
      Scales = new List<GradingScale>(ScaleHelper.Scales.OrderBy(x => x.Name));
      grade ??= scale.UnGraded;
      _selectedScale = scale;
      Grades = scale.Grades.OrderBy(x => x.Number).Select(x => new GradeViewModel(x, x.Equals(grade))).ToArray();
    }

    #endregion

    #region Properties

    public IEnumerable<GradingScale> Scales { get; }

    public GradingScale SelectedScale
    {
      get => _selectedScale;
      set
      {
        if (_selectedScale != value)
        {
          _selectedScale = value;
          double selection = SelectedGrade.Number;
          Grades = _selectedScale.Grades.OrderBy(x => x.Number).Select(x => new GradeViewModel(x, x.Number == selection)).ToArray();
          OnPropertyChanged(nameof(Grades));
        }
      }
    }

    public Grade SelectedGrade => Grades.Single(x => x.IsSelected).GetModel();

    public IEnumerable<GradeViewModel> Grades { get; private set; }

    #endregion
  }
}
