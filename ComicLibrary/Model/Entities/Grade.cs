using System.Globalization;

namespace ComicLibrary.Model.Entities
{
  /// <summary>
  /// Grade of the comic. 
  /// See also https://artfulinvestments.com/comic-book-grading/
  /// </summary>
  public class Grade : IComparable, IComparable<Grade>
  {
    public Grade(double number, string name, string description, string scale)
    {
      Number = number;
      Name = name;
      ShortName = description;
      Scale = scale;
    }

    public double Number { get; }

    public string Name { get; }

    public string ShortName { get; }

    public string Scale { get; }

    public override bool Equals(object obj)
    {
      return obj is Grade other && Number.Equals(other.Number);
    }

    public override int GetHashCode()
    {
      return Number.GetHashCode();
    }

    public override string ToString()
    {
      return GradeHasNumber ? $"{Number.ToString("0.0", CultureInfo.InvariantCulture)} - {Name}" : Name;
    }

    public int CompareTo(Grade other)
    {
      return other == null ? 1 : Number.CompareTo(other.Number);
    }

    public int CompareTo(object obj)
    {
      return obj is not Grade other ? 1 : CompareTo(other);
    }

    public bool GradeHasNumber => Number > 0;
  }
}