namespace ComicLibrary.Model.Entities
{
  public class Language : OptionEntity, IComparable, IComparable<Language>
  {
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