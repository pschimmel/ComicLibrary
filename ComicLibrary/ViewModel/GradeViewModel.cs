using ComicLibrary.Model.Entities;

namespace ComicLibrary.ViewModel
{
  public class GradeViewModel : ES.Tools.Core.MVVM.ViewModel
  {
    private readonly Grade _grade;
    private bool isSelected;

    public GradeViewModel(Grade grade, bool selected)
    {
      _grade = grade;
      Name = grade.Name;
      Description = grade.Description;
      Number = grade.Number > 0 ? grade.Number : null;
      IsSelected = selected;
    }

    public string Name { get; }

    public string Description { get; }

    public double? Number { get; }

    public bool IsSelected
    {
      get => isSelected;
      set
      {
        if (isSelected != value)
        {
          isSelected = value;
          OnPropertyChanged(nameof(IsSelected));
        }
      }
    }

    public Grade GetModel()
    {
      return _grade;
    }
  }
}
