namespace ComicLibrary.Model.Entities
{
  public class Country : OptionEntity, IComparable, IComparable<Country>
  {
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