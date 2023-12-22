namespace ComicLibrary.Model.Entities
{
  public class Publisher : Entity, IOption, IComparable, IComparable<Publisher>
  {
    public string Name { get; set; }

    public int CompareTo(object obj)
    {
      return obj is not Publisher other ? -1 : CompareTo(other);
    }

    public int CompareTo(Publisher other)
    {
      return string.Compare(Name, other.Name);
    }
  }
}