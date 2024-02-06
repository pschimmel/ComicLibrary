namespace ComicLibrary.Model.Entities
{
  public class Publisher : OptionEntity, IComparable, IComparable<Publisher>
  {
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