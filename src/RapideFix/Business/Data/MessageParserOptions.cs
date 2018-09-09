using RapideFix.DataTypes;

namespace RapideFix.Business.Data
{
  public class MessageParserOptions
  {
    /// <summary>
    /// Gets or sets to throw an Exception of validation error.
    /// </summary>
    public bool ThrowIfInvalidMessage { get; set; }

    /// <summary>
    /// Gets or sets the encoding type for the encoded fix tags.
    /// </summary>
    public MessageEncoding Encoding { get; set; }
  }
}
