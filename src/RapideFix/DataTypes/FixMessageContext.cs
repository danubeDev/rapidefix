using System.Collections.Generic;

namespace RapideFix.DataTypes
{
  public class FixMessageContext
  {
    public FixMessageContext()
    {
      CreatedParentTypes = new HashSet<int>();
      RepeatingGroupCounters = new Dictionary<int, RepeatingCounter>();
    }

    public class RepeatingCounter
    {
      public RepeatingCounter(int firstTagKey)
      {
        FirstTagKey = firstTagKey;
        Index = -1;
      }

      public int FirstTagKey { get; }

      public int Index { get; set; }
    }

    public SupportedFixVersion? FixVersion { get; set; }

    public int LengthTagStartIndex { get; set; }

    public int MessageTypeTagStartIndex { get; set; }

    public int ChecksumTagStartIndex { get; set; }

    public byte ChecksumValue { get; set; }

    public MessageEncoding? EncodedFields { get; set; }

    /// <summary>
    /// Holds a counter for a given tag (first tag within a repeating group and the number of its appearences.
    /// </summary>
    public Dictionary<int, RepeatingCounter> RepeatingGroupCounters { get; }

    public HashSet<int> CreatedParentTypes { get; }

  }
}
