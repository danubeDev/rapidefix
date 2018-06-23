namespace RapideFix.DataTypes
{
  public class FixMessageContext
  {
    public SupportedFixVersion FixVersion { get; set; }

    public int LengthTagStartIndex { get; set; }

    public int MessageTypeTagStartIndex { get; set; }

    public int ChecksumTagStartIndex { get; set; }

    public MessageEncoding EncodedFields { get; set; }
  }
}
