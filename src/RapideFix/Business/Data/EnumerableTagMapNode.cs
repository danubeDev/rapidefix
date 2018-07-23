using System;

namespace RapideFix.Business.Data
{
  public class EnumerableTagMapNode : TagMapNode, IEnumerableTag
  {
    public int RepeatingTagNumber { get; set; }

    public Type InnerType { get; set; }
  }
}
