using System;

namespace RapideFix.Business.Data
{
  public class EnumerableTagMapLeaf : TagMapLeaf, IEnumerableTag
  {
    public int RepeatingTagNumber { get; set; }

    public Type InnerType { get; set; }
  }
}
