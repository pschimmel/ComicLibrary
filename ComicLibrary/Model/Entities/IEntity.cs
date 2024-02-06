
namespace ComicLibrary.Model.Entities
{
  public interface IEntity
  {
    Guid ID { get; set; }

    DateTime CreatedDate { get; set; }

    DateTime ModifiedDate { get; set; }
  }
}