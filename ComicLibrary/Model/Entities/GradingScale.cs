using System.Collections.Generic;

namespace ComicLibrary.Model.Entities
{
  public class GradingScale
  {
    public GradingScale(string name)
    {
      Name = name;
      Grades = [];
    }

    public string Name { get; }

    public List<Grade> Grades { get; }

    public Grade UnGraded => new(-1.0, "(Unrated)", "", Name);
  }
}
