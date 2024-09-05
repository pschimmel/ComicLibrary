using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ComicLibrary.View.Comparers
{
  public partial class StringAsNumberComparer : IComparer<string>
  {
    [LibraryImport("shlwapi.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int StrCmpLogicalW(string psz1, string psz2);

    public int Compare(string x, string y)
    {
      return x == null
             ? -1
             : y == null
             ? 1
             : StrCmpLogicalW(x, y);
    }
  }
}
