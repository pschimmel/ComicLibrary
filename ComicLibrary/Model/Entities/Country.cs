namespace ComicLibrary.Model.Entities
{
  public class Country : Entity, IOption, IComparable, IComparable<Country>
  {
    public string Name { get; set; }

    public int CompareTo(object obj)
    {
      return obj is not Country other ? -1 : CompareTo(other);
    }

    public int CompareTo(Country other)
    {
      return string.Compare(Name, other.Name);
    }
  }
}