namespace ComicLibrary.Model.Entities
{
  public class Language : Entity, IOption, IComparable, IComparable<Language>
  {
    public string Name { get; set; }

    public int CompareTo(object obj)
    {
      return obj is not Language other ? -1 : CompareTo(other);
    }

    public int CompareTo(Language other)
    {
      return string.Compare(Name, other.Name);
    }
  }
}