namespace ComicLibrary.Model.Entities
{
  public class Entity
  {
    private Guid id = Guid.NewGuid();

    public Guid ID
    {
      get => id == Guid.Empty ? Guid.NewGuid() : id;
      set => id = value;
    }
  }
}