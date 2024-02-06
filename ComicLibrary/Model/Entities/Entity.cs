namespace ComicLibrary.Model.Entities
{
  public class Entity : IEntity
  {
    private Guid id;

    public Entity()
    {
      CreatedDate = DateTime.Now;
      ModifiedDate = DateTime.Now;
      id = Guid.NewGuid();
    }

    public Guid ID
    {
      get => id == Guid.Empty ? Guid.NewGuid() : id;
      set => id = value;
    }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }
  }
}