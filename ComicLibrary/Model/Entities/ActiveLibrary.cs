using System.Collections.Generic;

namespace ComicLibrary.Model.Entities
{
  public class ActiveLibrary
  {
    public HashSet<Comic> Comics { get; set; } = [];

    public DateTime SaveDate { get; set; }
  }
}
