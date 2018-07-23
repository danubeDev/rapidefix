using System.Collections.Generic;

namespace RapideFix.Business.Data
{
  public class TagMapLeaf : TagMapNode
  {
    public IList<TagMapNode> Parents { get; set; }

    public string TypeConverterName { get; set; }

    public bool IsEncoded { get; set; }
  }
}
